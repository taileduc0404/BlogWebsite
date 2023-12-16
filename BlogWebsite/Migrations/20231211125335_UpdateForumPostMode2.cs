using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class UpdateForumPostMode2 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "AnswerCount",
				table: "forumPosts");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "AnswerCount",
				table: "forumPosts",
				type: "int",
				nullable: false,
				defaultValue: 0);
		}
	}
}
