using System;

namespace ArletBank
{
    /// <summary>
    /// Transaction class. 
    /// </summary>
    public class Transaction
    {
        public string Account { get; set; }
        public int Type { get; set; }
        public decimal Amount { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public enum Types: int {
            DEBIT,
            CREDIT
        }
    }
}