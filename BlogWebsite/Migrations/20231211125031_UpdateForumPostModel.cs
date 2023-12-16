using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class UpdateForumPostModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "CommentCount",
				table: "forumPosts",
				newName: "AnswerCount");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "AnswerCount",
				table: "forumPosts",
				newName: "CommentCount");
		}
	}
}
