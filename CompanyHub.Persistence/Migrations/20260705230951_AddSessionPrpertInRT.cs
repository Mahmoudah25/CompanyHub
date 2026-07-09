using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionPrpertInRT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeviceInfo",
                table: "refreshTokens",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IpAddress",
                table: "refreshTokens",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceInfo",
                table: "refreshTokens");

            migrationBuilder.DropColumn(
                name: "IpAddress",
                table: "refreshTokens");
        }
    }
}
