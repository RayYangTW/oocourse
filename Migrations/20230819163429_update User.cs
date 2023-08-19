using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace personal_project.Migrations
{
    /// <inheritdoc />
    public partial class updateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Profiles_Users_userId",
                table: "Profiles");

            migrationBuilder.DropIndex(
                name: "IX_Teachers_userId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_TeacherApplications_userId",
                table: "TeacherApplications");

            migrationBuilder.DropIndex(
                name: "IX_Profiles_userId",
                table: "Profiles");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_userId",
                table: "Teachers",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherApplications_userId",
                table: "TeacherApplications",
                column: "userId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Teachers_userId",
                table: "Teachers");

            migrationBuilder.DropIndex(
                name: "IX_TeacherApplications_userId",
                table: "TeacherApplications");

            migrationBuilder.CreateIndex(
                name: "IX_Teachers_userId",
                table: "Teachers",
                column: "userId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeacherApplications_userId",
                table: "TeacherApplications",
                column: "userId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Profiles_userId",
                table: "Profiles",
                column: "userId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Profiles_Users_userId",
                table: "Profiles",
                column: "userId",
                principalTable: "Users",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
