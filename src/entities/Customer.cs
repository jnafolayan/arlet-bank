namespace ArletBank
{
    public class Customer : Entity
    {
        public string AccountNumber { get; set; }
        public bool Confirmed { get; set; }
        public string ConfirmedBy { get; set; }
    }
}