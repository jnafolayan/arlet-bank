using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ArletBank
{
    public class FileDatabase : IDatabase
    {
        public FileDatabase(string filename)
        {
            Filename = filename;
            Store = null;
        }

        /// <summary>
        /// Loads the store from disk
        /// </summary>
        public void Load()
        {
            string textContent = File.ReadAllText(Filename, Encoding.UTF8);
            Store = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(textContent);
        }

        /// <summary>
        /// Saves the in-memory store to disk
        /// </summary>
        public void Save()
        {
            string textContent = JsonConvert.SerializeObject(Store);
            File.WriteAllText(Filename, textContent);
        }

        /// <summary>
        /// Gets the contents of a collection. Creates a new key for the collection if it
        /// doesn't exist.
        /// </summary>
        /// <returns>The collection dictionary.</returns>
        /// <param name="name">The name of the collection</param>
        public List<Dictionary<string, object>> Collection(string name)
        {
            if (!Store.ContainsKey(name))
            {
                // create a new entry for it 
                Store[name] = new List<Dictionary<string, object>>();
                // save 
                Save();
            }
            return Store[name];
        }

        public string Filename { get; private set; }
        public Dictionary<string, List<Dictionary<string, object>>> Store { get; private set; }
    }
}