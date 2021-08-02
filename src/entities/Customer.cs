namespace ArletBank
{
    /// <summary>
    /// Customer class. Encapsulates fields around a customer.
    /// </summary>
    public class Customer : Entity
    {
        public string AccountNumber { get; set; }
        public int AccountType { get; set; }
        public bool Confirmed { get; set; }
        public string ConfirmedBy { get; set; }
    }
}