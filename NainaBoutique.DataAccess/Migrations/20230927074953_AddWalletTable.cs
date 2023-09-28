using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NainaBoutique.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppliedCoupons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CouponId = table.Column<int>(type: "int", nullable: false),
                    AppliedStatus = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppliedCoupons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppliedCoupons_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AppliedCoupons_Coupons_CouponId",
                        column: x => x.CouponId,
                        principalTable: "Coupons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WalletModels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    WalletBalance = table.Column<float>(type: "real", nullable: false),
                    OrderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletModels_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletModels_OrderSummaries_OrderId",
                        column: x => x.OrderId,
                        principalTable: "OrderSummaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppliedCoupons_CouponId",
                table: "AppliedCoupons",
                column: "CouponId");

            migrationBuilder.CreateIndex(
                name: "IX_AppliedCoupons_UserId",
                table: "AppliedCoupons",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletModels_OrderId",
                table: "WalletModels",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletModels_UserId",
                table: "WalletModels",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppliedCoupons");

            migrationBuilder.DropTable(
                name: "WalletModels");
        }
    }
}
