using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class MemberWithChatRemoveOwnerFromChat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatUsers_OwnerUserID",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_ChatUsers_Chats_ChatID",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_ChatUsers_ChatID",
                table: "ChatUsers");

            migrationBuilder.DropIndex(
                name: "IX_Chats_OwnerUserID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ChatID",
                table: "ChatUsers");

            migrationBuilder.DropColumn(
                name: "OwnerUserID",
                table: "Chats");

            migrationBuilder.CreateTable(
                name: "ChatChatUser",
                columns: table => new
                {
                    ChatsChatID = table.Column<int>(type: "integer", nullable: false),
                    MembersUserID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatChatUser", x => new { x.ChatsChatID, x.MembersUserID });
                    table.ForeignKey(
                        name: "FK_ChatChatUser_Chats_ChatsChatID",
                        column: x => x.ChatsChatID,
                        principalTable: "Chats",
                        principalColumn: "ChatID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatChatUser_ChatUsers_MembersUserID",
                        column: x => x.MembersUserID,
                        principalTable: "ChatUsers",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatChatUser_MembersUserID",
                table: "ChatChatUser",
                column: "MembersUserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatChatUser");

            migrationBuilder.AddColumn<int>(
                name: "ChatID",
                table: "ChatUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OwnerUserID",
                table: "Chats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatUsers_ChatID",
                table: "ChatUsers",
                column: "ChatID");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_OwnerUserID",
                table: "Chats",
                column: "OwnerUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatUsers_OwnerUserID",
                table: "Chats",
                column: "OwnerUserID",
                principalTable: "ChatUsers",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatUsers_Chats_ChatID",
                table: "ChatUsers",
                column: "ChatID",
                principalTable: "Chats",
                principalColumn: "ChatID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
