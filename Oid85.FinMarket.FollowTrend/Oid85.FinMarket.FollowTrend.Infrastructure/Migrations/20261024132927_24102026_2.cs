using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.FollowTrend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _24102026_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TickerListName",
                schema: "public",
                table: "PortfolioSettingEntities",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Tickers",
                schema: "public",
                table: "PortfolioSettingEntities",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TickerListName",
                schema: "public",
                table: "PortfolioSettingEntities");

            migrationBuilder.DropColumn(
                name: "Tickers",
                schema: "public",
                table: "PortfolioSettingEntities");
        }
    }
}
