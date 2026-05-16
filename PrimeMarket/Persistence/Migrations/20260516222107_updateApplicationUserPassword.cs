using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrimeMarket.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateApplicationUserPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019e220e-ff37-7a97-9f65-0a8fa4861efb",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJuH9MVr4WXWr80AbWqhUjonSEqteCFRibCWiraZvfi5L6zKKePg59rzPm21e5/0cg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "019e220e-ff37-7a97-9f65-0a8fa4861efb",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEFLEUDAL0M4ZHNAvTwDJV217wkBDV6BjVTzfd3XQq4oVemb15dQyfn9euXorxzz3ZA==");
        }
    }
}
