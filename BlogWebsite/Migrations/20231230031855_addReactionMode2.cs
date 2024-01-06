using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class addReactionMode2 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_Reaction_posts_PostId",
				table: "Reaction");

			migrationBuilder.DropPrimaryKey(
				name: "PK_Reaction",
				table: "Reaction");

			migrationBuilder.RenameTable(
				name: "Reaction",
				newName: "reactions");

			migrationBuilder.RenameIndex(
				name: "IX_Reaction_PostId",
				table: "reactions",
				newName: "IX_reactions_PostId");

			migrationBuilder.AddPrimaryKey(
				name: "PK_reactions",
				table: "reactions",
				column: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_reactions_posts_PostId",
				table: "reactions",
				column: "PostId",
				principalTable: "posts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropForeignKey(
				name: "FK_reactions_posts_PostId",
				table: "reactions");

			migrationBuilder.DropPrimaryKey(
				name: "PK_reactions",
				table: "reactions");

			migrationBuilder.RenameTable(
				name: "reactions",
				newName: "Reaction");

			migrationBuilder.RenameIndex(
				name: "IX_reactions_PostId",
				table: "Reaction",
				newName: "IX_Reaction_PostId");

			migrationBuilder.AddPrimaryKey(
				name: "PK_Reaction",
				table: "Reaction",
				column: "Id");

			migrationBuilder.AddForeignKey(
				name: "FK_Reaction_posts_PostId",
				table: "Reaction",
				column: "PostId",
				principalTable: "posts",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);
		}
	}
}
