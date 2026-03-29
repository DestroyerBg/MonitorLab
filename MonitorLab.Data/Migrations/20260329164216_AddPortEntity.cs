using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitorLab.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPortEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Version = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ports", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MonitorPorts",
                columns: table => new
                {
                    MonitorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PortId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonitorPorts", x => new { x.MonitorId, x.PortId });
                    table.ForeignKey(
                        name: "FK_MonitorPorts_Monitors_MonitorId",
                        column: x => x.MonitorId,
                        principalTable: "Monitors",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MonitorPorts_Ports_PortId",
                        column: x => x.PortId,
                        principalTable: "Ports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonitorPorts_PortId",
                table: "MonitorPorts",
                column: "PortId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonitorPorts");

            migrationBuilder.DropTable(
                name: "Ports");
        }
    }
}
