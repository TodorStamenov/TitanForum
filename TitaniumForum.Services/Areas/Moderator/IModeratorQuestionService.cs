namespace TitaniumForum.Services.Areas.Moderator
{
    using Services.Models.Questions;
    using System.Collections.Generic;

    public interface IModeratorQuestionService
    {
        int DeletedCount(string search);

        bool Delete(int id);

        bool Restore(int id);

        bool Lock(int id);

        bool Unlock(int id);

        bool Conceal(int id);

        IEnumerable<ListQuestionsServiceModel> Reported(int questionsCount);

        IEnumerable<ListQuestionsServiceModel> Deleted(int page, int pageSize, string search);
    }
}