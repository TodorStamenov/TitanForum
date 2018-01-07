using TitaniumForum.Services.Models.Comments;

namespace TitaniumForum.Services
{
    public interface ICommentService
    {
        bool CanEdit(int id, int userId);

        bool Edit(int id, string content);

        bool Delete(int id);

        bool Create(int answerId, int authorId, string content);

        CommentFormServiceModel GetForm(int id);
    }
}