namespace ArletBank
{
    /// <summary>
    /// Abstract Entity class. Base class for logical entities of the banking application.
    /// </summary>
    public abstract class Entity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string FullName 
        {
            get { return FirstName + " " + LastName; }
        }
    }
}