using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckInFieldsToBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CheckInTime",
                table: "189_Bookings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsCheckedIn",
                table: "189_Bookings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CheckInTime",
                table: "189_Bookings");

            migrationBuilder.DropColumn(
                name: "IsCheckedIn",
                table: "189_Bookings");
        }
    }
}
