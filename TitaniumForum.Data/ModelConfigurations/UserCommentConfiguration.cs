namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class UserCommentConfiguration : EntityTypeConfiguration<UserCommentVote>
    {
        public UserCommentConfiguration()
        {
            this.HasKey(uc => new { uc.UserId, uc.CommentId });

            this.HasRequired(uc => uc.User)
                .WithMany(u => u.CommentVotes)
                .HasForeignKey(uc => uc.UserId);

            this.HasRequired(uc => uc.Comment)
                .WithMany(c => c.Votes)
                .HasForeignKey(uc => uc.CommentId);
        }
    }
}