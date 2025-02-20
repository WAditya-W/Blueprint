using Blueprint.Models;

namespace Blueprint.Repositories
{
    public interface IAuthRepository
    {
        Task<User> GetByNameAsync(string Name);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }
}
