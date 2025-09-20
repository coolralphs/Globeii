using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItineraryPlaceStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "itinerary_place",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Planned");

            migrationBuilder.AddColumn<string>(
                name: "status",
                table: "itinerary_accomodation",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Planned");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "itinerary_place");

            migrationBuilder.DropColumn(
                name: "status",
                table: "itinerary_accomodation");
        }
    }
}
