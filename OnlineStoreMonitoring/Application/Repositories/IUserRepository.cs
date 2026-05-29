using System.Threading.Tasks;
using OnlineStoreMonitoring.Domain.Entities;

namespace OnlineStoreMonitoring.Application.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByUsernameAsync(string username);
        Task<User> GetByEmailAsync(string email);
    }
}