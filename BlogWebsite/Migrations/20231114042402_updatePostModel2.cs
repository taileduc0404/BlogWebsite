using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class updatePostModel2 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts");

			migrationBuilder.DropIndex(
				name: "IX_posts_TagId",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "ShortDescription",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "TagId",
				table: "posts");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "ShortDescription",
				table: "posts",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<int>(
				name: "TagId",
				table: "posts",
				type: "int",
				nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_posts_TagId",
				table: "posts",
				column: "TagId");

			migrationBuilder.AddForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts",
				column: "TagId",
				principalTable: "tags",
				principalColumn: "Id");
		}
	}
}
