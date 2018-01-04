namespace TitaniumForum.Services
{
    using Models.Questions;
    using System.Collections.Generic;

    public interface IQuestionService
    {
        int Total(string search);

        int TotalByCategory(int id);

        int TotalBySubCategory(int id);

        IEnumerable<ListQuestionsServiceModel> ByCategory(int page, int pageSize, int categoryId);

        IEnumerable<ListQuestionsServiceModel> BySubCategory(int page, int pageSize, int subCategoryId);

        IEnumerable<ListQuestionsServiceModel> All(int page, int pageSize, string search);
    }
}