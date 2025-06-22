using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FeedReader.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class FeedItemTableAdded2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedItemEntity_Feeds_FeedId",
                table: "FeedItemEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeedItemEntity",
                table: "FeedItemEntity");

            migrationBuilder.RenameTable(
                name: "FeedItemEntity",
                newName: "FeedItems");

            migrationBuilder.RenameIndex(
                name: "IX_FeedItemEntity_FeedId",
                table: "FeedItems",
                newName: "IX_FeedItems_FeedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeedItems",
                table: "FeedItems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedItems_Feeds_FeedId",
                table: "FeedItems",
                column: "FeedId",
                principalTable: "Feeds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FeedItems_Feeds_FeedId",
                table: "FeedItems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FeedItems",
                table: "FeedItems");

            migrationBuilder.RenameTable(
                name: "FeedItems",
                newName: "FeedItemEntity");

            migrationBuilder.RenameIndex(
                name: "IX_FeedItems_FeedId",
                table: "FeedItemEntity",
                newName: "IX_FeedItemEntity_FeedId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FeedItemEntity",
                table: "FeedItemEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FeedItemEntity_Feeds_FeedId",
                table: "FeedItemEntity",
                column: "FeedId",
                principalTable: "Feeds",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
