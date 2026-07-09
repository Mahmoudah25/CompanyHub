using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpadateUsage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "feature",
                table: "UsageRecord");

            migrationBuilder.RenameColumn(
                name: "date",
                table: "UsageRecord",
                newName: "LastUpdated");

            migrationBuilder.RenameColumn(
                name: "Count",
                table: "UsageRecord",
                newName: "UsersCount");

            migrationBuilder.AddColumn<int>(
                name: "RolesCount",
                table: "UsageRecord",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "StorageUsedGb",
                table: "UsageRecord",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RolesCount",
                table: "UsageRecord");

            migrationBuilder.DropColumn(
                name: "StorageUsedGb",
                table: "UsageRecord");

            migrationBuilder.RenameColumn(
                name: "UsersCount",
                table: "UsageRecord",
                newName: "Count");

            migrationBuilder.RenameColumn(
                name: "LastUpdated",
                table: "UsageRecord",
                newName: "date");

            migrationBuilder.AddColumn<string>(
                name: "feature",
                table: "UsageRecord",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
