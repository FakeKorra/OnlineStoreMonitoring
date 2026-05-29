using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineStoreMonitoring.Domain.Entities;
using OnlineStoreMonitoring.Infrastructure.Data;

namespace OnlineStoreMonitoring.Application.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly StoreDbContext _context;

        public UserRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}