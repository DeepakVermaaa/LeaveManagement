using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq; // Add this namespace

namespace LeaveManagement.Repositories
{
    public class LeaveLimitRepository : ILeaveLimitRepository
    {
        private readonly AppDbContext _context;
        public LeaveLimitRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieve all leave limits asynchronously
        public async Task<IEnumerable<LeaveLimit>> GetAllAsync()
        {
            return await _context.LeaveLimits.ToListAsync();
        }

        // Retrieve a leave limit by its unique identifier asynchronously
        public async Task<LeaveLimit> GetByIdAsync(int id)
        {
            return await _context.LeaveLimits.FindAsync(id);
        }

        // Retrieve a leave limit by its type asynchronously
        public async Task<LeaveLimit> GetByTypeAsync(string leaveType)
        {
            return await _context.LeaveLimits.FirstOrDefaultAsync(l => l.LeaveType == leaveType);
        }

        // Add a new leave limit to the database asynchronously
        public async Task AddAsync(LeaveLimit limit)
        {
            await _context.LeaveLimits.AddAsync(limit);
            await _context.SaveChangesAsync();
        }

        // Update an existing leave limit in the database asynchronously
        public async Task UpdateAsync(LeaveLimit limit)
        {
            _context.LeaveLimits.Update(limit);
            await _context.SaveChangesAsync();
        }

        // Delete a leave limit from the database asynchronously by its identifier
        public async Task DeleteAsync(int id)
        {
            var limit = await GetByIdAsync(id);
            if (limit != null)
            {
                _context.LeaveLimits.Remove(limit);
                await _context.SaveChangesAsync();
            }
        }
    }
}
