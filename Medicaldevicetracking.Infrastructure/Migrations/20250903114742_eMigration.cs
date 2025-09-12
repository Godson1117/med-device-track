using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MedicalDeviceTracking.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class eMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FloorMaps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ImagePath = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Width = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    Height = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FloorMaps", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Gateways",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GatewayId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Uuid = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    MacAddress = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: true),
                    GatewayName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    FloorMapId = table.Column<Guid>(type: "uuid", nullable: true),
                    CoordinatesX = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: true),
                    CoordinatesY = table.Column<decimal>(type: "numeric(10,6)", precision: 10, scale: 6, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Gateways", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Gateways_FloorMaps_FloorMapId",
                        column: x => x.FloorMapId,
                        principalTable: "FloorMaps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Uuid = table.Column<string>(type: "character varying(36)", maxLength: 36, nullable: true),
                    MacAddress = table.Column<string>(type: "character varying(17)", maxLength: 17, nullable: false),
                    TagType = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    DeviceType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AssignedTo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RssiThreshold = table.Column<int>(type: "integer", nullable: true),
                    CurrentGatewayId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastRssi = table.Column<int>(type: "integer", nullable: true),
                    LastSeenAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tags_Gateways_CurrentGatewayId",
                        column: x => x.CurrentGatewayId,
                        principalTable: "Gateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SensorAdvertisements",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GatewayId = table.Column<Guid>(type: "uuid", nullable: false),
                    TagId = table.Column<Guid>(type: "uuid", nullable: true),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MacAddress = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Rssi = table.Column<int>(type: "integer", nullable: true),
                    Battery = table.Column<int>(type: "integer", nullable: true),
                    Major = table.Column<int>(type: "integer", nullable: true),
                    Minor = table.Column<int>(type: "integer", nullable: true),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Uuid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RssiAtXm = table.Column<int>(type: "integer", nullable: true),
                    Temperature = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    Humidity = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorAdvertisements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SensorAdvertisements_Gateways_GatewayId",
                        column: x => x.GatewayId,
                        principalTable: "Gateways",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SensorAdvertisements_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Gateways_FloorMapId",
                table: "Gateways",
                column: "FloorMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Gateways_GatewayId",
                table: "Gateways",
                column: "GatewayId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gateways_MacAddress",
                table: "Gateways",
                column: "MacAddress",
                unique: true,
                filter: "\"MacAddress\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SensorAdvertisements_GatewayId_Timestamp",
                table: "SensorAdvertisements",
                columns: new[] { "GatewayId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorAdvertisements_MacAddress",
                table: "SensorAdvertisements",
                column: "MacAddress");

            migrationBuilder.CreateIndex(
                name: "IX_SensorAdvertisements_TagId_Timestamp",
                table: "SensorAdvertisements",
                columns: new[] { "TagId", "Timestamp" });

            migrationBuilder.CreateIndex(
                name: "IX_SensorAdvertisements_Timestamp",
                table: "SensorAdvertisements",
                column: "Timestamp");

            migrationBuilder.CreateIndex(
                name: "IX_SensorAdvertisements_Type",
                table: "SensorAdvertisements",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CurrentGatewayId",
                table: "Tags",
                column: "CurrentGatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_MacAddress",
                table: "Tags",
                column: "MacAddress",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Uuid",
                table: "Tags",
                column: "Uuid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SensorAdvertisements");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Gateways");

            migrationBuilder.DropTable(
                name: "FloorMaps");
        }
    }
}
