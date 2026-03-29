using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitorLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitorModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Monitors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Resolution = table.Column<int>(type: "int", nullable: false),
                    PanelType = table.Column<int>(type: "int", nullable: false),
                    ScreenSizeInches = table.Column<double>(type: "float", nullable: false),
                    RefreshRateHz = table.Column<int>(type: "int", nullable: false),
                    ResponseTimeMs = table.Column<double>(type: "float", nullable: false),
                    BrightnessNits = table.Column<int>(type: "int", nullable: false),
                    ContrastRatio = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    ReleaseYear = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Monitors", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Monitors");
        }
    }
}
