using Microsoft.EntityFrameworkCore.Migrations;

namespace Licenser.Server.Infrastructure.Data.Migrations
{
    public partial class DepricateSerialKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SerialKey",
                table: "Licenses");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SerialKey",
                table: "Licenses",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
