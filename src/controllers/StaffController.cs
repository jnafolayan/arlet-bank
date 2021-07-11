using System;
using System.Text;
using System.Collections.Generic;

namespace ArletBank
{
    /// <summary>
    /// Controller for the staff module
    /// </summary>
    public class StaffController : Controller
    {
        public StaffController(IDatabase db, Logger log, Models models) : base(db, log, models)
        {}
        
        public override void Run()
        {
            Greet();

            Staff user = Login();
            if (user == null)
            {
                Log.Error("Failed to log you in. Restart application");
                Environment.Exit(1);
                return;
            }

            Log.Success("Login successful!\n");
            Log.Info($"Welcome, {user.FullName}.");

            string[] actions = new string[] { 
                "View registrations", 
                "List customers", 
                "Quit" 
            };
            bool abort = false;
            
            while (!abort)
            {
                string action = Log.Select("What would you like to do", actions);
                switch (action) {
                    case "View registrations":
                        RunViewRegistrations();
                        break;
                    case "List customers":
                        RunListCustomers();
                        break;
                    // fallthrough
                    case "Quit":
                    default:
                        abort = true;
                        break;
                }
                Log.Info("");
            }
        }

        /// <summary>
        /// Prints a list of customer registrations
        /// </summary>
        public void RunViewRegistrations()
        {
            // fetch all unconfirmed customers
            var query = new Dictionary<string, object>();
            query.Add("Confirmed", false);
            var list = Models.customer.FindAll(query);
            
            if (list.Count == 0)
            {
                Log.Info("There are no pending registrations.");
                return;
            }

            Log.Info($"There are {list.Count} pending registrations");
            
            var options = new string[list.Count];
            uint i = 0;
            foreach (Dictionary<string, object> record in list)
            {   
                options[i++] = $"{record["FirstName"]} {record["LastName"]} ({record["Email"]})";
            }

            string row = Log.Select("Select one for approval or rejection", options);
            uint rowIndex = 0;
            foreach (string opt in options)
            {   
                if (row == opt) {
                    break;
                }
                rowIndex++;
            }

            // display registration
            var reg = Models.customer.FromDictionary(list[(int)rowIndex]);
            Log.Info("Displaying account information for selected customer:");
            Log.Info($"\tFirst name: {reg.FirstName}");
            Log.Info($"\tLast name: {reg.LastName}");
            Log.Info($"\tEmail: {reg.Email}");
            bool approve = Log.Confirm("Do you want to approve this?");
            if (approve)
            {
                query.Clear();
                query.Add("Email", reg.Email);
                var update = new Dictionary<string, object>();
                update.Add("Confirmed", true);
                bool result = Models.customer.UpdateOne(query, update);
                if (result)
                {
                    // create an account for the user
                    var accountDto = new Dictionary<string, object>();
                    accountDto.Add("CustomerEmail", reg.Email);
                    accountDto.Add("Balance", 0);
                    string accNo = GenerateUniqueAccountNumber();
                    accountDto.Add("Number", accNo);
                    // default PIN is 0000
                    accountDto.Add("PIN", PasswordHasher.Hash(Account.DEFAULT_PIN));
                    Models.account.Insert(accountDto);

                    Log.Success($"{reg.FullName}'s registration was approved.");
                    Log.Info($"Customer's account number is {accNo}.");
                }
                else
                {
                    Log.Error("An error occured while approving registration.");
                }
            }
            else
            {
                query.Clear();
                query.Add("Email", reg.Email);
                bool result = Models.customer.Remove(query);
                if (result)
                {
                    Log.Success($"{reg.FullName}'s registration was denied.");
                }
                else
                {
                    Log.Error("An error occured while denying registration.");
                }
            }
        }

        /// <summary>
        /// Generates a unique account number (10-digit number) for a newly approved customer.
        /// </summary>
        /// <returns>The account number</returns>
        private string GenerateUniqueAccountNumber()
        {
            Random random = new Random();
            StringBuilder result = new StringBuilder(10);
            bool valid = false;
            do {
                result.Clear();
                for (int i = 0; i < 10; i++)
                {
                    int c = random.Next(0, 10);
                    result.Append(c);
                }

                var q = new Dictionary<string, object>();
                q.Add("Number", result.ToString());
                valid = Models.account.Find(q) == null;
            } while (!valid);
            return result.ToString();
        }
        
        /// <summary>
        /// Prints a list of approved customers.
        /// </summary>
        private void RunListCustomers()
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
            Log.Info(" Welcome to Arlet's Staff module!");
            Log.Info(" Enter your username and password to login");
            Log.Info("");
            Log.Info("====================================================");
            Log.Info("");
        }

        /// <summary>
        /// Prompts to login as a staff
        /// </summary>
        /// <returns>A staff entity</returns>
        private Staff Login()
        {
            // collect username and password
            uint maxAttemptsAllowed = 3;
            uint attempts = 0;
            #nullable enable
            Staff? user = null;
            while (attempts < maxAttemptsAllowed) 
            {
                string username = Log.Question<string>("Enter your username", "").Trim();
                string password = Log.Password("Enter your password").Trim();
                
                Dictionary<string, object> query = new Dictionary<string, object>();
                query.Add("Username", username);

                var a = Models.staff.Find(query);
                bool valid = false;

                if (a != null) 
                {
                    string hashed = (string)a["Password"];
                    if (PasswordHasher.Verify(password, (string)hashed)) 
                    {
                        valid = true;
                    }
                }

                if (valid) 
                {
                    user = Models.staff.FromDictionary(a);
                    break;
                }
                else
                {
                    Log.Warn("Could not find a matching staff account. Please try again");
                    Log.Info("");
                    attempts++;
                }
            }

            if (user == null) return null;
            return user;
        }
    }
}