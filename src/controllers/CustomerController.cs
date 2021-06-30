namespace ArletBank
{
    public class CustomerController : Controller
    {
        public CustomerController(IDatabase db, Logger log) : base(db, log)
        {
        }
    }
}