using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolCompanyEstore.Migrations
{
    public partial class Add_ButtonLabel_And_ShortParagraph_To_CarouselImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ButtonLabel",
                table: "CarouselImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShortParagraph",
                table: "CarouselImages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonLabel",
                table: "CarouselImages");

            migrationBuilder.DropColumn(
                name: "ShortParagraph",
                table: "CarouselImages");
        }
    }
}
