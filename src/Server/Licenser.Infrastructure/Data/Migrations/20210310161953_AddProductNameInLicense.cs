using Microsoft.EntityFrameworkCore.Migrations;

namespace Licenser.Infrastructure.Data.Migrations
{
    public partial class AddProductNameInLicense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ObjectName",
                table: "Licenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "Licenses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductName",
                table: "ActivationRequest",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ObjectName",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "Licenses");

            migrationBuilder.DropColumn(
                name: "ProductName",
                table: "ActivationRequest");
        }
    }
}
