using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cartify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUseLessList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblInventory_ProductDetailId",
                table: "TblInventory");

            migrationBuilder.CreateIndex(
                name: "IX_TblInventory_ProductDetailId",
                table: "TblInventory",
                column: "ProductDetailId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_TblInventory_ProductDetailId",
                table: "TblInventory");

            migrationBuilder.CreateIndex(
                name: "IX_TblInventory_ProductDetailId",
                table: "TblInventory",
                column: "ProductDetailId");
        }
    }
}
