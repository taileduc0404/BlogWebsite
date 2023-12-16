using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class UpdateTopicModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "TopicId",
				table: "forumPosts",
				type: "int",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "topics",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_topics", x => x.Id);
				});

			migrationBuilder.CreateIndex(
				name: "IX_forumPosts_TopicId",
				table: "forumPosts",
				column: "TopicId");

			migrationBuilder.AddForeignKey(
				name: "FK_forumPosts_topics_TopicId",
				table: "forumPosts",
				column: "TopicId",
				principalTable: "topics",
				principalColumn: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_forumPosts_topics_TopicId",
				table: "forumPosts");

			migrationBuilder.DropTable(
				name: "topics");

			migrationBuilder.DropIndex(
				name: "IX_forumPosts_TopicId",
				table: "forumPosts");

			migrationBuilder.DropColumn(
				name: "TopicId",
				table: "forumPosts");
		}
	}
}
