using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace alposim.Migrations
{
    /// <inheritdoc />
    public partial class RepositoryUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SaleCode",
                table: "Sales",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Products",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SaleCode",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Products");
        }
    }
}
