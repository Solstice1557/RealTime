using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace RealTime.DAL.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Funds",
                columns: table => new
                {
                    FundId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Symbol = table.Column<string>(maxLength: 50, nullable: true),
                    Name = table.Column<string>(maxLength: 300, nullable: true),
                    Volume = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Funds", x => x.FundId);
                });

            migrationBuilder.CreateTable(
                name: "DailyPrices",
                columns: table => new
                {
                    DailyPriceId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FundId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Open = table.Column<decimal>(nullable: true),
                    Close = table.Column<decimal>(nullable: true),
                    High = table.Column<decimal>(nullable: true),
                    Low = table.Column<decimal>(nullable: true),
                    Volume = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyPrices", x => x.DailyPriceId);
                    table.ForeignKey(
                        name: "FK_DailyPrices_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Prices",
                columns: table => new
                {
                    PriceId = table.Column<long>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FundId = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTime>(nullable: false),
                    Open = table.Column<decimal>(nullable: true),
                    Close = table.Column<decimal>(nullable: true),
                    High = table.Column<decimal>(nullable: true),
                    Low = table.Column<decimal>(nullable: true),
                    Volume = table.Column<decimal>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Prices", x => x.PriceId);
                    table.ForeignKey(
                        name: "FK_Prices_Funds_FundId",
                        column: x => x.FundId,
                        principalTable: "Funds",
                        principalColumn: "FundId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 1, "Apple Inc", "AAPL", 1163246 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 344, "NVR Inc", "NVR", 13251 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 343, "Conagra Brands Inc", "CAG", 13373 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 342, "Waters Corp", "WAT", 13484 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 341, "Take-Two Interactive Software Inc", "TTWO", 13523 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 340, "Broadridge Financial Solutions Inc", "BR", 13533 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 339, "Quest Diagnostics Inc", "DGX", 13551 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 338, "Xylem Inc", "XYL", 13646 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 337, "MarketAxess Holdings Inc", "MKTX", 13648 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 336, "Extra Space Storage Inc", "EXR", 13685 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 335, "Akamai Technologies Inc", "AKAM", 13815 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 334, "Campbell Soup Co", "CPB", 13893 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 333, "UDR Inc", "UDR", 13899 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 332, "CenterPoint Energy Inc", "CNP", 13917 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 331, "Franklin Resources Inc", "BEN", 13925 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 330, "Darden Restaurants Inc", "DRI", 13993 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 329, "International Flavors & Fragrances Inc", "IFF", 14013 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 328, "NetApp Inc", "NTAP", 14231 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 327, "Ulta Beauty Inc", "ULTA", 14256 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 326, "Gartner Inc", "IT", 14302 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 325, "Evergy Inc", "EVRG", 14312 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 324, "Expedia Group Inc", "EXPE", 14339 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 323, "The Cooper Companies Inc", "COO", 14475 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 322, "Citrix Systems Inc", "CTXS", 14503 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 321, "CBS Corp", "CBS", 14598 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 320, "Arista Networks Inc", "ANET", 14608 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 319, "Loews Corp", "L", 15002 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 318, "Discovery Inc", "DISCK", 15045 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 345, "Masco Corp", "MAS", 13202 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 346, "Wynn Resorts Ltd", "WYNN", 13066 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 347, "Expeditors International of Washington Inc", "EXPD", 13036 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 348, "Honeywell International Inc", "HON", 12933 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 376, "Lamb Weston Holdings Inc", "LW", 11722 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 375, "United Rentals Inc", "URI", 11746 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 374, "Qorvo Inc", "QRVO", 11861 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 373, "The AES Corp", "AES", 11883 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 372, "JM Smucker Co", "SJM", 11897 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 371, "Lincoln National Corp", "LNC", 11955 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 370, "Hasbro Inc", "HAS", 12043 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 369, "Advance Auto Parts Inc", "AAP", 12046 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 368, "Hologic Inc", "HOLX", 12167 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 367, "Host Hotels & Resorts Inc", "HST", 12177 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 366, "SVB Financial Group", "SIVB", 12204 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 365, "IDEX Corp", "IEX", 12218 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 364, "Raymond James Financial Inc", "RJF", 12275 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 317, "WellCare Health Plans Inc", "WCG", 15069 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 363, "Diamondback Energy Inc", "FANG", 12304 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 361, "Dentsply Sirona Inc", "XRAY", 12386 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 360, "Leidos Holdings Inc", "LDOS", 12386 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 359, "Alliant Energy Corp", "LNT", 12406 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 358, "Vornado Realty Trust", "VNO", 12525 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 357, "Rollins Inc", "ROL", 12544 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 356, "JB Hunt Transport Services Inc", "JBHT", 12606 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 355, "AbbVie Inc", "ABBV", 12638 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 354, "Cboe Global Markets Inc", "CBOE", 12699 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 353, "Atmos Energy Corp", "ATO", 12726 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 352, "Arconic Inc", "ARNC", 12758 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 351, "Duke Realty Corp", "DRE", 12773 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 350, "FMC Corp", "FMC", 12773 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 349, "Jacobs Engineering Group Inc", "JEC", 12854 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 362, "Universal Health Services Inc", "UHS", 12338 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 377, "Molson Coors Brewing Co", "TAP", 11532 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 316, "Westinghouse Air Brake Technologies Corp", "WAB", 15075 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 314, "NortonLifeLock Inc", "NLOK", 15163 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 281, "AmerisourceBergen Corp", "ABC", 17491 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 280, "W.W. Grainger Inc", "GWW", 17599 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 279, "DISH Network Corp", "DISH", 17774 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 278, "Cincinnati Financial Corp", "CINF", 17814 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 277, "Alexandria Real Estate Equities Inc", "ARE", 17856 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 276, "International Paper Co", "IP", 18053 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 275, "Vulcan Materials Co", "VMC", 18061 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 274, "Incyte Corp", "INCY", 18222 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 273, "Garmin Ltd", "GRMN", 18398 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 272, "Clorox Co", "CLX", 18422 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 271, "CBRE Group Inc", "CBRE", 18438 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 270, "Halliburton Co", "HAL", 18583 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 269, "Lennar Corp", "LEN", 18688 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 268, "Ameren Corp", "AEE", 18775 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 267, "Cadence Design Systems Inc", "CDNS", 18917 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 266, "Copart Inc", "CPRT", 19234 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 265, "CDW Corp", "CDW", 19235 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 264, "D.R. Horton Inc", "DHI", 19371 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 263, "Ansys Inc", "ANSS", 19424 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 262, "Keysight Technologies Inc", "KEYS", 19789 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 261, "Corteva Inc", "CTVA", 19959 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 260, "Best Buy Co Inc", "BBY", 20105 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 259, "Align Technology Inc", "ALGN", 20383 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 258, "Chipotle Mexican Grill Inc", "CMG", 20617 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 257, "Synopsys Inc", "SNPS", 20651 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 256, "ResMed Inc", "RMD", 20662 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 255, "MSCI Inc", "MSCI", 20704 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 282, "Omnicom Group Inc", "OMC", 17259 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 283, "Mettler-Toledo International Inc", "MTD", 17229 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 284, "Skyworks Solutions Inc", "SWKS", 17208 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 285, "Arthur J. Gallagher & Co", "AJG", 17027 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 313, "Mid-America Apartment Communities Inc", "MAA", 15169 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 312, "Kansas City Southern", "KSU", 15184 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 311, "Tiffany & Co", "TIF", 15227 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 310, "Celanese Corp", "CE", 15302 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 309, "Seagate Technology PLC", "STX", 15339 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 308, "Huntington Bancshares Inc", "HBAN", 15367 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 307, "Genuine Parts Co", "GPC", 15408 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 306, "CarMax Inc", "KMX", 15438 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 305, "Western Digital Corp", "WDC", 15472 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 304, "Cardinal Health Inc", "CAH", 15648 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 303, "Amcor PLC", "AMCR", 15844 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 302, "Dover Corp", "DOV", 15916 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 301, "Martin Marietta Materials Inc", "MLM", 15947 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 315, "Teleflex Inc", "TFX", 15131 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 300, "Maxim Integrated Products Inc", "MXIM", 15971 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 298, "Freeport-McMoRan Inc", "FCX", 16105 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 297, "Regions Financial Corp", "RF", 16157 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 296, "MGM Resorts International", "MGM", 16211 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 295, "Discovery Inc", "DISCA", 16221 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 294, "Equifax Inc", "EFX", 16334 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 293, "Fortinet Inc", "FTNT", 16439 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 292, "CenturyLink Inc", "CTL", 16527 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 291, "Church & Dwight Co Inc", "CHD", 16584 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 290, "Healthpeak Properties Inc", "PEAK", 16595 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 289, "Citizens Financial Group Inc", "CFG", 16654 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 288, "Nasdaq Inc", "NDAQ", 16761 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 287, "Nucor Corp", "NUE", 16798 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 286, "CMS Energy Corp", "CMS", 16919 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 299, "Laboratory Corp of America Holdings", "LH", 16071 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 378, "The Western Union Co", "WU", 11445 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 379, "Jack Henry & Associates Inc", "JKHY", 11414 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 380, "Varian Medical Systems Inc", "VAR", 11407 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 471, "Deere & Co", "DE", 5662 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 470, "CSX Corp", "CSX", 5739 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 469, "Hanesbrands Inc", "HBI", 5801 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 468, "Nordstrom Inc", "JWN", 5802 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 467, "Harley-Davidson Inc", "HOG", 5932 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 466, "Quanta Services Inc", "PWR", 6078 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 465, "Sealed Air Corp", "SEE", 6102 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 464, "Unum Group", "UNM", 6155 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 463, "Flowserve Corp", "FLS", 6404 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 462, "Gap Inc", "GPS", 6425 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 461, "Becton, Dickinson and Co", "BDX", 6542 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 460, "Dominion Energy Inc", "D", 6551 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 459, "PNC Financial Services Group Inc", "PNC", 6668 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 458, "Perrigo Co PLC", "PRGO", 6677 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 457, "Intuit Inc", "INTU", 6707 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 456, "Robert Half International Inc", "RHI", 6818 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 455, "Chubb Ltd", "CB", 6851 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 454, "SL Green Realty Corp", "SLG", 6874 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 453, "Leggett & Platt Inc", "LEG", 7143 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 452, "FLIR Systems Inc", "FLIR", 7154 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 451, "Under Armour Inc", "UA", 7161 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 450, "PVH Corp", "PVH", 7165 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 449, "Albemarle Corp", "ALB", 7167 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 448, "Pentair PLC", "PNR", 7254 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 447, "Cabot Oil & Gas Corp", "COG", 7256 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 446, "People's United Financial Inc", "PBCT", 7334 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 445, "Tapestry Inc", "TPR", 7361 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 472, "Walgreens Boots Alliance Inc", "WBA", 5531 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 473, "Prologis Inc", "PLD", 5524 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 474, "Macy's Inc", "M", 4973 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 475, "Alliance Data Systems Corp", "ADS", 4961 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 503, "L3Harris Technologies Inc", "LHX", 431 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 502, "Capri Holdings Ltd", "CPRI", 551 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 501, "Nielsen Holdings PLC", "NLSN", 734 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 500, "Assurant Inc", "AIZ", 782 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 499, "F5 Networks Inc", "FFIV", 891 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 498, "WestRock Co", "WRK", 1018 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 497, "Concho Resources Inc", "CXO", 1454 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 496, "Principal Financial Group Inc", "PFG", 1538 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 495, "First Republic Bank", "FRC", 1841 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 494, "KeyCorp", "KEY", 1893 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 493, "Centene Corp", "CNC", 2234 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 492, "IQVIA Holdings Inc", "IQV", 2719 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 491, "Hilton Worldwide Holdings Inc", "HLT", 2766 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 444, "IPG Photonics Corp", "IPGP", 7578 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 490, "eBay Inc", "EBAY", 2866 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 488, "Newmont Goldcorp Corp", "NEM", 3012 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 487, "Welltower Inc", "WELL", 3375 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 486, "The Travelers Companies Inc", "TRV", 3446 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 485, "Advanced Micro Devices Inc", "AMD", 4038 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 484, "Affiliated Managers Group Inc", "AMG", 4291 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 483, "TripAdvisor Inc", "TRIP", 4432 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 482, "Helmerich & Payne Inc", "HP", 4501 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 481, "Capital One Financial Corp", "COF", 4517 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 480, "Cimarex Energy Co", "XEC", 4637 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 479, "Waste Management Inc", "WM", 4681 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 478, "L Brands Inc", "LB", 4836 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 477, "Las Vegas Sands Corp", "LVS", 4847 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 476, "H&R Block Inc", "HRB", 4937 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 489, "AvalonBay Communities Inc", "AVB", 2938 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 443, "DXC Technology Co", "DXC", 7667 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 442, "News Corp", "NWSA", 7715 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 441, "News Corp", "NWS", 7845 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 408, "PerkinElmer Inc", "PKI", 9709 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 407, "Marathon Oil Corp", "MRO", 9711 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 406, "NiSource Inc", "NI", 9756 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 405, "E*TRADE Financial Corp", "ETFC", 9768 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 404, "Whirlpool Corp", "WHR", 9842 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 403, "Henry Schein Inc", "HSIC", 9871 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 402, "Federal Realty Investment Trust", "FRT", 9919 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 401, "Abiomed Inc", "ABMD", 9943 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 400, "CF Industries Holdings Inc", "CF", 10073 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 399, "Charter Communications Inc", "CHTR", 10159 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 398, "Noble Energy Inc", "NBL", 10206 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 397, "Comerica Inc", "CMA", 10281 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 396, "Huntington Ingalls Industries Inc", "HII", 10302 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 409, "NRG Energy Inc", "NRG", 9688 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 395, "C.H. Robinson Worldwide Inc", "CHRW", 10404 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 393, "Textron Inc", "TXT", 10545 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 392, "United Parcel Service Inc", "UPS", 10546 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 391, "Mohawk Industries Inc", "MHK", 10728 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 390, "Allegion PLC", "ALLE", 10744 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 389, "Globe Life Inc", "GL", 10755 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 388, "Packaging Corp of America", "PKG", 10783 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 387, "LKQ Corp", "LKQ", 10787 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 386, "Regency Centers Corp", "REG", 10859 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 385, "Everest Re Group Ltd", "RE", 10873 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 384, "Norwegian Cruise Line Holdings Ltd", "NCLH", 11037 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 383, "Avery Dennison Corp", "AVY", 11136 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 382, "Eastman Chemical Co", "EMN", 11229 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 381, "Tractor Supply Co", "TSCO", 11405 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 394, "PulteGroup Inc", "PHM", 10495 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 254, "Ameriprise Financial Inc", "AMP", 20737 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 410, "Pinnacle West Capital Corp", "PNW", 9578 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 412, "Iron Mountain Inc", "IRM", 9447 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 440, "The Mosaic Co", "MOS", 7916 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 439, "Apartment Investment & Management Co", "AIV", 7926 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 438, "Booking Holdings Inc", "BKNG", 7931 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 437, "Under Armour Inc", "UAA", 7932 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 436, "Invesco Ltd", "IVZ", 8015 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 435, "Caterpillar Inc", "CAT", 8176 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 434, "Newell Brands Inc", "NWL", 8235 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 433, "Xerox Holdings Corp", "XRX", 8288 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 432, "A.O. Smith Corp", "AOS", 8447 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 431, "Ralph Lauren Corp", "RL", 8509 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 430, "Fortune Brands Home & Security Inc", "FBHS", 8556 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 429, "Devon Energy Corp", "DVN", 8657 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 428, "Zions Bancorp NA", "ZION", 8666 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 411, "BorgWarner Inc", "BWA", 9542 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 427, "The Interpublic Group of Companies Inc", "IPG", 8689 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 425, "Kimco Realty Corp", "KIM", 8816 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 424, "Juniper Networks Inc", "JNPR", 8832 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 423, "Alaska Air Group Inc", "ALK", 8851 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 422, "Mylan NV", "MYL", 8872 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 421, "National Oilwell Varco Inc", "NOV", 8893 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 420, "DaVita Inc", "DVA", 8984 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 419, "Apache Corp", "APA", 9002 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 418, "Kohl's Corp", "KSS", 9081 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 417, "Snap-on Inc", "SNA", 9192 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 416, "TechnipFMC PLC", "FTI", 9196 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 415, "CVS Health Corp", "CVS", 9324 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 414, "Viacom Inc", "VIAB", 9378 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 413, "Coty Inc", "COTY", 9397 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 426, "HollyFrontier Corp", "HFC", 8732 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 504, "Macerich Co", "MAC", 393 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 253, "Essex Property Trust Inc", "ESS", 20755 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 251, "American Water Works Co Inc", "AWK", 20948 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 91, "Boston Scientific Corp", "BSX", 55934 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 90, "Illinois Tool Works Inc", "ITW", 56368 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 89, "Target Corp", "TGT", 56391 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 88, "Colgate-Palmolive Co", "CL", 56942 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 87, "Northrop Grumman Corp", "NOC", 58988 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 86, "Allergan PLC", "AGN", 59503 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 85, "Raytheon Co", "RTN", 60126 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 84, "S&P Global Inc", "SPGI", 62282 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 83, "Intuitive Surgical Inc", "ISRG", 62896 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 82, "Southern Co", "SO", 63511 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 81, "Duke Energy Corp", "DUK", 63883 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 80, "ConocoPhillips", "COP", 64694 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 79, "The Estee Lauder Companies Inc", "EL", 67349 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 78, "T-Mobile US Inc", "TMUS", 68009 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 77, "Cigna Corp", "CI", 69834 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 76, "Automatic Data Processing Inc", "ADP", 70603 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 75, "TJX Companies Inc", "TJX", 70722 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 74, "Anthem Inc", "ANTM", 70739 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 73, "CME Group Inc", "CME", 71224 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 72, "Stryker Corp", "SYK", 74031 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 71, "Mondelez International Inc", "MDLZ", 74568 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 70, "Fiserv Inc", "FISV", 74836 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 69, "BlackRock Inc", "BLK", 75457 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 68, "Goldman Sachs Group Inc", "GS", 77555 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 67, "Celgene Corp", "CELG", 77954 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 66, "Morgan Stanley", "MS", 79408 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 65, "Fidelity National Information Services Inc", "FIS", 81176 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 92, "Zoetis Inc", "ZTS", 55646 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 93, "Charles Schwab Corp", "SCHW", 55356 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 94, "General Motors Co", "GM", 55222 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 95, "Crown Castle International Corp", "CCI", 54573 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 123, "Illumina Inc", "ILMN", 43409 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 122, "American Electric Power Co Inc", "AEP", 43902 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 121, "Kimberly-Clark Corp", "KMB", 44969 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 120, "Kinder Morgan Inc", "KMI", 45163 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 119, "MetLife Inc", "MET", 45209 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 118, "Bank of New York Mellon Corp", "BK", 45224 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 117, "Emerson Electric Co", "EMR", 45369 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 116, "Aon PLC", "AON", 45499 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 115, "Equinix Inc", "EQIX", 45748 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 114, "HCA Healthcare Inc", "HCA", 45846 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 113, "Simon Property Group Inc", "SPG", 47721 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 112, "American International Group Inc", "AIG", 48299 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 111, "Edwards Lifesciences Corp", "EW", 48895 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 64, "Gilead Sciences Inc", "GILD", 81551 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 110, "Schlumberger Ltd", "SLB", 50308 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 108, "Intercontinental Exchange Inc", "ICE", 50717 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 107, "Micron Technology Inc", "MU", 51411 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 106, "Global Payments Inc", "GPN", 51519 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 105, "Air Products & Chemicals Inc", "APD", 51708 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 104, "Vertex Pharmaceuticals Inc", "VRTX", 51915 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 103, "Marsh & McLennan Companies Inc", "MMC", 52096 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 102, "Applied Materials Inc", "AMAT", 52229 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 101, "Phillips 66", "PSX", 52687 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 100, "Biogen Inc", "BIIB", 52981 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 99, "General Dynamics Corp", "GD", 53339 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 98, "DuPont de Nemours Inc", "DD", 53434 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 97, "Sherwin-Williams Co", "SHW", 53503 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 96, "Ecolab Inc", "ECL", 54368 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 109, "Norfolk Southern Corp", "NSC", 50483 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 124, "Marriott International Inc", "MAR", 43093 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 63, "Altria Group Inc", "MO", 86774 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 61, "American Tower Corp", "AMT", 92043 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 28, "PepsiCo Inc", "PEP", 184455 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 27, "Oracle Corp", "ORCL", 185166 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 26, "Pfizer Inc", "PFE", 204043 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 25, "Cisco Systems Inc", "CSCO", 204637 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 24, "Comcast Corp", "CMCSA", 205535 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 23, "Boeing Co", "BA", 207152 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 22, "Merck & Co Inc", "MRK", 211698 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 21, "Wells Fargo & Co", "WFC", 228343 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 20, "Chevron Corp", "CVX", 228455 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 19, "UnitedHealth Group Inc", "UNH", 240046 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 18, "Verizon Communications Inc", "VZ", 246327 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 17, "The Home Depot Inc", "HD", 253571 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 16, "Intel Corp", "INTC", 253822 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 15, "Mastercard Inc", "MA", 278728 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 14, "AT&T Inc", "T", 287378 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 13, "Procter & Gamble Co", "PG", 297761 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 12, "Bank of America Corp", "BAC", 298277 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 11, "Exxon Mobil Corp", "XOM", 298335 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 10, "Walmart Inc", "WMT", 337986 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 9, "Johnson & Johnson", "JNJ", 346591 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 8, "JPMorgan Chase & Co", "JPM", 407366 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 7, "Berkshire Hathaway Inc", "BRK.B", 539621 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 6, "Facebook Inc", "FB", 540092 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 5, "Amazon.com Inc", "AMZN", 876906 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 4, "Alphabet Inc", "GOOGL", 894193 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 3, "Alphabet Inc", "GOOG", 895214 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 2, "Microsoft Corp", "MSFT", 1112966 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 29, "Citigroup Inc", "C", 165005 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 30, "Abbott Laboratories", "ABT", 147825 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 31, "McDonald's Corp", "MCD", 145015 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 32, "Medtronic PLC", "MDT", 144444 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 60, "U.S. Bancorp", "USB", 92248 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 59, "Bristol-Myers Squibb Co", "BMY", 94661 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 58, "Danaher Corp", "DHR", 96457 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 57, "Starbucks Corp", "SBUX", 97492 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 56, "3M Co", "MMM", 97816 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 55, "American Express Co", "AXP", 98888 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 54, "General Electric Co", "GE", 99387 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 53, "Qualcomm Inc", "QCOM", 105346 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 52, "Lockheed Martin Corp", "LMT", 107598 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 51, "Eli Lilly and Co", "LLY", 108139 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 50, "NextEra Energy Inc", "NEE", 108737 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 49, "Linde PLC", "LIN", 109145 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 48, "Texas Instruments Inc", "TXN", 110387 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 62, "Lowe's Companies Inc", "LOW", 88208 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 47, "Thermo Fisher Scientific Inc", "TMO", 117654 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 45, "PayPal Holdings Inc", "PYPL", 120366 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 44, "Accenture PLC", "ACN", 120739 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 43, "Union Pacific Corp", "UNP", 122554 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 42, "Broadcom Inc", "AVGO", 123904 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 41, "NVIDIA Corp", "NVDA", 126818 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 40, "United Technologies Corp", "UTX", 128022 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 39, "Netflix Inc", "NFLX", 129117 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 38, "Philip Morris International Inc", "PM", 130195 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 37, "Amgen Inc", "AMGN", 130779 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 36, "Costco Wholesale Corp", "COST", 132196 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 35, "Adobe Inc", "ADBE", 140445 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 34, "Nike Inc", "NKE", 140479 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 33, "Salesforce.com Inc", "CRM", 141468 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 46, "International Business Machines Corp", "IBM", 119853 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 125, "Progressive Corp", "PGR", 43075 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 126, "EOG Resources Inc", "EOG", 43009 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 127, "Exelon Corp", "EXC", 42801 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 218, "Synchrony Financial", "SYF", 23786 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 217, "Edison International", "EIX", 23832 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 216, "Royal Caribbean Cruises Ltd", "RCL", 23837 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 215, "Xilinx Inc", "XLNX", 23952 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 214, "Willis Towers Watson PLC", "WLTW", 23974 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 213, "Stanley Black & Decker Inc", "SWK", 23976 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 212, "Alexion Pharmaceuticals Inc", "ALXN", 24102 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 211, "Fortive Corp", "FTV", 24198 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 210, "Archer-Daniels Midland Co", "ADM", 24249 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 209, "The Walt Disney Co", "DIS", 24275 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 208, "PPL Corp", "PPL", 24308 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 207, "Aptiv PLC", "APTV", 24494 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 206, "Realty Income Corp", "O", 24945 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 205, "FirstEnergy Corp", "FE", 24951 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 204, "McKesson Corp", "MCK", 25201 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 203, "Fleetcor Technologies Inc", "FLT", 25239 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 202, "Digital Realty Trust Inc", "DLR", 25406 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 201, "Parker Hannifin Corp", "PH", 25527 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 200, "Eversource Energy", "ES", 25683 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 199, "Dollar Tree Inc", "DLTR", 25756 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 198, "SBA Communications Corp", "SBAC", 25796 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 197, "Discover Financial Services", "DFS", 26343 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 196, "State Street Corporation", "STT", 26537 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 195, "Williams Companies Inc", "WMB", 26749 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 194, "Cintas Corp", "CTAS", 26943 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 193, "Republic Services Inc", "RSG", 27433 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 192, "KLA Corp", "KLAC", 27533 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 219, "Agilent Technologies Inc", "A", 23723 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 220, "United Airlines Holdings Inc", "UAL", 23614 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 221, "DTE Energy Co", "DTE", 23189 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 222, "Corning Inc", "GLW", 23034 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 250, "Rockwell Automation Inc", "ROK", 21094 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 249, "Fastenal Co", "FAST", 21111 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 248, "Ball Corp", "BLL", 21129 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 247, "Fox Corp", "FOX", 21227 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 246, "The Kroger Co", "KR", 21291 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 245, "McCormick & Co Inc", "MKC", 21295 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 244, "Boston Properties Inc", "BXP", 21395 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 243, "Fifth Third Bancorp", "FITB", 21629 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 242, "Kellogg Co", "K", 21639 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 241, "IDEXX Laboratories Inc", "IDXX", 21641 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 240, "Fox Corp", "FOXA", 21699 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 239, "VeriSign Inc", "VRSN", 21773 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 238, "M&T Bank Corp", "MTB", 21946 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 191, "WEC Energy Group Inc", "WEC", 27534 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 237, "Weyerhaeuser Co", "WY", 21989 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 235, "Ventas Inc", "VTR", 22073 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 234, "Pioneer Natural Resources Co", "PXD", 22092 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 233, "The Hartford Financial Services Group Inc", "HIG", 22108 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 232, "Hess Corp", "HES", 22176 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 231, "Hormel Foods Corp", "HRL", 22213 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 230, "Coca-Cola Co", "KO", 22215 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 229, "Verisk Analytics Inc", "VRSK", 22479 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 228, "Northern Trust Corp", "NTRS", 22573 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 227, "Microchip Technology Inc", "MCHP", 22614 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 226, "Twitter Inc", "TWTR", 22692 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 225, "Entergy Corp", "ETR", 22743 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 224, "Baker Hughes Co", "BKR", 22764 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 223, "Hewlett Packard Enterprise Co", "HPE", 22769 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 236, "AMETEK Inc", "AME", 22027 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 190, "PACCAR Inc", "PCAR", 27614 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 189, "Motorola Solutions Inc", "MSI", 27658 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 188, "IHS Markit Ltd", "INFO", 27696 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 155, "Occidental Petroleum Corp", "OXY", 34785 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 154, "Roper Technologies Inc", "ROP", 35169 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 153, "Constellation Brands Inc", "STZ", 35345 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 152, "Allstate Corp", "ALL", 35418 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 151, "Ford Motor Co", "F", 36001 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 150, "Public Storage", "PSA", 37011 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 149, "Prudential Financial Inc", "PRU", 37052 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 148, "Delta Air Lines Inc", "DAL", 37064 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 147, "Regeneron Pharmaceuticals Inc", "REGN", 37846 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 146, "Eaton Corp PLC", "ETN", 37962 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 145, "Lam Research Corp", "LRCX", 39391 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 144, "Aflac Inc", "AFL", 39644 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 143, "The Kraft Heinz Co", "KHC", 39883 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 156, "VF Corp", "VFC", 34525 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 142, "Visa Inc", "V", 40014 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 140, "Dollar General Corp", "DG", 40241 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 139, "Baxter International Inc", "BAX", 40323 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 138, "Activision Blizzard Inc", "ATVI", 40325 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 137, "Ross Stores Inc", "ROST", 40502 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 136, "Sysco Corp", "SYY", 40853 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 135, "Moody's Corporation", "MCO", 41113 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 134, "Dow Inc", "DOW", 41293 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 133, "Valero Energy Corp", "VLO", 41369 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 132, "BB&T Corp", "BBT", 41564 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 131, "Analog Devices Inc", "ADI", 41576 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 130, "Humana Inc", "HUM", 41847 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 129, "FedEx Corp", "FDX", 42262 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 128, "Marathon Petroleum Corp", "MPC", 42679 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 141, "Sempra Energy", "SRE", 40119 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 252, "Cerner Corp", "CERN", 20802 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 157, "Cognizant Technology Solutions Corp", "CTSH", 34228 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 159, "O'Reilly Automotive Inc", "ORLY", 33236 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 187, "AutoZone Inc", "AZO", 28053 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 186, "T. Rowe Price Group Inc", "TROW", 28088 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 185, "Electronic Arts Inc", "EA", 28178 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 184, "Cummins Inc", "CMI", 28541 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 183, "ONEOK Inc", "OKE", 28849 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 182, "TransDigm Group Inc", "TDG", 28934 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 181, "Consolidated Edison Inc", "ED", 29104 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 180, "HP Inc", "HPQ", 29223 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 179, "Zimmer Biomet Holdings Inc", "ZBH", 29377 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 178, "Paychex Inc", "PAYX", 29566 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 177, "The Hershey Co", "HSY", 29572 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 176, "Yum Brands Inc", "YUM", 29907 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 175, "Tyson Foods Inc", "TSN", 30112 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 158, "Autodesk Inc", "ADSK", 33574 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 174, "Amphenol Corp", "APH", 30259 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 172, "Brown-Forman Corp", "BF.B", 30638 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 171, "Public Service Enterprise Group Inc", "PEG", 30748 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 170, "SunTrust Banks Inc", "STI", 31109 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 169, "Ingersoll-Rand PLC", "IR", 31135 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 168, "Southwest Airlines Co", "LUV", 31251 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 167, "Equity Residential", "EQR", 31275 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 166, "Monster Beverage Corp", "MNST", 31348 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 165, "TE Connectivity Ltd", "TEL", 31371 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 164, "Xcel Energy Inc", "XEL", 31499 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 163, "General Mills Inc", "GIS", 31609 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 162, "Carnival Corp", "CCL", 31822 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 161, "LyondellBasell Industries NV", "LYB", 32357 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 160, "Johnson Controls International PLC", "JCI", 32845 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 173, "PPG Industries Inc", "PPG", 30506 });

            migrationBuilder.InsertData(
                table: "Funds",
                columns: new[] { "FundId", "Name", "Symbol", "Volume" },
                values: new object[] { 505, "American Airlines Group Inc", "AAL", 134 });

            migrationBuilder.CreateIndex(
                name: "IX_DailyPrices_FundIdTimestamp",
                table: "DailyPrices",
                columns: new[] { "FundId", "Timestamp" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Funds_Symbol",
                table: "Funds",
                column: "Symbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Prices_FundIdTimestamp",
                table: "Prices",
                columns: new[] { "FundId", "Timestamp" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyPrices");

            migrationBuilder.DropTable(
                name: "Prices");

            migrationBuilder.DropTable(
                name: "Funds");
        }
    }
}
