using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PCM.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPricePerHourAndSummaryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Summary",
                table: "189_News",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Team2_Player1Id",
                table: "189_Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Team1_Player1Id",
                table: "189_Matches",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerHour",
                table: "189_Courts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 100000m);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "189_Bookings",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "189_Courts",
                keyColumn: "Id",
                keyValue: 1,
                column: "PricePerHour",
                value: 100000m);

            migrationBuilder.UpdateData(
                table: "189_Courts",
                keyColumn: "Id",
                keyValue: 2,
                column: "PricePerHour",
                value: 100000m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Summary",
                table: "189_News");

            migrationBuilder.DropColumn(
                name: "PricePerHour",
                table: "189_Courts");

            migrationBuilder.AlterColumn<int>(
                name: "Team2_Player1Id",
                table: "189_Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Team1_Player1Id",
                table: "189_Matches",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RowVersion",
                table: "189_Bookings",
                type: "rowversion",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "rowversion",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "Notes",
                table: "189_Bookings",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
