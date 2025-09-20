using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedItineraryColumnCasings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "itinerary",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "itinerary",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "itinerary",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "itinerary",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "itinerary",
                newName: "created_by");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "itinerary",
                newName: "created_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "itinerary",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "itinerary",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "itinerary",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "itinerary",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "itinerary",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "itinerary",
                newName: "CreatedAt");
        }
    }
}
