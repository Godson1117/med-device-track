using System.Text.Json;
using Confluent.Kafka;
using MedicalDeviceTracking.Application.DTOs;
using MedicalDeviceTracking.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MedicalDeviceTracking.Infrastructure.Services;

public class KafkaConsumerService : BackgroundService
{
    private readonly ILogger<KafkaConsumerService> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly string _topicName;
    private readonly ConsumerConfig _consumerConfig;

    public KafkaConsumerService(
        ILogger<KafkaConsumerService> logger,
        IServiceProvider serviceProvider,
        IConfiguration configuration)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
        _configuration = configuration;

        // Use indexer syntax instead of GetValue
        _topicName = _configuration["Kafka:TopicName"] ?? "mqtt_data_tls";

        _consumerConfig = new ConsumerConfig
        {
            GroupId = _configuration["Kafka:GroupId"] ?? "medical-device-tracking",
            BootstrapServers = _configuration["Kafka:BootstrapServers"] ?? "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = false,
            SessionTimeoutMs = 6000,
            AutoCommitIntervalMs = 5000,
            EnableAutoCommit = false
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Run(() => StartConsuming(stoppingToken), stoppingToken);
    }

    private async Task StartConsuming(CancellationToken cancellationToken)
    {
        using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig)
            .SetErrorHandler((_, e) => _logger.LogError("Medical Device Tracking Kafka error: {Error}", e.Reason))
            .SetStatisticsHandler((_, json) => _logger.LogDebug("Medical Device Tracking Kafka statistics: {Statistics}", json))
            .Build();

        consumer.Subscribe(_topicName);
        _logger.LogInformation("Medical Device Tracking: Started consuming messages from topic: {Topic}", _topicName);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);

                    if (consumeResult.IsPartitionEOF)
                    {
                        _logger.LogDebug("Reached end of topic {Topic} partition {Partition}, next message will be at offset {Offset}",
                            consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);
                        continue;
                    }

                    _logger.LogDebug("Medical Device Tracking: Received message from {Topic}/{Partition}@{Offset}",
                        consumeResult.Topic, consumeResult.Partition, consumeResult.Offset);

                    await ProcessMessage(consumeResult.Message.Value);

                    consumer.Commit(consumeResult);
                }
                catch (ConsumeException e)
                {
                    _logger.LogError(e, "Medical Device Tracking consume error: {Error}", e.Error.Reason);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Medical Device Tracking: Error processing message");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Medical Device Tracking: Kafka consumer cancelled");
        }
        finally
        {
            consumer.Close();
            _logger.LogInformation("Medical Device Tracking: Kafka consumer closed");
        }
    }

    private async Task ProcessMessage(string messageValue)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var kafkaDataService = scope.ServiceProvider.GetRequiredService<IKafkaDataService>();

            var kafkaMessage = JsonSerializer.Deserialize<KafkaMessageDto>(messageValue, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (kafkaMessage != null)
            {
                var result = await kafkaDataService.ProcessKafkaMessageAsync(kafkaMessage);

                if (result.Success)
                {
                    _logger.LogInformation("Medical Device Tracking: Successfully processed message for gateway: {Gateway}", kafkaMessage.Gateway);
                }
                else
                {
                    _logger.LogError("Medical Device Tracking: Failed to process message: {Error}", result.Message);
                }
            }
            else
            {
                _logger.LogWarning("Medical Device Tracking: Failed to deserialize Kafka message: {Message}", messageValue);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Medical Device Tracking: JSON deserialization error for message: {Message}", messageValue);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Medical Device Tracking: Error processing Kafka message: {Message}", messageValue);
        }
    }
}
