namespace TitaniumForum.Services.Areas.Moderator.Models.Categories
{
    using Common.Mapping;
    using Data.Models;

    public class CategoryInfoServiceModel : IMapFrom<Category>
    {
        public bool IsDeleted { get; set; }
    }
}