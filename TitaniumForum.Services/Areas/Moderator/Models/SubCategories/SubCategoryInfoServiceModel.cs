namespace TitaniumForum.Services.Areas.Moderator.Models.SubCategories
{
    using Common.Mapping;
    using Data.Models;

    public class SubCategoryInfoServiceModel : IMapFrom<SubCategory>
    {
        public bool IsDeleted { get; set; }
    }
}