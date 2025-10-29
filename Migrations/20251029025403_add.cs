using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortfolio.Migrations
{
    /// <inheritdoc />
    public partial class add : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Feedback",
                columns: table => new
                {
                    feedbackId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    goalId = table.Column<long>(type: "bigint", nullable: true),
                    competencyTrackerId = table.Column<long>(type: "bigint", nullable: true),
                    FeedbackText = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    dateUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedback", x => x.feedbackId);
                    table.ForeignKey(
                        name: "FK_Feedback_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Feedback_CompetencyTracker",
                        column: x => x.competencyTrackerId,
                        principalTable: "CompetencyTracker",
                        principalColumn: "competencyTrackerId");
                    table.ForeignKey(
                        name: "FK_Feedback_Goal",
                        column: x => x.goalId,
                        principalTable: "Goal",
                        principalColumn: "goalId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_competencyTrackerId",
                table: "Feedback",
                column: "competencyTrackerId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_goalId",
                table: "Feedback",
                column: "goalId");

            migrationBuilder.CreateIndex(
                name: "IX_Feedback_userId",
                table: "Feedback",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserLinks_feedbackID",
                table: "Feedback",
                column: "feedbackId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Feedback");
        }
    }
}
