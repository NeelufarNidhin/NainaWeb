using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NainaBoutique.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ModifyAddressTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Address",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Address");
        }
    }
}
