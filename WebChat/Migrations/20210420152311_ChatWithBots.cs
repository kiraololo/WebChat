using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebChat.Migrations
{
    public partial class ChatWithBots : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bot",
                columns: table => new
                {
                    BotId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bot", x => x.BotId);
                });

            migrationBuilder.CreateTable(
                name: "BotChat",
                columns: table => new
                {
                    BotsBotId = table.Column<int>(type: "integer", nullable: false),
                    ChatsChatID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BotChat", x => new { x.BotsBotId, x.ChatsChatID });
                    table.ForeignKey(
                        name: "FK_BotChat_Bot_BotsBotId",
                        column: x => x.BotsBotId,
                        principalTable: "Bot",
                        principalColumn: "BotId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BotChat_Chats_ChatsChatID",
                        column: x => x.ChatsChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BotChat_ChatsChatID",
                table: "BotChat",
                column: "ChatsChatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BotChat");

            migrationBuilder.DropTable(
                name: "Bot");
        }
    }
}
