using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedReader.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedFeedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Feeds",
                newName: "UpdatedDate");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Feeds",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Feeds",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdatedDate",
                table: "Feeds",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Feeds");

            migrationBuilder.DropColumn(
                name: "LastUpdatedDate",
                table: "Feeds");

            migrationBuilder.RenameColumn(
                name: "UpdatedDate",
                table: "Feeds",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Feeds",
                newName: "CreatedAt");
        }
    }
}
