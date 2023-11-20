using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTrilha.API.Migrations
{
    /// <inheritdoc />
    public partial class RecoveryPasswordCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecoveryPasswordCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Locked = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecoveryPasswordCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecoveryPasswordCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$zYp1grVUVhtJBVLlzL817eyNFuVUfL89t3MDaLRspfSDIQEIaYmoO");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RecoveryPasswordCodes_UserId",
                table: "RecoveryPasswordCodes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecoveryPasswordCodes");

            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$cRC4J9HPtfeZYi5Xh7vD8.Zz0GqMjObwIrka2k380hm2d6o8rGWnS");
        }
    }
}
