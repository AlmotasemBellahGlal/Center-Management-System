using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueConstraintAndFKToAttendence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendences_Groups_GroupId",
                table: "Attendences");

            migrationBuilder.DropIndex(
                name: "IX_Attendences_StudentId",
                table: "Attendences");

            migrationBuilder.CreateIndex(
                name: "IX_Attendences_StudentId_GroupId_Date",
                table: "Attendences",
                columns: new[] { "StudentId", "GroupId", "Date" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Attendences_Groups_GroupId",
                table: "Attendences",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendences_Groups_GroupId",
                table: "Attendences");

            migrationBuilder.DropIndex(
                name: "IX_Attendences_StudentId_GroupId_Date",
                table: "Attendences");

            migrationBuilder.CreateIndex(
                name: "IX_Attendences_StudentId",
                table: "Attendences",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendences_Groups_GroupId",
                table: "Attendences",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
