using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrierPortal.Migrations
{
    public partial class addedLove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Love",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNameIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BlogId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Love", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Love_BlogPosts_BlogId",
                        column: x => x.BlogId,
                        principalTable: "BlogPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Love_BlogId",
                table: "Love",
                column: "BlogId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Love");
        }
    }
}
