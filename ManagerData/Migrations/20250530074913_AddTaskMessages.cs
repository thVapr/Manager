using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskMessages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TaskMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Message = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskMessages_Members_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskMessages_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskMessages_CreatorId",
                table: "TaskMessages",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskMessages_TaskId",
                table: "TaskMessages",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TaskMessages");
        }
    }
}
