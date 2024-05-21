using LeaveManagement.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LeaveManagement.Repositories
{
    //Interface for LeaveRepository
    public interface ILeaveRepository
    {
        Task<Leave> GetByIdAsync(int id);
        Task AddAsync(Leave leave);
        Task UpdateAsync(Leave leave);
        Task DeleteAsync(int id);
        Task<IEnumerable<Leave>> GetAllAsync();
        Task<IEnumerable<Leave>> GetByUserIdAsync(int userId);
    }
}
