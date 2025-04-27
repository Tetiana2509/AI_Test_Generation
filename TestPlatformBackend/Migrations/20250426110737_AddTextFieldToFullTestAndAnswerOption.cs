using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestPlatformBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddTextFieldToFullTestAndAnswerOption : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "FullTests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Text",
                table: "AnswerOptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Text",
                table: "FullTests");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "AnswerOptions");
        }
    }
}
