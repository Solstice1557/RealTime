using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;

namespace RealTime.BL.Helpers
{
    public static class StringExtensions
    {
        private static readonly JsonSerializerOptions CamelCaseJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private static readonly JsonSerializerOptions DefaultJsonOptions = new JsonSerializerOptions();

        private static readonly JsonSerializerOptions CaseInsensitiveJsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions SnakeCaseJsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = SnakeCaseNamingPolicy.Instance
        };

        public static string ToJson<T>(this T value, bool isCamelCase = true)
        {
            return value == null
                ? null
                : JsonSerializer.Serialize(value, isCamelCase ? CamelCaseJsonOptions : DefaultJsonOptions);
        }

        public static T FromJson<T>(
            this string value,
            JsonSerializerBehaviour behaviour = JsonSerializerBehaviour.CamelCase)
        {
            var options = GetJsonSerializerOptions<T>(behaviour);

            return string.IsNullOrEmpty(value)
                ? default
                : JsonSerializer.Deserialize<T>(value, options);
        }

        public static T TryFromJson<T>(
            this string value,
            JsonSerializerBehaviour behaviour = JsonSerializerBehaviour.CamelCase)
        {
            try
            {
                return value.FromJson<T>(behaviour);
            }
            catch
            {
                return default;
            }
        }

        public static string ToXmlUtf8<T>(this T value)
        {
            var serializer = new XmlSerializer(typeof(T));

            using var stringWriter = new Utf8StringWriter();
            serializer.Serialize(stringWriter, value);
            return stringWriter.ToString();
        }

        public class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding => Encoding.UTF8;
        }

        private static JsonSerializerOptions GetJsonSerializerOptions<T>(JsonSerializerBehaviour behaviour)
        {
            return behaviour switch
            {
                JsonSerializerBehaviour.Default => DefaultJsonOptions,
                JsonSerializerBehaviour.CamelCase => CamelCaseJsonOptions,
                JsonSerializerBehaviour.CaseInsensitive => CaseInsensitiveJsonOptions,
                JsonSerializerBehaviour.SnakeCase => SnakeCaseJsonOptions,
                _ => throw new ArgumentOutOfRangeException(nameof(behaviour), behaviour, null)
            };
        }

        public class SnakeCaseNamingPolicy : JsonNamingPolicy
        {
            public static SnakeCaseNamingPolicy Instance { get; } = new SnakeCaseNamingPolicy();

            public override string ConvertName(string name)
            {
                return string.Concat(
                    name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x : x.ToString()))
                    .ToLower();
            }
        }
    }

    public enum JsonSerializerBehaviour
    {
        /// <summary>
        /// Default JsonSerializerOptions are used
        /// </summary>
        Default,
        /// <summary>
        /// PropertyNamingPolicy is CamelCase
        /// </summary>
        CamelCase,
        /// <summary>
        /// Property names are case insensitive
        /// </summary>
        CaseInsensitive,
        /// <summary>
        /// Property names are of snake_case type
        /// </summary>
        SnakeCase
    }
}
