using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_Management.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSubjectModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AcademicYears_Subjects_SubjectId",
                table: "AcademicYears");

            migrationBuilder.DropForeignKey(
                name: "FK_Matrials_Subjects_SubjectId",
                table: "Matrials");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Matrials_SubjectId",
                table: "Matrials");

            migrationBuilder.DropIndex(
                name: "IX_AcademicYears_SubjectId",
                table: "AcademicYears");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "Matrials");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "AcademicYears");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "Matrials",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "AcademicYears",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Matrials_SubjectId",
                table: "Matrials",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_AcademicYears_SubjectId",
                table: "AcademicYears",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AcademicYears_Subjects_SubjectId",
                table: "AcademicYears",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Matrials_Subjects_SubjectId",
                table: "Matrials",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
