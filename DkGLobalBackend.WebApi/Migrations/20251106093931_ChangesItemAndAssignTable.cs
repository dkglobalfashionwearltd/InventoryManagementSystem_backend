using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DkGLobalBackend.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class ChangesItemAndAssignTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ItemCondition",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Items");

            migrationBuilder.DropColumn(
                name: "AssignTimeCondition",
                table: "AssignItemUser");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "AssignItemUser",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AssignItemUser");

            migrationBuilder.AddColumn<string>(
                name: "ItemCondition",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Quantity",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Items",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "AssignTimeCondition",
                table: "AssignItemUser",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
