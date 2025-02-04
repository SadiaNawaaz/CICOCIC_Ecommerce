using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Done : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EanNumber",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EanNumber",
                table: "Products");
        }
    }
}
