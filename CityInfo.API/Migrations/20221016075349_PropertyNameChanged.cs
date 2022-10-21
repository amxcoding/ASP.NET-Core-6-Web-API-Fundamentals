using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CityInfo.API.Migrations
{
    public partial class PropertyNameChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointOfInterests_Cities_CityId",
                table: "PointOfInterests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointOfInterests",
                table: "PointOfInterests");

            migrationBuilder.RenameTable(
                name: "PointOfInterests",
                newName: "PointsOfInterests");

            migrationBuilder.RenameIndex(
                name: "IX_PointOfInterests_CityId",
                table: "PointsOfInterests",
                newName: "IX_PointsOfInterests_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointsOfInterests",
                table: "PointsOfInterests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointsOfInterests_Cities_CityId",
                table: "PointsOfInterests",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PointsOfInterests_Cities_CityId",
                table: "PointsOfInterests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PointsOfInterests",
                table: "PointsOfInterests");

            migrationBuilder.RenameTable(
                name: "PointsOfInterests",
                newName: "PointOfInterests");

            migrationBuilder.RenameIndex(
                name: "IX_PointsOfInterests_CityId",
                table: "PointOfInterests",
                newName: "IX_PointOfInterests_CityId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PointOfInterests",
                table: "PointOfInterests",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PointOfInterests_Cities_CityId",
                table: "PointOfInterests",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
