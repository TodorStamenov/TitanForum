namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class TagQuestionConfiguration : EntityTypeConfiguration<TagQuestion>
    {
        public TagQuestionConfiguration()
        {
            this.HasKey(tq => new { tq.TagId, tq.QuestionId });

            this.HasRequired(tq => tq.Tag)
                .WithMany(t => t.Questions)
                .HasForeignKey(tq => tq.TagId);

            this.HasRequired(tq => tq.Question)
                .WithMany(q => q.Tags)
                .HasForeignKey(tq => tq.QuestionId);
        }
    }
}