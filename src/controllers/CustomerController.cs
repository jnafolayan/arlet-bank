using System;
using System.Collections.Generic;

namespace ArletBank
{
    public class CustomerController : Controller
    {
        public CustomerController(IDatabase db, Logger log, Services services, Models models) : base(db, log, services, models)
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
                "Close account",
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
                    case "Close account":
                        RunCloseAccount(user);
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
            var account = Services.account.GetAccountByCustomerEmail(user.Email);

            // fetch transactions
            var query = new Dictionary<string, object>();
            query.Add("Account", account.Number);
            var transactions = Services.transaction.GetTransactions(query);
            // sort by date from most recent transaction to most stale
            transactions.Sort((Transaction a, Transaction b) => b.Date.CompareTo(a.Date));

            foreach (Transaction trans in transactions)
            {
                Log.Info($"\t[{trans.Date.ToString()}] {trans.Message}");
            }
        }
        
        /// <summary>
        /// Prints a list of transactions involving the customer
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunCloseAccount(Customer user)
        {
            bool sure = Log.Confirm("Are you sure you want to close your account?");
            if (!sure)
            {
                Log.Info("Phew. Thank you for choosing to remain with us.");
                return;
            }

            // delete customer
            var query = new Dictionary<string, object>();
            query.Add("CustomerEmail", user.Email);
            bool customerDeleted = Services.customer.RemoveCustomer(user.Email);

            // delete account
            bool accountDeleted = Services.account.RemoveAccountByCustomerEmail(user.Email);

            if (accountDeleted && customerDeleted) 
            {
                Log.Success("Your account has been closed. We hate to see you go, but thank you for banking with us!");
                Environment.Exit(0);
            }
            else
            {
                // ideally, there should be a rollback, but...
                Log.Error("An error occured while closing your account.");
            }
        }

        /// <summary>
        /// Prompts to transfer money from a customer to another
        /// </summary>
        /// <param name="user">The customer entity</param>
        public void RunTransfer(Customer user)
        {
            var account = Services.account.GetAccountByCustomerEmail(user.Email);
            
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

                var recipient = Services.account.GetAccountByAccountNumber(recipientAccNo);
                if (recipient == null)
                {
                    Log.Error("We could not find the receiving account.");
                    return;
                }

                bool success = Services.account.Transfer(user.AccountNumber, recipientAccNo, amount);

                if (success)
                {
                    // create a transaction record for the sender
                    Services.transaction.CreateTransaction(
                        account.Number,
                        (int)Transaction.Types.DEBIT,
                        amount,
                        $"Sent ${amount} to {recipient.Number}"
                    );

                    // create a transaction record for the receiver
                    Services.transaction.CreateTransaction(
                        recipient.Number,
                        (int)Transaction.Types.CREDIT,
                        amount,
                        $"Received ${amount} from {account.Number}"
                    );

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
            var account = Services.account.GetAccountByCustomerEmail(user.Email);

            decimal amount = Log.Question<decimal>("Enter amount");
            if (amount <= 0)
            {
                Log.Error("Amount must be greater than 0.");
            }
            else if (amount <= account.Balance)
            {
                bool success = Services.account.Withdraw(user.AccountNumber, amount);
                if (success)
                {
                    // create a transaction record
                    Services.transaction.CreateTransaction(
                        account.Number,
                        (int)Transaction.Types.DEBIT,
                        amount,
                        $"Withdrew ${amount}"
                    );

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
            var account = Services.account.GetAccountByCustomerEmail(user.Email);
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
            
            var account = Services.account.GetAccountByCustomerEmail(user.Email);
            bool success = Services.account.Deposit(user.AccountNumber, amount);
            if (success)
            {
                // create a transaction record
                Services.transaction.CreateTransaction(
                    account.Number,
                    (int)Transaction.Types.CREDIT,
                    amount,
                    $"Received a deposit of ${amount}"
                );

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
            var existingAccount = Services.account.GetAccountByCustomerEmail(user.Email);

            // it will always exist but this is for static checking
            if (existingAccount != null)
            {
                string newPIN = "";
                string hashed = existingAccount.PIN;
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
                    bool success = Services.account.ChangeAccountPIN(user.AccountNumber, newPIN);
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
            Log.Info("Register an account with us. We would love to have you!");
         
            string email = "";
            while (true)
            {
                email = Log.Question<string>("Enter your email");
                // ensure the email is unique
                if (Services.customer.CustomerExists(email))
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
            
            Services.customer.CreateCustomer(email, firstname, lastname);
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

            while (attempts++ < maxAttemptsAllowed) 
            {
                string accountNumber = Log.Question<string>("Enter your account number").Trim();
                string pin = Log.Password("Enter your 4-digit pin").Trim();
                
                var account = Services.account.Login(accountNumber, pin);

                if (account == null)
                {
                    Log.Warn("Could not find a matching account. Please try again");
                    Log.Info("");
                }
                else
                {
                    user = Services.customer.GetCustomer(account.CustomerEmail);
                    break;
                }
            }

            return user;
        }
    }
}