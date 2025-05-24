using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarStore.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class EditFavouriteTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddedOn",
                table: "Favorites");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Favorites",
                newName: "IsRemoved");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRemoved",
                table: "Favorites",
                newName: "IsDeleted");

            migrationBuilder.AddColumn<DateTime>(
                name: "AddedOn",
                table: "Favorites",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
