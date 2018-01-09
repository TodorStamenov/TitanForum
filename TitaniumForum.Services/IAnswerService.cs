namespace TitaniumForum.Services
{
    using Data.Models;
    using Models.Answers;
    using System.Collections.Generic;

    public interface IAnswerService
    {
        int TotalByQuestion(int questionId);

        bool CanEdit(int id, int userId);

        bool Create(int questionId, int authorId, string content);

        bool Edit(int id, string content);

        bool Vote(int id, int userId, Direction voteDirection);

        AnswerFormServiceModel GetForm(int id);

        IEnumerable<ListAnswersServiceModel> ByQuestion(int questionId, int userId, int page, int pageSize);
    }
}