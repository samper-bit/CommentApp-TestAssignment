using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommentApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommentIdColumnInFileTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_CommentId",
                table: "Files");

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "Files",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_CommentId",
                table: "Files",
                column: "CommentId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_CommentId",
                table: "Files");

            migrationBuilder.AlterColumn<Guid>(
                name: "CommentId",
                table: "Files",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Files_CommentId",
                table: "Files",
                column: "CommentId",
                unique: true,
                filter: "[CommentId] IS NOT NULL");
        }
    }
}
