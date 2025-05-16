using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class ExpandTaskHistoryModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ActionType",
                table: "TaskHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetMemberId",
                table: "TaskHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskHistories_TargetMemberId",
                table: "TaskHistories",
                column: "TargetMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskHistories_Members_TargetMemberId",
                table: "TaskHistories",
                column: "TargetMemberId",
                principalTable: "Members",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskHistories_Members_TargetMemberId",
                table: "TaskHistories");

            migrationBuilder.DropIndex(
                name: "IX_TaskHistories_TargetMemberId",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "ActionType",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "TargetMemberId",
                table: "TaskHistories");
        }
    }
}
