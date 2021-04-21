using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class MessageWithEditedFlag : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEdited",
                table: "Messages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEdited",
                table: "Messages");
        }
    }
}
