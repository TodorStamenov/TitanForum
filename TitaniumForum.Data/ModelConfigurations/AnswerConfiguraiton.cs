namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class AnswerConfiguraiton : EntityTypeConfiguration<Answer>
    {
        public AnswerConfiguraiton()
        {
            this.HasMany(a => a.Comments)
                .WithRequired(c => c.Answer)
                .HasForeignKey(c => c.AnswerId);
        }
    }
}