using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrierPortal.Migrations
{
    public partial class addedLoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Votes",
                table: "Actors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ActorLoved",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserNameIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActorId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActorLoved", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActorLoved_Actors_ActorId",
                        column: x => x.ActorId,
                        principalTable: "Actors",
                        principalColumn: "ActorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActorLoved_ActorId",
                table: "ActorLoved",
                column: "ActorId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActorLoved");

            migrationBuilder.DropColumn(
                name: "Votes",
                table: "Actors");
        }
    }
}
