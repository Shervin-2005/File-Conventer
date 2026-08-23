using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FileConverter.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class UserNameIsUiniqueNow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName_Unique",
                table: "Users",
                column: "UserName",
                unique: true,
                filter: "\"IsDeleted\" = false");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName_Unique",
                table: "Users");
        }
    }
}
