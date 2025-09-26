using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class PvfoRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductVariantFeatureOverrides",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductVariantId = table.Column<long>(type: "bigint", nullable: false),
                    ProductClusterFeatureId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVariantFeatureOverrides", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVariantFeatureOverrides_ProductClusterFeature_ProductClusterFeatureId",
                        column: x => x.ProductClusterFeatureId,
                        principalTable: "ProductClusterFeature",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ProductVariantFeatureOverrides_ProductVariants_ProductVariantId",
                        column: x => x.ProductVariantId,
                        principalTable: "ProductVariants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantFeatureOverrides_ProductClusterFeatureId",
                table: "ProductVariantFeatureOverrides",
                column: "ProductClusterFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVariantFeatureOverrides_ProductVariantId_ProductClusterFeatureId",
                table: "ProductVariantFeatureOverrides",
                columns: new[] { "ProductVariantId", "ProductClusterFeatureId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVariantFeatureOverrides");
        }
    }
}
