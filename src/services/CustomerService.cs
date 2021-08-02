using System;
using System.Collections.Generic;

namespace ArletBank 
{
    /// <summary>
    /// Customer service. Exposes methods to interact with the Customer model.
    /// </summary>
    public class CustomerService : Service<Customer> {
        public CustomerService(Model<Customer> model) : base(model)
        {}
        
        /// <summary>
        /// Creates a new customer
        /// </summary>
        /// <returns>The newly created customer entity</returns>
        /// <param name="email">The email</param>
        /// <param name="firstname">The first name</param>
        /// <param name="lastname">The last name</param>
        /// <param name="accountType">The type of account</param>
        public Customer CreateCustomer(string email, string firstname, string lastname, Account.Types accountType)
        {
            var customerDto = new Dictionary<string, object>();
            customerDto.Add("Email", email);
            customerDto.Add("FirstName", firstname);
            customerDto.Add("LastName", lastname);
            customerDto.Add("AccountNumber", "");
            customerDto.Add("AccountType", (int)accountType);
            // unconfirmed at first
            customerDto.Add("Confirmed", false);
            Model.Insert(customerDto);

            return GetCustomer(email);
        }

        /// <summary>
        /// Fetches a customer by their email
        /// </summary>
        /// <returns>The customer entity</returns>
        /// <param name="email">The customer's email</param>
        public Customer GetCustomer(string email)
        {
            var query = new Dictionary<string, object>();
            query.Add("Email", email);
            var doc = Model.Find(query);
            
            return (doc == null) ? null : Model.FromDictionary(doc); 
        }

        /// <summary>
        /// Checks if a customer exists
        /// </summary>
        /// <returns>Whether the customer exists</returns>
        /// <param name="email">The customer's email</param>
        public bool CustomerExists(string email)
        {
            return GetCustomer(email) != null;
        }

        /// <summary>
        /// Fetches customers that match a query
        /// </summary>
        /// <returns>A list of the customers</returns>
        /// <param name="query">The query dict</param>
        public List<Customer> GetCustomers(Dictionary<string, object> query) 
        {
            var list = Model.FindAll(query);
            return list.ConvertAll<Customer>((customer) => Model.FromDictionary(customer));
        }

        /// <summary>
        /// Removes the customer that matches an email
        /// </summary>
        /// <returns>answer</returns>
        /// <param name="email">The email</param>
        public bool RemoveCustomer(string email)
        {
            var query = new Dictionary<string, object>();
            query.Add("Email", email);
            return Model.Remove(query);
        }

        /// <summary>
        /// Update a customer by their email
        /// </summary>
        /// <returns>answer</returns>
        /// <param name="email">The email</param>
        /// <param name="update">The update dict</param>
        public bool UpdateCustomer(string email, Dictionary<string, object> update)
        {
            var query = new Dictionary<string, object>();
            query.Add("Email", email);

            return Model.UpdateOne(query, update);
        }

        public bool ApproveCustomer(string email, string confirmedBy)
        {
            var update = new Dictionary<string, object>();
            update.Add("Confirmed", true);
            update.Add("ConfirmedBy", confirmedBy);
            return UpdateCustomer(email, update);
            
        }
    }
}