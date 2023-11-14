using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GameTrilha.API.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatar : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$cRC4J9HPtfeZYi5Xh7vD8.Zz0GqMjObwIrka2k380hm2d6o8rGWnS");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("d0a4d3a0-4f0a-4f1b-9a3a-2b5d9d7c4b1a"),
                column: "Password",
                value: "$2a$11$q6GNOpfbQz5Sk6VJgXg4guiYDoBsJ60g.S6zAdEnbKeNNO.xLuQO2");
        }
    }
}
