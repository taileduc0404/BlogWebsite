using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class UpdateTopicModel1 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_forumPosts_tags_TagId",
				table: "forumPosts");

			migrationBuilder.DropForeignKey(
				name: "FK_forumPosts_topics_TopicId",
				table: "forumPosts");

			migrationBuilder.DropIndex(
				name: "IX_forumPosts_TagId",
				table: "forumPosts");

			migrationBuilder.DropColumn(
				name: "TagId",
				table: "forumPosts");

			migrationBuilder.AlterColumn<int>(
				name: "TopicId",
				table: "forumPosts",
				type: "int",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_forumPosts_topics_TopicId",
				table: "forumPosts",
				column: "TopicId",
				principalTable: "topics",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_forumPosts_topics_TopicId",
				table: "forumPosts");

			migrationBuilder.AlterColumn<int>(
				name: "TopicId",
				table: "forumPosts",
				type: "int",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");

			migrationBuilder.AddColumn<int>(
				name: "TagId",
				table: "forumPosts",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				name: "IX_forumPosts_TagId",
				table: "forumPosts",
				column: "TagId");

			migrationBuilder.AddForeignKey(
				name: "FK_forumPosts_tags_TagId",
				table: "forumPosts",
				column: "TagId",
				principalTable: "tags",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);

			migrationBuilder.AddForeignKey(
				name: "FK_forumPosts_topics_TopicId",
				table: "forumPosts",
				column: "TopicId",
				principalTable: "topics",
				principalColumn: "Id");
		}
	}
}
