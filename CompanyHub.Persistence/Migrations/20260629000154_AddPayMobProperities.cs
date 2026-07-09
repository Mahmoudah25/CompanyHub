using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPayMobProperities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentUrl",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymobOrderId",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymobTransactionId",
                table: "payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentUrl",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "PaymobOrderId",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "PaymobTransactionId",
                table: "payments");
        }
    }
}
