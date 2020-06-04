using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TestWalletApi.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IdempotentCaches",
                columns: table => new
                {
                    IdempotentKey = table.Column<string>(nullable: false),
                    ParametersHash = table.Column<string>(nullable: false),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdempotentCaches", x => new { x.UserId, x.IdempotentKey, x.ParametersHash });
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CurrencyTabs",
                columns: table => new
                {
                    Сurrency = table.Column<int>(nullable: false),
                    WalletId = table.Column<long>(nullable: false),
                    Amount = table.Column<decimal>(nullable: false),
                    ChangeVersion = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyTabs", x => new { x.WalletId, x.Сurrency });
                    table.ForeignKey(
                        name: "FK_CurrencyTabs_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrencyTabs");

            migrationBuilder.DropTable(
                name: "IdempotentCaches");

            migrationBuilder.DropTable(
                name: "Wallets");
        }
    }
}
