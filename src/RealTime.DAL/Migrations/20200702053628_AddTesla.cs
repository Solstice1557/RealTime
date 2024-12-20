using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTime.DAL.Migrations
{
    public partial class AddTesla : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 506, "Tesla Inc", "TSLA", 4794 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 506);
        }
    }
}
