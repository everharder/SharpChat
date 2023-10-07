using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace SharpChat.Utility
{
    internal class JsonSerializer : ISerializer
    {
        /// <inheritdoc/>
        public T Deserialize<T>(string data)
            => (T)Deserialize(data, typeof(T));

        /// <inheritdoc/>
        public object Deserialize(string data, Type type) 
            => System.Text.Json.JsonSerializer.Deserialize(data, type, GetJsonOptions());

        /// <inheritdoc/>
        public string Serialize(object data) 
            => System.Text.Json.JsonSerializer.Serialize(data, GetJsonOptions());

        private JsonSerializerOptions GetJsonOptions()
            => new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };
    }
}
