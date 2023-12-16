using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class AddTopicModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_comments_posts_PostId",
				table: "comments");

			migrationBuilder.AlterColumn<int>(
				name: "PostId",
				table: "comments",
				type: "int",
				nullable: true,
				oldClrType: typeof(int),
				oldType: "int");

			migrationBuilder.AddForeignKey(
				name: "FK_comments_posts_PostId",
				table: "comments",
				column: "PostId",
				principalTable: "posts",
				principalColumn: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_comments_posts_PostId",
				table: "comments");

			migrationBuilder.AlterColumn<int>(
				name: "PostId",
				table: "comments",
				type: "int",
				nullable: false,
				defaultValue: 0,
				oldClrType: typeof(int),
				oldType: "int",
				oldNullable: true);

			migrationBuilder.AddForeignKey(
				name: "FK_comments_posts_PostId",
				table: "comments",
				column: "PostId",
				principalTable: "posts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
