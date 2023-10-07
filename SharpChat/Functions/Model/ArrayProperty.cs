using SharpChat.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace SharpChat.Functions.Model
{
    /// <summary>
    /// An array of some type
    /// </summary>
    public class ArrayProperty : Property
    {
        /// <inheritdoc/>
        public override string SchemaType => "array";

        /// <summary>
        /// The items of the array
        /// </summary>
        public Property Item { get; }

        public ArrayProperty(
            string name, 
            string description, 
            Type dotNetType, 
            Property item, 
            bool isRequired,
            object defaultValue) : base(name, description, dotNetType, isRequired, defaultValue)
        {
            Item = item ?? throw new ArgumentNullException(nameof(item));
            if(!dotNetType.IsArray)
            {
                throw new ArgumentException($"{dotNetType.Name} is not an array type");
            }
        }

        /// <inheritdoc/>
        public override Dictionary<string, object> GetSchema()
        {
            Dictionary<string, object> schema = base.GetSchema();
            schema["items"] = Item.GetSchema();
            return schema;
        }

        /// <inheritdoc/>
        public override object DeserializeValue(string data, ISerializer serializer)
        {
            string[] strings = serializer.Deserialize<object[]>(data)?
                .Select(x => x?.ToString())
                .ToArray()
                ?? Array.Empty<string>();

            object[] arr = strings
                .Select(x => Item.DeserializeValue(x, serializer))
            .ToArray();

            // change object[] to DotNetType[]
            Array destinationArray = Array.CreateInstance(Item.DotNetType, arr.Length);
            Array.Copy(arr, destinationArray, arr.Length);

            return destinationArray;
        }
    }
}
