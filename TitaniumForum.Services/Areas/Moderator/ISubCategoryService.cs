namespace TitaniumForum.Services.Areas.Moderator
{
    using Models.SubCategories;

    public interface ISubCategoryService
    {
        bool IsDeleted(int id);

        bool Exists(int id);

        bool NameExists(string name);

        string GetName(int id);

        bool Create(int categoryId, string name);

        bool Edit(int id, int categoryId, string name);

        bool Delete(int id);

        bool Restore(int id);

        SubCategoryFormServiceModel GetForm(int id);
    }
}