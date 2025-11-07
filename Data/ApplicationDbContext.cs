using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentPortfolio.Models;

namespace StudentPortfolio.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<CareerDevelopmentPlan> CareerDevelopmentPlans { get; set; }

        public virtual DbSet<Cdl> Cdls { get; set; }

        public virtual DbSet<Competency> Competencies { get; set; }

        public virtual DbSet<CompetencyTracker> CompetencyTrackers { get; set; }

        public virtual DbSet<Goal> Goals { get; set; }

        public virtual DbSet<GoalStep> GoalSteps { get; set; }

        public virtual DbSet<IndustryContactInfo> IndustryContactInfos { get; set; }

        public virtual DbSet<IndustryContactLog> IndustryContactLogs { get; set; }

        public virtual DbSet<Level> Levels { get; set; }

        public virtual DbSet<NetworkingEvent> NetworkingEvents { get; set; }

        public virtual DbSet<NetworkingQuestion> NetworkingQuestions { get; set; }

        public virtual DbSet<UserLink> UserLinks { get; set; }

        public virtual DbSet<Feedback> Feedbacks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<CareerDevelopmentPlan>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.Year });

                entity.ToTable("CareerDevelopmentPlan");

                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.Year).HasColumnName("year");
                entity.Property(e => e.DevelopmentFocus)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("developmentFocus");
                entity.Property(e => e.EmployersOfInterest)
                    .HasMaxLength(255)
                    .HasColumnName("employersOfInterest");
                entity.Property(e => e.Extracurricular)
                    .HasMaxLength(255)
                    .HasColumnName("extracurricular");
                entity.Property(e => e.NetworkingPlan)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("networkingPlan");
                entity.Property(e => e.PersonalValues)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("personalValues");
                entity.Property(e => e.ProfessionalInterests)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("professionalInterests");

                entity.HasOne(d => d.User).WithMany(p => p.CareerDevelopmentPlans)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_CareerDevelopmentPlan_AspNetUsers");
            });

            modelBuilder.Entity<Cdl>(entity =>
            {
                entity.ToTable("CDL");

                entity.HasIndex(e => e.CdlId, "UQ_CDL_CdlId").IsUnique();

                entity.Property(e => e.CdlId).HasColumnName("cdlId");
                entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("dateCreated");
                entity.Property(e => e.Description)
                    .HasMaxLength(500)
                    .IsUnicode(false)
                    .HasColumnName("description");
                entity.Property(e => e.LastUpdated)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("lastUpdated");
                entity.Property(e => e.Link)
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasColumnName("link");
                entity.Property(e => e.Title)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("title");
                entity.Property<byte[]>("IconImage")
                    .HasColumnType("varbinary(max)");
            });

            modelBuilder.Entity<Competency>(entity =>
            {
                entity.ToTable("Competency");

                entity.HasIndex(e => e.CompetencyId, "UQ_Competency_Id").IsUnique();

                entity.Property(e => e.CompetencyId).HasColumnName("competencyId");
                entity.Property(e => e.CompetencyDisplayId)
                    .HasMaxLength(50)
                    .HasColumnName("competencyDisplayId");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("description");
                entity.Property(e => e.LastUpdated)
                    .HasDefaultValueSql("(sysdatetimeoffset())")
                    .HasColumnName("lastUpdated");
                entity.Property(e => e.LinkToIndicators)
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasColumnName("linkToIndicators");
                entity.Property(e => e.EndDate).HasColumnName("endDate");
                entity.Property(e => e.ParentCompetencyId).HasColumnName("parentCompetencyId");
            });

            modelBuilder.Entity<CompetencyTracker>(entity =>
            {
                entity.HasKey(e => e.CompetencyTrackerId);

                entity.ToTable("CompetencyTracker");

                entity.HasIndex(e => e.CompetencyTrackerId, "UQ_CompetencyTracker_Id").IsUnique();

                entity.Property(e => e.CompetencyTrackerId).HasColumnName("competencyTrackerId");
                entity.Property(e => e.UserId).HasColumnName("userId");
                entity.Property(e => e.CompetencyId).HasColumnName("competencyId");
                entity.Property(e => e.LevelId).HasColumnName("levelId");
                entity.Property(e => e.Created)
                    .HasDefaultValueSql("(sysdatetimeoffset())")
                    .HasColumnName("created");
                entity.Property(e => e.EndDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("endDate");
                entity.Property(e => e.Evidence)
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasColumnName("evidence");
                entity.Property(e => e.LastUpdated)
                    .HasDefaultValueSql("(sysdatetimeoffset())")
                    .HasColumnName("lastUpdated");
                entity.Property(e => e.SkillsReview)
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasColumnName("skillsReview");
                entity.Property(e => e.StartDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("startDate");

                entity.HasOne(d => d.Competency).WithMany(p => p.CompetencyTrackers)
                    .HasForeignKey(d => d.CompetencyId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompetencyTracker_Competency");

                entity.HasOne(d => d.User).WithMany(p => p.CompetencyTrackers)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_CompetencyTracker_AspNetUsers");

                //entity.HasOne(d => d.Level).WithMany(p => p.CompetencyTrackers)
                //    .HasForeignKey(d => d.LevelId)
                //    .OnDelete(DeleteBehavior.ClientSetNull)
                //    .HasConstraintName("FK_CompetencyTracker_Level");
            });

            modelBuilder.Entity<Goal>(entity =>
            {
                entity.ToTable("Goal");

                entity.HasIndex(e => e.GoalId, "UQ_Goal_goalId").IsUnique();

                entity.Property(e => e.GoalId).HasColumnName("goalId");
                entity.Property(e => e.CompleteDate).HasColumnName("completeDate");
                entity.Property(e => e.CompletionNotes)
                    .HasMaxLength(255)
                    .HasColumnName("completionNotes");
                entity.Property(e => e.DateSet)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("dateSet");
                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .HasColumnName("description");
                entity.Property(e => e.EndDate).HasColumnName("endDate");
                entity.Property(e => e.Learnings)
                    .HasMaxLength(300)
                    .HasColumnName("learnings");
                entity.Property(e => e.Progress)
                    .HasMaxLength(255)
                    .HasColumnName("progress");
                entity.Property(e => e.StartDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("startDate");
                entity.Property(e => e.Timeline)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .HasColumnName("timeline");
                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("userId");

                entity.HasOne(d => d.User).WithMany(p => p.Goals)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Goal_AspNetUsers");
            });

            modelBuilder.Entity<GoalStep>(entity =>
            {
                entity.HasKey(e => e.StepId);

                entity.HasIndex(e => e.StepId, "UQ_GoalSteps_stepId").IsUnique();

                entity.Property(e => e.StepId).HasColumnName("stepId");
                entity.Property(e => e.GoalId).HasColumnName("goalId");
                entity.Property(e => e.Step)
                    .HasMaxLength(255)
                    .HasColumnName("step");

                entity.HasOne(d => d.Goal).WithMany(p => p.GoalSteps)
                    .HasForeignKey(d => d.GoalId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GoalSteps_Goal");
            });

            modelBuilder.Entity<IndustryContactInfo>(entity =>
            {
                entity.HasKey(e => e.ContactInfoId);

                entity.ToTable("IndustryContactInfo");

                entity.HasIndex(e => e.ContactInfoId, "UQ_IndustryContactInfo_Id").IsUnique();

                entity.Property(e => e.ContactInfoId).HasColumnName("contactInfoId");
                entity.Property(e => e.ContactDetails)
                    .HasMaxLength(255)
                    .HasColumnName("contactDetails");
                entity.Property(e => e.ContactId).HasColumnName("contactId");
                entity.Property(e => e.ContactType)
                    .HasMaxLength(255)
                    .HasColumnName("contactType");

                entity.HasOne(d => d.Contact).WithMany(p => p.IndustryContactInfos)
                    .HasForeignKey(d => d.ContactId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndustryContactInfo_IndustryContactLog");
            });

            modelBuilder.Entity<IndustryContactLog>(entity =>
            {
                entity.HasKey(e => e.ContactId);

                entity.ToTable("IndustryContactLog");

                entity.HasIndex(e => e.ContactId, "UQ_IndustryContactLog_contactId").IsUnique();

                entity.Property(e => e.ContactId).HasColumnName("contactId");
                entity.Property(e => e.Company)
                    .HasMaxLength(250)
                    .HasColumnName("company");
                entity.Property(e => e.DateMet).HasColumnName("dateMet");
                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");
                entity.Property(e => e.Notes)
                    .HasMaxLength(500)
                    .HasColumnName("notes");
                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("userId");

                entity.HasOne(d => d.User).WithMany(p => p.IndustryContactLogs)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_IndustryContactLog_AspNetUsers");
            });

            modelBuilder.Entity<Level>(entity =>
            {
                entity.HasKey(e => e.LevelId);

                entity.ToTable("Level");

                entity.HasIndex(e => e.LevelId, "UQ_Level_levelId").IsUnique();

                entity.Property(e => e.LevelId).HasColumnName("levelId");
                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
                entity.Property(e => e.Rank)
                    .HasMaxLength(50)
                    .HasColumnName("rank");
            });

            modelBuilder.Entity<NetworkingEvent>(entity =>
            {
                entity.HasKey(e => e.EventId);

                entity.ToTable("NetworkingEvent");

                entity.HasIndex(e => e.EventId, "UQ_NetworkingEvent_eventId").IsUnique();

                entity.Property(e => e.EventId).HasColumnName("eventId");
                entity.Property(e => e.Date).HasColumnName("date");
                entity.Property(e => e.Details)
                    .HasMaxLength(255)
                    .HasColumnName("details");
                entity.Property(e => e.Location)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .HasColumnName("location");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .HasColumnName("name");
                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("userId");

                entity.HasOne(d => d.User).WithMany(p => p.NetworkingEvents)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NetworkingEvent_AspNetUsers");
            });

            modelBuilder.Entity<NetworkingQuestion>(entity =>
            {
                entity.HasKey(e => e.NetworkingQuestionsId);

                entity.HasIndex(e => e.NetworkingQuestionsId, "UQ_NetworkingQuestions_networkingQuestionsId").IsUnique();

                entity.Property(e => e.NetworkingQuestionsId).HasColumnName("networkingQuestionsId");
                entity.Property(e => e.EventId).HasColumnName("eventId");
                entity.Property(e => e.Question)
                    .HasMaxLength(255)
                    .HasColumnName("question");

                entity.HasOne(d => d.Event).WithMany(p => p.NetworkingQuestions)
                    .HasForeignKey(d => d.EventId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_NetworkingQuestions_eventId");
            });

            modelBuilder.Entity<UserLink>(entity =>
            {
                entity.HasKey(e => e.LinkId);

                entity.HasIndex(e => e.LinkId, "UQ_UserLinks_linkId").IsUnique();

                entity.Property(e => e.LinkId).HasColumnName("linkId");
                entity.Property(e => e.Link)
                    .HasMaxLength(2048)
                    .IsUnicode(false)
                    .HasColumnName("link");
                entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("userId");

                entity.HasOne(d => d.User).WithMany(p => p.UserLinks)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_UserLinks_AspNetUsers");
            });

          modelBuilder.Entity<Feedback>(entity => {

              entity.HasKey(e => e.FeedbackId);

              entity.ToTable("Feedback");

              entity.HasIndex(e => e.FeedbackId, "UQ_UserLinks_feedbackID").IsUnique();

              entity.Property(e => e.FeedbackId).HasColumnName("feedbackId");

              entity.Property(e => e.UserId)
                    .HasMaxLength(450)
                    .HasColumnName("userId");
              entity.Property(e => e.GoalId).HasColumnName("goalId");
              entity.Property(e => e.CompetencyTrackerId).HasColumnName("competencyTrackerId");
              entity.Property(e => e.FeedbackText)
                    .HasMaxLength(255)
                    .HasColumnName("FeedbackText");
              entity.Property(e => e.DateCreated)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("dateCreated");
              entity.Property(e => e.DateUpdated)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnName("dateUpdated");

              entity.HasOne(d => d.User).WithMany(p => p.Feedbacks)
                   .HasForeignKey(d => d.UserId)
                   .OnDelete(DeleteBehavior.ClientSetNull)
                   .HasConstraintName("FK_Feedback_AspNetUsers");

              entity.HasOne(d => d.Goal).WithMany(p => p.Feedbacks)
                  .HasForeignKey(d => d.GoalId)
                  .IsRequired(false) 
                  .HasConstraintName("FK_Feedback_Goal"); 

              entity.HasOne(d => d.CompetencyTracker).WithMany(p => p.Feedbacks) 
                  .HasForeignKey(d => d.CompetencyTrackerId)
                  .IsRequired(false) 
                  .HasConstraintName("FK_Feedback_CompetencyTracker"); 



          });


        }

    }
}
