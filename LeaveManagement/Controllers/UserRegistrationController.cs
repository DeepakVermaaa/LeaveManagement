using LeaveManagement.Models;
using LeaveManagement.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace LeaveManagement.Controllers
{
	public class UserRegistrationController : Controller
	{
		private readonly IUserRepository _userRepository;
		private readonly ILeaveRepository _leaveRepository;

		public UserRegistrationController(IUserRepository userRepository, ILeaveRepository leaveRepository)
		{
			_userRepository = userRepository;
			_leaveRepository = leaveRepository;
		}

		// GET: UserRegistration/Register
		[HttpGet]
		public IActionResult Register()
		{
			return View();
		}

		// POST: UserRegistration/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (!string.IsNullOrEmpty(user.Username) && !string.IsNullOrEmpty(user.Password))
            {
                try
                {
                    // Setting default role for registered users
                    user.Role = "Employee";

                    await _userRepository.AddAsync(user);

                    // Redirect to employee dashboard after successful registration
                    return RedirectToAction("EmployeeDashboard", new { username = user.Username });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error occurred during user registration: {ex.Message}");
                    ModelState.AddModelError("", "Error occurred during user registration. Please try again.");
                }
            }

            // If username or password is missing, return to registration form with user data
            return View(user);
        }


        // GET: UserRegistration/Dashboard
        public async Task<IActionResult> EmployeeDashboard(string username)
        {
            // Retrieve employee data from the repository based on the username
            var user = await _userRepository.GetByUsernameAsync(username);

            if (user == null || user.Role != "Employee")
            {
                return NotFound();
            }

            // Retrieve leave-related data for the employee from the leave repository
            var requestedLeaves = await _leaveRepository.GetByUserIdAsync(user.Id);
            var approvedLeaves = requestedLeaves.Where(l => l.Status == "Approved").ToList();
            var rejectedLeaves = requestedLeaves.Where(l => l.Status == "Rejected").ToList();

            // Create the view model and populating with the data
            var viewModel = new EmployeeDashboardViewModel
            {
                Username = user.Username,
                RequestedLeavesCount = requestedLeaves.Count(),
                ApprovedLeavesCount = approvedLeaves.Count(),
                RejectedLeavesCount = rejectedLeaves.Count()
            };

            // Pass the view model to the EmployeeDashboard view
            return View("EmployeeDashboard", viewModel);
        }

		//Manage profile
		// GET: UserRegistration/ManageProfile
		[HttpGet]
		public async Task<IActionResult> ManageProfile(string username)
		{
			var user = await _userRepository.GetByUsernameAsync(username);

			if (user == null)
			{
				return NotFound();
			}

			return View(user);
		}

		// POST: UserRegistration/ManageProfile
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ManageProfile(User user)
		{
			if (ModelState.IsValid)
			{
				try
				{
					await _userRepository.UpdateAsync(user);
					TempData["SuccessMessage"] = "Profile updated successfully.";
					return RedirectToAction(nameof(EmployeeDashboard), new { username = user.Username });
				}
				catch (Exception ex)
				{
					TempData["ErrorMessage"] = $"Error occurred while updating profile: {ex.Message}";
				}
			}

			return View(user);
		}

	}
}
