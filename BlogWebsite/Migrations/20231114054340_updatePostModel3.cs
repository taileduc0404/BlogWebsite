using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class updatePostModel3 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
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
		}
	}
}
