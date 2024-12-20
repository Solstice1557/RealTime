using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTime.DAL.Migrations
{
    public partial class NewFunds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 507, "Masimo Corporation", "MASI", 137 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 508, "DexCom, Inc.", "DXCM", 1105 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 509, "ServiceNow Inc.", "NOW", 172 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 510, "Shopify Inc.", "SHOP", 1691 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 511, "Wix.com Ltd.", "WIX", 637 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 512, "Okta, Inc.", "OKTA", 1165 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 513, "Veeva Systems Inc.", "VEEV", 627 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 514, "The Trade Desk, Inc.", "TTD", 954 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 515, "Zoom Video Communications, Inc.", "ZM", 634 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 516, "RingCentral, Inc.", "RNG", 1228 });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 507);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 508);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 509);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 510);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 511);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 512);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 513);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 514);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 515);

            migrationBuilder.DeleteData(
                table: "Funds",
                keyColumn: "FundId",
                keyValue: 516);
        }
    }
}
