using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class ChatWithCrDateAndOwner : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Chats",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

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
                name: "Created",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "OwnerUserID",
                table: "Chats");
        }
    }
}
