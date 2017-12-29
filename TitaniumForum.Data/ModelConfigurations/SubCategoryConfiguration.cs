namespace TitaniumForum.Data.ModelConfigurations
{
    using Models;
    using System.Data.Entity.ModelConfiguration;

    public class SubCategoryConfiguration : EntityTypeConfiguration<SubCategory>
    {
        public SubCategoryConfiguration()
        {
            this.HasMany(sc => sc.Questions)
                .WithRequired(q => q.SubCategory)
                .HasForeignKey(q => q.SubCategoryId);
        }
    }
}