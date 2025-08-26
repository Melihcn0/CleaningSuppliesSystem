using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleaningSuppliesSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCompanyAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddressTitle",
                table: "CompanyAddresses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AddressTitle",
                table: "CompanyAddresses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
