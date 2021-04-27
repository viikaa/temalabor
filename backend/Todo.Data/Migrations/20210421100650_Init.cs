using Microsoft.EntityFrameworkCore.Migrations;

namespace Todo.DAL.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Columns",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Columns", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Todos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    ColumnId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Todos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Todos_Columns_ColumnId",
                        column: x => x.ColumnId,
                        principalTable: "Columns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Columns",
                columns: new[] { "Id", "Title" },
                values: new object[] { 1, "Első" });

            migrationBuilder.InsertData(
                table: "Columns",
                columns: new[] { "Id", "Title" },
                values: new object[] { 2, "Második" });

            migrationBuilder.InsertData(
                table: "Todos",
                columns: new[] { "Id", "ColumnId", "Description", "Priority", "Title" },
                values: new object[] { 1, 1, "lol", 0, "egyeske" });

            migrationBuilder.InsertData(
                table: "Todos",
                columns: new[] { "Id", "ColumnId", "Description", "Priority", "Title" },
                values: new object[] { 2, 2, "xd", 0, "ketteske" });

            migrationBuilder.CreateIndex(
                name: "IX_Todos_ColumnId",
                table: "Todos",
                column: "ColumnId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Todos");

            migrationBuilder.DropTable(
                name: "Columns");
        }
    }
}
