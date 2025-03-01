using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RestApiLabb.Migrations
{
    /// <inheritdoc />
    public partial class seeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "PersonId", "Description", "Email", "FullName", "MobileNumber" },
                values: new object[] { 1, "A future fullstack developer within the .NET. family of languages and practises. OOP, Agile, C#, EF Core, API, T-SQL, Database, Azure, React.", "simon3eke@gmail.com", "Simon Eke", "+46705516182" });

            migrationBuilder.InsertData(
                table: "Educations",
                columns: new[] { "EducationId", "Degree", "EndDate", "PersonId_FK", "School", "StartDate" },
                values: new object[,]
                {
                    { 1, "Full-stack web-dev, backend focus, APIs & services", null, 1, "Chas Academy", new DateOnly(2024, 9, 2) },
                    { 2, "1-year acting program, stage skills & performance", new DateOnly(2023, 6, 9), 1, "Sundsgården", new DateOnly(2022, 8, 22) }
                });

            migrationBuilder.InsertData(
                table: "Experiences",
                columns: new[] { "ExperienceId", "Company", "Description", "EndYear", "JobTitle", "PersonId_FK", "StartYear" },
                values: new object[,]
                {
                    { 1, "Simon Eke Consulting", "Worked on developing a website for a café", null, "Website developer", 1, new DateOnly(2025, 1, 23) },
                    { 2, "Life, the Universe and Everything", "Self-development. Rode the roller-coaster of Life", null, "Learner", 1, new DateOnly(1998, 4, 29) },
                    { 3, "Big Tech", "Manifesting that I'll be doing databases, APIs", new DateOnly(2025, 1, 1), "Backend-developer", 1, new DateOnly(2024, 12, 31) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Educations",
                keyColumn: "EducationId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Educations",
                keyColumn: "EducationId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "ExperienceId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "ExperienceId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Experiences",
                keyColumn: "ExperienceId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "PersonId",
                keyValue: 1);
        }
    }
}
