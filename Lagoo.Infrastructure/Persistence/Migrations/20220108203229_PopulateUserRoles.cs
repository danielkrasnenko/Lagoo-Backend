using Lagoo.Domain.Types;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lagoo.Infrastructure.Persistence.Migrations
{
    public partial class PopulateUserRoles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var roleNames = new List<string> { AppUserRole.Admin, AppUserRole.Manager, AppUserRole.User };
            
            foreach (var roleName in roleNames)
            {
                migrationBuilder.Sql($"INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) Values ('{Guid.NewGuid()}', '{roleName}', '{roleName.Normalize().ToUpper()}', '{Guid.NewGuid()}')");
            }
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
