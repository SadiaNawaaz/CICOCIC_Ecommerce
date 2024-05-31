using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Template : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TemplateMasters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TemplateClusters",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TemplateMasterId = table.Column<long>(type: "bigint", nullable: false),
                    ClusterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateClusters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateClusters_TemplateMasters_TemplateMasterId",
                        column: x => x.TemplateMasterId,
                        principalTable: "TemplateMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TemplateClusterFeatures",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FeatureId = table.Column<long>(type: "bigint", nullable: false),
                    TemplateClusterId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TemplateClusterFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TemplateClusterFeatures_Features_FeatureId",
                        column: x => x.FeatureId,
                        principalTable: "Features",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TemplateClusterFeatures_TemplateClusters_TemplateClusterId",
                        column: x => x.TemplateClusterId,
                        principalTable: "TemplateClusters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TemplateClusterFeatures_FeatureId",
                table: "TemplateClusterFeatures",
                column: "FeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateClusterFeatures_TemplateClusterId",
                table: "TemplateClusterFeatures",
                column: "TemplateClusterId");

            migrationBuilder.CreateIndex(
                name: "IX_TemplateClusters_TemplateMasterId",
                table: "TemplateClusters",
                column: "TemplateMasterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TemplateClusterFeatures");

            migrationBuilder.DropTable(
                name: "TemplateClusters");

            migrationBuilder.DropTable(
                name: "TemplateMasters");
        }
    }
}
