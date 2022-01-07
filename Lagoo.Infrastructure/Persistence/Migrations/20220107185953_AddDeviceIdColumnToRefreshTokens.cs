using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lagoo.Infrastructure.Persistence.Migrations
{
    public partial class AddDeviceIdColumnToRefreshTokens : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "DeviceId",
                table: "RefreshTokens",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_DeviceId",
                table: "RefreshTokens",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RefreshTokens_DeviceId",
                table: "RefreshTokens");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "RefreshTokens");
        }
    }
}
