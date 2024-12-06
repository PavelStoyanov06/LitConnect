using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LitConnect.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserBookClubSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "UsersBookClubs",
                type: "bit",
                nullable: false,
                defaultValue: false,
                comment: "Indicates if the membership is deleted");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "admin_role_id", "6e543a40-699c-4521-8a85-1669bd519256", "Administrator", "ADMINISTRATOR" });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "IsDeleted", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "admin_user_id", 0, "d590ac91-0581-434e-8662-9accd6d68923", "admin@litconnect.com", true, "Admin", false, "User", false, null, "ADMIN@LITCONNECT.COM", "ADMIN@LITCONNECT.COM", "AQAAAAIAAYagAAAAECcpL7i6eM+bRWBF794BIAauc/9N2JaQWT3RP7uv0ldIELriSCldzrxzjf0Zs8rZQw==", null, false, "30a5a553-f416-4c9f-a304-76cc042adb2c", false, "admin@litconnect.com" });

            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "Description", "IsDeleted", "Title" },
                values: new object[,]
                {
                    { "book_1", "John Smith", "An epic journey through unknown lands.", false, "The Great Adventure" },
                    { "book_2", "Jane Doe", "A thrilling detective story.", false, "Mystery at Midnight" },
                    { "book_3", "Robert Johnson", "A glimpse into tomorrow's world.", false, "The Future Now" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { "genre_biography", false, "Biography" },
                    { "genre_fantasy", false, "Fantasy" },
                    { "genre_fiction", false, "Fiction" },
                    { "genre_historical", false, "Historical Fiction" },
                    { "genre_horror", false, "Horror" },
                    { "genre_mystery", false, "Mystery" },
                    { "genre_nonfiction", false, "Non-Fiction" },
                    { "genre_romance", false, "Romance" },
                    { "genre_scifi", false, "Science Fiction" },
                    { "genre_thriller", false, "Thriller" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "admin_role_id", "admin_user_id" });

            migrationBuilder.InsertData(
                table: "BookClubs",
                columns: new[] { "Id", "CurrentBookId", "Description", "IsDeleted", "Name", "OwnerId" },
                values: new object[,]
                {
                    { "club_1", null, "A club for fantasy book enthusiasts", false, "Fantasy Lovers", "admin_user_id" },
                    { "club_2", null, "Join us to solve literary mysteries", false, "Mystery Solvers", "admin_user_id" }
                });

            migrationBuilder.InsertData(
                table: "BooksGenres",
                columns: new[] { "BookId", "GenreId", "IsDeleted" },
                values: new object[,]
                {
                    { "book_1", "genre_fantasy", false },
                    { "book_1", "genre_fiction", false },
                    { "book_2", "genre_mystery", false },
                    { "book_2", "genre_thriller", false },
                    { "book_3", "genre_scifi", false }
                });

            migrationBuilder.InsertData(
                table: "UsersBookClubs",
                columns: new[] { "BookClubId", "UserId", "IsAdmin", "IsDeleted", "JoinedOn" },
                values: new object[,]
                {
                    { "club_1", "admin_user_id", true, false, new DateTime(2024, 12, 6, 23, 21, 30, 201, DateTimeKind.Utc).AddTicks(5694) },
                    { "club_2", "admin_user_id", true, false, new DateTime(2024, 12, 6, 23, 21, 30, 201, DateTimeKind.Utc).AddTicks(5699) }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "admin_role_id", "admin_user_id" });

            migrationBuilder.DeleteData(
                table: "BooksGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { "book_1", "genre_fantasy" });

            migrationBuilder.DeleteData(
                table: "BooksGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { "book_1", "genre_fiction" });

            migrationBuilder.DeleteData(
                table: "BooksGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { "book_2", "genre_mystery" });

            migrationBuilder.DeleteData(
                table: "BooksGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { "book_2", "genre_thriller" });

            migrationBuilder.DeleteData(
                table: "BooksGenres",
                keyColumns: new[] { "BookId", "GenreId" },
                keyValues: new object[] { "book_3", "genre_scifi" });

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_biography");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_historical");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_horror");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_nonfiction");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_romance");

            migrationBuilder.DeleteData(
                table: "UsersBookClubs",
                keyColumns: new[] { "BookClubId", "UserId" },
                keyValues: new object[] { "club_1", "admin_user_id" });

            migrationBuilder.DeleteData(
                table: "UsersBookClubs",
                keyColumns: new[] { "BookClubId", "UserId" },
                keyValues: new object[] { "club_2", "admin_user_id" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "admin_role_id");

            migrationBuilder.DeleteData(
                table: "BookClubs",
                keyColumn: "Id",
                keyValue: "club_1");

            migrationBuilder.DeleteData(
                table: "BookClubs",
                keyColumn: "Id",
                keyValue: "club_2");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: "book_1");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: "book_2");

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: "book_3");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_fantasy");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_fiction");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_mystery");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_scifi");

            migrationBuilder.DeleteData(
                table: "Genres",
                keyColumn: "Id",
                keyValue: "genre_thriller");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "admin_user_id");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "UsersBookClubs");
        }
    }
}
