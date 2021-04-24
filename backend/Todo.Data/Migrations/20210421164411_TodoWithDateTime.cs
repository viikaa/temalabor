using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Todo.DAL.Migrations
{
    public partial class TodoWithDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Todos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "Todos",
                keyColumn: "Id",
                keyValue: 1,
                column: "Deadline",
                value: new DateTime(2021, 4, 21, 18, 44, 10, 125, DateTimeKind.Local).AddTicks(2835));

            migrationBuilder.UpdateData(
                table: "Todos",
                keyColumn: "Id",
                keyValue: 2,
                column: "Deadline",
                value: new DateTime(2021, 4, 21, 18, 44, 10, 130, DateTimeKind.Local).AddTicks(1886));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Todos");
        }
    }
}
