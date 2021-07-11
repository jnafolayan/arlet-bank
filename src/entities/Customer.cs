namespace ArletBank
{
    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string AccountNumber { get; set; }
        public bool Confirmed { get; set; }
        public string FullName 
        {
            get { return FirstName + " " + LastName; }
        }
    }
}