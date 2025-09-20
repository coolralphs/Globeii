using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedItineraryFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Temporarily rename the old column
            migrationBuilder.RenameColumn(
                name: "created_by",
                table: "itinerary",
                newName: "created_by_old");

            //migrationBuilder.AlterColumn<Guid>(
            //    name: "created_by",
            //    table: "itinerary",
            //    type: "uuid",
            //    nullable: false,
            //    defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            //    oldClrType: typeof(string),
            //    oldType: "text",
            //    oldNullable: true);
            // 2.Add new GUID column

            migrationBuilder.AddColumn<Guid>(
                name: "created_by",
                table: "itinerary",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty); // or nullable: true if needed

            // 4. Drop old column
            migrationBuilder.DropColumn(
                name: "created_by_old",
                table: "itinerary");

            migrationBuilder.CreateIndex(
                name: "IX_itinerary_created_by",
                table: "itinerary",
                column: "created_by");

            migrationBuilder.AddForeignKey(
                name: "f_k_itineraries_app_users_creator_id",
                table: "itinerary",
                column: "created_by",
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

            migrationBuilder.DropIndex(
                name: "IX_itinerary_created_by",
                table: "itinerary");

            migrationBuilder.AlterColumn<string>(
                name: "created_by",
                table: "itinerary",
                type: "text",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
