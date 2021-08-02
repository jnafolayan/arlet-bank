using System.Collections.Generic;

namespace ArletBank 
{
    /// <summary>
    /// Account service
    /// </summary>
    public class AccountService : Service<Account> {
        public AccountService(Model<Account> model) : base(model)
        {}

        /// <summary>
        /// Log into an account
        /// </summary>
        /// <returns>The account entity</returns>
        /// <param name="accNo">The account number</param>
        /// <param name="pin">The account PIN</param>
        public Account Login(string accNo, string pin)
        {
            var account = GetAccountByAccountNumber(accNo);
            if (account == null) return null;

            if (PasswordHasher.Verify(pin, account.PIN)) return account;
            return null;
        }

        /// <summary>
        /// Creates a new account record
        /// </summary>
        /// <returns>The account</returns>
        /// <param name="email">Email of the customer</param>
        /// <param name="number">The account number</param>
        /// <param name="pin">The PIN</param>
        /// <param name="accountType">The account type</param>
        public Account CreateAccount(string email, string number, string pin, Account.Types accountType)
        {
            var accountDto = new Dictionary<string, object>();
            accountDto.Add("CustomerEmail", email);
            accountDto.Add("Number", number);
            accountDto.Add("PIN", PasswordHasher.Hash(pin));
            accountDto.Add("Balance", 0);
            accountDto.Add("Type", (int)accountType);
            Model.Insert(accountDto);
            return GetAccountByCustomerEmail(email);
        }

        /// <summary>
        /// Fetches an account by the customer's email
        /// </summary>
        /// <returns>The account</returns>
        /// <param name="email">The customer's email</param>
        public Account GetAccountByCustomerEmail(string email)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", email);
            var doc = Model.Find(query);
            
            return (doc == null) ? null : Model.FromDictionary(doc); 
        }

        /// <summary>
        /// Fetches an account by its account number
        /// </summary>
        /// <returns>The account</returns>
        /// <param name="accNo">The account number</param>
        public Account GetAccountByAccountNumber(string accNo)
        {
            var query = new Dictionary<string, object>();
            query.Add("Number", accNo);
            var doc = Model.Find(query);
            
            return (doc == null) ? null : Model.FromDictionary(doc); 
        }
        
        /// <summary>
        /// Removes an account by the customer's email
        /// </summary>
        /// <returns>The account</returns>
        /// <param name="email">The customer's email</param>
        public bool RemoveAccountByCustomerEmail(string email)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", email);
            return Model.Remove(query);
        }

        /// <summary>
        /// Updates an account by the customer's email
        /// </summary>
        /// <returns>Whether the update was successful</returns>
        /// <param name="email">The customer's email</param>
        /// <param name="update">The update dict</param>
        public bool UpdateAccountByCustomerEmail(string email, Dictionary<string, object> update)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", email);

            return Model.UpdateOne(query, update);
        }

        /// <summary>
        /// Updates an account by the customer's account number
        /// </summary>
        /// <returns>Whether the update was successful</returns>
        /// <param name="accountNumber">The customer's account number</param>
        /// <param name="update">The update dict</param>
        public bool UpdateAccountByAccountNumber(string accountNumber, Dictionary<string, object> update)
        {
            var query = new Dictionary<string, object>();
            query.Add("Number", accountNumber);

            return Model.UpdateOne(query, update);
        }

        /// <summary>
        /// Change account PIN
        /// </summary>
        /// <returns>success or not</returns>
        /// <param name="accountNumber">The customer's account number</param>
        /// <param name="pin">The new pin</param>
        public bool ChangeAccountPIN(string accountNumber, string newPIN)
        {
            var update = new Dictionary<string, object>();
            update.Add("PIN", PasswordHasher.Hash(newPIN));
            
            return UpdateAccountByAccountNumber(accountNumber, update);
        }

        /// <summary>
        /// Deposit
        /// </summary>
        /// <returns>success or not</returns>
        /// <param name="accountNumber">The customer's account number</param>
        /// <param name="amount">The amount to deposit</param>
        public bool Deposit(string accountNumber, decimal amount)
        {
            var account = GetAccountByAccountNumber(accountNumber);
            if (account == null) return false;

            var update = new Dictionary<string, object>();
            update.Add("Balance", account.Balance + amount);

            return UpdateAccountByAccountNumber(accountNumber, update);
        }

        /// <summary>
        /// Withdraw
        /// </summary>
        /// <returns>success or not</returns>
        /// <param name="accountNumber">The customer's account number</param>
        /// <param name="amount">The amount to withdraw</param>
        public bool Withdraw(string accountNumber, decimal amount)
        {
            var account = GetAccountByAccountNumber(accountNumber);
            if (account == null) return false;

            // ensure we can't withdraw below zero
            if (account.Balance - amount < 0) return false;

            var update = new Dictionary<string, object>();
            update.Add("Balance", account.Balance - amount);

            return UpdateAccountByAccountNumber(accountNumber, update);
        }

        /// <summary>
        /// Transfer
        /// </summary>
        /// <returns>success or not</returns>
        /// <param name="sender">The sender's account number</param>
        /// <param name="receiver">The receiver's account number</param>
        /// <param name="amount">The amount to transfer</param>
        public bool Transfer(string sender, string receiver, decimal amount)
        {
            bool success = Withdraw(sender, amount);
            // transfer if withdrawal was a success
            if (success) return Deposit(receiver, amount);
            return false;
        }
    }
}