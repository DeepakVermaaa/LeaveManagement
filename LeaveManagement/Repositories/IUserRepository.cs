using LeaveManagement.Models;

namespace LeaveManagement.Repositories
{
    // IUserRepository interface outlines methods for interacting with user data in the repository
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(int id);
        Task<User> GetByUsernameAsync(string username);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        Task<IEnumerable<User>> GetAllAsync();
    }
}
