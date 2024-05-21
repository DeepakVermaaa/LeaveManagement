using LeaveManagement.Models;
using LeaveManagement.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManagement.Controllers
{
    public class LeaveManagementController : Controller
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILeaveLimitRepository _leaveLimitRepository;

        public LeaveManagementController(ILeaveRepository leaveRepository, IUserRepository userRepository, ILeaveLimitRepository leaveLimitRepository)
        {
            _leaveRepository = leaveRepository;
            _userRepository = userRepository;
            _leaveLimitRepository = leaveLimitRepository;
        }

        // GET: LeaveManagement/RequestLeave
        [HttpGet]
        public IActionResult RequestLeave()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestLeave(string username, string leaveType, DateTime startDate, DateTime endDate, string reason)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(leaveType) || startDate == default || endDate == default || string.IsNullOrEmpty(reason))
            {
                TempData["ErrorMessage"] = "All fields are required.";
                return RedirectToAction("EmployeeDashboard", "UserRegistration", new { username = username });
            }

            try
            {
                // Get the user by username
                var user = await _userRepository.GetByUsernameAsync(username);

                if (user == null)
                {
                    TempData["ErrorMessage"] = "User not found.";
                    return RedirectToAction("EmployeeDashboard", "UserRegistration", new { username = username });
                }

                // Get the maximum days allowed for the requested leave type
                var leaveLimit = await _leaveLimitRepository.GetByTypeAsync(leaveType);

                if (leaveLimit != null)
                {
                    // Retrieve all leave requests for the user of the requested leave type
                    var requestedLeavesOfType = (await _leaveRepository.GetByUserIdAsync(user.Id))
                        .Where(l => l.LeaveType == leaveType && l.Status != "Rejected")
                        .ToList();

                    // Calculate the total number of days requested for the leave type
                    var totalDaysRequested = requestedLeavesOfType
                        .Sum(l => (l.EndDate - l.StartDate).Days) + (endDate - startDate).Days;

                    // Check if the total days requested plus the new request exceed the limit
                    if (totalDaysRequested > leaveLimit.MaxDays)
                    {
                        TempData["ErrorMessage"] = $"You have requested {totalDaysRequested} days of {leaveType} leave. The maximum limit is {leaveLimit.MaxDays} days.";
                        return RedirectToAction("EmployeeDashboard", "UserRegistration", new { username = username });
                    }
                    else if (totalDaysRequested + (endDate - startDate).Days > leaveLimit.MaxDays)
                    {
                        endDate = startDate.AddDays(leaveLimit.MaxDays - totalDaysRequested);
                    }
                }

                // Create a new leave request
                var leave = new Leave
                {
                    UserId = user.Id,
                    UserName = username,
                    LeaveType = leaveType,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = "Pending",
                    Comments = reason
                };

                await _leaveRepository.AddAsync(leave);

                // Success message to be displayed on the EmployeeDashboard view
                TempData["SuccessMessage"] = "Leave request submitted successfully.";

                // Redirect to the dashboard
                return RedirectToAction("EmployeeDashboard", "UserRegistration", new { username = username });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error occurred while requesting leave: {ex.Message}");
                return RedirectToAction("EmployeeDashboard", "UserRegistration", new { username = username });
            }
        }




        // Private method to load employee dashboard view
        private async Task<IActionResult> LoadEmployeeDashboard(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null || user.Role != "Employee")
            {
                return NotFound();
            }

            var requestedLeaves = await _leaveRepository.GetByUserIdAsync(user.Id);
            var approvedLeaves = requestedLeaves.Where(l => l.Status == "Approved").ToList();
            var rejectedLeaves = requestedLeaves.Where(l => l.Status == "Rejected").ToList();

            var viewModel = new EmployeeDashboardViewModel
            {
                Username = user.Username,
                RequestedLeavesCount = requestedLeaves.Count(),
                ApprovedLeavesCount = approvedLeaves.Count(),
                RejectedLeavesCount = rejectedLeaves.Count()
            };

            return View("EmployeeDashboard", viewModel);
        }


    }
}
