using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LeaveManagement.Migrations
{
    /// <inheritdoc />
    public partial class AddedMigrationToCheck : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LeaveLimits",
                keyColumn: "Id",
                keyValue: 1,
                column: "LeaveType",
                value: "Paid");

            migrationBuilder.UpdateData(
                table: "LeaveLimits",
                keyColumn: "Id",
                keyValue: 2,
                column: "LeaveType",
                value: "Sick");

            migrationBuilder.UpdateData(
                table: "LeaveLimits",
                keyColumn: "Id",
                keyValue: 3,
                column: "LeaveType",
                value: "Vacation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "LeaveLimits",
                keyColumn: "Id",
                keyValue: 1,
                column: "LeaveType",
                value: "Paid Leave");

            migrationBuilder.UpdateData(
                table: "LeaveLimits",
                keyColumn: "Id",
                keyValue: 2,
                column: "LeaveType",
                value: "Sick Leave");

            migrationBuilder.UpdateData(
                table: "LeaveLimits",
                keyColumn: "Id",
                keyValue: 3,
                column: "LeaveType",
                value: "Vacation Leave");
        }
    }
}
