using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ManagerData.Migrations
{
    /// <inheritdoc />
    public partial class ChangeOnDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartLinks_Parts_MasterId",
                table: "PartLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_PartLinks_Parts_SlaveId",
                table: "PartLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_PartLinks_Parts_MasterId",
                table: "PartLinks",
                column: "MasterId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartLinks_Parts_SlaveId",
                table: "PartLinks",
                column: "SlaveId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartLinks_Parts_MasterId",
                table: "PartLinks");

            migrationBuilder.DropForeignKey(
                name: "FK_PartLinks_Parts_SlaveId",
                table: "PartLinks");

            migrationBuilder.AddForeignKey(
                name: "FK_PartLinks_Parts_MasterId",
                table: "PartLinks",
                column: "MasterId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartLinks_Parts_SlaveId",
                table: "PartLinks",
                column: "SlaveId",
                principalTable: "Parts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
