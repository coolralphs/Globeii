using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class MiscFixes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary");

            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_updated_by_id",
                table: "itinerary");

            migrationBuilder.DropForeignKey(
                name: "f_k_itinerary_places_app_users_creator_id",
                table: "itinerary_place");

            migrationBuilder.DropForeignKey(
                name: "f_k_itinerary_places_app_users_updated_by_id",
                table: "itinerary_place");

            migrationBuilder.RenameColumn(
                name: "updated_by_id",
                table: "itinerary_place",
                newName: "updated_by");

            migrationBuilder.RenameIndex(
                name: "i_x_itinerary_places_updated_by_id",
                table: "itinerary_place",
                newName: "IX_itinerary_place_updated_by");

            migrationBuilder.RenameColumn(
                name: "updated_by_id",
                table: "itinerary",
                newName: "updated_by");

            migrationBuilder.RenameIndex(
                name: "i_x_itineraries_updated_by_id",
                table: "itinerary",
                newName: "IX_itinerary_updated_by");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "itinerary",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "itinerary",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP",
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_updator_id",
                table: "itinerary",
                column: "updated_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_itinerary_places_app_users_creator_id",
                table: "itinerary_place",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "f_k_itinerary_places_app_users_updator_id",
                table: "itinerary_place",
                column: "updated_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary");

            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_updator_id",
                table: "itinerary");

            migrationBuilder.DropForeignKey(
                name: "f_k_itinerary_places_app_users_creator_id",
                table: "itinerary_place");

            migrationBuilder.DropForeignKey(
                name: "f_k_itinerary_places_app_users_updator_id",
                table: "itinerary_place");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "itinerary_place",
                newName: "updated_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_itinerary_place_updated_by",
                table: "itinerary_place",
                newName: "i_x_itinerary_places_updated_by_id");

            migrationBuilder.RenameColumn(
                name: "updated_by",
                table: "itinerary",
                newName: "updated_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_itinerary_updated_by",
                table: "itinerary",
                newName: "i_x_itineraries_updated_by_id");

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "itinerary",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_updated_by_id",
                table: "itinerary",
                column: "updated_by_id",
                principalTable: "app_user",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "f_k_itinerary_places_app_users_creator_id",
                table: "itinerary_place",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "f_k_itinerary_places_app_users_updated_by_id",
                table: "itinerary_place",
                column: "updated_by_id",
                principalTable: "app_user",
                principalColumn: "id");
        }
    }
}
