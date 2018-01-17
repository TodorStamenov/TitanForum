namespace TitaniumForum.Services.Areas.Moderator.Implementations
{
    using AutoMapper.QueryableExtensions;
    using Data;
    using Data.Models;
    using Models.Categories;
    using Services.Models.Categories;
    using Services.Models.SubCategories;
    using System.Collections.Generic;
    using System.Linq;

    public class CategoryService : ICategoryService
    {
        private readonly UnitOfWork db;

        public CategoryService(UnitOfWork db)
        {
            this.db = db;
        }

        public bool Exists(int id)
        {
            return this.db
                .Categories
                .Get()
                .Any(sc => sc.Id == id);
        }

        public bool IsDeleted(int id)
        {
            return this.db
                .Categories
                .Get(filter: c => c.Id == id)
                .Select(c => c.IsDeleted)
                .FirstOrDefault();
        }

        public bool HasQuestions(int id)
        {
            return this.db
                .Categories
                .Get(filter: c => c.Id == id)
                .SelectMany(c => c.SubCategories.SelectMany(sc => sc.Questions))
                .Where(q => !q.IsDeleted)
                .Any();
        }

        public bool NameExists(string name)
        {
            return this.db
                .Categories
                .Get()
                .Any(c => c.Name == name);
        }

        public string GetName(int id)
        {
            return this.db
                .Categories
                .Get(filter: c => c.Id == id)
                .Select(c => c.Name)
                .FirstOrDefault();
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

            this.db.Categories.Add(category);
            this.db.Save();

            return true;
        }

        public bool Edit(int id, string name)
        {
            Category category = this.db.Categories.Find(id);

            if (category == null
                || (this.NameExists(name)
                    && category.Name != name))
            {
                return false;
            }

            category.Name = name;

            this.db.Save();

            return true;
        }

        public bool Delete(int id)
        {
            Category category = this.db.Categories.Find(id);

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

            this.db.Save();

            return true;
        }

        public bool Restore(int id)
        {
            Category category = this.db.Categories.Find(id);

            if (category == null
                || !category.IsDeleted)
            {
                return false;
            }

            category.IsDeleted = false;

            this.db.Save();

            return true;
        }

        public CategoryFormServiceModel GetForm(int id)
        {
            return this.db
                .Categories
                .Get(filter: c => c.Id == id)
                .AsQueryable()
                .ProjectTo<CategoryFormServiceModel>()
                .FirstOrDefault();
        }

        public IEnumerable<MenuCategoryServiceModel> GetMenu()
        {
            return this.db
                .Categories
                .Get(filter: c => !c.IsDeleted
                    && c.SubCategories.Any(sc => sc.Questions.Any(q => !q.IsDeleted)))
                .Select(c => new MenuCategoryServiceModel
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
                })
                .ToList();
        }

        public IEnumerable<ListCategoriesServiceModel> All()
        {
            return this.db
                .Categories
                .Get()
                .AsQueryable()
                .ProjectTo<ListCategoriesServiceModel>()
                .ToList();
        }
    }
}