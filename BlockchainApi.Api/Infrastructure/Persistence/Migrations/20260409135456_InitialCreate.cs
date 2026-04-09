using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlockchainApi.Api.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlockCyphers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Coin = table.Column<string>(type: "TEXT", nullable: false),
                    RawData = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockCyphers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlockCyphers_Coin",
                table: "BlockCyphers",
                column: "Coin");

            migrationBuilder.CreateIndex(
                name: "IX_BlockCyphers_Coin_CreatedAt",
                table: "BlockCyphers",
                columns: new[] { "Coin", "CreatedAt" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "IX_BlockCyphers_CreatedAt",
                table: "BlockCyphers",
                column: "CreatedAt");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlockCyphers");
        }
    }
}
