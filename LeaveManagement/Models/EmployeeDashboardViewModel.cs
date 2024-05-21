namespace LeaveManagement.Models
{
	public class EmployeeDashboardViewModel
	{
		public string Username { get; set; }
		public int RequestedLeavesCount { get; set; }
		public int ApprovedLeavesCount { get; set; }
		public int RejectedLeavesCount { get; set; }
	}
}
