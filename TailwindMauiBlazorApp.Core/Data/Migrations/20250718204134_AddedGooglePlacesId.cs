using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedGooglePlacesId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "google_place_id",
                table: "place",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_places_itinerary_id",
                table: "itinerary_place",
                column: "itinerary_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_itinerary_places_itineraries_itinerary_id",
                table: "itinerary_place",
                column: "itinerary_id",
                principalTable: "itinerary",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itinerary_places_itineraries_itinerary_id",
                table: "itinerary_place");

            migrationBuilder.DropIndex(
                name: "i_x_itinerary_places_itinerary_id",
                table: "itinerary_place");

            migrationBuilder.DropColumn(
                name: "google_place_id",
                table: "place");
        }
    }
}
