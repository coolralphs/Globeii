using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedConfigStartTimeToItinerary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "config_start_time",
                table: "itinerary",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<int>(
                name: "config_time_increment",
                table: "itinerary",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "config_start_time",
                table: "itinerary");

            migrationBuilder.DropColumn(
                name: "config_time_increment",
                table: "itinerary");
        }
    }
}
