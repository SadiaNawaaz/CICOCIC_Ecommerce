﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Shared.Migrations
{
    /// <inheritdoc />
    public partial class _45 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Categories_CategoryId",
                table: "Catalogs");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Categories_CategoryId",
                table: "Catalogs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_Categories_CategoryId",
                table: "Catalogs");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_Categories_CategoryId",
                table: "Catalogs",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");
        }
    }
}