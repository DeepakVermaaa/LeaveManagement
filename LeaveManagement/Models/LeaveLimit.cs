namespace LeaveManagement.Models
{
    public class LeaveLimit
    {
        public int Id { get; set; }
        public string LeaveType { get; set; }
        public int MaxDays { get; set; }
    }

}
