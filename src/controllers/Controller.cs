namespace ArletBank
{
    public class Controller : IController
    {
        public Controller(IDatabase db, Logger log)
        {
            Database = db;
            Log = log;
        }

        public void Run()
        {}

        public IDatabase Database { get; protected set; } 
        public Logger Log { get; }
    }
}