using System;

namespace ArletBank
{
    /// <summary>
    /// SavingsAccount class. Encapsulates logic around savings account ownership.
    /// </summary>
    public class SavingsAccount : Account
    {
        public SavingsAccount() : base() 
        {
            Type = (int)SavingsAccount.Types.SAVINGS_ACCOUNT;
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
                throw new AccountOperationException("You cannot credit negative amounts.");
            }
            else if (amount > SavingsAccount.MAX_DEPOSIT)
            {
                throw new AccountOperationException($"You cannot deposit more than ${SavingsAccount.MAX_WITHDRAWAL}.");
            }

            if (nextBalance > SavingsAccount.MAX_BALANCE)
            {
                throw new AccountOperationException("You cannot exceed the allowed balance limit.");
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
                throw new AccountOperationException("You cannot withdraw negative amounts.");
            }
            else if (amount > SavingsAccount.MAX_WITHDRAWAL)
            {
                throw new AccountOperationException($"You cannot withdraw more than ${SavingsAccount.MAX_WITHDRAWAL}.");
            }

            if (nextBalance < 0)
            {
                throw new AccountOperationException("You cannot withdraw below 0.");
            }
            else if (nextBalance == 0)
            {
                throw new AccountOperationException("You cannot withdraw to 0.");
            }
        }

        public const decimal MAX_DEPOSIT = 50_000;
        public const decimal MAX_WITHDRAWAL = 50_000;
        public const decimal MAX_BALANCE = Decimal.MaxValue;
    }
}