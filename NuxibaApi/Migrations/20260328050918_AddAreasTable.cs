using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NuxibaApi.Migrations
{
    /// <inheritdoc />
    public partial class AddAreasTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ccRIACat_Areas",
                columns: table => new
                {
                    IDArea = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AreaName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusArea = table.Column<int>(type: "int", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ccRIACat_Areas", x => x.IDArea);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ccRIACat_Areas");
        }
    }
}
