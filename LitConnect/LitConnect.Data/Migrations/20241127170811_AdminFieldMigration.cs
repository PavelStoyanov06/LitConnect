using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class AdminFieldMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedOn",
                table: "UsersBookClubs",
                type: "datetime2",
                nullable: false,
                comment: "Date when the user joined the book club",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date and time when the user joined or was approved to the book club");

            migrationBuilder.AlterColumn<string>(
                name: "BookClubId",
                table: "UsersBookClubs",
                type: "nvarchar(450)",
                nullable: false,
                comment: "ID of the book club",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "ID of the book club the user is a member of");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UsersBookClubs",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the user is an admin of the book club");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UsersBookClubs");

            migrationBuilder.AlterColumn<DateTime>(
                name: "JoinedOn",
                table: "UsersBookClubs",
                type: "datetime2",
                nullable: false,
                comment: "Date and time when the user joined or was approved to the book club",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldComment: "Date when the user joined the book club");

            migrationBuilder.AlterColumn<string>(
                name: "BookClubId",
                table: "UsersBookClubs",
                type: "nvarchar(450)",
                nullable: false,
                comment: "ID of the book club the user is a member of",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldComment: "ID of the book club");
        }
    }
}
