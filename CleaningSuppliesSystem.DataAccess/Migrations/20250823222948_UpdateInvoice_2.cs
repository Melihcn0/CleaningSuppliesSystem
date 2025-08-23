using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleaningSuppliesSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateInvoice_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApartmentNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "BuildingNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "City",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "District",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "Street",
                table: "Invoices",
                newName: "InvoiceType");

            migrationBuilder.RenameColumn(
                name: "RecipientName",
                table: "Invoices",
                newName: "DistrictName");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Invoices",
                newName: "CityName");

            migrationBuilder.RenameColumn(
                name: "Neighborhood",
                table: "Invoices",
                newName: "Address");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NationalId",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxOffice",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "NationalId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "TaxOffice",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "InvoiceType",
                table: "Invoices",
                newName: "Street");

            migrationBuilder.RenameColumn(
                name: "DistrictName",
                table: "Invoices",
                newName: "RecipientName");

            migrationBuilder.RenameColumn(
                name: "CityName",
                table: "Invoices",
                newName: "PostalCode");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Invoices",
                newName: "Neighborhood");

            migrationBuilder.AddColumn<string>(
                name: "ApartmentNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BuildingNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
