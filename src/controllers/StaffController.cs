namespace ArletBank
{
    public class StaffController : Controller
    {
        public StaffController(IDatabase db, Logger log) : base(db, log)
        {
        }
    }
}