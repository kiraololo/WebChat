using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace WebChat.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    ChatID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.ChatID);
                });

            migrationBuilder.CreateTable(
                name: "ChatEvents",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventKey = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ChatID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatEvents", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_ChatEvents_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatUsers",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NikName = table.Column<string>(type: "text", nullable: true),
                    LoginName = table.Column<string>(type: "text", nullable: true),
                    ChatID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUsers", x => x.UserID);
                    table.ForeignKey(
                        name: "FK_ChatUsers_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ChatUserEvents",
                columns: table => new
                {
                    EventID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventKey = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    UserID = table.Column<int>(type: "integer", nullable: true),
                    ChatID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatUserEvents", x => x.EventID);
                    table.ForeignKey(
                        name: "FK_ChatUserEvents_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChatUserEvents_ChatUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "ChatUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    MessageID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FromUserID = table.Column<int>(type: "integer", nullable: true),
                    MessageText = table.Column<string>(type: "text", nullable: true),
                    SentDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsReaded = table.Column<bool>(type: "boolean", nullable: false),
                    ChatID = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.MessageID);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_ChatID",
                        column: x => x.ChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Messages_ChatUsers_FromUserID",
                        column: x => x.FromUserID,
                        principalTable: "ChatUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatEvents_ChatID",
                table: "ChatEvents",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUserEvents_ChatID",
                table: "ChatUserEvents",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUserEvents_UserID",
                table: "ChatUserEvents",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_ChatID",
                table: "ChatUsers",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ChatID",
                table: "Messages",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_FromUserID",
                table: "Messages",
                column: "FromUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatEvents");

            migrationBuilder.DropTable(
                name: "ChatUserEvents");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "ChatUsers");

            migrationBuilder.DropTable(
                name: "Chats");
        }
    }
}
