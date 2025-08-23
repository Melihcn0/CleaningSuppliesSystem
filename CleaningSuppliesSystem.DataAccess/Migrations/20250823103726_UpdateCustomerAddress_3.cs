using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleaningSuppliesSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCustomerAddress_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "District",
                table: "CustomerIndividualAddresses",
                newName: "DistrictName");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "CustomerIndividualAddresses",
                newName: "CityName");

            migrationBuilder.RenameColumn(
                name: "District",
                table: "CustomerCorporateAddresses",
                newName: "DistrictName");

            migrationBuilder.RenameColumn(
                name: "City",
                table: "CustomerCorporateAddresses",
                newName: "CityName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DistrictName",
                table: "CustomerIndividualAddresses",
                newName: "District");

            migrationBuilder.RenameColumn(
                name: "CityName",
                table: "CustomerIndividualAddresses",
                newName: "City");

            migrationBuilder.RenameColumn(
                name: "DistrictName",
                table: "CustomerCorporateAddresses",
                newName: "District");

            migrationBuilder.RenameColumn(
                name: "CityName",
                table: "CustomerCorporateAddresses",
                newName: "City");
        }
    }
}
