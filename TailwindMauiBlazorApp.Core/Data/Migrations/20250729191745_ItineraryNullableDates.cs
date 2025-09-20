using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItineraryNullableDates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_itinerary_place_place_PlaceId1",
                table: "itinerary_place");

            migrationBuilder.DropIndex(
                name: "IX_itinerary_place_PlaceId1",
                table: "itinerary_place");

            migrationBuilder.DropColumn(
                name: "PlaceId1",
                table: "itinerary_place");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated_at",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "updated_by_id",
                table: "itinerary",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_itineraries_updated_by_id",
                table: "itinerary",
                column: "updated_by_id");

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_updated_by_id",
                table: "itinerary",
                column: "updated_by_id",
                principalTable: "app_user",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_updated_by_id",
                table: "itinerary");

            migrationBuilder.DropIndex(
                name: "i_x_itineraries_updated_by_id",
                table: "itinerary");

            migrationBuilder.DropColumn(
                name: "updated_at",
                table: "itinerary");

            migrationBuilder.DropColumn(
                name: "updated_by_id",
                table: "itinerary");

            migrationBuilder.AddColumn<int>(
                name: "PlaceId1",
                table: "itinerary_place",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_place_PlaceId1",
                table: "itinerary_place",
                column: "PlaceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_itinerary_place_place_PlaceId1",
                table: "itinerary_place",
                column: "PlaceId1",
                principalTable: "place",
                principalColumn: "id");
        }
    }
}
