using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialSetup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary");

            migrationBuilder.AlterColumn<string>(
                name: "provider_user_id",
                table: "user_login",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "provider",
                table: "user_login",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "app_user",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "place",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    formatted_address = table.Column<string>(type: "text", nullable: true),
                    lat = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    lng = table.Column<double>(type: "double precision", precision: 9, scale: 6, nullable: false),
                    opening_hours_json = table.Column<string>(type: "jsonb", nullable: true),
                    thumbnail_url = table.Column<string>(type: "text", nullable: true),
                    administrative_area = table.Column<string>(type: "text", nullable: true),
                    locality = table.Column<string>(type: "text", nullable: true),
                    postal_code = table.Column<string>(type: "text", nullable: true),
                    region_code = table.Column<string>(type: "text", nullable: true),
                    rating = table.Column<double>(type: "double precision", nullable: true),
                    user_rating_count = table.Column<int>(type: "integer", nullable: true),
                    primary_type = table.Column<string>(type: "text", nullable: true),
                    primary_type_display_name = table.Column<string>(type: "text", nullable: true),
                    types = table.Column<string[]>(type: "text[]", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_places", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "itinerary_place",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    itinerary_id = table.Column<int>(type: "integer", nullable: false),
                    place_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    was_skipped = table.Column<bool>(type: "boolean", nullable: false),
                    is_booked = table.Column<bool>(type: "boolean", nullable: false),
                    is_paid = table.Column<bool>(type: "boolean", nullable: false),
                    booking_required = table.Column<bool>(type: "boolean", nullable: false),
                    url = table.Column<string>(type: "text", nullable: true),
                    user_rating = table.Column<int>(type: "integer", nullable: true),
                    notes = table.Column<string>(type: "text", nullable: true),
                    pre_payment_required = table.Column<bool>(type: "boolean", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    PlaceId1 = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_itinerary_places", x => x.id);
                    table.ForeignKey(
                        name: "FK_itinerary_place_place_PlaceId1",
                        column: x => x.PlaceId1,
                        principalTable: "place",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "f_k_itinerary_places__places_place_id",
                        column: x => x.place_id,
                        principalTable: "place",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_itinerary_places_app_users_creator_id",
                        column: x => x.created_by,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_places_place_id",
                table: "itinerary_place",
                column: "place_id");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_place_created_by",
                table: "itinerary_place",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_place_PlaceId1",
                table: "itinerary_place",
                column: "PlaceId1");

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary");

            migrationBuilder.DropTable(
                name: "itinerary_place");

            migrationBuilder.DropTable(
                name: "place");

            migrationBuilder.AlterColumn<string>(
                name: "provider_user_id",
                table: "user_login",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "provider",
                table: "user_login",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<Guid>(
                name: "id",
                table: "app_user",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary",
                column: "created_by",
                principalTable: "app_user",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
