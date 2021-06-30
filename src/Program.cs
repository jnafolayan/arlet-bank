using System;

namespace ArletBank
{
    class Program
    {
        public static Logger Log = new Logger(Logger.Levels.DEBUG);
        static void Main(string[] args)
        {
            Log.Info("====================================================");
            Log.Info("");
            Log.Info(" Welcome to banking made easy with Arlet!");
            Log.Info(" What do you want to do today?");
            Log.Info("");
            Log.Info("====================================================");

            string userType = GetUserTypeFromArgs(args);
            Controller controller;
            if (userType == "admin") 
            {
                controller = new AdminController(Log);
            }
            else if (userType == "staff")
            {
                controller = new StaffController(Log);
            }
            else if (userType == "customer")
            {
                controller = new CustomerController(Log);
            }
            else
            {
                Log.Error($"A controller for a user '{userType}' does not exist");
            }
            // var m = new InquirerCore.Prompts.ListInput("hello", "can i ask you", new string[]{"show"});
            // m.Ask();
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
