using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarrierPortal.Migrations
{
    public partial class addedUserToQnAVote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVote_Answers_AnswerId",
                table: "AnswerVote");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionVote_Questions_QuestionId",
                table: "QuestionVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionVote",
                table: "QuestionVote");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerVote",
                table: "AnswerVote");

            migrationBuilder.RenameTable(
                name: "QuestionVote",
                newName: "QuestionVotes");

            migrationBuilder.RenameTable(
                name: "AnswerVote",
                newName: "AnswerVotes");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionVote_QuestionId",
                table: "QuestionVotes",
                newName: "IX_QuestionVotes_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerVote_AnswerId",
                table: "AnswerVotes",
                newName: "IX_AnswerVotes_AnswerId");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "QuestionVotes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AnswerVotes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionVotes",
                table: "QuestionVotes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerVotes",
                table: "AnswerVotes",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionVotes_UserId",
                table: "QuestionVotes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerVotes_UserId",
                table: "AnswerVotes",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVotes_Answers_AnswerId",
                table: "AnswerVotes",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVotes_AspNetUsers_UserId",
                table: "AnswerVotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionVotes_AspNetUsers_UserId",
                table: "QuestionVotes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionVotes_Questions_QuestionId",
                table: "QuestionVotes",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVotes_Answers_AnswerId",
                table: "AnswerVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_AnswerVotes_AspNetUsers_UserId",
                table: "AnswerVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionVotes_AspNetUsers_UserId",
                table: "QuestionVotes");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionVotes_Questions_QuestionId",
                table: "QuestionVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_QuestionVotes",
                table: "QuestionVotes");

            migrationBuilder.DropIndex(
                name: "IX_QuestionVotes_UserId",
                table: "QuestionVotes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnswerVotes",
                table: "AnswerVotes");

            migrationBuilder.DropIndex(
                name: "IX_AnswerVotes_UserId",
                table: "AnswerVotes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "QuestionVotes");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AnswerVotes");

            migrationBuilder.RenameTable(
                name: "QuestionVotes",
                newName: "QuestionVote");

            migrationBuilder.RenameTable(
                name: "AnswerVotes",
                newName: "AnswerVote");

            migrationBuilder.RenameIndex(
                name: "IX_QuestionVotes_QuestionId",
                table: "QuestionVote",
                newName: "IX_QuestionVote_QuestionId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerVotes_AnswerId",
                table: "AnswerVote",
                newName: "IX_AnswerVote_AnswerId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_QuestionVote",
                table: "QuestionVote",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnswerVote",
                table: "AnswerVote",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerVote_Answers_AnswerId",
                table: "AnswerVote",
                column: "AnswerId",
                principalTable: "Answers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionVote_Questions_QuestionId",
                table: "QuestionVote",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
