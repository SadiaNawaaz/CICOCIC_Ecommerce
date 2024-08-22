using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class CatalogsSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Catalogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Slug = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Code = table.Column<long>(type: "bigint", nullable: false),
                    Thumbnail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShortDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CategoryId = table.Column<long>(type: "bigint", nullable: false),
                    BrandId = table.Column<long>(type: "bigint", nullable: true),
                    GeneralSizeId = table.Column<long>(type: "bigint", nullable: true),
                    GeneralColorId = table.Column<long>(type: "bigint", nullable: true),
                    ModelYearId = table.Column<long>(type: "bigint", nullable: true),
                    Media = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Catalogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Catalogs_Brands_BrandId",
                        column: x => x.BrandId,
                        principalTable: "Brands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Catalogs_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Catalogs_Colors_GeneralColorId",
                        column: x => x.GeneralColorId,
                        principalTable: "Colors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Catalogs_ModelYears_ModelYearId",
                        column: x => x.ModelYearId,
                        principalTable: "ModelYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Catalogs_Sizes_GeneralSizeId",
                        column: x => x.GeneralSizeId,
                        principalTable: "Sizes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CatalogCluster",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogId = table.Column<long>(type: "bigint", nullable: false),
                    ClusterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogCluster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogCluster_Catalogs_CatalogId",
                        column: x => x.CatalogId,
                        principalTable: "Catalogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogCluster_Clusters_ClusterId",
                        column: x => x.ClusterId,
                        principalTable: "Clusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CatalogClusterFeature",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CatalogClusterId = table.Column<long>(type: "bigint", nullable: false),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CatalogClusterFeature", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CatalogClusterFeature_CatalogCluster_CatalogClusterId",
                        column: x => x.CatalogClusterId,
                        principalTable: "CatalogCluster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CatalogClusterFeature_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCluster_CatalogId",
                table: "CatalogCluster",
                column: "CatalogId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogCluster_ClusterId",
                table: "CatalogCluster",
                column: "ClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogClusterFeature_CatalogClusterId",
                table: "CatalogClusterFeature",
                column: "CatalogClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_CatalogClusterFeature_FeatureId",
                table: "CatalogClusterFeature",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_BrandId",
                table: "Catalogs",
                column: "BrandId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_CategoryId",
                table: "Catalogs",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_GeneralColorId",
                table: "Catalogs",
                column: "GeneralColorId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_GeneralSizeId",
                table: "Catalogs",
                column: "GeneralSizeId");

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_ModelYearId",
                table: "Catalogs",
                column: "ModelYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CatalogClusterFeature");

            migrationBuilder.DropTable(
                name: "CatalogCluster");

            migrationBuilder.DropTable(
                name: "Catalogs");
        }
    }
}
