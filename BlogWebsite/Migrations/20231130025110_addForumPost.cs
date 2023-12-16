using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class addForumPost : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "ForumPostId",
				table: "comments",
				type: "int",
				nullable: true);

			migrationBuilder.CreateTable(
				name: "forumPosts",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
					Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
					CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
					ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
					ViewCount = table.Column<int>(type: "int", nullable: false),
					CommentCount = table.Column<int>(type: "int", nullable: false),
					TagId = table.Column<int>(type: "int", nullable: false),
					Slug = table.Column<string>(type: "nvarchar(max)", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_forumPosts", x => x.Id);
					table.ForeignKey(
						name: "FK_forumPosts_AspNetUsers_ApplicationUserId",
						column: x => x.ApplicationUserId,
						principalTable: "AspNetUsers",
						principalColumn: "Id");
					table.ForeignKey(
						name: "FK_forumPosts_tags_TagId",
						column: x => x.TagId,
						principalTable: "tags",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_comments_ForumPostId",
				table: "comments",
				column: "ForumPostId");

			migrationBuilder.CreateIndex(
				name: "IX_forumPosts_ApplicationUserId",
				table: "forumPosts",
				column: "ApplicationUserId");

			migrationBuilder.CreateIndex(
				name: "IX_forumPosts_TagId",
				table: "forumPosts",
				column: "TagId");

			migrationBuilder.AddForeignKey(
				name: "FK_comments_forumPosts_ForumPostId",
				table: "comments",
				column: "ForumPostId",
				principalTable: "forumPosts",
				principalColumn: "Id");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_comments_forumPosts_ForumPostId",
				table: "comments");

			migrationBuilder.DropTable(
				name: "forumPosts");

			migrationBuilder.DropIndex(
				name: "IX_comments_ForumPostId",
				table: "comments");

			migrationBuilder.DropColumn(
				name: "ForumPostId",
				table: "comments");
		}
	}
}
