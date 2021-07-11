using System.Collections.Generic;

namespace ArletBank 
{
    /// <summary>
    /// Staff service
    /// </summary>
    public class StaffService : Service<Staff> {
        public StaffService(Model<Staff> model) : base(model)
        {}

        /// <summary>
        /// Attempts to login as a staff
        /// </summary>
        /// <returns>The staff entity</returns>
        /// <param name="username">The staff's username</param>
        /// <param name="password">The staff's password</param>
        public Staff Login(string username, string password)
        {
            var query = new Dictionary<string, object>();
            query.Add("Username", username);

            var staffDoc = Model.Find(query);

            if (staffDoc != null) 
            {
                string hashed = (string)staffDoc["Password"];
                if (PasswordHasher.Verify(password, (string)hashed)) 
                {
                    return Model.FromDictionary(staffDoc);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new staff record
        /// </summary>
        /// <returns>The staff</returns>
        /// <param name="email">The staff's email</param>
        /// <param name="firstname">The staff's first name</param>
        /// <param name="lastname">The staff's last name</param>
        /// <param name="password">The staff's password</param>
        /// <param name="createdBy">The admin who creates this</param>
        public Staff CreateStaff(string email, string firstname, string lastname, string password, string createdBy)
        {
            var staffDto = new Dictionary<string, object>();
            staffDto.Add("Email", email);
            staffDto.Add("FirstName", firstname);
            staffDto.Add("LastName", lastname);
            staffDto.Add("Username", email.ToLower());
            staffDto.Add("Password", PasswordHasher.Hash(password));
            staffDto.Add("CreatedBy", createdBy);

            Model.Insert(staffDto);
            return GetStaff(email);
        }

        /// <summary>
        /// Fetches a staff by their email
        /// </summary>
        /// <returns>The staff</returns>
        /// <param name="email">The staff's email</param>
        public Staff GetStaff(string email)
        {
            var query = new Dictionary<string, object>();
            query.Add("Email", email);
            var doc = Model.Find(query);
            
            return (doc == null) ? null : Model.FromDictionary(doc); 
        }

        /// <summary>
        /// Checks if the staff exists
        /// </summary>
        /// <returns>The answer</returns>
        /// <param name="email">The staff email</param>
        public bool StaffExists(string email)
        {
            return GetStaff(email) != null;
        }

        /// <summary>
        /// Checks if the staff exists
        /// </summary>
        /// <returns>The answer</returns>
        /// <param name="email">The staff email</param>
        public List<Staff> GetAllStaffs() 
        {
            var list = Model.FindAll();
            return list.ConvertAll<Staff>((staff) => Model.FromDictionary(staff));
        }

        /// <summary>
        /// Removes a staff record
        /// </summary>
        /// <returns>Whether it was successful</returns>
        /// <param name="email">The staff email</param>
        public bool RemoveStaff(string email)
        {
            var query = new Dictionary<string, object>();
            query.Add("Email", email);
            return Model.Remove(query);
        }
    }
}