using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NainaBoutique.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedCartTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_SizeModel_SizeId",
                table: "Carts");

            migrationBuilder.DropTable(
                name: "SizeModel");

            migrationBuilder.DropIndex(
                name: "IX_Carts_SizeId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "SizeId",
                table: "Carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SizeId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "SizeModel",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Size = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SizeModel", x => x.ID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_SizeId",
                table: "Carts",
                column: "SizeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_SizeModel_SizeId",
                table: "Carts",
                column: "SizeId",
                principalTable: "SizeModel",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
