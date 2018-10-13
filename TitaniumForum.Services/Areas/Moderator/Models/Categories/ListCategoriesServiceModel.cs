namespace TitaniumForum.Services.Areas.Moderator.Models.Categories
{
    using SubCategories;
    using System.Collections.Generic;

    public class ListCategoriesServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public IEnumerable<ListSubCategoriesServiceModel> SubCategories { get; set; }
    }
}