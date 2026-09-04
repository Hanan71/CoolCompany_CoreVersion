using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoolCompanyEstore.Migrations
{
    public partial class AddImageFieldsToCarouselImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImageUrl",
                table: "CarouselItems");

            migrationBuilder.AddColumn<byte[]>(
                name: "BackgroundImage",
                table: "CarouselItems",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageContentType",
                table: "CarouselItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageContentType",
                table: "CarouselImages",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "ImageData",
                table: "CarouselImages",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackgroundImage",
                table: "CarouselItems");

            migrationBuilder.DropColumn(
                name: "BackgroundImageContentType",
                table: "CarouselItems");

            migrationBuilder.DropColumn(
                name: "ImageContentType",
                table: "CarouselImages");

            migrationBuilder.DropColumn(
                name: "ImageData",
                table: "CarouselImages");

            migrationBuilder.AddColumn<string>(
                name: "BackgroundImageUrl",
                table: "CarouselItems",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
