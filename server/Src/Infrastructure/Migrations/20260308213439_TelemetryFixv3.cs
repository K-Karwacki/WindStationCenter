using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TelemetryFixv3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "Telemetries",
                newName: "TimeStamp");

            migrationBuilder.RenameIndex(
                name: "IX_Telemetries_FarmId_TurbineInternalId_Timestamp",
                table: "Telemetries",
                newName: "IX_Telemetries_FarmId_TurbineInternalId_TimeStamp");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeStamp",
                table: "Telemetries",
                newName: "Timestamp");

            migrationBuilder.RenameIndex(
                name: "IX_Telemetries_FarmId_TurbineInternalId_TimeStamp",
                table: "Telemetries",
                newName: "IX_Telemetries_FarmId_TurbineInternalId_Timestamp");
        }
    }
}
