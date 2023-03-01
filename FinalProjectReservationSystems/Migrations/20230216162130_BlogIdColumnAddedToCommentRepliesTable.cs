using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinalProjectReservationSystems.Migrations
{
    public partial class BlogIdColumnAddedToCommentRepliesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BlogId",
                table: "CommentReplies",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommentReplies_BlogId",
                table: "CommentReplies",
                column: "BlogId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommentReplies_Blogs_BlogId",
                table: "CommentReplies",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommentReplies_Blogs_BlogId",
                table: "CommentReplies");

            migrationBuilder.DropIndex(
                name: "IX_CommentReplies_BlogId",
                table: "CommentReplies");

            migrationBuilder.DropColumn(
                name: "BlogId",
                table: "CommentReplies");
        }
    }
}
