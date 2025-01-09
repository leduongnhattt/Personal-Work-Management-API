using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWorkManagement.Migrations
{
    /// <inheritdoc />
    public partial class RemovepropertyApointmentIdonWorkTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkTasks_Apointsments_ApointmentId",
                table: "WorkTasks");

            migrationBuilder.DropIndex(
                name: "IX_WorkTasks_ApointmentId",
                table: "WorkTasks");

            migrationBuilder.DropColumn(
                name: "ApointmentId",
                table: "WorkTasks");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ApointmentId",
                table: "WorkTasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkTasks_ApointmentId",
                table: "WorkTasks",
                column: "ApointmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkTasks_Apointsments_ApointmentId",
                table: "WorkTasks",
                column: "ApointmentId",
                principalTable: "Apointsments",
                principalColumn: "ApointmentId");
        }
    }
}
