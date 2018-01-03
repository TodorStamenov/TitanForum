namespace TitaniumForum.Services.Areas.Moderator.Models.Categories
{
    using Common.Mapping;
    using Data.Models;
    using SubCategories;
    using System.Collections.Generic;

    public class ListCategoriesServiceModel : IMapFrom<Category>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<ListSubCategoriesServiceModel> SubCategories { get; set; }
    }
}