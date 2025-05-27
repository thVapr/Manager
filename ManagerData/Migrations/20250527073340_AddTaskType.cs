using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TaskTypeId",
                table: "Tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PartTaskTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartTaskTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PartTaskTypes_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskTypeId",
                table: "Tasks",
                column: "TaskTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PartTaskTypes_PartId",
                table: "PartTaskTypes",
                column: "PartId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_PartTaskTypes_TaskTypeId",
                table: "Tasks",
                column: "TaskTypeId",
                principalTable: "PartTaskTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_PartTaskTypes_TaskTypeId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "PartTaskTypes");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_TaskTypeId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "TaskTypeId",
                table: "Tasks");
        }
    }
}
