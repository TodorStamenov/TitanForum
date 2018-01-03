namespace TitaniumForum.Services.Areas.Moderator.Models.SubCategories
{
    using Common.Mapping;
    using Data.Models;

    public class ListSubCategoriesServiceModel : IMapFrom<SubCategory>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }
    }
}