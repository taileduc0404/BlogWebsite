using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class RemoveRoleReader : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Name = 'Reader'");
			migrationBuilder.DropColumn(
				name: "Tag",
				table: "posts");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<string>(
				name: "Tag",
				table: "posts",
				type: "nvarchar(max)",
				nullable: true);
		}
	}
}
