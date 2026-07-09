using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CompanyHub.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntityId",
                table: "auditLogs");

            migrationBuilder.DropColumn(
                name: "EntityName",
                table: "auditLogs");

            migrationBuilder.DropColumn(
                name: "NewValues",
                table: "auditLogs");

            migrationBuilder.RenameColumn(
                name: "Oldvalues",
                table: "auditLogs",
                newName: "Details");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "auditLogs",
                newName: "Oldvalues");

            migrationBuilder.AddColumn<string>(
                name: "EntityId",
                table: "auditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EntityName",
                table: "auditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NewValues",
                table: "auditLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
