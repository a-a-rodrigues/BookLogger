using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLogger.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstPublishYearToBookMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "BookMetadatas",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstPublishYear",
                table: "BookMetadatas",
                type: "INTEGER",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstPublishYear",
                table: "BookMetadatas");

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "BookMetadatas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }
    }
}
