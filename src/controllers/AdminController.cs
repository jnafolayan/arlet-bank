using System;
using System.Collections.Generic;

namespace ArletBank
{
    /// <summary>
    /// Controller for the admin module
    /// </summary>
    public class AdminController : Controller
    {
        public AdminController(IDatabase db, Logger log, Services services, Models models) : base(db, log, services, models)
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
                        RunCreateStaff(user);
                        break;
                    case "Remove staff":
                        RunRemoveStaff();
                        break;
                    case "List staffs":
                        RunListStaffs();
                        break;
                    case "Create admin":
                        RunCreateAdmin(user);
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
            var list = Services.staff.GetAllStaffs();
            if (list.Count == 0)
            {
                Log.Info("There are no staffs.");
                return;
            }

            Log.Info("Here are all the staffs of Arlet:");
            foreach (Staff staff in list)
            {
                Log.Info($"\t{staff.FirstName} {staff.LastName} ({staff.Email})");
            }
        }
        
        /// <summary>
        /// Prompts to remove a staff
        /// </summary>
        public void RunRemoveStaff()
        {
            string email = Log.Question<string>("Enter staff email", "").Trim();
            if (!Services.staff.StaffExists(email))
            {
                Log.Error("We could not find an account with that email");
            }
            else
            {
                bool result = Services.staff.RemoveStaff(email);
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
        public void RunCreateStaff(Admin currentAdmin)
        {
            string email = "";
            while (true) 
            {
                email = Log.Question<string>("Enter staff email", "").Trim();
                // ensure the email is unique
                if (Services.staff.StaffExists(email))
                {
                    Log.Warn("A staff already exists with that email.");
                }
                else
                {
                    break;
                }
            }

            string firstname = Log.Question<string>("Enter staff first name", "").Trim();            
            string lastname = Log.Question<string>("Enter staff last name", "").Trim();            
            
            string password = firstname.ToLower();
            Services.staff.CreateStaff(email, firstname, lastname, password, currentAdmin.Username);

            Log.Success("Staff account was created successfully.");
        }
        
        /// <summary>
        /// Prompts to create a new admin account
        /// </summary>
        public void RunCreateAdmin(Admin currentAdmin)
        {
            string username = ""; 
            int attempts = 3;
            bool valid = false;
            
            while (attempts-- > 0) 
            {
                username = Log.Question<string>("Enter admin username", "").Trim();
                // ensure the username is unique
                if (Services.admin.AdminExists(username))
                {
                    Log.Warn("An admin already exists with that username.");
                }
                else
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
            {
                Log.Error("You tried too many times. Please try again.");
                return;
            }

            string name = Log.Question<string>("Enter admin's name", "").Trim();            
            string password = "";
            
            attempts = 3;
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
            
            // hash password and create
            Services.admin.CreateAdmin(username, password, name, currentAdmin.Username);

            Log.Success("Admin account was created successfully.");
        }

        /// <summary>
        /// Prompts to remove an admin
        /// </summary>
        public void RunRemoveAdmin(Admin currentAdmin)
        {
            string username = Log.Question<string>("Enter admin username", "").Trim();
            // cannot delete current admin with current admin
            if (username == currentAdmin.Username)
            {
                Log.Error("Cannot delete currently logged in account.");
                return;
            }

            if (!Services.admin.AdminExists(username))
            {
                Log.Error("We could not find an account with that username");
            }
            else
            {
                bool result = Services.admin.RemoveAdmin(username);
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
            var admins = Services.admin.GetAllAdmins();
            Log.Info("Here are all the admins of Arlet:");
            foreach (Admin admin in admins)
            {
                Log.Info($"\t{admin.Name} ({admin.Username})");
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
                string password = Log.Password("Enter your password");
                
                admin = Services.admin.Login(username, password);

                if (admin == null)
                {
                    Log.Warn("Could not find a matching admin account. Please try again");
                    Log.Info("");
                    attempts++;
                }
                else
                {
                    break;
                }
            }

            return admin;
        }
    }
}