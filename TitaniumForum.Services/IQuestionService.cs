namespace TitaniumForum.Services
{
    using Data.Models;
    using Models.Questions;
    using System.Collections.Generic;

    public interface IQuestionService
    {
        bool CanEdit(int id, int userId);

        bool TitleExists(string title);

        string GetTitle(int id);

        int Total(string search);

        int TotalByCategory(int categoryId);

        int TotalBySubCategory(int subCategoryId);

        int TotalByTag(int tagId);

        int Create(int authorId, string title, string content, string tags, int subCategoryId);

        bool Edit(int id, string title, string content, string tags, int subCategoryId);

        bool Delete(int id);

        bool Restore(int id);

        bool Lock(int id);

        bool Unlock(int id);

        bool Report(int id);

        bool Conceal(int id);

        bool Vote(int id, int userId, Direction voteDirection);

        QuestionFormServiceModel GetForm(int id);

        QuestionDetailsServiceModel Details(int id, int userId);

        IEnumerable<ListQuestionsServiceModel> ByCategory(int page, int pageSize, int categoryId);

        IEnumerable<ListQuestionsServiceModel> BySubCategory(int page, int pageSize, int subCategoryId);

        IEnumerable<ListQuestionsServiceModel> ByTag(int page, int pageSize, int tagId);

        IEnumerable<ListQuestionsServiceModel> All(int page, int pageSize, string search);
    }
}