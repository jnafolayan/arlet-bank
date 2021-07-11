namespace ArletBank
{
    public abstract class Controller
    {
        public Controller(IDatabase db, Logger log, Models models)
        {
            Database = db;
            Log = log;
            Models = models;
        }

        public virtual void Run()
        {}
        protected virtual void Greet() 
        {}

        public IDatabase Database { get; protected set; } 
        public Logger Log { get; }
        public Models Models { get; }
    }
}