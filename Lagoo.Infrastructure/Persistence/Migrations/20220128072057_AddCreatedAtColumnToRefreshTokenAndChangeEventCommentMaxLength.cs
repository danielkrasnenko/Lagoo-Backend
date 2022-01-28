using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lagoo.Infrastructure.Persistence.Migrations
{
    public partial class AddCreatedAtColumnToRefreshTokenAndChangeEventCommentMaxLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "RefreshTokens",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "getutcdate()");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Events",
                type: "nvarchar(1024)",
                maxLength: 1024,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1026)",
                oldMaxLength: 1026);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "RefreshTokens");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Events",
                type: "nvarchar(1026)",
                maxLength: 1026,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1024)",
                oldMaxLength: 1024);
        }
    }
}
