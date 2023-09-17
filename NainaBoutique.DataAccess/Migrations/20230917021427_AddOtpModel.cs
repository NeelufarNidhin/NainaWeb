using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NainaBoutique.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RememberMe",
                table: "OtpModels");

            migrationBuilder.RenameColumn(
                name: "OtpCode",
                table: "OtpModels",
                newName: "Otp");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "OtpModels",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "OtpModels");

            migrationBuilder.RenameColumn(
                name: "Otp",
                table: "OtpModels",
                newName: "OtpCode");

            migrationBuilder.AddColumn<bool>(
                name: "RememberMe",
                table: "OtpModels",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
