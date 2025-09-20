using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TailwindMauiBlazorApp.Core.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedUserLoginFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "user_login",
                newName: "app_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_login_user_id",
                table: "user_login",
                newName: "i_x_user_logins_app_user_id");

            migrationBuilder.AddColumn<string>(
                name: "provider_avatar_url",
                table: "user_login",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "provider_display_name",
                table: "user_login",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "provider_avatar_url",
                table: "user_login");

            migrationBuilder.DropColumn(
                name: "provider_display_name",
                table: "user_login");

            migrationBuilder.RenameColumn(
                name: "app_user_id",
                table: "user_login",
                newName: "user_id");

            migrationBuilder.RenameIndex(
                name: "i_x_user_logins_app_user_id",
                table: "user_login",
                newName: "IX_user_login_user_id");
        }
    }
}
