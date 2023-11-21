using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTrilha.API.Migrations
{
    /// <inheritdoc />
    public partial class SelectedBoardSkin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BoardId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SkinId",
                table: "Users",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                columns: new[] { "BoardId", "Password", "SkinId" },
                values: new object[] { null, "$2a$11$/oDGDm.T.IfFxndzuvTnA.7HerMyerC5Tt6wc9wW1q02Mxe2FeP8q", null });

            migrationBuilder.CreateIndex(
                name: "IX_Users_BoardId",
                table: "Users",
                column: "BoardId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_SkinId",
                table: "Users",
                column: "SkinId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Boards_BoardId",
                table: "Users",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Skins_SkinId",
                table: "Users",
                column: "SkinId",
                principalTable: "Skins",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Boards_BoardId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Skins_SkinId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_BoardId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_SkinId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BoardId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SkinId",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$zYp1grVUVhtJBVLlzL817eyNFuVUfL89t3MDaLRspfSDIQEIaYmoO");
        }
    }
}
