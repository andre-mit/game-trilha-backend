using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTrilha.API.Migrations
{
    /// <inheritdoc />
    public partial class BorderColors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StrokeColor",
                table: "Boards",
                newName: "LineColor");

            migrationBuilder.AddColumn<string>(
                name: "BorderLineColor",
                table: "Boards",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$q6GNOpfbQz5Sk6VJgXg4guiYDoBsJ60g.S6zAdEnbKeNNO.xLuQO2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BorderLineColor",
                table: "Boards");

            migrationBuilder.RenameColumn(
                name: "LineColor",
                table: "Boards",
                newName: "StrokeColor");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$Ww8h84XQdCMeYEfLJWyDT.GYoRUPiG/gSIvl7hQRxRhlnvARHe9im");
        }
    }
}
