using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Itineraries",
                table: "Itineraries");

            migrationBuilder.RenameTable(
                name: "Itineraries",
                newName: "itinerary");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "itinerary",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_itinerary",
                table: "itinerary",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_itinerary",
                table: "itinerary");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "itinerary");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "itinerary");

            migrationBuilder.RenameTable(
                name: "itinerary",
                newName: "Itineraries");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Itineraries",
                table: "Itineraries",
                column: "Id");
        }
    }
}
