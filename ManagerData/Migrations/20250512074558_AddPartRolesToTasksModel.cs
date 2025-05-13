using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class AddPartRolesToTasksModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "PartRoleId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_PartRoleId",
                table: "Tasks",
                column: "PartRoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_PartRoles_PartRoleId",
                table: "Tasks",
                column: "PartRoleId",
                principalTable: "PartRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_PartRoles_PartRoleId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_PartRoleId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "PartRoleId",
                table: "Tasks");
        }
    }
}
