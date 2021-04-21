using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class ChatWithOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerUserID",
                table: "Chats",
                type: "integer",
                nullable: true);

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatUsers_OwnerUserID",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_OwnerUserID",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "OwnerUserID",
                table: "Chats");
        }
    }
}
