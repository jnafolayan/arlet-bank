using System.Collections.Generic;

namespace ArletBank 
{
    /// <summary>
    /// Admin service
    /// </summary>
    public class AdminService : Service<Admin> {
        public AdminService(Model<Admin> model) : base(model)
        {}
        
        /// <summary>
        /// Attempts to log in an admin
        /// </summary>
        /// <returns>The admin entity</returns>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        public Admin Login(string username, string password)
        {
            var query = new Dictionary<string, object>();
            query.Add("Username", username);

            var adminDoc = Model.Find(query);

            if (adminDoc != null) 
            {
                string hashed = (string)adminDoc["Password"];
                if (PasswordHasher.Verify(password, (string)hashed)) 
                {
                    return Model.FromDictionary(adminDoc);
                }
            }

            return null;
        }

        /// <summary>
        /// Creates a new admin record
        /// </summary>
        /// <returns>The admin entity</returns>
        /// <param name="username">The username</param>
        /// <param name="password">The password</param>
        /// <param name="name">The admin's name</param>
        /// <param name="createdBy">The creator of this account</param>
        public Admin CreateAdmin(string username, string password, string name, string createdBy)
        {
            var adminDto = new Dictionary<string, object>();
            adminDto.Add("Name", name);
            adminDto.Add("Username", username);
            adminDto.Add("Password", PasswordHasher.Hash(password));
            adminDto.Add("CreatedBy", createdBy);
            Model.Insert(adminDto);

            return GetAdmin(username);
        }

        /// <summary>
        /// Fetches an admin by their username
        /// </summary>
        /// <returns>The admin entity</returns>
        /// <param name="username">The username</param>
        public Admin GetAdmin(string username)
        {
            var query = new Dictionary<string, object>();
            query.Add("Username", username);
            var doc = Model.Find(query);
            
            return (doc == null) ? null : Model.FromDictionary(doc); 
        }

        /// <summary>
        /// Checks if the admin record exists
        /// </summary>
        /// <returns>Whether it exists or not</returns>
        /// <param name="username">The username</param>
        public bool AdminExists(string username)
        {
            return GetAdmin(username) != null;
        }

        /// <summary>
        /// Fetches all admin records
        /// </summary>
        /// <returns>A list of the admins</returns>
        public List<Admin> GetAllAdmins() 
        {
            var list = Model.FindAll();
            return list.ConvertAll<Admin>((admin) => Model.FromDictionary(admin));
        }

        /// <summary>
        /// Removes an admin by their username
        /// </summary>
        /// <returns>Whether it was successful or not</returns>
        /// <param name="username">The username</param>
        public bool RemoveAdmin(string username)
        {
            var query = new Dictionary<string, object>();
            query.Add("Username", username);
            return Model.Remove(query);
        }
    }
}