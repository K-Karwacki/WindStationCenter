using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserCommandChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commands_Users_UserInternalId",
                table: "Commands");

            migrationBuilder.DropIndex(
                name: "IX_Commands_UserInternalId",
                table: "Commands");

            migrationBuilder.DropColumn(
                name: "UserInternalId",
                table: "Commands");

            // Use explicit SQL with USING to convert existing text values to uuid
            migrationBuilder.Sql("ALTER TABLE \"Commands\" ALTER COLUMN \"UserId\" TYPE uuid USING (\"UserId\"::uuid);");

            migrationBuilder.CreateIndex(
                name: "IX_Commands_UserId",
                table: "Commands",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commands_Users_UserId",
                table: "Commands",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Commands_Users_UserId",
                table: "Commands");

            migrationBuilder.DropIndex(
                name: "IX_Commands_UserId",
                table: "Commands");

            // Convert the column back to text if rolling back
            migrationBuilder.Sql("ALTER TABLE \"Commands\" ALTER COLUMN \"UserId\" TYPE text USING (\"UserId\"::text);");

            migrationBuilder.AddColumn<Guid>(
                name: "UserInternalId",
                table: "Commands",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Commands_UserInternalId",
                table: "Commands",
                column: "UserInternalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Commands_Users_UserInternalId",
                table: "Commands",
                column: "UserInternalId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
