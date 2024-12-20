using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTime.DAL.Migrations
{
    public partial class AddSPY : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 517, "SPDR S&P 500 ETF Trust", "SPY", 62774 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 517);
        }
    }
}
