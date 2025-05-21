using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class AddBackgroundTaskQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BackgroundTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PartId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    MemberId = table.Column<Guid>(type: "uuid", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Timeline = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BackgroundTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BackgroundTasks_Members_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Members",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackgroundTasks_Parts_PartId",
                        column: x => x.PartId,
                        principalTable: "Parts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BackgroundTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundTasks_MemberId",
                table: "BackgroundTasks",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundTasks_PartId",
                table: "BackgroundTasks",
                column: "PartId");

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundTasks_TaskId",
                table: "BackgroundTasks",
                column: "TaskId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BackgroundTasks");
        }
    }
}
