using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

namespace TestGitHub.Libraries.Templates
{
    /// <summary>
    /// JsonUtility(文字列:UTF-8保存前提).
    /// </summary>
    public static class JsonUtility
    {
        private static readonly DataContractJsonSerializerSettings Settings =
            new DataContractJsonSerializerSettings
            {
                UseSimpleDictionaryFormat = true,
            };

        /// <summary>
        /// Serialize(JSON to Object).
        /// </summary>
        /// <param name="graph">graph.</param>
        /// <param name="isDictionaryFormat">isDictionaryFormat.</param>
        /// <returns>serialized string.</returns>
        public static string Serialize(object graph, bool isDictionaryFormat)
        {
            if (graph == null)
            {
                throw new ArgumentNullException(nameof(graph));
            }

            var stream = new MemoryStream();
            if (isDictionaryFormat)
            {
                // 改行込み.
                var writer =
                    JsonReaderWriterFactory.CreateJsonWriter(
                    stream, Encoding.UTF8, true, true, "  ");

                var serializer = new DataContractJsonSerializer(graph.GetType(), Settings);
                serializer.WriteObject(writer, graph);
                writer.Flush();
                var json = Encoding.UTF8.GetString(stream.ToArray());

                writer.Dispose();
                stream.Dispose();

                return json;
            }
            else
            {// 改行除外.
                var serializer = new DataContractJsonSerializer(graph.GetType());
                serializer.WriteObject(stream, graph);
                var val = stream.ToArray();

                stream.Dispose();

                return Encoding.UTF8.GetString(val);
            }
        }

        /// <summary>
        /// Deserialize(Object to JSON).
        /// </summary>
        /// <typeparam name="T">type.</typeparam>
        /// <param name="message">message.</param>
        /// <returns>Deserialized Type.</returns>
        public static T Deserialize<T>(string message)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(message));
            var serializer = new DataContractJsonSerializer(typeof(T) /*, setting*/);
            var val = (T)serializer.ReadObject(stream);
            stream.Dispose();
            return val;
        }
    }
}
