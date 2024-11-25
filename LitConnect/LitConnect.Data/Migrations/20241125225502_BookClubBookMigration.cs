using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LitConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class BookClubBookMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentBookId",
                table: "BookClubs",
                type: "nvarchar(450)",
                nullable: true,
                comment: "ID of the book currently being read");

            migrationBuilder.CreateIndex(
                name: "IX_BookClubs_CurrentBookId",
                table: "BookClubs",
                column: "CurrentBookId");

            migrationBuilder.AddForeignKey(
                name: "FK_BookClubs_Books_CurrentBookId",
                table: "BookClubs",
                column: "CurrentBookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookClubs_Books_CurrentBookId",
                table: "BookClubs");

            migrationBuilder.DropIndex(
                name: "IX_BookClubs_CurrentBookId",
                table: "BookClubs");

            migrationBuilder.DropColumn(
                name: "CurrentBookId",
                table: "BookClubs");
        }
    }
}
