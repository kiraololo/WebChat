using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class ChatWithBots2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotChat_Bot_BotsBotId",
                table: "BotChat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bot",
                table: "Bot");

            migrationBuilder.RenameTable(
                name: "Bot",
                newName: "Bots");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bots",
                table: "Bots",
                column: "BotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotChat_Bots_BotsBotId",
                table: "BotChat",
                column: "BotsBotId",
                principalTable: "Bots",
                principalColumn: "BotId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BotChat_Bots_BotsBotId",
                table: "BotChat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bots",
                table: "Bots");

            migrationBuilder.RenameTable(
                name: "Bots",
                newName: "Bot");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bot",
                table: "Bot",
                column: "BotId");

            migrationBuilder.AddForeignKey(
                name: "FK_BotChat_Bot_BotsBotId",
                table: "BotChat",
                column: "BotsBotId",
                principalTable: "Bot",
                principalColumn: "BotId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
