using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeletBehaviourForTaskType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_PartTaskTypes_TaskTypeId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_PartTaskTypes_TaskTypeId",
                table: "Tasks",
                column: "TaskTypeId",
                principalTable: "PartTaskTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_PartTaskTypes_TaskTypeId",
                table: "Tasks");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_PartTaskTypes_TaskTypeId",
                table: "Tasks",
                column: "TaskTypeId",
                principalTable: "PartTaskTypes",
                principalColumn: "Id");
        }
    }
}
