using System.Collections.Generic;
using System.Data;

namespace AS.Domain.Interfaces
{
    /// <summary>
    /// An Adapter Pattern Interface to execute database commands for any db provider type
    /// </summary>
    public interface IDatabase
    {
        int ExecuteNonQuery(string command, CommandType commandType, Dictionary<string, object> parameters);
    }
}