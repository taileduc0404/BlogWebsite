using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class addReactionModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "Reaction",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					PostId = table.Column<int>(type: "int", nullable: false),
					UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
					IsLike = table.Column<bool>(type: "bit", nullable: false)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_Reaction", x => x.Id);
					table.ForeignKey(
						name: "FK_Reaction_posts_PostId",
						column: x => x.PostId,
						principalTable: "posts",
						principalColumn: "Id",
						onDelete: ReferentialAction.Cascade);
				});

			migrationBuilder.CreateIndex(
				name: "IX_Reaction_PostId",
				table: "Reaction",
				column: "PostId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "Reaction");
		}
	}
}
