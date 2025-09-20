using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItineraryPlaceNullableDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "itinerary_place",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by_id",
                table: "itinerary_place",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_places_updated_by_id",
                table: "itinerary_place",
                column: "updated_by_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_itinerary_places_app_users_updated_by_id",
                table: "itinerary_place",
                column: "updated_by_id",
                principalTable: "app_user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itinerary_places_app_users_updated_by_id",
                table: "itinerary_place");

            migrationBuilder.DropIndex(
                name: "i_x_itinerary_places_updated_by_id",
                table: "itinerary_place");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "itinerary_place");

            migrationBuilder.DropColumn(
                name: "updated_by_id",
                table: "itinerary_place");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);
        }
    }
}
