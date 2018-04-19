#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using ESS.FW.Common.Serializing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

#endregion

namespace ESS.FW.Common.Serializing
{
    /// <summary>
    ///     Json.Net implementationof IJsonSerializer.
    /// </summary>
    public class NewtonsoftJsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public NewtonsoftJsonSerializer(params Type[] creationWithoutConstructorTypes)
        {
            _settings = new JsonSerializerSettings
            {
                Converters =
                    new List<JsonConverter>
                    {
                        new IsoDateTimeConverter(),
                        new CreateObjectWithoutConstructorConverter(creationWithoutConstructorTypes),
                        new StringEnumConverter { CamelCaseText = true }
                    },
                ContractResolver = new CustomContractResolver(),
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
        }

        /// <summary>
        ///     Serialize an object to json string.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string Serialize(object obj)
        {
            return obj == null ? null : JsonConvert.SerializeObject(obj, _settings);
        }

        /// <summary>
        ///     Deserialize a json string to an object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, _settings);
        }

        /// <summary>
        ///     Deserialize a json string to a strong type object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Deserialize<T>(string value) where T : class
        {
            if (value.StartsWith("["))
            {
                return JsonConvert.DeserializeObject<T>(JArray.Parse(value)
                .ToString(), _settings);
            }
            else {
                return JsonConvert.DeserializeObject<T>(JObject.Parse(value)
                .ToString(), _settings);
            }            
        }

        private class CreateObjectWithoutConstructorConverter : JsonConverter
        {
            private readonly IEnumerable<Type> _creationWithoutConstructorTypes;

            public CreateObjectWithoutConstructorConverter(IEnumerable<Type> creationWithoutConstructorTypes)
            {
                _creationWithoutConstructorTypes = creationWithoutConstructorTypes;
            }

            public override bool CanWrite { get { return false; } }

            public override bool CanConvert(Type objectType)
            {
                return _creationWithoutConstructorTypes.Any(x => x.IsAssignableFrom(objectType));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotSupportedException("CreateObjectWithoutConstructorConverter should only be used while deserializing.");
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                {
                    return null;
                }
                var target = FormatterServices.GetUninitializedObject(objectType);
                serializer.Populate(reader, target);
                return target;
            }
        }

        private class CustomContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var jsonProperty = base.CreateProperty(member, memberSerialization);
                if (jsonProperty.Writable)
                {
                    return jsonProperty;
                }
                var property = member as PropertyInfo;
                if (property == null)
                {
                    return jsonProperty;
                }
                var hasPrivateSetter = property.GetSetMethod(true) != null;
                jsonProperty.Writable = hasPrivateSetter;

                return jsonProperty;
            }
        }
    }
}