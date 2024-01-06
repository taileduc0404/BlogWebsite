using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogWebsite.Migrations
{
	public partial class deleteLike : Migration
	{
		protected override void Up(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.DropColumn(
				name: "CountDisLike",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "CountLike",
				table: "posts");

			migrationBuilder.DropColumn(
				name: "IsLike",
				table: "posts");
		}

		protected override void Down(MigrationBuilder migrationBuilder)
		{
			migrationBuilder.AddColumn<int>(
				name: "CountDisLike",
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
	}
}
