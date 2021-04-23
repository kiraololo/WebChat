using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChatData.Migrations
{
    public partial class UsersWithLeavedChats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatUserUserID",
                table: "LeavedChats",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LeavedChats_ChatUserUserID",
                table: "LeavedChats",
                column: "ChatUserUserID");

            migrationBuilder.AddForeignKey(
                name: "FK_LeavedChats_ChatUsers_ChatUserUserID",
                table: "LeavedChats",
                column: "ChatUserUserID",
                principalTable: "ChatUsers",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LeavedChats_ChatUsers_ChatUserUserID",
                table: "LeavedChats");

            migrationBuilder.DropIndex(
                name: "IX_LeavedChats_ChatUserUserID",
                table: "LeavedChats");

            migrationBuilder.DropColumn(
                name: "ChatUserUserID",
                table: "LeavedChats");
        }
    }
}
