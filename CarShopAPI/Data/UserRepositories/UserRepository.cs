using CarShopAPI.Data.UserRepositories;
using Entites;
using Microsoft.EntityFrameworkCore;

namespace CarShopAPI.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> CheckUsername(string username)
        {
            return await _context.Users.AnyAsync(u => u.UserName == username);
        }
    }
}
