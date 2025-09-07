using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentPortfolio.Data.Migrations
{
    /// <inheritdoc />
    public partial class addtables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CareerDevelopmentPlan",
                columns: table => new
                {
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    year = table.Column<short>(type: "smallint", nullable: false),
                    professionalInterests = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    employersOfInterest = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    personalValues = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    developmentFocus = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    extracurricular = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    networkingPlan = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CareerDevelopmentPlan", x => new { x.userId, x.year });
                    table.ForeignKey(
                        name: "FK_CareerDevelopmentPlan_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CDL",
                columns: table => new
                {
                    cdlId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    dateCreated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())"),
                    title = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    link = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CDL", x => x.cdlId);
                });

            migrationBuilder.CreateTable(
                name: "Competency",
                columns: table => new
                {
                    competencyId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    parentCompetencyId = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    linkToIndicators = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetimeoffset())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Competency", x => x.competencyId);
                });

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

            migrationBuilder.CreateTable(
                name: "Goal",
                columns: table => new
                {
                    goalId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    dateSet = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    timeline = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    progress = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    learnings = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    startDate = table.Column<DateOnly>(type: "date", nullable: true, defaultValueSql: "(getdate())"),
                    endDate = table.Column<DateOnly>(type: "date", nullable: true),
                    completeDate = table.Column<DateOnly>(type: "date", nullable: true),
                    completionNotes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Goal", x => x.goalId);
                    table.ForeignKey(
                        name: "FK_Goal_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IndustryContactLog",
                columns: table => new
                {
                    contactId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    company = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    dateMet = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryContactLog", x => x.contactId);
                    table.ForeignKey(
                        name: "FK_IndustryContactLog_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NetworkingEvent",
                columns: table => new
                {
                    eventId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    location = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    details = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkingEvent", x => x.eventId);
                    table.ForeignKey(
                        name: "FK_NetworkingEvent_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserLinks",
                columns: table => new
                {
                    linkId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    link = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLinks", x => x.linkId);
                    table.ForeignKey(
                        name: "FK_UserLinks_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CompetencyTracker",
                columns: table => new
                {
                    userId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    competencyId = table.Column<long>(type: "bigint", nullable: false),
                    level = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false, defaultValue: "Emerging"),
                    startDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    endDate = table.Column<DateOnly>(type: "date", nullable: false, defaultValueSql: "(getdate())"),
                    skillsReview = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    evidence = table.Column<string>(type: "varchar(2048)", unicode: false, maxLength: 2048, nullable: true),
                    created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetimeoffset())"),
                    lastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetimeoffset())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompetencyTracker", x => new { x.userId, x.competencyId, x.level });
                    table.ForeignKey(
                        name: "FK_CompetencyTracker_AspNetUsers",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CompetencyTracker_Competency",
                        column: x => x.competencyId,
                        principalTable: "Competency",
                        principalColumn: "competencyId");
                });

            migrationBuilder.CreateTable(
                name: "GoalSteps",
                columns: table => new
                {
                    stepId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    goalId = table.Column<long>(type: "bigint", nullable: false),
                    step = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoalSteps", x => x.stepId);
                    table.ForeignKey(
                        name: "FK_GoalSteps_Goal",
                        column: x => x.goalId,
                        principalTable: "Goal",
                        principalColumn: "goalId");
                });

            migrationBuilder.CreateTable(
                name: "IndustryContactInfo",
                columns: table => new
                {
                    contactInfoId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contactId = table.Column<long>(type: "bigint", nullable: false),
                    contactType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    contactDetails = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IndustryContactInfo", x => x.contactInfoId);
                    table.ForeignKey(
                        name: "FK_IndustryContactInfo_IndustryContactLog",
                        column: x => x.contactId,
                        principalTable: "IndustryContactLog",
                        principalColumn: "contactId");
                });

            migrationBuilder.CreateTable(
                name: "NetworkingQuestions",
                columns: table => new
                {
                    networkingQuestionsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    eventId = table.Column<long>(type: "bigint", nullable: false),
                    question = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NetworkingQuestions", x => x.networkingQuestionsId);
                    table.ForeignKey(
                        name: "FK_NetworkingQuestions_eventId",
                        column: x => x.eventId,
                        principalTable: "NetworkingEvent",
                        principalColumn: "eventId");
                });

            migrationBuilder.CreateIndex(
                name: "UQ_CDL_CdlId",
                table: "CDL",
                column: "cdlId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompetencyTracker_competencyId",
                table: "CompetencyTracker",
                column: "competencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ContactsOfInterest_userId",
                table: "ContactsOfInterest",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_ContactsOfInterest_Id",
                table: "ContactsOfInterest",
                column: "contactOfInterestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Goal_userId",
                table: "Goal",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_Goal_goalId",
                table: "Goal",
                column: "goalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GoalSteps_goalId",
                table: "GoalSteps",
                column: "goalId");

            migrationBuilder.CreateIndex(
                name: "UQ_GoalSteps_stepId",
                table: "GoalSteps",
                column: "stepId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryContactInfo_contactId",
                table: "IndustryContactInfo",
                column: "contactId");

            migrationBuilder.CreateIndex(
                name: "UQ_IndustryContactInfo_Id",
                table: "IndustryContactInfo",
                column: "contactInfoId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IndustryContactLog_userId",
                table: "IndustryContactLog",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_IndustryContactLog_contactId",
                table: "IndustryContactLog",
                column: "contactId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkingEvent_userId",
                table: "NetworkingEvent",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_NetworkingEvent_eventId",
                table: "NetworkingEvent",
                column: "eventId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NetworkingQuestions_eventId",
                table: "NetworkingQuestions",
                column: "eventId");

            migrationBuilder.CreateIndex(
                name: "UQ_NetworkingQuestions_networkingQuestionsId",
                table: "NetworkingQuestions",
                column: "networkingQuestionsId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserLinks_userId",
                table: "UserLinks",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "UQ_UserLinks_linkId",
                table: "UserLinks",
                column: "linkId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CareerDevelopmentPlan");

            migrationBuilder.DropTable(
                name: "CDL");

            migrationBuilder.DropTable(
                name: "CompetencyTracker");

            migrationBuilder.DropTable(
                name: "ContactsOfInterest");

            migrationBuilder.DropTable(
                name: "GoalSteps");

            migrationBuilder.DropTable(
                name: "IndustryContactInfo");

            migrationBuilder.DropTable(
                name: "NetworkingQuestions");

            migrationBuilder.DropTable(
                name: "UserLinks");

            migrationBuilder.DropTable(
                name: "Competency");

            migrationBuilder.DropTable(
                name: "Goal");

            migrationBuilder.DropTable(
                name: "IndustryContactLog");

            migrationBuilder.DropTable(
                name: "NetworkingEvent");
        }
    }
}
