namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class UserQuestionConfiguration : EntityTypeConfiguration<UserQuestionVote>
    {
        public UserQuestionConfiguration()
        {
            this.HasKey(uq => new { uq.UserId, uq.QuestionId });

            this.HasRequired(uq => uq.User)
                .WithMany(u => u.QuestionVotes)
                .HasForeignKey(uq => uq.UserId);

            this.HasRequired(uq => uq.Question)
                .WithMany(q => q.Votes)
                .HasForeignKey(uq => uq.QuestionId);
        }
    }
}