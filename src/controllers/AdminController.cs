using System;
using System.Collections.Generic;

namespace ArletBank
{
    public class AdminController : Controller
    {
        public AdminController(IDatabase db, Logger log, Models models) : base(db, log, models)
        {
            
        }

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

            string[] actions = new string[] { "Create staff", "Remove staff", "List staffs", "Quit" };
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
                    // fallthrough
                    case "Quit":
                    default:
                        abort = true;
                        break;
                }
                Log.Info("");
            }
        }

        public void RunListStaffs()
        {
            var query = new Dictionary<string, object>();
            var list = Models.staff.FindAll(query);
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