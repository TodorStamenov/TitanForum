namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Models.Categories;
    using Models.SubCategories;
    using Services.Implementations;
    using System.Collections.Generic;
    using System.Linq;
    using TitaniumForum.Services.Models.SubCategories;

    public class SubCategoryService : Service, ISubCategoryService
    {
        public SubCategoryService(IDatabase database)
            : base(database)
        {
        }

        public bool Exists(int id)
        {
            return this.Database
                 .SubCategories
                 .Any(sc => sc.Id == id);
        }

        public bool IsDeleted(int id)
        {
            return this.Database
                .SubCategories
                .ProjectSingle(
                    projection: c => c.IsDeleted,
                    filter: c => c.Id == id);
        }

        public bool HasQuestions(int id)
        {
            return this.Database
                .SubCategories
                .Any(sc => sc.Id == id
                    && sc.Questions.Where(q => !q.IsDeleted).Any());
        }

        public bool NameExists(string name)
        {
            return this.Database
                .SubCategories
                .Any(sc => sc.Name == name);
        }

        public string GetName(int id)
        {
            return this.Database
                .SubCategories
                .ProjectSingle(
                    projection: sc => sc.Name,
                    filter: sc => sc.Id == id);
        }

        public bool Create(int categoryId, string name)
        {
            CategoryInfoServiceModel categoryInfo = this.GetCategoryInfo(categoryId);

            if (this.NameExists(name)
                || categoryInfo == null
                || categoryInfo.IsDeleted)
            {
                return false;
            }

            SubCategory subCategory = new SubCategory
            {
                CategoryId = categoryId,
                Name = name
            };

            this.Database.SubCategories.Add(subCategory);
            this.Database.Save();

            return true;
        }

        public bool Edit(int id, int categoryId, string name)
        {
            SubCategory subCategory = this.Database.SubCategories.Find(id);

            CategoryInfoServiceModel categoryInfo = this.GetCategoryInfo(categoryId);

            if (subCategory == null
                || categoryInfo == null
                || categoryInfo.IsDeleted
                || (this.NameExists(name)
                    && subCategory.Name != name))
            {
                return false;
            }

            subCategory.CategoryId = categoryId;
            subCategory.Name = name;

            this.Database.Save();

            return true;
        }

        public bool Delete(int id)
        {
            SubCategory subCategory = this.Database.SubCategories.Find(id);

            if (subCategory == null
                || subCategory.IsDeleted)
            {
                return false;
            }

            subCategory.IsDeleted = true;

            foreach (var question in subCategory.Questions)
            {
                foreach (var answer in question.Answers)
                {
                    foreach (var comment in answer.Comments)
                    {
                        comment.IsDeleted = true;
                    }

                    answer.IsDeleted = true;
                }

                question.IsDeleted = true;
            }

            this.Database.Save();

            return true;
        }

        public bool Restore(int id)
        {
            SubCategory subCategory = this.Database.SubCategories.Find(id);

            if (subCategory == null
                || !subCategory.IsDeleted
                || subCategory.Category.IsDeleted)
            {
                return false;
            }

            subCategory.IsDeleted = false;

            this.Database.Save();

            return true;
        }

        public SubCategoryFormServiceModel GetForm(int id)
        {
            return this.Database
                .SubCategories
                .ProjectSingle(
                    projection: sc => new SubCategoryFormServiceModel { Name = sc.Name },
                    filter: sc => sc.Id == id);
        }

        public IEnumerable<MenuSubCategoryServiceModel> GetMenu()
        {
            return this.Database
                .SubCategories
                .Project(
                    projection: sc => new MenuSubCategoryServiceModel
                    {
                        Id = sc.Id,
                        Name = sc.Name
                    },
                    filter: sc => !sc.IsDeleted,
                    orderBy: q => q.OrderBy(sc => sc.Name));
        }

        private CategoryInfoServiceModel GetCategoryInfo(int categoryId)
        {
            return this.Database
                .Categories
                .ProjectSingle(
                    projection: c => new CategoryInfoServiceModel { IsDeleted = c.IsDeleted },
                    filter: c => c.Id == categoryId);
        }
    }
}