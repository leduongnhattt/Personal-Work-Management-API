using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWorkManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsforRefreshtoken : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IPAddress",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreviousToken",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RevokedAt",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RevokedReason",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserAgent",
                table: "RefreshTokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IPAddress",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "PreviousToken",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokedAt",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "RevokedReason",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "UserAgent",
                table: "RefreshTokens");
        }
    }
}
