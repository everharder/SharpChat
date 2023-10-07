using System;
using System.Collections.Generic;
using System.Text;

namespace SharpChat.Functions
{
    /// <summary>
    /// Interface for classes that can provide a schema
    /// </summary>
    internal interface ISchemaProvider
    {
        /// <summary>
        /// Create the json schema for the property
        /// </summary>
        Dictionary<string, object> GetSchema();
    }
}
