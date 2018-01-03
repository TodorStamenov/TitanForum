using System.Collections.Generic;
using TitaniumForum.Services.Areas.Moderator.Models.Categories;

namespace TitaniumForum.Services.Areas.Moderator
{
    public interface ICategoryService
    {
        bool NameExists(string name);

        string GetName(int id);

        bool Create(string name);

        bool Edit(int id, string name);

        bool Delete(int id);

        bool Restore(int id);

        CategoryFormServiceModel GetForm(int id);

        IEnumerable<ListCategoriesServiceModel> All();
    }
}