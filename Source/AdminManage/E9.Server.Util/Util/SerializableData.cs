using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using NM.OP;

namespace NM.Util
{
    [Flags]
    public enum EntityStatus :int 
    {
        Original=0,
        New=1,
        Modified=2,
        Delete=4
    }

    public enum EntityOperater
    {
        Create,
        Modified,
        Delete
    }

    public class EntityManagerArg : EventArgs
    {
        public EntityManagerArg(SerializableData data, EntityOperater entityOperater)
        {
            Data = data;
            EntityOperater = entityOperater;
        }
        public SerializableData Data { get; set; }
        public EntityOperater EntityOperater { get; set; }
    }

    public class DataItem// : TJson
    {
        public static readonly string TRUESTRING = "1";
        public static readonly string FALSESTRING = "0";
        public static readonly string DATETIMEFORMAT = "yyyy-MM-dd HH:mm:ss";
        public static readonly DateTime MinDateTime = DateTime.MinValue;//DateTime.Parse("2000-01-01 00:00:00");
        public DataItem() { }
        public DataItem(string key, object value,TypeCode  tc)
        {
            K = key;
            S = EntityStatus.New;
            T = tc;
            SetValue(value);
        }
        public DataItem(string key, object value)
        {
            K = key;
            S = EntityStatus.New;

            SetValue(value);
        }

        public TypeCode T { get; set; }
        public string O { get; set; }
        public string K { get; set; }
        [IgnoreDataMember]
        public EntityStatus S { get; set; }

        public EntityStatus _S { get; set; }
        string _V;
        public string V
        {
            get { return _V; }
            set
            {
                if (_V != value)
                {
                    O = _V;
                    _V = value;

#if SILVERLIGHT
                    if (!K.EndsWith("_G", StringComparison.InvariantCultureIgnoreCase)
                        && K.Trim().ToLower() != "checked"
                        && K.Trim().ToLower() != "id")
                    {
                        S = EntityStatus.Modified;   // 只要是和上次数值不同就认为是修改了。
                    }
#endif
                }
            }
        }

        internal bool IsSameValue(object value, ref string newValue)
        {
            var TT = Type.GetTypeCode(value.GetType());
            if (TT == TypeCode.Boolean)
            {
                newValue = (value as Boolean?).Value ? TRUESTRING : FALSESTRING;
            }
            else if (TT == TypeCode.DateTime)
            {
                newValue = (value as DateTime?).Value.ToString(DATETIMEFORMAT);
            }
            else
            {
                newValue = value.ToString();
            }

            if (T == TypeCode.Empty)
            {
                T = TT;
            }
            return newValue == V;
        }

        internal void SetNewValue(string newValue)
        {
            if (S != EntityStatus.New)
                S = EntityStatus.Modified;
            V = newValue;
        }

        public bool SetValue(object value)  // 可以保存value实际类型（TyprCode）
        {
            string newValue=string.Empty;
            if (IsSameValue(value, ref newValue))
            {
                return false;
            }
            else
            {
                SetNewValue(newValue);
                return true;
            }
        } 

        public object GetValue()  // 返回实际类型的值
        {
            if (T == TypeCode.Boolean)
            {
                return V == TRUESTRING;
            }
            else
            {
                return string.IsNullOrEmpty(V) ? DefaultValue(T) : Convert.ChangeType(V, T, null);
            }
        }

        public DataItem DeepColne()
        {
            var result = new DataItem(K, V) {S = S, T = T};
            return result;
        }

        public void SaveStatus()
        {
            _S = S;
        }

        public void LoadStatus()
        {
            S = _S;
        }

        public static object DefaultValue(TypeCode typeCode)
        {
            object result = null;

            switch (typeCode)
            {
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Single:
                case TypeCode.Char:
                    result = 0;
                    break;
                case TypeCode.Decimal:
                case TypeCode.Double:
             
                    result = 0.0;
                    break;
                case TypeCode.String:
                    result = string.Empty;
                    break;
                case TypeCode.Boolean:
                    result = false;
                    break;
                case TypeCode.DateTime:
                    result = DateTime.Now;
                    break;
                case TypeCode.Empty:
                case TypeCode.DBNull:
                default:
                    result = null;
                    break;

            }
            return result;
        }
    }



    public class SerializableData : TJson
    {
        public static PropertyInfo GetObjPro(string ProName, Type EntityType)
        {
            PropertyInfo _Property = EntityType.GetProperty(ProName);
            if (_Property == null)
            {
                PropertyInfo[] _pros = EntityType.GetProperties();
                foreach (PropertyInfo p in _pros)
                {
                    if (p.Name.ToUpper().CompareTo(ProName.ToUpper()) == 0)
                    {
                        _Property = p;
                        break;
                    }
                }
            }
            return _Property;
        }
        public static bool IsDataLoading = false;
        public SerializableData()
        {
            Items = new List<DataItem>();
            Dels = new List<DataItem>();
            S = EntityStatus.New;
        }

        public List<DataItem> Items ;//{ get; set; }
        public List<DataItem> Dels ;//{ get; set; }
        public event Action<EntityStatus> OnStatusChanged;
        EntityStatus _ES;
        [IgnoreDataMember]
        public EntityStatus S
        {
            get { return _ES; }
            set
            {
                if (_ES != value)
                {
                    _ES = value;
                    OnPropertyChanged("S");
                    if (OnStatusChanged != null)
                        OnStatusChanged(_ES);
                    if (_ES == EntityStatus.Modified)
                        SaveState();
                    else if (_ES == EntityStatus.Original)
                        Confirm();
                }
            }
        }
         
        void MakeModify()
        {
            if (!IsDataLoading)
            {
                if (S != EntityStatus.New && S != EntityStatus.Modified
                     && S != EntityStatus.Delete)
                    S = EntityStatus.Modified;
            }
        }

        public object this[string key]
        {
            get
            {
                var item = GetItem(key);
                return item == null ? null : item.GetValue();
            }
            set
            {
                SetValue(key, value, TypeCode.Empty);
                OnPropertyChanged(key);
            }
        }

        public virtual bool IsEmpty()
        {
            var item = Items.FirstOrDefault(e => !string.IsNullOrEmpty(e.V));  // not empty
            return item == null ? true : false;
        }     

        public void ResetValue() // todo
        {
            Items.ForEach(e =>
            {
                e.SetValue(DataItem.DefaultValue(e.T));
                OnPropertyChanged(e.K);
            });

            Items.Clear();
        }

        DataItem GetItem(string key)
        {
            return Items.FirstOrDefault(c => c.K.ToUpper () == key.ToUpper ());
        }

        DataItem GetDelItem(string key)
        {
            return Dels.FirstOrDefault(c => c.K.ToUpper () == key.ToUpper ());
        }

        public T GetValue<T>(string key)
        {
            var value = this[key];

            if (value != null)
            {
                return (T)value;
            }
            else
            {
                return Type.GetTypeCode(typeof(T))== TypeCode.DateTime? (T)(Object)DataItem.MinDateTime:  default(T);// (T)DataItem.DefaultValue(Type.GetTypeCode(typeof(T)));
            }
        }

        protected virtual void SetValue(string key, object value,TypeCode  tc)
        {
            //this[key] = value;
            var item = GetItem(key);
            if (item == null)
            {
                if (value != null)
                {
                    MakeModify();
                    item = new DataItem(key, value,tc);
                    Items.Add(item);
                }
            }
            else
            {
                if (value != null)
                {
                    string newvalue = string.Empty;
                    if (!item.IsSameValue(value, ref newvalue))
                    {
                        MakeModify();
                        item.SetNewValue(newvalue);
                    }
                }
                else
                {
                    MakeModify();
                    Items.Remove(item);
                    Dels.Add(item);
                }
            }
        }

        public string GetString(string key)
        {
            return GetValue<string>(key);
        }

        public void SetString(string key, string value)
        {
           // this[key] = value;
            SetValue(key, value, TypeCode.String);
            OnPropertyChanged(key);
        }

        public int GetInt32(string key)
        {
            return GetValue<int>(key);
        }

        public void SetInt32(string key, int value)
        {
            this[key] = value;
        }

        public double GetDouble(string key)
        {
            return GetValue<double>(key);
        }

        public void SetDouble(string key, Double value)
        {
            this[key] = value;
        }

        public decimal GetDecimal(string key)
        {
            return GetValue<decimal>(key);
        }

        public void SetDecimal(string key, Decimal value)
        {
            this[key] = value;
        }

        public long GetLong(string key)
        {
            return GetValue<long>(key);
        }

        public void SetLong(string key, long value)
        {
            this[key] = value;
        }

        public Int64 GetInt64(string key)
        {
            return GetValue<Int64>(key);
        }

        public void SetInt64(string key, Int64 value)
        {
            this[key] = value;
        }

        public DateTime GetDateTime(string key)
        {
            return GetValue<DateTime>(key);
        }

        public void SetDateTime(string key, DateTime value)
        {
             // this[key] = value;
            SetValue(key, value, TypeCode.DateTime);
            OnPropertyChanged(key);
        }

        public bool GetBoolean(string key)
        {
            return GetValue<bool>(key);
        }

        public void SetBoolean(string key, bool value)
        {
            this[key] = value;
        }

        public void Delete()
        {
            this.S = EntityStatus.Delete;
            OnPropertyChanged("");
        }

        public SerializableData DeepClone()
        {
            var result = new SerializableData();
            foreach (var item in Items)
            {
                result.Items.Add(item.DeepColne());
            }
            result._ES = this._ES;
            return result;
        }


        public void CopyFrom(SerializableData source)
        {
            SerializableData s=source.DeepClone();
            Items.Clear();
            Items.AddRange(s.Items);
            foreach (var item in Items)
            {
                OnPropertyChanged(item.K);
            }
        }

#if SILVERLIGHT
        protected void Validate(string propertyName)
        {
          //  var proinfo = this.GetType().GetProperty(propertyName);
           var proinfo = this.GetType().GetProperty(propertyName);
            if (proinfo != null)
            {
                object value = proinfo.GetValue(this, null);
                var context = new ValidationContext(this, null, null) {MemberName = propertyName};
                Validator.ValidateProperty(value, context);
            }
        }
#else
        protected void Validate(string propertyName)
        {

        }
#endif 
 

#if SILVERLIGHT
        public T Create<T>() where T : SerializableData
        {
            return TJson.DeepColne<T>(this as T);
        }

        public static Action<EntityManagerArg> EntityMananged;

        public static void EntityManager(SerializableData data, EntityOperater entiotyOperator)
        {
            if (null != EntityMananged)
                EntityMananged(new EntityManagerArg(data, entiotyOperator));
        }

        public static void SubForms_CollectionChanged<T>(object sender, NotifyCollectionChangedEventArgs e, PropertyChangedEventHandler handler) where T : SerializableData
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    e.NewItems.ForEach<T>(c => { c.PropertyChanged += handler; c.S = EntityStatus.New; EntityManager(c, EntityOperater.Create); });

                    break;
                case NotifyCollectionChangedAction.Remove:
                    e.OldItems.ForEach<T>(c => { c.PropertyChanged -= handler; c.S = EntityStatus.Delete; EntityManager(c, EntityOperater.Delete); });
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Reset:
                    e.NewItems.ForEach<T>(c => { c.PropertyChanged += handler; c.S = EntityStatus.New; EntityManager(c, EntityOperater.Create); });
                    e.OldItems.ForEach<T>(c => { c.PropertyChanged -= handler; c.S = EntityStatus.Delete; EntityManager(c, EntityOperater.Delete); });
                    break;
                default:
                    break;
            }
        }

#endif
        #region IUndoable

        Stack<SerializableData> _Baskup = new Stack<SerializableData>();
        public void SaveState()
        {
            _Baskup.Push(this.DeepClone());
        }

        public void Undo()
        {
            if (_Baskup.Count <= 0)
            {
                return;
            }

            SerializableData bak = _Baskup.Pop();
            if (bak != null)
            {
                Restore(bak);
            }
        }

        public void Confirm()
        {
            _Baskup.Clear();
        }

        void Restore(SerializableData data)
        {
            var backData = new Dictionary<string, string>();
            data.Items.ForEach(e => backData.Add(e.K, e.V));

            for (int index = Items.Count - 1; index >= 0; index--)
            {
                var item = Items[index];
                if (backData.ContainsKey(item.K))
                {
                    if (item.V != backData[item.K])
                    {
                        item.V = backData[item.K];
                        OnPropertyChanged(item.K);
                    }
                }
                else
                {
                    Items.RemoveAt(index);
                    OnPropertyChanged(item.K);
                }
            }

            S = data.S;
        }
        #endregion IUndo


        public static EntityStatus MergerStatus(EntityStatus s1, EntityStatus s2)
        {
            return s1 | s2;
        }
    }
}