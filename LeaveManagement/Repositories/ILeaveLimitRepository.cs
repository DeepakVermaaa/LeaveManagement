using LeaveManagement.Models;

namespace LeaveManagement.Repositories
{
    public interface ILeaveLimitRepository
    {
        Task<IEnumerable<LeaveLimit>> GetAllAsync();
        Task<LeaveLimit> GetByIdAsync(int id);
        Task<LeaveLimit> GetByTypeAsync(string leaveType);
        Task AddAsync(LeaveLimit limit);
        Task UpdateAsync(LeaveLimit limit);
        Task DeleteAsync(int id);
    }
}
