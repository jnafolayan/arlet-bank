using System;

namespace ArletBank
{
    /// <summary>
    /// CurrentAccount class. Encapsulates logic around current account ownership
    /// </summary>
    public class CurrentAccount : Account
    {
        public CurrentAccount() : base() 
        {
            Type = (int)CurrentAccount.Types.CURRENT_ACCOUNT;
        }
        
        /// <summary>
        /// Validates specified rules for credit operations. It throws an exception if a rule
        /// fails.
        /// </summary>
        /// <param name="amount">The credit amount</param>
        public override void ValidateCreditConstraints(decimal amount)
        {
            decimal nextBalance = Balance + amount;
            
            if (amount < 0)
            {
                throw new AccountOperationException("Cannot credit negative amounts");
            }
            else if (amount > CurrentAccount.MAX_DEPOSIT)
            {
                throw new AccountOperationException($"Cannot deposit more than ${CurrentAccount.MAX_WITHDRAWAL}");
            }

            if (nextBalance > CurrentAccount.MAX_BALANCE)
            {
                throw new AccountOperationException("Cannot exceed the allowed balance limit");
            }
        }

        /// <summary>
        /// Validates specified rules for debit operations. It throws an exception if a rule
        /// fails.
        /// </summary>
        /// <param name="amount">The debit amount</param>
        public override void ValidateDebitConstraints(decimal amount)
        {
            decimal nextBalance = Balance - amount;
            
            if (amount < 0)
            {
                throw new AccountOperationException("Cannot withdraw negative amounts");
            }
            else if (amount > CurrentAccount.MAX_WITHDRAWAL)
            {
                throw new AccountOperationException($"Cannot withdraw more than ${CurrentAccount.MAX_WITHDRAWAL}");
            }

            if (nextBalance < 0)
            {
                throw new AccountOperationException("Cannot withdraw below 0");
            }
        }

        public const decimal MAX_DEPOSIT = Decimal.MaxValue;
        public const decimal MAX_WITHDRAWAL = Decimal.MaxValue;
        public const decimal MAX_BALANCE = Decimal.MaxValue;
    }
}