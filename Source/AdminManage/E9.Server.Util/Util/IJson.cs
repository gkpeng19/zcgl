using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
#if SILVERLIGHT
using System.ComponentModel.DataAnnotations;
using System.Collections.ObjectModel;
#else

#endif

namespace NM.Util
{
    public interface IJson
    {
        void BeforeSerialize();
        void AfterDeserialize();
        string ToJson();
    }

    public class TJson : IJson, INotifyPropertyChanged
    {
        #region Static
        private static Dictionary<Type, Dictionary<Type, List<PropertyInfo>>> _CacheProperty;
        public static Dictionary<Type, Dictionary<Type, List<PropertyInfo>>> CacheProperty
        {
            get
            {
                if (_CacheProperty == null)
                {
                    _CacheProperty = new Dictionary<Type, Dictionary<Type, List<PropertyInfo>>>();
                }
                return _CacheProperty;
            }
        }

        public static T Parse<T>(string json) where T : IJson
        {
            return (T)Parse(typeof(T), json);
        }

        public static object Parse(Type type, string json)
        {
            object result = TJson.Deserialize(type, json);
            IJson jObj = result as IJson;
            if (jObj != null)
                jObj.AfterDeserialize();
            return result;
        }

        public static T DeepColne<T>(IJson source) where T : IJson
        {
            string jsonString = source.ToJson();
            return Parse<T>(jsonString);
        }

        public static IEnumerable<PropertyInfo> LoadTypeProperty(Type type, Type propertyType)
        {
            if (!CacheProperty.ContainsKey(type))
                CacheProperty.Add(type, new Dictionary<Type, List<PropertyInfo>>());
            if (!CacheProperty[type].ContainsKey(propertyType))
                CacheProperty[type].Add(propertyType, new List<PropertyInfo>(GetPropertyByReturnType(type, propertyType)));
            return CacheProperty[type][propertyType];
        }

        static IEnumerable<PropertyInfo> GetPropertyByReturnType(Type type, Type propertyType)
        {
            if (propertyType.IsInterface)
            {
                return (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        where p.PropertyType.GetInterface(propertyType.Name, true) != null
                        select p);
            }
            else
            {
                return (from p in type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        where p.PropertyType.IsSubclassOf(propertyType)
                        select p);
            }
        }

        internal static string ToJson(IJson source, Type dataType)
        {
            var serilializer = new DataContractJsonSerializer(dataType);
            using (Stream stream = new MemoryStream())
            {
                serilializer.WriteObject(stream, source);
                stream.Flush();
                stream.Position = 0;
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }

        internal static T Deserialize<T>(string json)
        {          
            return (T)Deserialize(typeof(T),json);
        }

            internal static object Deserialize(Type type, string json)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                var serializer = new DataContractJsonSerializer(type);
                return  serializer.ReadObject(ms);
            }
        }

        #endregion

        public string ToJson()
        {
            BeforeSerialize();
            return TJson.ToJson(this, GetJasonDataType());
        }


        protected virtual Type GetJasonDataType()
        {
            return this.GetType();
        }

        public void BeforeSerialize()
        {
            BeforeSerialize(this);
            foreach (PropertyInfo p in LoadTypeProperty(this.GetType(), typeof(IJson)))
            {
                var v = p.GetValue(this, null) as IJson;
                if (v != null)
                    v.BeforeSerialize();
            }
        }

        public void AfterDeserialize()
        {
            AfterDeserialize(this);
            foreach (PropertyInfo p in LoadTypeProperty(this.GetType(), typeof(IJson)))
            {
                var v = p.GetValue(this, null) as IJson;
                if (v != null)
                    v.AfterDeserialize();
            }
        }

        protected virtual void BeforeSerialize(TJson obj)
        {

        }

        protected virtual void AfterDeserialize(TJson obj)
        {

        }

        public void FirePropertyChange(string propertyName)
        {
            OnPropertyChanged(propertyName);
        }

        #region PropertyChange
        public event PropertyChangedEventHandler PropertyChanged;

        bool _busy;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                _busy = true;
                try
                {
                    PropertyChanged(this, e);
                }
                finally
                {
                    _busy = false;
                }
            }
        }
        #endregion
    }

    public class TJsonList<T> :
#if SILVERLIGHT
 ObservableCollection<T>
#else
 List<T>
#endif
, IJson
        where T : IJson
    {
        public TJsonList()
        {
        }

        public TJsonList(IEnumerable<T> e)
            : this()
        {
            AddRange(e);
        }

#if SILVERLIGHT
        public void AddRange(IEnumerable<T> source)
        {
            foreach (var item in source)
                Add(item);
        }

        public void ForEach(Action<T> action)
        {
            foreach (var item in this)
                action(item);
        }

        public void Sort()
        {
            Sort(Comparer<T>.Default);
        }

        public void Sort(Comparison<T> comparison)
        {
            int i, j;
            T index;
            for (i = 1; i < this.Count; i++)
            {
                index = Items[i];
                j = i;
                while ((j > 0) && (comparison(Items[j - 1], index) == 1))
                {
                    Items[j] = Items[j - 1];
                    j = j - 1;
                }
                Items[j] = index;
            }
        }

        public void Sort(IComparer<T> comparer)
        {
            int i, j;
            T index;
            for (i = 1; i < this.Count; i++)
            {
                index = Items[i];
                j = i;
                while ((j > 0) && (comparer.Compare(Items[j - 1], index) == 1))
                {
                    Items[j] = Items[j - 1];
                    j = j - 1;
                }
                Items[j] = index;
            }
        }

        public List<T> ToList()
        {
            List<T> list = new List<T>();
            if (this.Count > 0)
            {
                this.ForEach(e => { list.Add(e); });
            }

            return list;
        }

#endif

        #region IIJson Members


        public void AfterDeserialize()
        {
            ForEach(e => e.AfterDeserialize());
        }

        public void BeforeSerialize()
        {
            ForEach(e => e.BeforeSerialize());
        }

        public string ToJson()
        {
            BeforeSerialize();
            return TJson.ToJson(this, this.GetType());
        }

        #endregion


    }
}
