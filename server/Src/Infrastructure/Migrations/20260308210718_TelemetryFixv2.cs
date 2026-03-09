using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TelemetryFixv2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TurbineId",
                table: "Turbines",
                newName: "TurbineExternalId");

            migrationBuilder.RenameIndex(
                name: "IX_Turbines_TurbineId",
                table: "Turbines",
                newName: "IX_Turbines_TurbineExternalId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TurbineExternalId",
                table: "Turbines",
                newName: "TurbineId");

            migrationBuilder.RenameIndex(
                name: "IX_Turbines_TurbineExternalId",
                table: "Turbines",
                newName: "IX_Turbines_TurbineId");
        }
    }
}
