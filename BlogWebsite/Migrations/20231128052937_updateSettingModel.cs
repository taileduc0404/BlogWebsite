using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class updateSettingModel : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "FacebookUrl",
				table: "settings");

			migrationBuilder.DropColumn(
				name: "GithubUrl",
				table: "settings");

			migrationBuilder.RenameColumn(
				name: "TwitterUrl",
				table: "settings",
				newName: "ShortDescription");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.RenameColumn(
				name: "ShortDescription",
				table: "settings",
				newName: "TwitterUrl");

			migrationBuilder.AddColumn<string>(
				name: "FacebookUrl",
				table: "settings",
				type: "nvarchar(max)",
				nullable: true);

			migrationBuilder.AddColumn<string>(
				name: "GithubUrl",
				table: "settings",
				type: "nvarchar(max)",
				nullable: true);
		}
	}
}
