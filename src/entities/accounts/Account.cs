using System;

namespace ArletBank
{
    public interface IAccount
    {
        /// <summary>
        /// Validates specified rules for credit operations. It throws an exception if a rule
        /// fails.
        /// </summary>
        /// <param name="amount">The credit amount</param>
        void ValidateCreditConstraints(decimal amount);

        /// <summary>
        /// Validates specified rules for debit operations. It throws an exception if a rule
        /// fails.
        /// </summary>
        /// <param name="amount">The debit amount</param>
        void ValidateDebitConstraints(decimal amount);
    }

    /// <summary>
    /// Account class. Encapsulates logic around account holding.
    /// </summary>
    public class Account : IAccount 
    {
        /// <summary>
        /// Validates specified rules for credit operations. It throws an exception if a rule
        /// fails.
        /// </summary>
        /// <param name="amount">The credit amount</param>
        /// <exception cref="AccountOperationException">Thrown when a rule is violated</exception>
        public virtual void ValidateCreditConstraints(decimal amount)
        {}

        /// <summary>
        /// Validates specified rules for debit operations. It throws an exception if a rule
        /// fails.
        /// </summary>
        /// <param name="amount">The debit amount</param>
        /// <exception cref="AccountOperationException">Thrown when a rule is violated</exception>
        public virtual void ValidateDebitConstraints(decimal amount)
        {}
        
        /// <summary>
        /// Copies the field values to another account object.
        /// </summary>
        /// <param name="other">The other account</param>
        public void CopyTo(Account other)
        {
            other.CustomerEmail = CustomerEmail;
            other.Number = Number;
            other.PIN = PIN;
            other.Balance = Balance;
            other.Type = Type;
        }

        /// <summary>
        /// Creates an instance of the account type
        /// </summary>
        /// <param name="accountType">The account</param>
        public static IAccount CreateInstanceForType(Types accountType)
        {
            switch (accountType) 
            {
                case Types.CURRENT_ACCOUNT:
                    return new CurrentAccount();
                case Types.SAVINGS_ACCOUNT:
                    return new SavingsAccount();
                default:
                    return new Account();
            }
        }

        public static string ConvertTypeToString(Types accountType)
        {
            switch (accountType) 
            {
                case Types.CURRENT_ACCOUNT:
                    return "Current Account";
                case Types.SAVINGS_ACCOUNT:
                    return "Savings Account";
                default:
                    return "Undefined";
            }
        }

        public string CustomerEmail { get; set; }
        public string Number { get; set; }
        public string PIN { get; set; }
        public decimal Balance { get; set; }
        public int Type { get; set; }
        public static string DEFAULT_PIN =  "0000";
        public enum Types {
            UNDEFINED,
            CURRENT_ACCOUNT,
            SAVINGS_ACCOUNT
        };
    }

    /// <summary>
    /// Exception class for exceptions raised when validating account
    /// operations.
    /// </summary>
    public class AccountOperationException : Exception 
    {
        public AccountOperationException(string message) : base(message)
        {}
    }
}
