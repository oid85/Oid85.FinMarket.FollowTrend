using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Oid85.FinMarket.FollowTrend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _24102026_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MoneySum",
                schema: "public",
                table: "PortfolioSettingEntities",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MoneySum",
                schema: "public",
                table: "PortfolioSettingEntities");
        }
    }
}
