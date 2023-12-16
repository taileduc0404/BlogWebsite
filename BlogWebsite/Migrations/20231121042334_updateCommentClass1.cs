using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class updateCommentClass1 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "ParentCommentId",
				table: "comments",
				type: "int",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_comments_ParentCommentId",
				table: "comments",
				column: "ParentCommentId");

			migrationBuilder.AddForeignKey(
				name: "FK_comments_comments_ParentCommentId",
				table: "comments",
				column: "ParentCommentId",
				principalTable: "comments",
				principalColumn: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_comments_comments_ParentCommentId",
				table: "comments");

			migrationBuilder.DropIndex(
				name: "IX_comments_ParentCommentId",
				table: "comments");

			migrationBuilder.DropColumn(
				name: "ParentCommentId",
				table: "comments");
		}
	}
}
