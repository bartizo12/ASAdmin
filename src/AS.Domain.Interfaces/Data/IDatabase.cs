using System.Collections.Generic;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// An Adapter Pattern Interface to execute database commands in db agnostic way.
    /// </summary>
    public interface IDatabase
    {
        int ExecuteNonQuery(string spName, Dictionary<string, object> parameters);
    }
}