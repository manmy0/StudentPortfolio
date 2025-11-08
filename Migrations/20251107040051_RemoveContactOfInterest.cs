using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class RemoveContactOfInterest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContactsOfInterest");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContactsOfInterest",
                columns: table => new
                {
                    contactOfInterestId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    contactDetails = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContactsOfInterest", x => x.contactOfInterestId);
                    table.ForeignKey(
                        name: "FK_ContactsOfInterest_Users",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactsOfInterest_userId",
                table: "ContactsOfInterest",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_ContactsOfInterest_Id",
                table: "ContactsOfInterest",
                column: "contactOfInterestId",
                unique: true);
        }
    }
}
