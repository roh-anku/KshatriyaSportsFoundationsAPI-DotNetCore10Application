using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KshatriyaSportsFoundations.API.Migrations
{
    /// <inheritdoc />
    public partial class addednewfieldstoEnquiryDomain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminComments",
                table: "Enquiries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Fullfilled",
                table: "Enquiries",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminComments",
                table: "Enquiries");

            migrationBuilder.DropColumn(
                name: "Fullfilled",
                table: "Enquiries");
        }
    }
}
