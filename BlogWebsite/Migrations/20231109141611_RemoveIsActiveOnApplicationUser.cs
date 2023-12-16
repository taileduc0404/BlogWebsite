using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class RemoveIsActiveOnApplicationUser : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "IsActive",
				table: "AspNetUsers");

		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{

		}
	}
}
