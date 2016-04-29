using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.IO.IsolatedStorage;

namespace NM.Util
{
    public static class JsonHelper
    {
        public static string ToJson(this IJson source)
        {
            return ToJson(source, source.GetType());
        }

        public static string ToJson(this IJson source, Type type)
        {
            source.BeforeSerialize();
            DataContractJsonSerializer serilializer = new DataContractJsonSerializer(type);
            using (Stream stream = new MemoryStream())
            {
                serilializer.WriteObject(stream, source);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
              //  return HttpUtility.UrlEncode(reader.ReadToEnd());                
            }
        }

        public static string ToJson(object obj)
        {
            DataContractJsonSerializer serilializer = new DataContractJsonSerializer(obj.GetType());
            using (Stream stream = new MemoryStream())
            {
                serilializer.WriteObject(stream, obj);
                stream.Flush();
                stream.Position = 0;
                StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
               // return HttpUtility.UrlEncode(reader.ReadToEnd());                
            }
        }


        public static T ParseJSON<T>(this string str)
        {
            ///----------------
            T result = (T)ParseJSON(typeof(T), str);
            if (result != null && result is IJson)
            {
                (result as IJson).AfterDeserialize();
            }
            return result;
        }

        public static object ParseJSON(Type type, string str)
        {
            if (string.IsNullOrEmpty(str)) return null;
            object result = null;
            // string strTemp = HttpUtility.UrlDecode(str);
            using (MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(str)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(type);
                result = serializer.ReadObject(ms);
                if (result != null && result is IJson)
                    (result as IJson).AfterDeserialize();
            }
            return result;
        }

        public static T DeepClone<T>(this TJson Source) where T : TJson
        {
            string jsonString = Source.ToJson();
            return jsonString.ParseJSON<T>();
        }

        public static bool IsSame(this IJson source, IJson dest)
        {
            if (source == dest) return true;
            Type sourceType = source.GetType();
            Type destType = dest.GetType();
            if (sourceType != destType) return false;

            foreach (PropertyInfo property in sourceType.GetProperties())
            {
                IComparable sourceValue = property.GetValue(source, null) as IComparable;
                IComparable destValue = property.GetValue(dest, null) as IComparable;

                if (sourceType == null && destValue == null)
                    continue;
                else if ((sourceType == null && destValue != null) ||
                     (sourceType != null && destValue == null))
                    return false;
                else
                {
                    if (sourceValue.CompareTo(destValue) != 0)
                        return false;
                }
            }
            return true;
        }

        public static T Deserialize<T>(Stream stream)
        {
            DataContractJsonSerializer serilializer = new DataContractJsonSerializer(typeof(T));
            return (T)serilializer.ReadObject(stream);
        }

        public static T Deserialize<T>(string json)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(json);
                writer.Flush();
                stream.Position = 0;
                T result = Deserialize<T>(stream);
                return result;
            }
        } 
    }
}