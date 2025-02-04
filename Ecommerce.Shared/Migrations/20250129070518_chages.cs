using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class chages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductVariantMedias",
                newName: "MediaUrl");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "ProductMedias",
                newName: "MediaUrl");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductVariantMedias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "ProductMedias",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductVariantMedias");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "ProductMedias");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "ProductVariantMedias",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "MediaUrl",
                table: "ProductMedias",
                newName: "ImageUrl");
        }
    }
}
