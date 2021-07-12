using System;
using System.Collections.Generic;

namespace ArletBank
{
    public class Models
    {
        public Model<Admin> admin;
        public Model<Staff> staff;
        public Model<Customer> customer;
        public Model<Account> account;
        public Model<Transaction> transaction;
    }

    public class Services
    {
        public AdminService admin;
        public StaffService staff;
        public CustomerService customer;
        public AccountService account;
        public TransactionService transaction;
    }
    
    class Program
    {
        public static Logger log = new Logger(Logger.Levels.DEBUG);
        static void Main(string[] args)
        {
            string userType = GetUserTypeFromArgs(args);
            FileDatabase db = new FileDatabase("./db.json");
            db.Load();
            Controller controller = null;

            var models = new Models();
            models.admin = new Model<Admin>(db, "admins");
            models.staff = new Model<Staff>(db, "staffs");
            models.customer = new Model<Customer>(db, "customers");
            models.account = new Model<Account>(db, "accounts");
            models.transaction = new Model<Transaction>(db, "transactions");

            var services = new Services();
            services.admin = new AdminService(models.admin);
            services.staff = new StaffService(models.staff);
            services.customer = new CustomerService(models.customer);
            services.account = new AccountService(models.account);
            services.transaction = new TransactionService(models.transaction);

            if (userType == "admin") 
            {
                controller = new AdminController(db, log, services, models);
            }
            else if (userType == "staff")
            {
                controller = new StaffController(db, log, services, models);
            }
            else if (userType == "customer")
            {
                controller = new CustomerController(db, log, services, models);
            }
            else
            {
                log.Error($"A controller for a '{userType}' does not exist");
            }

            if (controller != null)
            {
                controller.Run();
            }

            log.Info("");
        }

        static string GetUserTypeFromArgs(string[] args)
        {
            if (args.Length == 1) 
            {
                return args[0];
            }
            
            return "customer";
        }
    }
}
