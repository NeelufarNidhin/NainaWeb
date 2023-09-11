using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NainaBoutique.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddedGiftcardfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Giftcards_AspNetUsers_ApplicationUserId",
                table: "Giftcards");

            migrationBuilder.DropIndex(
                name: "IX_Giftcards_ApplicationUserId",
                table: "Giftcards");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Giftcards");

            migrationBuilder.AddColumn<int>(
                name: "UserrId",
                table: "Giftcards",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserrId",
                table: "Giftcards");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Giftcards",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Giftcards_ApplicationUserId",
                table: "Giftcards",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Giftcards_AspNetUsers_ApplicationUserId",
                table: "Giftcards",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
