using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedReader.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ApplicationUserTableAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Feeds",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastLoggedInDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feeds_CreatedById",
                table: "Feeds",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Feeds_ApplicationUsers_CreatedById",
                table: "Feeds",
                column: "CreatedById",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Feeds_ApplicationUsers_CreatedById",
                table: "Feeds");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropIndex(
                name: "IX_Feeds_CreatedById",
                table: "Feeds");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Feeds");
        }
    }
}
