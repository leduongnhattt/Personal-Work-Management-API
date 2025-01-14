using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalWorkManagement.Migrations
{
    /// <inheritdoc />
    public partial class RenameColumnStartandEndDataofApointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateTask",
                table: "Apointsments",
                newName: "StartDateApoint");

            migrationBuilder.RenameColumn(
                name: "EndDateTask",
                table: "Apointsments",
                newName: "EndDateApoint");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDateApoint",
                table: "Apointsments",
                newName: "StartDateTask");

            migrationBuilder.RenameColumn(
                name: "EndDateApoint",
                table: "Apointsments",
                newName: "EndDateTask");
        }
    }
}
