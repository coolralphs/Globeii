using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class ItineraryAccomodation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "itinerary_accomodation",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    itinerary_id = table.Column<int>(type: "integer", nullable: false),
                    place_id = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "date", nullable: false),
                    end_date = table.Column<DateTime>(type: "date", nullable: false),
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
                    updated_by = table.Column<Guid>(type: "uuid", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_itinerary_accomodations", x => x.id);
                    table.ForeignKey(
                        name: "f_k_itinerary_accomodations__places_place_id",
                        column: x => x.place_id,
                        principalTable: "place",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "f_k_itinerary_accomodations_app_users_creator_id",
                        column: x => x.created_by,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_itinerary_accomodations_app_users_updator_id",
                        column: x => x.updated_by,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "f_k_itinerary_accomodations_itineraries_itinerary_id",
                        column: x => x.itinerary_id,
                        principalTable: "itinerary",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_accomodations_itinerary_id",
                table: "itinerary_accomodation",
                column: "itinerary_id");

            migrationBuilder.CreateIndex(
                name: "i_x_itinerary_accomodations_place_id",
                table: "itinerary_accomodation",
                column: "place_id");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_accomodation_created_by",
                table: "itinerary_accomodation",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_accomodation_updated_by",
                table: "itinerary_accomodation",
                column: "updated_by");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "itinerary_accomodation");
        }
    }
}
