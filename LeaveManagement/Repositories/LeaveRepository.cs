using LeaveManagement.Data;
using LeaveManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Repositories
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;
        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves a leave by its unique identifier asynchronously
        public async Task<Leave> GetByIdAsync(int id)
        {
            return await _context.Leaves.FindAsync(id);
        }

        // Adds a new leave record to the database asynchronously
        public async Task AddAsync(Leave leave)
        {
            await _context.Leaves.AddAsync(leave);
            await _context.SaveChangesAsync();
        }

        // Updates an existing leave record in the database asynchronously
        public async Task UpdateAsync(Leave leave)
        {
            _context.Leaves.Update(leave);
            await _context.SaveChangesAsync();
        }

        // Deletes a leave record from the database asynchronously by its identifier
        public async Task DeleteAsync(int id)
        {
            var leave = await GetByIdAsync(id);
            if (leave != null)
            {
                _context.Leaves.Remove(leave);
                await _context.SaveChangesAsync();
            }
        }

        // Retrieves all leave records from the database asynchronously
        public async Task<IEnumerable<Leave>> GetAllAsync()
        {
            return await _context.Leaves.ToListAsync();
        }

        // Retrieves all leave records associated with a specific user asynchronously
        public async Task<IEnumerable<Leave>> GetByUserIdAsync(int userId)
        {
            return await _context.Leaves.Where(l => l.UserId == userId).ToListAsync();
        }
    }
}
