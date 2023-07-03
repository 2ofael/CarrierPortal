using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrierPortal.Migrations
{
    public partial class need : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_AspNetUsers_UserID",
                table: "Actors");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "Actors",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Actors_UserID",
                table: "Actors",
                newName: "IX_Actors_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_AspNetUsers_UserId",
                table: "Actors",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Actors_AspNetUsers_UserId",
                table: "Actors");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Actors",
                newName: "UserID");

            migrationBuilder.RenameIndex(
                name: "IX_Actors_UserId",
                table: "Actors",
                newName: "IX_Actors_UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_Actors_AspNetUsers_UserID",
                table: "Actors",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
