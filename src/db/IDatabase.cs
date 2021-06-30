using System.Collections.Generic;

namespace ArletBank
{
    public interface IDatabase
    {
        Dictionary<string, Dictionary<string, object>> Collection(string name);
    }
}