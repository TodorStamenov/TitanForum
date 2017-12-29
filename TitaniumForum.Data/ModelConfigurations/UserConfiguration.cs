namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class UserConfiguration : EntityTypeConfiguration<User>
    {
        public UserConfiguration()
        {
            this.HasMany(u => u.Questions)
                .WithRequired(q => q.Author)
                .HasForeignKey(q => q.AuthorId)
                .WillCascadeOnDelete(false);

            this.HasMany(u => u.Answers)
                .WithRequired(q => q.Author)
                .HasForeignKey(q => q.AuthorId)
                .WillCascadeOnDelete(false);

            this.HasMany(u => u.Comments)
                .WithRequired(q => q.Author)
                .HasForeignKey(q => q.AuthorId)
                .WillCascadeOnDelete(false);
        }
    }
}