using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class kl4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MainImageUrl",
                table: "Sliders",
                newName: "FrontImageUrl");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Sliders",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "StartingFrom",
                table: "Sliders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Sliders");

            migrationBuilder.DropColumn(
                name: "StartingFrom",
                table: "Sliders");

            migrationBuilder.RenameColumn(
                name: "FrontImageUrl",
                table: "Sliders",
                newName: "MainImageUrl");
        }
    }
}
