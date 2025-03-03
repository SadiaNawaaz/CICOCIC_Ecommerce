using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class Clusters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_FeatureTranslations_LanguageId",
                table: "FeatureTranslations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClusterTranslations_LanguageId",
                table: "ClusterTranslations",
                column: "LanguageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClusterTranslations_Languages_LanguageId",
                table: "ClusterTranslations",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FeatureTranslations_Languages_LanguageId",
                table: "FeatureTranslations",
                column: "LanguageId",
                principalTable: "Languages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClusterTranslations_Languages_LanguageId",
                table: "ClusterTranslations");

            migrationBuilder.DropForeignKey(
                name: "FK_FeatureTranslations_Languages_LanguageId",
                table: "FeatureTranslations");

            migrationBuilder.DropIndex(
                name: "IX_FeatureTranslations_LanguageId",
                table: "FeatureTranslations");

            migrationBuilder.DropIndex(
                name: "IX_ClusterTranslations_LanguageId",
                table: "ClusterTranslations");
        }
    }
}
