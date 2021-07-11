using System.Collections.Generic;

namespace ArletBank
{
    public interface IDatabase
    {
        void Save();
        List<Dictionary<string, object>> Collection(string name);
    }
}