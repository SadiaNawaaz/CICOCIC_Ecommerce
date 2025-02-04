using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class MarkProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MarkProduct",
                table: "Catalogs",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarkProduct",
                table: "Catalogs");
        }
    }
}
