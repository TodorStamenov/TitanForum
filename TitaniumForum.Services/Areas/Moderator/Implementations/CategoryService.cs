namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using Data.Contracts;
    using Data.Models;
    using Models.Categories;
    using Services.Areas.Moderator.Models.SubCategories;
    using Services.Implementations;
    using Services.Models.Categories;
    using Services.Models.SubCategories;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryService : Service, ICategoryService
    {
        public CategoryService(IDatabase database)
            : base(database)
        {
        }

        public bool Exists(int id)
        {
            return this.Database
                .Categories
                .Any(sc => sc.Id == id);
        }

        public bool IsDeleted(int id)
        {
            return this.Database
                .Categories
                .Any(c => c.Id == id && c.IsDeleted);
        }

        public bool HasQuestions(int id)
        {
            return this.Database
                .Categories
                .Any(c => c.Id == id
                    && c.SubCategories
                        .SelectMany(sc => sc.Questions)
                        .Where(q => !q.IsDeleted)
                        .Any());
        }

        public bool NameExists(string name)
        {
            return this.Database
                .Categories
                .Any(c => c.Name == name);
        }

        public string GetName(int id)
        {
            return this.Database
                .Categories
                .ProjectSingle(
                    projection: c => c.Name,
                    filter: c => c.Id == id);
        }

        public bool Create(string name)
        {
            if (this.NameExists(name))
            {
                return false;
            }

            Category category = new Category
            {
                Name = name
            };

            this.Database.Categories.Add(category);
            this.Database.Save();

            return true;
        }

        public bool Edit(int id, string name)
        {
            Category category = this.Database.Categories.Find(id);

            if (category == null
                || (this.NameExists(name)
                    && category.Name != name))
            {
                return false;
            }

            category.Name = name;

            this.Database.Save();

            return true;
        }

        public bool Delete(int id)
        {
            Category category = this.Database.Categories.Find(id);

            if (category == null
                || category.IsDeleted)
            {
                return false;
            }

            category.IsDeleted = true;

            foreach (var subCategory in category.SubCategories)
            {
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

                subCategory.IsDeleted = true;
            }

            this.Database.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Category category = this.Database.Categories.Find(id);

            if (category == null
                || !category.IsDeleted)
            {
                return false;
            }

            category.IsDeleted = false;

            this.Database.Save();

            return true;
        }

        public CategoryFormServiceModel GetForm(int id)
        {
            return this.Database
                .Categories
                .ProjectSingle(
                    projection: c => new CategoryFormServiceModel { Name = c.Name },
                    filter: c => c.Id == id);
        }

        public IEnumerable<MenuCategoryServiceModel> GetMenu()
        {
            return this.Database
                .Categories
                .Project(
                    projection: c => new MenuCategoryServiceModel
                    {
                        Id = c.Id,
                        Name = c.Name,
                        SubCategories = c.SubCategories
                            .Where(sc => !sc.IsDeleted)
                            .Select(sc => new MenuSubCategoryServiceModel
                            {
                                Id = sc.Id,
                                Name = sc.Name
                            })
                    },
                    filter: c => !c.IsDeleted
                        && c.SubCategories.Any(sc => sc.Questions.Any(q => !q.IsDeleted)));
        }

        public IEnumerable<ListCategoriesServiceModel> All()
        {
            return this.Database
                .Categories
                .Project(c => new ListCategoriesServiceModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    IsDeleted = c.IsDeleted,
                    SubCategories = c.SubCategories
                        .Select(s => new ListSubCategoriesServiceModel
                        {
                            Id = s.Id,
                            Name = s.Name,
                            IsDeleted = s.IsDeleted
                        })
                });
        }
    }
}