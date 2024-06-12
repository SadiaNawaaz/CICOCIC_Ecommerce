using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class VariantsChange12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ModelYears_ModelYearId",
                table: "ProductVariants");

            migrationBuilder.AlterColumn<long>(
                name: "SizeId",
                table: "ProductVariants",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ModelYearId",
                table: "ProductVariants",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ColorId",
                table: "ProductVariants",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ModelYears_ModelYearId",
                table: "ProductVariants",
                column: "ModelYearId",
                principalTable: "ModelYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariants_ModelYears_ModelYearId",
                table: "ProductVariants");

            migrationBuilder.AlterColumn<long>(
                name: "SizeId",
                table: "ProductVariants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ModelYearId",
                table: "ProductVariants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ColorId",
                table: "ProductVariants",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariants_ModelYears_ModelYearId",
                table: "ProductVariants",
                column: "ModelYearId",
                principalTable: "ModelYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
