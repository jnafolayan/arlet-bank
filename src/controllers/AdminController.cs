namespace ArletBank
{
    public class AdminController : Controller
    {
        public AdminController(IDatabase db, Logger log) : base(db, log)
        {
        }
    }
}