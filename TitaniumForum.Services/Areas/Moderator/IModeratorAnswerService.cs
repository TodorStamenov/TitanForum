namespace TitaniumForum.Services.Areas.Moderator
{
    using Models.Answers;
    using System.Collections.Generic;

    public interface IModeratorAnswerService
    {
        int DeletedCount(string search);

        bool Delete(int id);

        bool Restore(int id);

        IEnumerable<ListDeletedAnswersServiceModel> Deleted(int page, int pageSize, string search);
    }
}