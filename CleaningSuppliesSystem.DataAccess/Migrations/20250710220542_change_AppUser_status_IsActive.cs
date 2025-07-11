using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleaningSuppliesSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class change_AppUser_status_IsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Status",
                table: "AspNetUsers",
                newName: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsActive",
                table: "AspNetUsers",
                newName: "Status");
        }
    }
}
