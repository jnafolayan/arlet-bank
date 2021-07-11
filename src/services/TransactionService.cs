using System;
using System.Collections.Generic;

namespace ArletBank 
{
    /// <summary>
    /// Transaction service
    /// </summary>
    public class TransactionService : Service<Transaction> {
        public TransactionService(Model<Transaction> model) : base(model)
        {}

        /// <summary>
        /// Creates a new transaction record
        /// </summary>
        /// <param name="account">The affected account number</param> 
        /// <param name="type">The type of transaction</param> 
        /// <param name="account">The amount involved</param> 
        /// <param name="message">Human-readable message</param> 
        public void CreateTransaction(string account, int type, decimal amount, string message)
        {
            var transDto = new Dictionary<string, object>();
            transDto.Add("Account", account);
            transDto.Add("Type", type);
            transDto.Add("Amount", amount);
            transDto.Add("Message", message);
            transDto.Add("Date", DateTime.UtcNow);
            Model.Insert(transDto);
        }

        /// <summary>
        /// Fetches all transactions that match a query
        /// </summary>
        /// <param name="query">Query dict</param>
        public List<Transaction> GetTransactions(Dictionary<string, object> query) 
        {
            var list = Model.FindAll(query);
            return list.ConvertAll<Transaction>((admin) => Model.FromDictionary(admin));
        }
    }
}