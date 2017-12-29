namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class UserAnswerConfiguration : EntityTypeConfiguration<UserAnswerVote>
    {
        public UserAnswerConfiguration()
        {
            this.HasKey(ua => new { ua.UserId, ua.AnswerId });

            this.HasRequired(ua => ua.User)
                .WithMany(u => u.AnswerVotes)
                .HasForeignKey(ua => ua.UserId);

            this.HasRequired(ua => ua.Answer)
                .WithMany(a => a.Votes)
                .HasForeignKey(ua => ua.AnswerId);
        }
    }
}