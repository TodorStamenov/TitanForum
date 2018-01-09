namespace TitaniumForum.Services
{
    using Data.Models;
    using Services.Models.Comments;

    public interface ICommentService
    {
        bool CanEdit(int id, int userId);

        bool Create(int answerId, int authorId, string content);

        bool Edit(int id, string content);

        bool Vote(int id, int userId, Direction voteDirection);

        CommentFormServiceModel GetForm(int id);
    }
}