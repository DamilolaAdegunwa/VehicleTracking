using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VehicleTracking.Data.Migrations
{
    public partial class init3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Vehicles");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Vehicles");

            migrationBuilder.CreateTable(
                name: "VehiclePositions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    VehicleId = table.Column<long>(type: "bigint", nullable: false),
                    CreationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatorUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeleterUserId = table.Column<long>(type: "bigint", nullable: true),
                    DeletionTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModificationTime = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LastModifierUserId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehiclePositions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehiclePositions");

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Vehicles",
                type: "decimal(18,15)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Vehicles",
                type: "decimal(18,15)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
