namespace ArletBank
{
    public class Account
    {
        public string CustomerEmail { get; set; }
        public string Number { get; set; }
        public string PIN { get; set; }
        public decimal Balance { get; set; }

        public static string DEFAULT_PIN =  "0000";
    }
}