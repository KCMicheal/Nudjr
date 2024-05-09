using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nudjr_Api.Migrations.Nudjr
{
    /// <inheritdoc />
    public partial class ModifiedAlarmEntity_KE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "AlarmDatTime",
                table: "Alarms",
                newName: "AlarmTimestamp");

            migrationBuilder.AddColumn<DateTime>(
                name: "AlarmDateTime",
                table: "Alarms",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "AlarmTitle",
                table: "Alarms",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlarmDateTime",
                table: "Alarms");

            migrationBuilder.DropColumn(
                name: "AlarmTitle",
                table: "Alarms");

            migrationBuilder.RenameColumn(
                name: "AlarmTimestamp",
                table: "Alarms",
                newName: "AlarmDatTime");
        }
    }
}
