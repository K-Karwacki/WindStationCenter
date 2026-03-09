using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TelemetryFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Telemetries_Farms_FarmId",
                table: "Telemetries");

            migrationBuilder.AlterColumn<string>(
                name: "FarmId",
                table: "Telemetries",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "FarmInternalId",
                table: "Telemetries",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "Farms",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Telemetries_FarmInternalId",
                table: "Telemetries",
                column: "FarmInternalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Telemetries_Farms_FarmInternalId",
                table: "Telemetries",
                column: "FarmInternalId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Telemetries_Farms_FarmInternalId",
                table: "Telemetries");

            migrationBuilder.DropIndex(
                name: "IX_Telemetries_FarmInternalId",
                table: "Telemetries");

            migrationBuilder.DropColumn(
                name: "FarmInternalId",
                table: "Telemetries");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Farms");

            migrationBuilder.AlterColumn<Guid>(
                name: "FarmId",
                table: "Telemetries",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Telemetries_Farms_FarmId",
                table: "Telemetries",
                column: "FarmId",
                principalTable: "Farms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
