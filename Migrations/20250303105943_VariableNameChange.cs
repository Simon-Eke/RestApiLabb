using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RestApiLabb.Migrations
{
    /// <inheritdoc />
    public partial class VariableNameChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartYear",
                table: "Experiences",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "EndYear",
                table: "Experiences",
                newName: "EndDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Experiences",
                newName: "StartYear");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Experiences",
                newName: "EndYear");
        }
    }
}
