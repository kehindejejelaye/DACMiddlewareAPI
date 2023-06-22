using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DACMiddlewareAPI.Migrations
{
    public partial class Db2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Users",
                table: "AssignedUsers",
                newName: "UserAssigned");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserAssigned",
                table: "AssignedUsers",
                newName: "Users");
        }
    }
}
