using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Maidan.Migrations
{
    /// <inheritdoc />
    public partial class IdentityRoleAdd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "4f598d0b-7381-4b78-bdbb-e72e50de6b77", "10", "user", "USER" },
                    { "a0aa21f6-a451-47a2-9415-4bbeef62a1be", "1", "admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4f598d0b-7381-4b78-bdbb-e72e50de6b77");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a0aa21f6-a451-47a2-9415-4bbeef62a1be");
        }
    }
}
