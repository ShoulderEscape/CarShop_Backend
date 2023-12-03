using Entites;

namespace CarShopAPI.Data.UserRepositories
{
    public interface IUserRepository
    {
        Task<User> AddUser(User user);

    }
}
