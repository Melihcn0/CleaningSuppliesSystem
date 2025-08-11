using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleaningSuppliesSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class added_category_entities_ımageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TopCategoryId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_TopCategoryId",
                table: "Categories",
                column: "TopCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_TopCategories_TopCategoryId",
                table: "Categories",
                column: "TopCategoryId",
                principalTable: "TopCategories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_TopCategories_TopCategoryId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_TopCategoryId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "TopCategoryId",
                table: "Categories");
        }
    }
}
