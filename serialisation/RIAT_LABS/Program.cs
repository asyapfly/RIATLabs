using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RIAT_LABS
{
    public class Input
    {
        public int K { get; set; }
        public decimal[] Sums { get; set; }
        public int[] Muls { get; set; }
    }
    public class Output
    {
        public decimal SumResult { get; set; }
        public int MulResult { get; set; }
        public decimal[] SortedInputs { get; set; }
        public Output()
        {
            this.SumResult = 0;
            this.MulResult = 1;
        }
    }
    public interface ISerializer
    {
        string Serialize<T>(T obj);
        T Deserialize<T>(string data);
    }
    public class XMLSerializer : ISerializer
    {
        public string Serialize<T>(T obj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            XmlWriterSettings xmlsettings = new XmlWriterSettings();
            xmlsettings.OmitXmlDeclaration = true;
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlWriter writer = XmlWriter.Create(stream, xmlsettings);
            string res;
            serializer.Serialize(writer, obj, ns);
            using (StreamReader sr = new StreamReader(stream))
            {
                stream.Position = 0;
                res = sr.ReadToEnd();
                sr.Close();
                sr.Dispose();
            }
            return res;
        }
        public T Deserialize<T>(string data)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            MemoryStream stream = new MemoryStream();
            StreamWriter sw = new StreamWriter(stream);
            stream.Position = 0;
            sw.WriteLine(data);
            sw.Flush();
            stream.Position = 0;
            XmlReader reader = XmlReader.Create(stream);
            T res = (T)serializer.Deserialize(reader);
            return res;
        }
    }
    public class JSONSerializer : ISerializer
    {
        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public T Deserialize<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }
    }
    class Program
    {
        public static Output MakeOutputFromInput(Input someInput)
        {
            Output Res = new Output();
            Res.SumResult = someInput.Sums.Sum() * someInput.K;
            Res.MulResult = someInput.Muls.Aggregate((acc, i) => acc * i);
            Res.SortedInputs = someInput.Sums.Concat(someInput.Muls.Select(i => (decimal)i)).ToArray();
            Array.Sort(Res.SortedInputs);
            return Res;
        }
        static void Main(string[] args)
        {
            string serializeType = Console.ReadLine();

            string someText = Console.ReadLine();
            if (!String.IsNullOrEmpty(someText))
            {
                Input li = new Input();
                Output output = new Output();
                XMLSerializer xml = new XMLSerializer();
                JSONSerializer json = new JSONSerializer();
                if (serializeType == "Json")
                {
                    li = json.Deserialize<Input>(someText);
                    output = MakeOutputFromInput(li);
                    Console.WriteLine(json.Serialize<Output>(output));
                }
                else
                {
                    li = xml.Deserialize<Input>(someText);
                    output = MakeOutputFromInput(li);
                    Console.WriteLine(xml.Serialize<Output>(output));
                }
            }
        }
    }
}
