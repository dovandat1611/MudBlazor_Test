namespace MudFarmework.Models
{
    public class EmployeeModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Department { get; set; } = string.Empty;

        // ✅ CRITICAL: Ensure Skills property is properly typed
        public List<string> Skills { get; set; } = new List<string>();

        // Optional: Override ToString for better debugging
        public override string ToString()
        {
            return $"ID: {Id}, Name: {Name}, Dept: {Department}, Skills: [{string.Join(", ", Skills ?? new List<string>())}]";
        }
    }
}
