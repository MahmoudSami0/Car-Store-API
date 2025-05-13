using CarStore.Application.Common.Consts;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarStore.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Roles", 
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { Guid.NewGuid(), Roles.Admin.GetDescription()}
                );
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { Guid.NewGuid(), Roles.Moderator.GetDescription() }
                );
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { Guid.NewGuid(), Roles.Guest.GetDescription() }
                );
            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleName" },
                values: new object[] { Guid.NewGuid(), Roles.User.GetDescription() }
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM Roles");
        }
    }
}
