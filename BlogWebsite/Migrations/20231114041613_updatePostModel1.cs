using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class updatePostModel1 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts");

			migrationBuilder.AlterColumn<int>(
				name: "TagId",
				table: "posts",
				type: "int",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");

			migrationBuilder.AddForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts",
				column: "TagId",
				principalTable: "tags",
				principalColumn: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts");

			migrationBuilder.AlterColumn<int>(
				name: "TagId",
				table: "posts",
				type: "int",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_posts_tags_TagId",
				table: "posts",
				column: "TagId",
				principalTable: "tags",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
