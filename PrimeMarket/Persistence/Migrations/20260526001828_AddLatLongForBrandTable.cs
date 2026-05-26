using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeMarket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddLatLongForBrandTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Brands",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Brands",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Brands");
        }
    }
}
