namespace TitaniumForum.Services.Areas.Moderator
{
    public interface IModeratorAnswerService
    {
        bool Delete(int id);

        bool Restore(int id);
    }
}