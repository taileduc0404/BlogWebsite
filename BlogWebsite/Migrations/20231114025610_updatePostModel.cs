using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class updatePostModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_tags_posts_PostId",
				table: "tags");

			migrationBuilder.DropIndex(
				name: "IX_tags_PostId",
				table: "tags");

			migrationBuilder.DropColumn(
				name: "PostId",
				table: "tags");

			migrationBuilder.AddColumn<int>(
				name: "TagId",
				table: "posts",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.CreateIndex(
				name: "IX_posts_TagId",
				table: "posts",
				column: "TagId");

			migrationBuilder.AddForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts",
				column: "TagId",
				principalTable: "tags",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts");

			migrationBuilder.DropIndex(
				name: "IX_posts_TagId",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "TagId",
				table: "posts");

			migrationBuilder.AddColumn<int>(
				name: "PostId",
				table: "tags",
				type: "int",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_tags_PostId",
				table: "tags",
				column: "PostId");

			migrationBuilder.AddForeignKey(
				name: "FK_tags_posts_PostId",
				table: "tags",
				column: "PostId",
				principalTable: "posts",
				principalColumn: "Id");
		}
	}
}
