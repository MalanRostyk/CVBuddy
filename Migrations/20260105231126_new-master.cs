using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CVBuddy.Migrations
{
    /// <inheritdoc />
    public partial class newmaster : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublisDate",
                table: "Projects",
                newName: "PublishDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "PublishDate",
                table: "Cvs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishDate",
                table: "Cvs");

            migrationBuilder.RenameColumn(
                name: "PublishDate",
                table: "Projects",
                newName: "PublisDate");
        }
    }
}
