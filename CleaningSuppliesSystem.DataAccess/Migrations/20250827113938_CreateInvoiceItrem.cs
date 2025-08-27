using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CleaningSuppliesSystem.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class CreateInvoiceItrem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "TaxOffice",
                table: "Invoices",
                newName: "CustomerTaxOffice");

            migrationBuilder.RenameColumn(
                name: "TaxNumber",
                table: "Invoices",
                newName: "CustomerTaxNumber");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Invoices",
                newName: "CustomerPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "NationalId",
                table: "Invoices",
                newName: "CustomerNationalId");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Invoices",
                newName: "CustomerLastName");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Invoices",
                newName: "CustomerFirstName");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Invoices",
                newName: "CustomerCompanyName");

            migrationBuilder.RenameColumn(
                name: "DistrictName",
                table: "Invoices",
                newName: "InvoiceCompanyTaxOffice");

            migrationBuilder.RenameColumn(
                name: "CityName",
                table: "Invoices",
                newName: "InvoiceCompanyTaxNumber");

            migrationBuilder.RenameColumn(
                name: "AddressTitle",
                table: "Invoices",
                newName: "InvoiceCompanyName");

            migrationBuilder.RenameColumn(
                name: "Address",
                table: "Invoices",
                newName: "InvoiceCompanyDistrictName");

            migrationBuilder.AddColumn<string>(
                name: "AdminFirstName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "AdminId",
                table: "Invoices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AdminLastName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AdminPhoneNumber",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddress",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerAddressTitle",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerCityName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CustomerDistrictName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyAddress",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InvoiceCompanyCityName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "InvoiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceItems_Invoices_InvoiceId",
                        column: x => x.InvoiceId,
                        principalTable: "Invoices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_InvoiceId",
                table: "InvoiceItems",
                column: "InvoiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "AdminFirstName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AdminLastName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "AdminPhoneNumber",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerAddress",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerAddressTitle",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerCityName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "CustomerDistrictName",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceCompanyAddress",
                table: "Invoices");

            migrationBuilder.DropColumn(
                name: "InvoiceCompanyCityName",
                table: "Invoices");

            migrationBuilder.RenameColumn(
                name: "InvoiceCompanyTaxOffice",
                table: "Invoices",
                newName: "DistrictName");

            migrationBuilder.RenameColumn(
                name: "InvoiceCompanyTaxNumber",
                table: "Invoices",
                newName: "CityName");

            migrationBuilder.RenameColumn(
                name: "InvoiceCompanyName",
                table: "Invoices",
                newName: "AddressTitle");

            migrationBuilder.RenameColumn(
                name: "InvoiceCompanyDistrictName",
                table: "Invoices",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "CustomerTaxOffice",
                table: "Invoices",
                newName: "TaxOffice");

            migrationBuilder.RenameColumn(
                name: "CustomerTaxNumber",
                table: "Invoices",
                newName: "TaxNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerPhoneNumber",
                table: "Invoices",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "CustomerNationalId",
                table: "Invoices",
                newName: "NationalId");

            migrationBuilder.RenameColumn(
                name: "CustomerLastName",
                table: "Invoices",
                newName: "LastName");

            migrationBuilder.RenameColumn(
                name: "CustomerFirstName",
                table: "Invoices",
                newName: "FirstName");

            migrationBuilder.RenameColumn(
                name: "CustomerCompanyName",
                table: "Invoices",
                newName: "Email");

            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
