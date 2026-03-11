using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KshatriyaSportsFoundations.API.Migrations
{
    /// <inheritdoc />
    public partial class addedLocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Location",
                table: "Enquiries");
        }
    }
}
