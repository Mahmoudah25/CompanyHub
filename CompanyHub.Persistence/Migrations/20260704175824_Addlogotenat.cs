using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Addlogotenat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoUrl",
                table: "tenants",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoUrl",
                table: "tenants");
        }
    }
}
