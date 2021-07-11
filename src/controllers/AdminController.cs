using System;
using System.Collections.Generic;

namespace ArletBank
{
    /// <summary>
    /// Controller for the admin module
    /// </summary>
    public class AdminController : Controller
    {
        public AdminController(IDatabase db, Logger log, Models models) : base(db, log, models)
        {}

        public override void Run()
        {
            Greet();

            Admin user = Login();
            if (user == null)
            {
                Log.Error("Failed to log you in. Restart application");
                Environment.Exit(1);
                return;
            }

            Log.Success("Login successful!\n");
            Log.Info($"Welcome, {user.Name}.");

            string[] actions = new string[] { 
                "Create staff", 
                "Remove staff", 
                "List staffs", 
                "Create admin", 
                "Remove admin", 
                "List admins", 
                "Quit" 
            };
            bool abort = false;
            
            while (!abort)
            {
                string action = Log.Select("What would you like to do", actions);
                switch (action) {
                    case "Create staff":
                        RunCreateStaff();
                        break;
                    case "Remove staff":
                        RunRemoveStaff();
                        break;
                    case "List staffs":
                        RunListStaffs();
                        break;
                    case "Create admin":
                        RunCreateAdmin();
                        break;
                    case "Remove admin":
                        RunRemoveAdmin(user);
                        break;
                    case "List admins":
                        RunListAdmins();
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
        /// Prints a list of all registered staffs
        /// </summary>
        public void RunListStaffs()
        {
            var list = Models.staff.FindAll();
            if (list.Count == 0)
            {
                Log.Info("There are no staffs.");
                return;
            }

            Log.Info("Here are all the staffs of Arlet:");
            foreach (Dictionary<string, object> record in list)
            {
                Log.Info($"\t{record["FirstName"]} {record["LastName"]} ({record["Email"]})");
            }
        }
        
        /// <summary>
        /// Prompts to remove a staff
        /// </summary>
        public void RunRemoveStaff()
        {
            string email = Log.Question<string>("Enter staff email");
            var query = new Dictionary<string, object>();
            query.Add("Email", email);
            var staff = Models.staff.Find(query);
            if (staff == null)
            {
                Log.Error("We could not find an account with that email");
            }
            else
            {
                bool result = Models.staff.Remove(query);
                if (result) 
                {
                    Log.Success($"Staff account with email '{email}' was deleted.");
                }
                else
                {
                    Log.Success("Staff account was not deleted.");
                }
            }
        }
        
        /// <summary>
        /// Prompts to create a new staff account
        /// </summary>
        public void RunCreateStaff()
        {
            Dictionary<string, object> staffDto = new Dictionary<string, object>();
            string email = ""; 

            while (true) 
            {
                email = Log.Question<string>("Enter staff email");
                // ensure the email is unique
                var query = new Dictionary<string, object>();
                query.Add("Email", email);
                var existingStaff = Models.staff.Find(query);
                if (existingStaff != null)
                {
                    Log.Warn("A staff already exists with that email.");
                }
                else
                {
                    break;
                }
            }

            string firstname = Log.Question<string>("Enter staff first name");            
            string lastname = Log.Question<string>("Enter staff last name");            
            
            staffDto.Add("Email", email);
            staffDto.Add("FirstName", firstname);
            staffDto.Add("LastName", lastname);
            staffDto.Add("Username", email);
            staffDto.Add("Password", PasswordHasher.Hash(firstname));

            Models.staff.Insert(staffDto);
            Log.Success("Staff account was created successfully.");
        }
        
        /// <summary>
        /// Prompts to create a new admin account
        /// </summary>
        public void RunCreateAdmin()
        {
            Dictionary<string, object> adminDto = new Dictionary<string, object>();
            string username = ""; 

            while (true) 
            {
                username = Log.Question<string>("Enter admin username");
                // ensure the username is unique
                var query = new Dictionary<string, object>();
                query.Add("Username", username);
                var existingAdmin = Models.admin.Find(query);
                if (existingAdmin != null)
                {
                    Log.Warn("An admin already exists with that username.");
                }
                else
                {
                    break;
                }
            }

            string name = Log.Question<string>("Enter admin's name");            
            string password = "";
            
            int attempts = 3;
            while (attempts-- > 0) 
            {
                password = Log.Password("Enter admin's password");

                if (password.Length < 8) 
                {
                    Log.Warn("Password cannot contain less than 8 characters. Try again.");
                    continue;
                }

                string confirmPass = Log.Password("Confirm admin's password");            
                if (password != confirmPass)
                {
                    Log.Warn("The passwords don't match. Try again.");
                    continue;
                }

                break;
            }

            if (password == "")
            {
                Log.Error("You tried too many times. Please try again.");
                return;
            }
            
            adminDto.Add("Name", name);
            adminDto.Add("Username", username);
            adminDto.Add("Password", PasswordHasher.Hash(password));

            Models.admin.Insert(adminDto);
            Log.Success("Admin account was created successfully.");
        }

        /// <summary>
        /// Prompts to remove an admin
        /// </summary>
        public void RunRemoveAdmin(Admin currentAdmin)
        {
            string username = Log.Question<string>("Enter admin username");
            // cannot delete current admin with current admin
            if (username == currentAdmin.Username)
            {
                Log.Error("Cannot delete currently logged in account.");
                return;
            }

            var query = new Dictionary<string, object>();
            query.Add("Username", username);
            var admin = Models.admin.Find(query);
            if (admin == null)
            {
                Log.Error("We could not find an account with that username");
            }
            else
            {
                bool result = Models.admin.Remove(query);
                if (result) 
                {
                    Log.Success($"Admin account with username '{username}' was deleted.");
                }
                else
                {
                    Log.Success("Admin account was not deleted.");
                }
            }
        }

        /// <summary>
        /// Prints a list of all created admins
        /// </summary>
        public void RunListAdmins()
        {
            var list = Models.admin.FindAll();
            Log.Info("Here are all the admins of Arlet:");
            foreach (Dictionary<string, object> record in list)
            {
                Log.Info($"\t{record["Name"]} ({record["Username"]})");
            }
        }

        protected override void Greet()
        {

            Log.Info("====================================================");
            Log.Info("");
            Log.Info(" Welcome to Arlet's Admin module!");
            Log.Info(" Enter your username and password to login");
            Log.Info("");
            Log.Info("====================================================");
            Log.Info("");
        }

        /// <summary>
        /// Prompts to login as an admin
        /// </summary>
        /// <returns>The admin entity if successful</returns>
        public Admin Login()
        {
            // collect username and password
            uint maxAttemptsAllowed = 3;
            uint attempts = 0;
            #nullable enable
            Admin? admin = null;
            while (attempts < maxAttemptsAllowed) 
            {
                string username = Log.Question<string>("Enter your username", "").Trim();
                string password = Log.Password("Enter your password").Trim();
                
                Dictionary<string, object> query = new Dictionary<string, object>();
                query.Add("Username", username);

                var a = Models.admin.Find(query);
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
                    admin = Models.admin.FromDictionary(a);
                    break;
                }
                else
                {
                    Log.Warn("Could not find a matching admin account. Please try again");
                    Log.Info("");
                    attempts++;
                }
            }

            if (admin == null) return null;
            return admin;
        }
    }
}