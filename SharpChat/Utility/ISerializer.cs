using System;
using System.Collections.Generic;
using System.Text;

namespace SharpChat.Utility
{
    public interface ISerializer
    {
        /// <summary>
        /// Serializes the given <paramref name="data"/>
        /// </summary>
        string Serialize(object data);

        /// <summary>
        /// Deserializes the given <paramref name="data"/>
        /// </summary>
        T Deserialize<T>(string data);

        /// <summary>
        /// Deserializes the given <paramref name="data"/> as type <paramref name="type"/>
        /// </summary>
        object Deserialize(string data, Type type);
    }
}
