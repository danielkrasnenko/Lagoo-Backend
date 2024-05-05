using Lagoo.Domain.Types;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Lagoo.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class PopulateAppRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql($@"
            INSERT INTO asp_net_roles
                (id, name, normalized_name, concurrency_stamp) VALUES
                ('00000000-0000-0000-0000-000000000001', '{AppUserRole.Admin}', '{AppUserRole.Admin.Normalize().ToUpperInvariant()}', '{Guid.NewGuid()}'),
                ('00000000-0000-0000-0000-000000000002', '{AppUserRole.Manager}', '{AppUserRole.Manager.Normalize().ToUpperInvariant()}', '{Guid.NewGuid()}'),
                ('00000000-0000-0000-0000-000000000003', '{AppUserRole.User}', '{AppUserRole.User.Normalize().ToUpperInvariant()}', '{Guid.NewGuid()}');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DELETE FROM asp_net_roles WHERE id IN
            ('00000000-0000-0000-0000-000000000001',
             '00000000-0000-0000-0000-000000000002',
             '00000000-0000-0000-0000-000000000003');
            ");
        }
    }
}
