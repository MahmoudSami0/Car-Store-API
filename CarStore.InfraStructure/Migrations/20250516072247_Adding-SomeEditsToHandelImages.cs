using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarStore.InfraStructure.Migrations
{
    /// <inheritdoc />
    public partial class AddingSomeEditsToHandelImages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Picture",
                table: "ModelGalleries");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "ModelGalleries",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "ModelGalleries");

            migrationBuilder.AddColumn<byte[]>(
                name: "Picture",
                table: "ModelGalleries",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
