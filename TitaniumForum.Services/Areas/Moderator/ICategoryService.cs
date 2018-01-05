namespace TitaniumForum.Services.Areas.Moderator
{
    using Models.Categories;
    using Services.Models.Categories;
    using System.Collections.Generic;

    public interface ICategoryService
    {
        bool IsDeleted(int id);

        bool Exists(int id);

        bool NameExists(string name);

        string GetName(int id);

        bool Create(string name);

        bool Edit(int id, string name);

        bool Delete(int id);

        bool Restore(int id);

        CategoryFormServiceModel GetForm(int id);

        IEnumerable<MenuCategoryServiceModel> GetMenu();

        IEnumerable<ListCategoriesServiceModel> All();
    }
}