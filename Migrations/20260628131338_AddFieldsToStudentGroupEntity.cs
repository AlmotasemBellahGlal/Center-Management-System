using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Center_Management.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldsToStudentGroupEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "EnrollmentDate",
                table: "StudentGroups",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StudentGroups",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StudentGroups");
        }
    }
}
