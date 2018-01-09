namespace TitaniumForum.Services.Areas.Moderator
{
    public interface IModeratorCommentService
    {
        bool Delete(int id);

        bool Restore(int id);
    }
}