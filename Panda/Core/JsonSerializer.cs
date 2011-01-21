using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;

namespace Panda.Core
{
    /// <summary>
    /// Provides serialization and deserialization functionality for AJAX-enabled applications.
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        private readonly JavaScriptSerializer _serializer;
 
        /// <summary>
        /// Initializes a new instance of the Panda.Core.JSONSerializer class.
        /// </summary>
        public JsonSerializer()
        {
            _serializer = new JavaScriptSerializer();
        }

        /// <summary>
        /// Gets or sets the maximum length of JSON strings that are accepted by the
        /// Panda.Core.JSONSerializer class.
        /// 
        /// Returns:
        ///     The maximum length of JSON strings. The default is 2097152 characters, which
        ///     is equivalent to 4 MB of Unicode string data.
        /// 
        /// System.ArgumentOutOfRangeException:
        ///     The property is set to a value that is less than one.
        /// </summary>
        public int MaxJsonLength
        {
            get
            {
                return _serializer.MaxJsonLength;
            }
            set
            {
                _serializer.MaxJsonLength = value;
            }
        }
    
        /// <summary>
        /// Gets or sets the limit for constraining the number of object levels to process.
        ///
        /// Returns:
        ///     The number of object levels. The default is 100.
        ///
        /// Exceptions:
        ///   System.ArgumentOutOfRangeException:
        ///     The property is set to a value that is less than one.
        /// </summary>
        public int RecursionLimit
        {
            get
            {
                return _serializer.RecursionLimit;
            }
            set
            {
                _serializer.RecursionLimit = value;
            }
        }

        /// <summary>
        /// Converts the given object to the specified type.
        /// 
        /// Exceptions:
        ///   System.InvalidOperationException:
        ///     obj (or a nested member of obj) contains a "__type" property that indicates
        ///     a custom type, but the type resolver that is associated with the serializer
        ///     cannot find a corresponding managed type.  -or- obj (or a nested member of
        ///     obj) contains a “__type” property that indicates a custom type, but the result
        ///     of deserializing the corresponding JSON string cannot be assigned to the
        ///     expected target type.  -or- obj (or a nested member of obj) contains a “__type”
        ///     property that indicates either System.Object or a non-instantiable type (for
        ///     example, an abstract type or an interface).  -or- An attempt was made to
        ///     convert obj to an array-like managed type, which is not supported for use
        ///     as a deserialization target. -or- It is not possible to convert obj to T.
        ///
        ///   System.ArgumentException:
        ///     obj is a dictionary type and a non-string key value was encountered. -or-
        ///     obj includes member definitions that are not available on type T.
        /// </summary>
        /// <typeparam name="T">The type to which obj will be converted.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The object that has been converted to the target type.</returns>
        public T ConvertToType<T>(object obj)
        {
            return _serializer.ConvertToType<T>(obj);
        }
 
        /// <summary>
        /// Converts the specified JSON string to an object of type T.
        /// 
        /// Exceptions:
        ///   System.ArgumentException:
        ///     The input length exceeds the value of Panda.Core.JSONSerializer.MaxJsonLength.
        ///     -or- The recursion limit defined by Panda.Core.JSONSerializer.RecursionLimit
        ///     was exceeded. -or- input contains an unexpected character sequence. -or-
        ///     input is a dictionary type and a non-string key value was encountered. -or-
        ///     input includes member definitions that are not available on type T.
        ///
        ///   System.ArgumentNullException:
        ///     input is null.
        ///
        ///   System.InvalidOperationException:
        ///     input contains a “__type” property that indicates a custom type, but the
        ///     type resolver associated with the serializer cannot find a corresponding
        ///     managed type. -or- input contains a “__type” property that indicates a custom
        ///     type, but the result of deserializing the corresponding JSON string cannot
        ///     be assigned to the expected target type. -or- input contains a “__type” property
        ///     that indicates either System.Object or a non-instantiable type (for example,
        ///     an abstract types or an interface). -or- An attempt was made to convert a
        ///     JSON array to an array-like managed type that is not supported for use as
        ///     a JSON deserialization target. -or- It is not possible to convert input to
        ///     T.
        /// </summary>
        /// <typeparam name="T">The type of the resulting object.</typeparam>
        /// <param name="input">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public T Deserialize<T>(string input)
        {
            return _serializer.Deserialize<T>(input);
        }

        /// <summary>
        /// Converts the specified JSON string to an object graph.
        /// 
        /// Exceptions:
        ///   System.ArgumentNullException:
        ///     input is null.
        ///
        ///   System.ArgumentException:
        ///     The input length exceeds the value of Panda.Core.JSONSerializer.MaxJsonLength.
        ///     -or- The recursion limit defined by Panda.Core.JSONSerializer.RecursionLimit
        ///     was exceeded. -or- input contains an unexpected character sequence. -or-
        ///     input is a dictionary type and a non-string key value was encountered. -or-
        ///     input includes member definitions that are not available on the target type.
        ///
        ///   System.InvalidOperationException:
        ///     input contains a “__type” property that indicates a custom type, but the
        ///     type resolver that is currently associated with the serializer cannot find
        ///     a corresponding managed type. -or- input contains a “__type” property that
        ///     indicates a custom type, but the result of deserializing the corresponding
        ///     JSON string cannot be assigned to the expected target type. -or- input contains
        ///     a “__type” property that indicates either System.Object or a non-instantiable
        ///     type (for example, an abstract type or an interface).  -or- An attempt was
        ///     made to convert a JSON array to an array-like managed type that is not supported
        ///     for use as a JSON deserialization target. -or- It is not possible to convert
        ///     input to the target type.
        /// </summary>
        /// <param name="input">The JSON string to be deserialized.</param>
        /// <returns>The deserialized object.</returns>
        public object DeserializeObject(string input)
        {
            return _serializer.DeserializeObject(input);
        }

        /// <summary>
        /// Converts an object to a JSON string.
        /// 
        /// Exceptions:
        ///   System.InvalidOperationException:
        ///     The resulting JSON string exceeds the value of Panda.Core.JSONSerializer.MaxJsonLength.
        ///     -or- obj contains a circular reference. A circular reference occurs when
        ///     a child object has a reference to a parent object, and the parent object
        ///     has a reference to the child object.
        ///
        ///   System.ArgumentException:
        ///     The recursion limit defined by Panda.Core.JSONSerializer.RecursionLimit
        ///     was exceeded.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized JSON string.</returns>
        public string Serialize(object obj)
        {
            return _serializer.Serialize(obj);
        }
 
        /// <summary>
        /// Serializes an object and writes the resulting JSON string to the specified
        ///     System.Text.StringBuilder object.
        ///     
        /// Exceptions:
        ///   System.InvalidOperationException:
        ///     The resulting JSON string exceeds the value of Panda.Core.JSONSerializer.MaxJsonLength.
        ///     -or- obj contains a circular reference. A circular reference occurs when
        ///     a child object has a reference to a parent object, and the parent object
        ///     has a reference to the child object.
        ///
        ///   System.ArgumentException:
        ///     The recursion limit defined by Panda.Core.JSONSerializer.RecursionLimit
        ///     was exceeded.
        ///
        ///   System.ArgumentNullException:
        ///     output is null.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <param name="output">The System.Text.StringBuilder object that is used to write the JSON string.</param>
        public void Serialize(object obj, StringBuilder output)
        {
            _serializer.Serialize(obj, output);
        }
    }
}
