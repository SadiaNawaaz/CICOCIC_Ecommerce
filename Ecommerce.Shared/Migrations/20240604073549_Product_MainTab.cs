using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Product_MainTab : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TemplateCategories_TemplateMasterId",
                table: "TemplateCategories");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateMasterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Products_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_TemplateMasters_TemplateMasterId",
                        column: x => x.TemplateMasterId,
                        principalTable: "TemplateMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductFeatureValues",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateClusterFeatureId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFeatureValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductFeatureValues_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductFeatureValues_TemplateClusterFeatures_TemplateClusterFeatureId",
                        column: x => x.TemplateClusterFeatureId,
                        principalTable: "TemplateClusterFeatures",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateCategories_TemplateMasterId_CategoryId",
                table: "TemplateCategories",
                columns: new[] { "TemplateMasterId", "CategoryId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureValues_ProductId",
                table: "ProductFeatureValues",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductFeatureValues_TemplateClusterFeatureId",
                table: "ProductFeatureValues",
                column: "TemplateClusterFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CategoryId",
                table: "Products",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TemplateMasterId",
                table: "Products",
                column: "TemplateMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductFeatureValues");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropIndex(
                name: "IX_TemplateCategories_TemplateMasterId_CategoryId",
                table: "TemplateCategories");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateCategories_TemplateMasterId",
                table: "TemplateCategories",
                column: "TemplateMasterId");
        }
    }
}
