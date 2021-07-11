namespace ArletBank
{
    /// <summary>
    /// A controller is a combination of handlers for a specific module
    /// </summary>
    public abstract class Controller
    {
        /// <param name="db">A reference to the app database</param>
        /// <param name="log">A reference to the app logger</param>
        /// <param name="models">A collection of models the controller has access to</param>
        public Controller(IDatabase db, Logger log, Models models)
        {
            Database = db;
            Log = log;
            Models = models;
        }

        /// <summary>
        /// Starts execution of the controller
        /// </summary>
        public virtual void Run()
        {}

        /// <summary>
        /// Prints a welcome message out to the console
        /// </summary>
        protected virtual void Greet() 
        {}
        
        public IDatabase Database { get; protected set; } 
        public Logger Log { get; }
        public Models Models { get; }
    }
}