namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class QuestionConfiguration : EntityTypeConfiguration<Question>

    {
        public QuestionConfiguration()
        {
            this.HasMany(q => q.Answers)
                .WithRequired(a => a.Question)
                .HasForeignKey(a => a.QuestionId);
        }
    }
}