using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChatData.Migrations
{
    public partial class MeesageWithBotMarkers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BotName",
                table: "Messages",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FromBot",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BotName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "FromBot",
                table: "Messages");
        }
    }
}
