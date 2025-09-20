using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedItineraryReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "itinerary_reservation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    itinerary_id = table.Column<int>(type: "integer", nullable: false),
                    booking_reference = table.Column<string>(type: "text", nullable: true),
                    confirmation_number = table.Column<string>(type: "text", nullable: true),
                    place_id = table.Column<int>(type: "integer", nullable: true),
                    provider_name = table.Column<string>(type: "text", nullable: true),
                    departure_place_id = table.Column<int>(type: "integer", nullable: true),
                    destination_place_id = table.Column<int>(type: "integer", nullable: true),
                    pickup_location = table.Column<string>(type: "text", nullable: true),
                    drop_off_location = table.Column<string>(type: "text", nullable: true),
                    url = table.Column<string>(type: "text", nullable: true),
                    is_available_in_app = table.Column<bool>(type: "boolean", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time", nullable: false),
                    booking_date = table.Column<DateTime>(type: "date", nullable: false),
                    reservation_type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    party_size = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Confirmed"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP"),
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_itinerary_reservations", x => x.id);
                    table.ForeignKey(
                        name: "f_k_itinerary_reservations__places_departure_place_id",
                        column: x => x.departure_place_id,
                        principalTable: "place",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_itinerary_reservations__places_destination_place_id",
                        column: x => x.destination_place_id,
                        principalTable: "place",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_itinerary_reservations__places_place_id",
                        column: x => x.place_id,
                        principalTable: "place",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_itinerary_reservations_app_users_creator_id",
                        column: x => x.created_by,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_itinerary_reservations_app_users_updator_id",
                        column: x => x.updated_by,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_itinerary_reservations_itineraries_itinerary_id",
                        column: x => x.itinerary_id,
                        principalTable: "itinerary",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_reservations_departure_place_id",
                table: "itinerary_reservation",
                column: "departure_place_id");

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_reservations_destination_place_id",
                table: "itinerary_reservation",
                column: "destination_place_id");

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_reservations_itinerary_id",
                table: "itinerary_reservation",
                column: "itinerary_id");

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_reservations_place_id",
                table: "itinerary_reservation",
                column: "place_id");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_reservation_created_by",
                table: "itinerary_reservation",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_reservation_updated_by",
                table: "itinerary_reservation",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "itinerary_reservation");
        }
    }
}
