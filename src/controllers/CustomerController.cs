using System;
using System.Collections.Generic;

namespace ArletBank
{
    public class CustomerController : Controller
    {
        public CustomerController(IDatabase db, Logger log, Models models) : base(db, log, models)
        {}

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
                "Change PIN",
                "View balance",
                "Deposit",
                "Withdraw",
                "Transfer",
                "View transactions",
                "Quit" 
            };
            bool abort = false;
            
            while (!abort)
            {
                string action = Log.Select("What would you like to do", actions);
                switch (action) {
                    case "Change PIN":
                        RunChangePIN(user);
                        break;
                    case "View balance":
                        RunViewBalance(user);
                        break;
                    case "Deposit":
                        RunDeposit(user);
                        break;
                    case "Withdraw":
                        RunWithdraw(user);
                        break;
                    case "Transfer":
                        RunTransfer(user);
                        break;
                    case "View transactions":
                        RunViewTransactions(user);
                        break;
                    case "Quit":
                    default:
                        abort = true;
                        break;
                }
                Log.Info("");
            }
        }
        
        /// <summary>
        /// Prints a list of transactions involving the customer
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunViewTransactions(Customer user)
        {
            // fetch user account
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);
            var account = Models.account.FromDictionary(Models.account.Find(query));

            // fetch transactions
            query.Clear();
            query.Add("Account", account.Number);
            var transactions = Models.transaction
                .FindAll(query)
                .ConvertAll<Transaction>(Models.transaction.FromDictionary);
            // sort by date from most recent transaction to most stale
            transactions.Sort((Transaction a, Transaction b) => b.Date.CompareTo(a.Date));

            foreach (Transaction trans in transactions)
            {
                Log.Info($"\t[{trans.Date.ToString()}] {trans.Message}");
            }
        }

        /// <summary>
        /// Prompts to transfer money from a customer to another
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunTransfer(Customer user)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);
            var account = Models.account.FromDictionary(Models.account.Find(query));

            decimal amount = Log.Question<decimal>("Enter amount");
            if (amount <= 0)
            {
                Log.Error("Amount must be greater than 0.");
            }
            else if (amount <= account.Balance)
            {
                // collect reciepient
                var recipientAccNo = Log.Question<string>("Enter recipient's account number");

                // ensure it is not the same customer
                if (recipientAccNo == account.Number)
                {
                    Log.Error("You cannot transfer money between the same accounts.");
                    return;
                }

                var recipientQuery = new Dictionary<string, object>();
                recipientQuery.Add("Number", recipientAccNo);
                var recipientDoc = Models.account.Find(recipientQuery);

                if (recipientDoc == null)
                {
                    Log.Error("We could not find the receiving account.");
                    return;
                }

                // update the sender's account
                var update = new Dictionary<string, object>();
                update.Add("Balance", account.Balance - amount);
                bool success = Models.account.UpdateOne(query, update);

                // update the receiver's account
                var recipient = Models.account.FromDictionary(recipientDoc);
                var update2 = new Dictionary<string, object>();
                update2.Add("Balance", recipient.Balance + amount);
                bool success2 = Models.account.UpdateOne(recipientQuery, update2);

                if (success || success2)
                {
                    if (success) 
                    {
                        // create a transaction record for the sender
                        var transDto = new Dictionary<string, object>();
                        transDto.Add("Account", account.Number);
                        transDto.Add("Type", Transaction.Types.DEBIT);
                        transDto.Add("Amount", amount);
                        transDto.Add("Message", $"Sent ${amount} to {recipient.Number}");
                        transDto.Add("Date", DateTime.UtcNow);
                        Models.transaction.Insert(transDto);
                    }

                    if (success2)
                    {
                        // create a transaction record for the receiver
                        var transDto = new Dictionary<string, object>();
                        transDto.Add("Account", recipient.Number);
                        transDto.Add("Type", Transaction.Types.CREDIT);
                        transDto.Add("Amount", amount);
                        transDto.Add("Message", $"Received ${amount} from {account.Number}");
                        transDto.Add("Date", DateTime.UtcNow);
                        Models.transaction.Insert(transDto);
                    }

                    Log.Success($"You have successfully transferred ${amount} to {recipient.Number}.");
                }
                else
                {
                    Log.Error("An error occured while withdrawing.");
                }
            }
            else
            {
                Log.Error("You have an insufficient balance. Fund your account and try again.");
            }
        }

        /// <summary>
        /// Prompts to debit the customer's account.
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunWithdraw(Customer user)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);
            var account = Models.account.FromDictionary(Models.account.Find(query));

            decimal amount = Log.Question<decimal>("Enter amount");
            if (amount <= 0)
            {
                Log.Error("Amount must be greater than 0.");
            }
            else if (amount <= account.Balance)
            {
                var update = new Dictionary<string, object>();
                update.Add("Balance", account.Balance - amount);
                bool success = Models.account.UpdateOne(query, update);
                if (success)
                {
                    // create a transaction record
                    var transDto = new Dictionary<string, object>();
                    transDto.Add("Account", account.Number);
                    transDto.Add("Type", Transaction.Types.DEBIT);
                    transDto.Add("Amount", amount);
                    transDto.Add("Message", $"Withdrew ${amount}");
                    transDto.Add("Date", DateTime.UtcNow);
                    Models.transaction.Insert(transDto);

                    Log.Success($"You have successfully withdrawn ${amount}.");
                }
                else
                {
                    Log.Error("An error occured while withdrawing.");
                }
            }
            else
            {
                Log.Error("You have an insufficient balance. Fund your account and try again.");
            }
        }

        /// <summary>
        /// Prints the balance of the customer's account
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunViewBalance(Customer user)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);
            var account = Models.account.FromDictionary(Models.account.Find(query));
            Log.Info($"\tYour account balance is ${account.Balance}");
        }

        /// <summary>
        /// Prompts to credit the customer's account
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunDeposit(Customer user)
        {
            decimal amount = Log.Question<decimal>("Enter amount");
            if (amount <= 0)
            {
                Log.Error("Amount must be greater than 0.");
                return;
            }
            
            var update = new Dictionary<string, object>();
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);

            var account = Models.account.FromDictionary(Models.account.Find(query));
            update.Add("Balance", account.Balance + amount);
            bool success = Models.account.UpdateOne(query, update);
            if (success)
            {
                // create a transaction record
                var transDto = new Dictionary<string, object>();
                transDto.Add("Account", account.Number);
                transDto.Add("Type", Transaction.Types.CREDIT);
                transDto.Add("Amount", amount);
                transDto.Add("Message", $"Received a deposit of ${amount}");
                transDto.Add("Date", DateTime.UtcNow);
                Models.transaction.Insert(transDto);

                Log.Success($"Account deposit of ${amount} was completed.");
            }
            else
            {
                Log.Error("We could not complete your deposit.");
            }
        }

        /// <summary>
        /// Prompts to change the customer's PIN
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunChangePIN(Customer user)
        {
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);
            var existingAccount = Models.account.Find(query);


            // it will always exist but this is for static checking
            if (existingAccount != null)
            {
                string newPIN = "";
                string hashed = (string)existingAccount["PIN"];
                bool valid = false;

                int attempts = 3;
                while (attempts-- > 0)
                {
                    string oldPIN = Log.Question<string>("Enter old PIN");
                    if (PasswordHasher.Verify(oldPIN, hashed))
                    {
                        valid = true;
                        break;
                    }
                    else
                    {
                        Log.Warn("Your old PIN is incorrect. Try again.");
                        Log.Info("");
                    }
                }

                if (valid)
                {
                    // collect new PIN
                    attempts = 3;
                    valid = false;
                    while (attempts-- > 0) 
                    {
                        newPIN = Log.Question<string>("Enter new PIN");
                        if (newPIN.Length == 4)
                        {
                            valid = true;
                            break;
                        }
                        else
                        {
                            Log.Warn("Please enter a 4-digit number.");
                            Log.Info("");
                        }
                    }
                }

                if (valid)
                {
                    var update = new Dictionary<string, object>();
                    update.Add("PIN", PasswordHasher.Hash(newPIN));
                    bool success = Models.account.UpdateOne(query, update);
                    if (success)
                    {
                        Log.Success("Your PIN was changed successfully.");
                    }
                    else
                    {
                        Log.Error("An error occured while updating your PIN.");
                    }
                }
                else
                {
                    Log.Error("You have attempted too many times. Restart application to try again.");
                    Environment.Exit(1);
                }
            }
        }

        /// <summary>
        /// Prompts to create a new customer
        /// </summary>
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
        
        protected override void Greet()
        {
            Log.Info("====================================================");
            Log.Info("");
            Log.Info(" Welcome to Arlet's Customer module!");
            Log.Info("");
            Log.Info("====================================================");
            Log.Info("");
        }

        /// <summary>
        /// Prints a list of transactions involving the customer
        /// </summary>
        /// <returns>The customer entity if successful</returns>
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