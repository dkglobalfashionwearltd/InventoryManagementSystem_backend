using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DkGLobalBackend.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class addHistoryandStocktableChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelNumber",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                table: "Stocks",
                newName: "TotalGivenQuantity");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "Stocks",
                newName: "StockCount");

            migrationBuilder.AddColumn<int>(
                name: "CurrentQuantity",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ItemId",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastQuantity",
                table: "Stocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StockOutAt",
                table: "Stocks",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Histories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ActionTitle = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionBysId = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionBysName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Histories", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Stocks_ItemId",
                table: "Stocks",
                column: "ItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Stocks_Items_ItemId",
                table: "Stocks",
                column: "ItemId",
                principalTable: "Items",
                principalColumn: "ItemId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Stocks_Items_ItemId",
                table: "Stocks");

            migrationBuilder.DropTable(
                name: "Histories");

            migrationBuilder.DropIndex(
                name: "IX_Stocks_ItemId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "CurrentQuantity",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "ItemId",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "LastQuantity",
                table: "Stocks");

            migrationBuilder.DropColumn(
                name: "StockOutAt",
                table: "Stocks");

            migrationBuilder.RenameColumn(
                name: "TotalGivenQuantity",
                table: "Stocks",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "StockCount",
                table: "Stocks",
                newName: "DeletedBy");

            migrationBuilder.AddColumn<string>(
                name: "ModelNumber",
                table: "Stocks",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
