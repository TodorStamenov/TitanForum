namespace TitaniumForum.Services.Models.Categories
{
    using SubCategories;
    using System.Collections.Generic;

    public class MenuCategoryServiceModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public IEnumerable<MenuSubCategoryServiceModel> SubCategories { get; set; }
    }
}