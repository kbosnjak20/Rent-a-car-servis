namespace AutoRent.Models
{
    public enum EmployeeRole
    {
        Zaposlenik,
        VoditeljServisa
    }

    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public EmployeeRole Role { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
