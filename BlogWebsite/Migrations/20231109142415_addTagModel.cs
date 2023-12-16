using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class addTagModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.CreateTable(
				name: "tags",
				columns: table => new
				{
					Id = table.Column<int>(type: "int", nullable: false)
						.Annotation("SqlServer:Identity", "1, 1"),
					Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
					PostId = table.Column<int>(type: "int", nullable: true)
				},
				constraints: table =>
				{
					table.PrimaryKey("PK_tags", x => x.Id);
					table.ForeignKey(
						name: "FK_tags_posts_PostId",
						column: x => x.PostId,
						principalTable: "posts",
						principalColumn: "Id");
				});

			migrationBuilder.CreateIndex(
				name: "IX_tags_PostId",
				table: "tags",
				column: "PostId");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropTable(
				name: "tags");

			migrationBuilder.AddColumn<bool>(
				name: "IsActive",
				table: "AspNetUsers",
				type: "bit",
				nullable: true);
		}
	}
}
