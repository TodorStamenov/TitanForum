namespace TitaniumForum.Services
{
    public interface IUserService
    {
        bool AddProfileImage(int userId, byte[] imageContent);
    }
}