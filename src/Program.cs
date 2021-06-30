using System;

namespace ArletBank
{
    class Program
    {
        public static Logger log = new Logger(Logger.Levels.DEBUG);
        static void Main(string[] args)
        {
            log.Info("====================================================");
            log.Info("");
            log.Info(" Welcome to banking made easy with Arlet!");
            log.Info(" What do you want to do today?");
            log.Info("");
            log.Info("====================================================");
            log.Info("");

            string userType = GetUserTypeFromArgs(args);
            FileDatabase db = new FileDatabase("./db.json");
            db.Load();
            Controller controller = null;

            if (userType == "admin") 
            {
                controller = new AdminController(db, log);
            }
            else if (userType == "staff")
            {
                controller = new StaffController(db, log);
            }
            else if (userType == "customer")
            {
                controller = new CustomerController(db, log);
            }
            else
            {
                log.Error($"A controller for a '{userType}' does not exist");
            }
            // var m = new InquirerCore.Prompts.ListInput("hello", "can i ask you", new string[]{"show"});
            // m.Ask();

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
