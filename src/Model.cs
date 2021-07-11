using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArletBank
{
    /// <summary>
    /// Model is an abstraction of an entity in the database. It contains
    /// useful methods to CRUD records
    /// </summary>
    public class Model<T> where T : new()
    {
        /// <summary>
        /// Instantiates a new model, providing the db and collection is belongs to
        /// </summary>
        public Model(IDatabase db, string collection)
        {
            ID = Guid.NewGuid().ToString();
            Database = db;
            Collection = collection;
        }

        /// <summary>
        /// Finds a record by its id
        /// </summary>
        /// <returns>The record if found, otherwise, null.</returns>
        /// <param name="id">The id of the record</param>
        public Dictionary<string, object> FindByID(string id)
        {
            var col = Database.Collection(Collection);
            var record = col.Find((rec) => (string)rec["ID"] == id);
            if (record != null)
            {
                return record;
            }
            return null;
        }

        /// <summary>
        /// Finds a list of records that match a query
        /// </summary>
        /// <returns>A list of the records found.</returns>
        /// <param name="query">The query in a key/value form.</param>
        public List<Dictionary<string, object>> FindAll(Dictionary<string, object> query)
        {
            var col = Database.Collection(Collection);
            var keys = query.Keys;

            var records = col.FindAll((rec) => {
                foreach (string key in keys)
                {
                    if (!rec[key].Equals(query[key]))
                    {
                        return false;
                    }
                }
                return true;
            });

            return records;
        }

        /// <summary>
        /// Returns all the records
        /// </summary>
        /// <returns>A list of the records.</returns>
        public List<Dictionary<string, object>> FindAll()
        {
            var query = new Dictionary<string, object>();
            return FindAll(query);
        }

        /// <summary>
        /// Finds the first record that matches a query.
        /// </summary>
        /// <returns>The matching record.</returns>
        /// <param name="query">The query in a key/value form.</param>
        public Dictionary<string, object> Find(Dictionary<string, object> query)
        {
            var col = Database.Collection(Collection);
            var keys = query.Keys;
            
            Dictionary<string, object> record = col.Find((rec) => {
                foreach (KeyValuePair<string, object> q in query)
                {
                    if (!rec[q.Key].Equals(q.Value))
                    {
                        return false;
                    }
                }
                return true;
            });
 
            return record;
        }

        /// <summary>
        /// Inserts a new record into the collection
        /// </summary>
        /// <param name="dto">The new record.</param>
        public void Insert(Dictionary<string, object> dto)
        {
            var col = Database.Collection(Collection);
            col.Add(dto);
            Database.Save();
        }

        /// <summary>
        /// Removes the first record that matches the query.
        /// </summary>
        /// <returns>Whether the operation was successful or not.</returns>
        /// <param name="query">The query in a key/value form.</param>
        public bool Remove(Dictionary<string, object> query)
        {
            var col = Database.Collection(Collection);
            var record = Find(query);
            if (record == null) 
            {
                return false;
            }
            bool result = col.Remove(record);
            Database.Save();
            return result;
        }

        /// <summary>
        /// Updates the first record that matches the query.
        /// </summary>
        /// <returns>Whether the operation was successful or not.</returns>
        /// <param name="query">The query in a key/value form.</param>
        /// <param name="update">The patch object</param>
        public bool UpdateOne(Dictionary<string, object> query, Dictionary<string, object> update)
        {
            var col = Database.Collection(Collection);
            var record = Find(query);
            if (record == null) 
            {
                return false;
            }
            foreach (KeyValuePair<string, object> entry in update)
            {
                record[entry.Key] = entry.Value;
            }
            Database.Save();
            return true;
        }

        /// <summary>
        /// Converts a dictionary to an instance of the entity this model wraps.
        /// </summary>
        /// <returns>An instance of the entity</returns>
        /// <param name="dict">The dictionary.</param>
        public T FromDictionary(Dictionary<string, object> dict)
        {
            var t = new T();
            PropertyInfo[] properties = t.GetType().GetProperties();
 
            foreach (PropertyInfo property in properties)
            {
                if (!dict.Any(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase)))
                    continue;
 
                KeyValuePair<string, object> item = dict.First(x => x.Key.Equals(property.Name, StringComparison.InvariantCultureIgnoreCase));
 
                // Find which property type (int, string, double? etc) the CURRENT property is...
                Type tPropertyType = t.GetType().GetProperty(property.Name).PropertyType;
 
                // Fix nullables...
                Type newT = Nullable.GetUnderlyingType(tPropertyType) ?? tPropertyType;
 
                // ...and change the type
                object newA = Convert.ChangeType(item.Value, newT);
                t.GetType().GetProperty(property.Name).SetValue(t, newA, null);
            }
            return t;
        }
        public string ID { get; private set; }
        public IDatabase Database { get; private set; }
        public string Collection { get; private set; }

    }
}