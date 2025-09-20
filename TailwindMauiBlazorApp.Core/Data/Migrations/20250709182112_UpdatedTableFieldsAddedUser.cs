using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedTableFieldsAddedUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_itinerary",
                table: "itinerary");

            migrationBuilder.AddPrimaryKey(
                name: "p_k_itineraries",
                table: "itinerary",
                column: "id");

            migrationBuilder.CreateTable(
                name: "app_user",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    display_name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_app_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "text", nullable: false),
                    provider_user_id = table.Column<string>(type: "text", nullable: false),
                    provider_email = table.Column<string>(type: "text", nullable: true),
                    linked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_user_logins", x => x.id);
                    table.ForeignKey(
                        name: "f_k_user_logins_app_users_app_user_id",
                        column: x => x.user_id,
                        principalTable: "app_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_user_login_provider_provider_user_id",
                table: "user_login",
                columns: new[] { "provider", "provider_user_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_user_login_user_id",
                table: "user_login",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_login");

            migrationBuilder.DropTable(
                name: "app_user");

            migrationBuilder.DropPrimaryKey(
                name: "p_k_itineraries",
                table: "itinerary");

            migrationBuilder.AddPrimaryKey(
                name: "PK_itinerary",
                table: "itinerary",
                column: "id");
        }
    }
}
