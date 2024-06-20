using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Values12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantFeatureValue_ProductVariants_ProductVariantId",
                table: "ProductVariantFeatureValue");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantFeatureValue_TemplateClusterFeatures_TemplateClusterFeatureId",
                table: "ProductVariantFeatureValue");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVariantFeatureValue",
                table: "ProductVariantFeatureValue");

            migrationBuilder.RenameTable(
                name: "ProductVariantFeatureValue",
                newName: "ProductVariantFeatureValues");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantFeatureValue_TemplateClusterFeatureId",
                table: "ProductVariantFeatureValues",
                newName: "IX_ProductVariantFeatureValues_TemplateClusterFeatureId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantFeatureValue_ProductVariantId",
                table: "ProductVariantFeatureValues",
                newName: "IX_ProductVariantFeatureValues_ProductVariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVariantFeatureValues",
                table: "ProductVariantFeatureValues",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantFeatureValues_ProductVariants_ProductVariantId",
                table: "ProductVariantFeatureValues",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantFeatureValues_TemplateClusterFeatures_TemplateClusterFeatureId",
                table: "ProductVariantFeatureValues",
                column: "TemplateClusterFeatureId",
                principalTable: "TemplateClusterFeatures",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantFeatureValues_ProductVariants_ProductVariantId",
                table: "ProductVariantFeatureValues");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductVariantFeatureValues_TemplateClusterFeatures_TemplateClusterFeatureId",
                table: "ProductVariantFeatureValues");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductVariantFeatureValues",
                table: "ProductVariantFeatureValues");

            migrationBuilder.RenameTable(
                name: "ProductVariantFeatureValues",
                newName: "ProductVariantFeatureValue");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantFeatureValues_TemplateClusterFeatureId",
                table: "ProductVariantFeatureValue",
                newName: "IX_ProductVariantFeatureValue_TemplateClusterFeatureId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductVariantFeatureValues_ProductVariantId",
                table: "ProductVariantFeatureValue",
                newName: "IX_ProductVariantFeatureValue_ProductVariantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductVariantFeatureValue",
                table: "ProductVariantFeatureValue",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantFeatureValue_ProductVariants_ProductVariantId",
                table: "ProductVariantFeatureValue",
                column: "ProductVariantId",
                principalTable: "ProductVariants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductVariantFeatureValue_TemplateClusterFeatures_TemplateClusterFeatureId",
                table: "ProductVariantFeatureValue",
                column: "TemplateClusterFeatureId",
                principalTable: "TemplateClusterFeatures",
                principalColumn: "Id");
        }
    }
}
