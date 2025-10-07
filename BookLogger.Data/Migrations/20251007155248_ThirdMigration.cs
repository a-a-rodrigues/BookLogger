using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLogger.Data.Migrations
{
    /// <inheritdoc />
    public partial class ThirdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookMetadata_BookMetadataId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookMetadata",
                table: "BookMetadata");

            migrationBuilder.RenameTable(
                name: "BookMetadata",
                newName: "BookMetadatas");

            migrationBuilder.AlterColumn<string>(
                name: "Review",
                table: "Books",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Rating",
                table: "Books",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "BookMetadatas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "BookMetadatas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<string>(
                name: "CoverUrl",
                table: "BookMetadatas",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookMetadatas",
                table: "BookMetadatas",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookMetadatas_BookMetadataId",
                table: "Books",
                column: "BookMetadataId",
                principalTable: "BookMetadatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_BookMetadatas_BookMetadataId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookMetadatas",
                table: "BookMetadatas");

            migrationBuilder.RenameTable(
                name: "BookMetadatas",
                newName: "BookMetadata");

            migrationBuilder.AlterColumn<string>(
                name: "Review",
                table: "Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Rating",
                table: "Books",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ISBN",
                table: "BookMetadata",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Genre",
                table: "BookMetadata",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverUrl",
                table: "BookMetadata",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookMetadata",
                table: "BookMetadata",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_BookMetadata_BookMetadataId",
                table: "Books",
                column: "BookMetadataId",
                principalTable: "BookMetadata",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
