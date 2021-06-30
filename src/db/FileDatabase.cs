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

        public void Load()
        {
            string textContent = File.ReadAllText(Filename, Encoding.UTF8);
            Store = JsonConvert.DeserializeObject<Dictionary<string, object>>(textContent);
            Console.WriteLine(textContent);
        }

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
        public Dictionary<string, Dictionary<string, object>> Collection(string name)
        {
            if (!Store.ContainsKey(name))
            {
                // create a new entry for it 
                Store[name] = new Dictionary<string, Dictionary<string, object>>();
                // save 
                Save();
            }
            return Store[name] as Dictionary<string, Dictionary<string, object>>;
        }

        public string Filename { get; private set; }
        public Dictionary<string, object> Store { get; private set; }
    }
}