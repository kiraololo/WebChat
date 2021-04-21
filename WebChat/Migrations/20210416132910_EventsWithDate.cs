using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebChat.Migrations
{
    public partial class EventsWithDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "ChatUserEvents",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EventDate",
                table: "ChatEvents",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "ChatUserEvents");

            migrationBuilder.DropColumn(
                name: "EventDate",
                table: "ChatEvents");
        }
    }
}
