using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lagoo.Infrastructure.Persistence.Migrations
{
    public partial class CreateEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Type = table.Column<byte>(type: "tinyint", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(1026)", maxLength: 1026, nullable: true),
                    Duration = table.Column<TimeSpan>(type: "time", nullable: false),
                    BeginsAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "getutcdate()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Events");
        }
    }
}
