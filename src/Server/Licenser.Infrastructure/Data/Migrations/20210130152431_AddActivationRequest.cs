using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Licenser.Infrastructure.Data.Migrations
{
    public partial class AddActivationRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivationRequest",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActivationId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestedClientId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivationRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ActivationRequest_AspNetUsers_RequestedClientId",
                        column: x => x.RequestedClientId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivationRequest_RequestedClientId",
                table: "ActivationRequest",
                column: "RequestedClientId",
                unique: true,
                filter: "[RequestedClientId] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivationRequest");
        }
    }
}
