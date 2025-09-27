using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class updateCompetency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CompetencyTracker",
                table: "CompetencyTracker");

            migrationBuilder.DropColumn(
                name: "level",
                table: "CompetencyTracker");

            migrationBuilder.AddColumn<long>(
                name: "competencyTrackerId",
                table: "CompetencyTracker",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "levelId",
                table: "CompetencyTracker",
                type: "bigint",
                nullable: false,
                defaultValue: 1L);

            migrationBuilder.AddColumn<string>(
                name: "competencyDisplayId",
                table: "Competency",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompetencyTracker",
                table: "CompetencyTracker",
                column: "competencyTrackerId");

            migrationBuilder.CreateTable(
                name: "Level",
                columns: table => new
                {
                    levelId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    rank = table.Column<short>(type: "smallint", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Level", x => x.levelId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompetencyTracker_levelId",
                table: "CompetencyTracker",
                column: "levelId");

            migrationBuilder.CreateIndex(
                name: "IX_CompetencyTracker_userId",
                table: "CompetencyTracker",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_CompetencyTracker_Id",
                table: "CompetencyTracker",
                column: "competencyTrackerId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Competency_Id",
                table: "Competency",
                column: "competencyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_Level_levelId",
                table: "Level",
                column: "levelId",
                unique: true);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_CompetencyTracker_Level",
            //    table: "CompetencyTracker",
            //    column: "levelId",
            //    principalTable: "Level",
            //    principalColumn: "levelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompetencyTracker_Level",
                table: "CompetencyTracker");

            migrationBuilder.DropTable(
                name: "Level");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CompetencyTracker",
                table: "CompetencyTracker");

            migrationBuilder.DropIndex(
                name: "IX_CompetencyTracker_levelId",
                table: "CompetencyTracker");

            migrationBuilder.DropIndex(
                name: "IX_CompetencyTracker_userId",
                table: "CompetencyTracker");

            migrationBuilder.DropIndex(
                name: "UQ_CompetencyTracker_Id",
                table: "CompetencyTracker");

            migrationBuilder.DropIndex(
                name: "UQ_Competency_Id",
                table: "Competency");

            migrationBuilder.DropColumn(
                name: "competencyTrackerId",
                table: "CompetencyTracker");

            migrationBuilder.DropColumn(
                name: "levelId",
                table: "CompetencyTracker");

            migrationBuilder.DropColumn(
                name: "competencyDisplayId",
                table: "Competency");

            migrationBuilder.AddColumn<string>(
                name: "level",
                table: "CompetencyTracker",
                type: "varchar(50)",
                unicode: false,
                maxLength: 50,
                nullable: false,
                defaultValue: "Emerging");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CompetencyTracker",
                table: "CompetencyTracker",
                columns: new[] { "userId", "competencyId", "level" });
        }
    }
}
