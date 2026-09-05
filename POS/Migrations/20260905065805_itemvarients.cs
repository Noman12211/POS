using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace POS.Migrations
{
    /// <inheritdoc />
    public partial class itemvarients : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "FoodItems");

            migrationBuilder.AddColumn<int>(
                name: "FoodItemVariantId",
                table: "InvoiceItems",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VariantName",
                table: "InvoiceItems",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "FoodItemVariant",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FoodItemId = table.Column<int>(type: "INTEGER", nullable: false),
                    VariantName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    IsCustomPrice = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoodItemVariant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoodItemVariant_FoodItems_FoodItemId",
                        column: x => x.FoodItemId,
                        principalTable: "FoodItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceItems_FoodItemVariantId",
                table: "InvoiceItems",
                column: "FoodItemVariantId");

            migrationBuilder.CreateIndex(
                name: "IX_FoodItemVariant_FoodItemId",
                table: "FoodItemVariant",
                column: "FoodItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_InvoiceItems_FoodItemVariant_FoodItemVariantId",
                table: "InvoiceItems",
                column: "FoodItemVariantId",
                principalTable: "FoodItemVariant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InvoiceItems_FoodItemVariant_FoodItemVariantId",
                table: "InvoiceItems");

            migrationBuilder.DropTable(
                name: "FoodItemVariant");

            migrationBuilder.DropIndex(
                name: "IX_InvoiceItems_FoodItemVariantId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "FoodItemVariantId",
                table: "InvoiceItems");

            migrationBuilder.DropColumn(
                name: "VariantName",
                table: "InvoiceItems");

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "FoodItems",
                type: "decimal(18, 2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
