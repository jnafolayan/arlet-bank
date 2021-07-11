using System;
using System.Collections.Generic;

namespace ArletBank
{
    public class CustomerController : Controller
    {
        public CustomerController(IDatabase db, Logger log, Models models) : base(db, log, models)
        {
            
        }

        public override void Run()
        {
            Greet();

            bool hasAccount = Log.Confirm("Do you have an account with us?");
            if (!hasAccount)
            {
                RunCreateAccountDialogue();
                return;
            }

            Customer user = Login();
            if (user == null)
            {
                Log.Error("Failed to log you in. Restart application");
                Environment.Exit(1);
                return;
            }

            Log.Success("Login successful!\n");
            Log.Info($"Welcome, {user.FullName}.");

            string[] actions = new string[] { 
                "Quit" 
            };
            bool abort = false;
            
            while (!abort)
            {
                string action = Log.Select("What would you like to do", actions);
                switch (action) {
                    case "Quit":
                    default:
                        abort = true;
                        break;
                }
                Log.Info("");
            }
        }
        public void RunCreateAccountDialogue()
        {
            var customerDto = new Dictionary<string, object>();
            string email = "";

            Log.Info("Register an account with us. We would love to have you!");

            while (true)
            {
                email = Log.Question<string>("Enter your email");
                // ensure the email is unique
                var query = new Dictionary<string, object>();
                query.Add("Email", email);
                var existingCustomer = Models.customer.Find(query);
                if (existingCustomer != null)
                {
                    Log.Warn("An account already exists with that email.");
                }
                else
                {
                    break;
                }
            }

            string firstname = Log.Question<string>("Enter your first name");            
            string lastname = Log.Question<string>("Enter your last name");            
            
            customerDto.Add("Email", email);
            customerDto.Add("FirstName", firstname);
            customerDto.Add("LastName", lastname);
            customerDto.Add("AccountNumber", "");
            // unconfirmed at first
            customerDto.Add("Confirmed", false);

            Models.customer.Insert(customerDto);
            Log.Success("Your account was created successfully.");
        }
        public void RunListCustomers()
        {
            // fetch all confirmed customers
            var query = new Dictionary<string, object>();
            query.Add("Confirmed", true);
            var list = Models.customer.FindAll(query);
            if (list.Count == 0)
            {
                Log.Info("There are no customers.");
                return;
            }
            
            Log.Info($"Here are all the {list.Count} customers of Arlet:");
            foreach (Dictionary<string, object> record in list)
            {   
                query.Clear();
                query.Add("CustomerEmail", record["Email"]);
                var account = Models.account.Find(query);
                Log.Info($"\t{record["FirstName"]} {record["LastName"]} ({account["Number"]} - {account["Balance"]})");
            }
        }
        protected override void Greet()
        {

            Log.Info("====================================================");
            Log.Info("");
            Log.Info(" Welcome to Arlet's Customer module!");
            Log.Info("");
            Log.Info("====================================================");
            Log.Info("");
        }

        public Customer Login()
        {
            // collect username and password
            uint maxAttemptsAllowed = 3;
            uint attempts = 0;
            #nullable enable
            Customer? user = null;

            Log.Info("Enter your account number and pin to continue");

            while (attempts < maxAttemptsAllowed) 
            {
                string accountNumber = Log.Question<string>("Enter your account number").Trim();
                string pin = Log.Password("Enter your 4-digit pin").Trim();
                
                Dictionary<string, object> query = new Dictionary<string, object>();
                query.Add("Number", accountNumber);

                var account = Models.account.Find(query);
                bool valid = false;

                if (account != null) 
                {
                    string hashed = (string)account["PIN"];
                    if (PasswordHasher.Verify(pin, (string)hashed)) 
                    {
                        valid = true;
                    }
                }

                if (valid && account != null)
                {
                    query.Clear();
                    query.Add("AccountNumber", (string)account["Number"]);
                    var record = Models.customer.Find(query);
                    user = Models.customer.FromDictionary(record);
                    break;
                }
                else
                {
                    Log.Warn("Could not find a matching account. Please try again");
                    Log.Info("");
                    attempts++;
                }
            }

            if (user == null) return null;
            return user;
        }
    }
}