using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Checkpoint03.Classes
{
    public static class Serializer
    {
        #region Сериализация и десериализация по одному экземпляру

        public static void SaveToBinnary<T>(String fileName, T serializableObject)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            using (FileStream fs = File.Create(fileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, serializableObject);
            }
        }

        public static T LoadFromBinnary<T>(String fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (T)formatter.Deserialize(fs);
            }
        }

        //====================================================================

        public static void SaveToXml<T>(String fileName, T serializableObject)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            if(!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            using (TextWriter textWriter = new StreamWriter(fileName))
            {
                serializer.Serialize(textWriter, serializableObject);
            }
        }

        public static T LoadFromXml<T>(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader textReader = new StreamReader(fileName))
            {
                return (T)serializer.Deserialize(textReader);
            }
        }
        #endregion

        #region Сериализация и десериализация списка объектов

        public static void SaveListToBinnary<T>(String fileName, List<T> serializableObjects)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            using (FileStream fs = File.Create(fileName))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(fs, serializableObjects);
            }
        }

        public static List<T> LoadListFromBinnary<T>(String fileName)
        {
            using (FileStream fs = File.Open(fileName, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (List<T>)formatter.Deserialize(fs);
            }
        }

        //====================================================================

        public static void SaveListToXml<T>(String fileName, List<T> serializableObjects)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));
            }
            using (TextWriter textWriter = new StreamWriter(fileName))
            {
                serializer.Serialize(textWriter, serializableObjects);
            }
        }

        public static List<T> LoadListFromXml<T>(String fileName)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextReader textReader = new StreamReader(fileName))
            {
                return (List<T>)serializer.Deserialize(textReader);
            }
        }
        #endregion
    }
}
