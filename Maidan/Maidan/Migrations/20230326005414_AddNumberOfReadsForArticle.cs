using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maidan.Migrations
{
    /// <inheritdoc />
    public partial class AddNumberOfReadsForArticle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<int>(
                name: "NumberOfReads",
                table: "Articles",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfReads",
                table: "Articles");
        }
    }
}
