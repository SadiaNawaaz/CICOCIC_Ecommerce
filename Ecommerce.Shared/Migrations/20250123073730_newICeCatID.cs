using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class newICeCatID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "IntegratedId",
                table: "Catalogs",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IntegratedId",
                table: "Catalogs");
        }
    }
}
