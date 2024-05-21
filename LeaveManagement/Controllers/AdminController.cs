using LeaveManagement.Models;
using LeaveManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    public class AdminController : Controller
    {
        private readonly IUserRepository _userRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILeaveLimitRepository _leaveLimitRepository;

        public AdminController(IUserRepository userRepository, ILeaveRepository leaveRepository, ILeaveLimitRepository leaveLimitRepository)
        {
            _userRepository = userRepository;
            _leaveRepository = leaveRepository;
            _leaveLimitRepository = leaveLimitRepository;
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            IEnumerable<User> users = await _userRepository.GetAllAsync();
            return View(users); // Return view with list of users
        }

        // POST: Admin/Create
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                // Set default role for registered users
                user.Role = "Employee";

                await _userRepository.AddAsync(user);
                return RedirectToAction("Users"); // Redirect to user list after successful creation
            }
            return View(user); // If model state is invalid, return to create form with user data
        }

        // GET: Admin/Edit/id
        public async Task<IActionResult> Edit(int id)
        {
            User user = await _userRepository.GetByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user); // Return view with user data
        }

        // POST: Admin/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(User user)
        {
            if (ModelState.IsValid)
            {
                await _userRepository.UpdateAsync(user);
                return RedirectToAction("Users"); // Redirect to user list after successful update
            }
            return View(user); // If model state is invalid, return to edit form with user data
        }

        // GET: Admin/Delete/id
        public async Task<IActionResult> Delete(int id)
        {
            await _userRepository.DeleteAsync(id);
            return RedirectToAction("Users"); // Redirect to user list after successful deletion
        }

        // GET: Admin/LeaveRequests
        public async Task<IActionResult> LeaveRequests()
        {
            IEnumerable<Leave> leaveRequests = await _leaveRepository.GetAllAsync();
            return View(leaveRequests);
        }

        // POST: Admin/ApproveLeaveRequest/id
        [HttpPost]
        public async Task<IActionResult> ApproveLeaveRequest(int id)
        {
            var leave = await _leaveRepository.GetByIdAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            // Update the status of the leave request to "Approved"
            leave.Status = "Approved";
            await _leaveRepository.UpdateAsync(leave);

            // Redirect back to the LeaveRequests page
            return RedirectToAction(nameof(LeaveRequests));
        }

        // POST: Admin/RejectLeaveRequest/id
        [HttpPost]
        public async Task<IActionResult> RejectLeaveRequest(int id)
        {
            var leave = await _leaveRepository.GetByIdAsync(id);
            if (leave == null)
            {
                return NotFound();
            }

            // Update the status of the leave request to "Rejected"
            leave.Status = "Rejected";
            await _leaveRepository.UpdateAsync(leave);

            // Redirect back to the LeaveRequests page
            return RedirectToAction(nameof(LeaveRequests));
        }

		// GET: Admin/Dashboard
		public async Task<IActionResult> Dashboard(string username)
		{
			// Retrieve employee data from the repository based on the username
			var user = await _userRepository.GetByUsernameAsync(username);

			if (user == null || user.Role != "Employee")
			{
				return NotFound();
			}

			// Retrieve leave related data for the employee from the leave repository
			var requestedLeaves = await _leaveRepository.GetByUserIdAsync(user.Id);
			var approvedLeaves = requestedLeaves.Where(l => l.Status == "Approved").ToList();
			var rejectedLeaves = requestedLeaves.Where(l => l.Status == "Rejected").ToList();

			// Create the view model and populate it with the data
			var viewModel = new EmployeeDashboardViewModel
			{
				Username = user.Username,
				RequestedLeavesCount = requestedLeaves.Count(),
				ApprovedLeavesCount = approvedLeaves.Count(),
				RejectedLeavesCount = rejectedLeaves.Count()
			};

			// Pass the view model to the EmployeeDashboard view
			return View(viewModel);
		}

        //Leave Limits
        // GET: Admin/LeaveLimits
        public async Task<IActionResult> LeaveLimits()
        {
            IEnumerable<LeaveLimit> limits = await _leaveLimitRepository.GetAllAsync();
            return View(limits);
        }

        // GET: Admin/EditLimit/id
        public async Task<IActionResult> EditLimit(int id)
        {
            LeaveLimit limit = await _leaveLimitRepository.GetByIdAsync(id);
            if (limit == null)
            {
                return NotFound();
            }
            return View(limit);
        }

        // POST: Admin/EditLimit
        [HttpPost]
        public async Task<IActionResult> EditLimit(LeaveLimit limit)
        {
            if (ModelState.IsValid)
            {
                await _leaveLimitRepository.UpdateAsync(limit);
                return RedirectToAction("LeaveLimits"); // Redirect to leave limits list after successful update
            }
            return View(limit); // If model state is invalid, return to edit form with limit data
        }

        // GET: Admin/DeleteLimit/id
        public async Task<IActionResult> DeleteLimit(int id)
        {
            await _leaveLimitRepository.DeleteAsync(id);
            return RedirectToAction("LeaveLimits"); // Redirect to leave limits list after successful deletion
        }

        // POST: Admin/AddLimit
        [HttpPost]
        public async Task<IActionResult> AddLimit(string leaveType, int maxDays)
        {
            if (ModelState.IsValid)
            {
                // Check if a leave limit with the same type already exists
                var existingLimit = await _leaveLimitRepository.GetByTypeAsync(leaveType);

                if (existingLimit != null)
                {
                    // If a leave limit with the same type exists, update its max days
                    existingLimit.MaxDays = maxDays;
                    await _leaveLimitRepository.UpdateAsync(existingLimit);
                }
                else
                {
                    // If no leave limit with the same type exists, create a new one
                    var newLimit = new LeaveLimit
                    {
                        LeaveType = leaveType,
                        MaxDays = maxDays
                    };
                    await _leaveLimitRepository.AddAsync(newLimit);
                }

                return RedirectToAction("LeaveLimits"); // Redirect to leave limits page
            }

            return View(); // If model state is invalid, return to the add limit form
        }


    }
}
