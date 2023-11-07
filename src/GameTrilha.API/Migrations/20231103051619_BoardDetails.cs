using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTrilha.API.Migrations
{
    /// <inheritdoc />
    public partial class BoardDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StrokeColor",
                table: "Boards",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BulletColor",
                table: "Boards",
                type: "nvarchar(7)",
                maxLength: 7,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Boards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Boards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$Ww8h84XQdCMeYEfLJWyDT.GYoRUPiG/gSIvl7hQRxRhlnvARHe9im");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Boards");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Boards");

            migrationBuilder.AlterColumn<string>(
                name: "StrokeColor",
                table: "Boards",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7);

            migrationBuilder.AlterColumn<string>(
                name: "BulletColor",
                table: "Boards",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(7)",
                oldMaxLength: 7);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$Rn3DHK8YmzJ0oGY1CNewle/ob4WJ4.Lw43X2oUyh8IY5kOxA3xjt6");
        }
    }
}
