using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class AddHistoryModelToBackgroundTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HistoryId",
                table: "BackgroundTasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BackgroundTasks_HistoryId",
                table: "BackgroundTasks",
                column: "HistoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_BackgroundTasks_TaskHistories_HistoryId",
                table: "BackgroundTasks",
                column: "HistoryId",
                principalTable: "TaskHistories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BackgroundTasks_TaskHistories_HistoryId",
                table: "BackgroundTasks");

            migrationBuilder.DropIndex(
                name: "IX_BackgroundTasks_HistoryId",
                table: "BackgroundTasks");

            migrationBuilder.DropColumn(
                name: "HistoryId",
                table: "BackgroundTasks");
        }
    }
}
