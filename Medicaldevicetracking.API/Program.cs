using System.Text.Json;
using System.Text.Json.Serialization;
using MedicalDeviceTracking.API.Middleware;
using MedicalDeviceTracking.Application.Interfaces;
using MedicalDeviceTracking.Application.Services;
using MedicalDeviceTracking.Domain.Interfaces;
using MedicalDeviceTracking.Infrastructure.Data;
using MedicalDeviceTracking.Infrastructure.Repositories;
using MedicalDeviceTracking.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Medical Device Tracking API",
        Version = "v1",
        Description = "API for tracking medical devices through IoT sensors and MQTT data"
    });
});

// Database - PostgreSQL with migrations support
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// AutoMapper - Now built into the main package (no separate Extensions package needed)
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddSingleton<ICloudImageService, CloudinaryImageService>();

// Repositories
builder.Services.AddScoped<IGatewayRepository, GatewayRepository>();
builder.Services.AddScoped<ISensorAdvertisementRepository, SensorAdvertisementRepository>();

// Services
builder.Services.AddScoped<IKafkaDataService, KafkaDataService>();

// Background Services
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddScoped<IGatewayRepository, GatewayRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IFloorMapRepository, FloorMapRepository>();
builder.Services.AddScoped<ISensorAdvertisementRepository, SensorAdvertisementRepository>();

// Services
builder.Services.AddScoped<IGatewayService, GatewayService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<IFloorMapService, FloorMapService>();
builder.Services.AddScoped<IKafkaDataService, KafkaDataService>();
// Logging
builder.Services.AddLogging();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Medical Device Tracking API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Apply database migrations automatically
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        // Apply any pending migrations
        context.Database.Migrate();
        app.Logger.LogInformation("Database migrations applied successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error applying database migrations");
        throw;
    }
}

app.Run();
