namespace TitaniumForum.Services.Areas.Moderator
{
    using Services.Models.Questions;
    using System.Collections.Generic;

    public interface IModeratorQuestionService
    {
        bool Delete(int id);

        bool Restore(int id);

        bool Lock(int id);

        bool Unlock(int id);

        bool Conceal(int id);

        IEnumerable<ListQuestionsServiceModel> Reported(int page, int pageSize);

        IEnumerable<ListQuestionsServiceModel> Locked(int page, int pageSize, string search);

        IEnumerable<ListQuestionsServiceModel> Deleted(int page, int pageSize, string search);
    }
}