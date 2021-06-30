namespace ArletBank
{
    public class CustomerController : Controller
    {
        public CustomerController(IDatabase db, Logger log) : base(db, log)
        {
        }

        public override void Run()
        {
            // var user = new Model<User>();
        }
    }
}