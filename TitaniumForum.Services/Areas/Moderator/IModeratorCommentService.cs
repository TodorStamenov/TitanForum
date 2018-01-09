namespace TitaniumForum.Services.Areas.Moderator
{
    using Models.Comments;
    using System.Collections.Generic;

    public interface IModeratorCommentService
    {
        int DeletedCount(string search);

        bool Delete(int id);

        bool Restore(int id);

        IEnumerable<ListDeletedCommentsServiceModel> Deleted(int page, int pageSize, string search);
    }
}