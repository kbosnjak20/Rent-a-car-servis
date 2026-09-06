namespace AutoRent.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Contact { get; set; }

        public override string ToString()
        {
            return $"{FirstName} {LastName}";
        }
    }
}
