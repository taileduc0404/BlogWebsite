using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class AddStatusOnPost3 : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "CountDislike",
				table: "posts",
				newName: "CountDisLike");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "CountDisLike",
				table: "posts",
				newName: "CountDislike");
		}
	}
}
