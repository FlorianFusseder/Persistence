using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

/// <summary>
/// To load with another Programm you have to:
/// 
/// 1. Set the Namespace to "" (-> [DataContract(Namespace = "")] instead of [DataContract])
/// 2. Projects have to have the same Namespace!
/// </summary>

namespace Persistenz
{
    public class Serializer
    {
        private Resolver resolver;
        private Assembly ass;

        public Serializer()
        {
            ass = Assembly.Load(Assembly.GetCallingAssembly().GetName().Name);
            resolver = new Resolver(ass);
        }

        /// <summary>
        /// Lädt ein Objekt aus einer XML Datei und gibt diesen zurück. (Null wenn keine Datei vorhanden)
        /// Loads a Object from a XML File and returns it. Null if no File exists.
        /// </summary>
        /// <typeparam name="T">Type of the Instance to load</typeparam>
        /// <param name="path">Path to XML File</param>
        /// <returns>Type of Object</returns>
        public T Load<T>(string path)
        {
            if (!File.Exists(path))
                return default(T);

            XmlReader xmlReader = null;

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    using (xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings
                    {
                        CloseInput = true
                    }))
                    {
                        var dataContractSerializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, true, null, resolver);
                        return (T)dataContractSerializer.ReadObject(xmlReader);
                    }
                }
            }
            catch (Exception e)
            {

                throw new Exception("Persistence failure! Could not load " + e.Message);
            }
        }

        /// <summary>
        /// Saves a XML Object Graph to the given Adress
        /// </summary>
        /// <typeparam name="T">Type of the Object</typeparam>
        /// <param name="path">Path to XML file</param>
        /// <param name="Object">Instance to save</param>
        public void Save<T>(string path, T Object)
        {

            if (File.Exists(path))
                File.Delete(path);

            File.Create(path).Close();

            XmlWriter xmlWriter = null;

            try
            {
                using (var fileStream = new FileStream(path, FileMode.Open))
                {
                    using (xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings
                    {
                        Indent = true,
                        IndentChars = "    ",
                        CloseOutput = true
                    }))
                    {

                        var dataContractSerializer = new DataContractSerializer(typeof(T), null, int.MaxValue, false, true, null, resolver);
                        dataContractSerializer.WriteObject(xmlWriter, Object);
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Persistence failure! Could not save " + e.Message);
            }
        }
    }
}
