using System;
using System.Collections.Generic;

namespace ArletBank
{
    /// <summary>
    /// Model is an abstraction of an entity in the database. It contains
    /// useful methods to CRUD records
    /// </summary>
    public class Model<T> where T : class
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
        public T FindByID(string id)
        {
            var col = Database.Collection(Collection);
            if (col.ContainsKey(id))
            {
                return col[id] as T;
            }
            return null;
        }

        /// <summary>
        /// Finds a list of records that match a query
        /// </summary>
        /// <returns>A list of the records found.</returns>
        /// <param name="query">The query in a key/value form.</param>
        public List<T> FindAll(Dictionary<string, object> query)
        {
            var col = Database.Collection(Collection);
            List<T> results = new List<T>();
            var keys = query.Keys;

            foreach (var doc in col)
            {
                bool valid = true;
                foreach (string key in keys)
                {
                    if (doc.Value[key] != query[key])
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    results.Add(doc.Value as T);
                }
            }
            return results;
        }

        public string ID { get; private set; }
        public IDatabase Database { get; private set; }
        public string Collection { get; private set; }

    }
}