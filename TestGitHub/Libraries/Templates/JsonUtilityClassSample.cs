using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TestGitHub.Libraries.Templates
{
    /// <summary>
    /// AppConfigSample(JSON File).
    /// https://takachan.hatenablog.com/entry/2017/01/18/120000.
    /// </summary>
    [DataContract]
    public class JsonUtilityClassSample
    {
        /// <inheritdoc/>
        [DataMember(Name = "id")]
        public int ID { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        // 配列
        [DataMember(Name = "numbers")]
        public int[] Numbers { get; set; }

        /// <inheritdoc/>
        // リスト型
        [DataMember(Name = "list")]
        public List<int> NumberList { get; private set; } = new List<int>();

        /// <inheritdoc/>
        // ハッシュマップ型
        [DataMember(Name = "map")]
        public IDictionary<string, string> Attributes { get; private set; } = new Dictionary<string, string>();

        /*
            // JSON デシリアライズ.
            string path = Environment.CurrentDirectory + "\\test.json";
            string body = File.ReadAllText(path);

            var deserializedList = JsonUtility.Deserialize<IList<JsonUtilityClassSample>>(body);

            // 内容の出力
            foreach (var p in deserializedList) {
                Debug.WriteLine("ID = " + p.ID);
                Debug.WriteLine("Name = " + p.Name);

                Debug.WriteLine("Numbers:");
                foreach (var a in p.Numbers) {
                    Debug.WriteLine(a);
                }

                Debug.WriteLine("NumberList:");
                foreach (var a in p.NumberList) {
                    Debug.WriteLine(a);
                }

                foreach (KeyValuePair<string, string> att in p.Attributes) {
                    Debug.WriteLine(att.Key + " = " + att.Value);
                }
            }

            var onDict = JsonUtility.Serialize(deserializedList, true);
            var offDict = JsonUtility.Serialize(deserializedList, false);

            File.WriteAllText(Environment.CurrentDirectory + "\\testON.json", onDict);
            File.WriteAllText(Environment.CurrentDirectory + "\\testOFF.json", offDict);

         */
    }
}
