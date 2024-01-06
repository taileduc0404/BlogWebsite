using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class AddStatusOnPost : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "CountDislike",
				table: "posts",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<int>(
				name: "CountLike",
				table: "posts",
				type: "int",
				nullable: false,
				defaultValue: 0);

			migrationBuilder.AddColumn<bool>(
				name: "IsLike",
				table: "posts",
				type: "bit",
				nullable: false,
				defaultValue: false);
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CountDislike",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "CountLike",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "IsLike",
				table: "posts");
		}
	}
}
