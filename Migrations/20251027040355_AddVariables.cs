using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class AddVariables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "endDate",
                table: "Competency",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "IconImage",
                table: "CDL",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferedFirstName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "endDate",
                table: "Competency");

            migrationBuilder.DropColumn(
                name: "IconImage",
                table: "CDL");

            migrationBuilder.DropColumn(
                name: "PreferedFirstName",
                table: "AspNetUsers");
        }
    }
}
