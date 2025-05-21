using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class AddDescriptionForTaskHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "TaskHistories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "TaskHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "TaskHistories");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "TaskHistories");
        }
    }
}
