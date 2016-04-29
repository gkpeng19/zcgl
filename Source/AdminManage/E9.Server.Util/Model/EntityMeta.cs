using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using NM.Util;
using NM.Service;
#if SILVERLIGHT
#else
using NM.Config;
using System.Web;
#endif
using System.IO;
using System.Text;
using NM.OP;
using NM.Log;

namespace NM.Model
{
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class EntityTableAttribute : Attribute
    {
        public EntityTableAttribute()
        {
        }
        public EntityTableAttribute(string tableName)
        {
            TableName = tableName;
            IdentityFieldName = "ID";
        }

        public string TableName { get; set; }
        public string IdentityFieldName { get; set; }
        public string Keys { get; set; }
        public string NotNulls { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EntityFieldAttribute : Attribute
    {
        public EntityFieldAttribute()
        {
        }
        public EntityFieldAttribute(string fieldName)
        {
            FieldName = fieldName;
        }

        public EntityFieldAttribute(string fieldName, string seedName)
        {
            FieldName = fieldName;
            AutoNoSeedName = seedName;
        }
        public string FieldName { get; set; }
        public string AutoNoSeedName { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class ForeignKeyAttribute : Attribute
    {
        public ForeignKeyAttribute(string foreignKey, string masterKey)
        {
            ForeignKey = foreignKey;
            MasterKey = masterKey;
        }
        public string ForeignKey { get; set; }
        public string MasterKey { get; set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IdentityKeyAttribute : Attribute
    {

    }

    public enum ForgeignType
    {
        Reference,
        Children
    }

    public class FieldMeta
    {
        public FieldMeta(string fieldName, TypeCode type)
        {
            FieldName = fieldName;
            DataType = type;
        }

        public string FieldName { get; set; }
        public TypeCode DataType { get; set; }
        public bool Nullable { get; set; }
        public string AutoNoSeed { get; set; }
    }

    public class ForeignPropertyMeta
    {
        public ForeignPropertyMeta(string propertyName)
        {
            PropertyName = propertyName;
            ForgeignType = ForgeignType.Reference;
        }

        public string PropertyName { get; set; }
        public PropertyInfo Property { get; set; }
        public ForgeignType ForgeignType { get; set; }
        public string MasterKey { get; set; }
        public Type DataType { get; set; }
        public string ForeignKey { get; set; }
    }

    public class EntityOPMeta
    {
        public Type OPType { get; set; }

#if SILVERLIGHT
#else
        IEntityProviderOP _CacheObject;
        public IEntityProviderOP GetOP(LoginInfo user, DataProvider datasource)
        {
            if (_CacheObject != null)
            {
                //(_CacheObject as CertifiedProviderOP).Account = user;
                //(_CacheObject as CertifiedProviderOP).DataProvider = datasource;
                //return _CacheObject; ;
                CertifiedProviderOP _cp = _CacheObject as CertifiedProviderOP;
                if (_cp != null) //如果是认证的服务，则检查是否是同一个登录用户;如果缓存的和当前登录的不一致，则不使用缓存的对象
                {
                    if ((_cp != null) && (_cp.Account != null) && (_cp.Account.User != null) && (user != null) && (user.User != null))
                    {
                        //if ((_cp.Account.User.UserID == user.User.UserID) && (_cp.Account.User.OrgId == user.User.OrgId))只要是同一个组织，就可以用缓存
                        if (_cp.Account.User.OrgId == user.User.OrgId)
                        {
                            return _CacheObject; ;
                        }
                    }
                }
                else
                {
                    return _CacheObject;
                }
            }
            IEntityProviderOP op = null;

            try
            {
                op = (IEntityProviderOP)Activator.CreateInstance(OPType, user, datasource);
            }
            catch (Exception ex)
            {
                LogManager.LogException(ex);
            }

            _CacheObject = op;
            return op;
        }
#endif

    }

    public class TableMeta
    {
        public TableMeta()
        {
            Keys = new List<string>();
            NotNulls = new List<string>();
            Fields = new List<FieldMeta>();
            ReferProperties = new List<ForeignPropertyMeta>();
            IdentityFieldName = "ID";
//            #if SILVERLIGHT
//#else 
//            CustomOPs = new Dictionary<string, Type>();
//#endif
        }

        public string TableName { get; set; }
        public string IdentityFieldName { get; set; }
        public List<string> Keys { get; set; }
        public List<string> NotNulls { get; set; }
        public List<FieldMeta> Fields { get; set; } 
        public Type EntityType { get; set; }
        public List<ForeignPropertyMeta> ReferProperties { get; set; }
         

        public void SetKeys(string keys)
        {
            if (!string.IsNullOrEmpty(keys))
            {
                Keys.AddRange(keys.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries));
            }
        }

        public ForeignPropertyMeta GetForeignProperty(PropertyInfo p)
        {
            return ReferProperties.FirstOrDefault(e => { return e.Property == p; });
        }

        public FieldMeta this[string fieldName]
        {
            get
            {
                return Fields.FirstOrDefault(e => { return e.FieldName == fieldName; });
            }
        }

        public IEnumerable<FieldMeta> GetAuoNoField()
        {
            return Fields.Where(e => { return !string.IsNullOrEmpty(e.AutoNoSeed); });
        }
#if SILVERLIGHT
#else 
        //public Dictionary<string, Type> CustomOPs { get; set; }
        //public void AddCustomOP(CustomEntityOPAttribute toAdd)
        //{
        //    CustomOPs.Add(toAdd.FriendName,toAdd.EntityType);
        //}
        Type _CustomOPType;

        public Type CustomOPType
        { 
            get
            {
                if(_CustomOPType==null)
                    _CustomOPType = typeof(EntityProviderOP<>).MakeGenericType(new Type[] { EntityType });
                return _CustomOPType;
            }
            set
            {
                _CustomOPType = value;
                _CacheObject = null;
            }
        }

        IEntityProviderOP _CacheObject;
        public IEntityProviderOP GetOP(LoginInfo user, DataProvider datasource)
        {
            /* liang  2013-4-10 临时去掉使用缓存的 服务的功能，为调试报
            if (_CacheObject != null)
            {
                //(_CacheObject as CertifiedProviderOP).Account = user;
                //(_CacheObject as CertifiedProviderOP).DataProvider = datasource;

                CertifiedProviderOP _cp = _CacheObject as CertifiedProviderOP;
                if (_cp != null) //如果是认证的服务，则检查是否是同一个登录用户;如果缓存的和当前登录的不一致，则不使用缓存的对象
                {
                    if ((_cp != null) && (_cp.Account != null) && (_cp.Account.User != null) && (user != null) && (user.User != null))
                    {
                        //if ((_cp.Account.User.UserID == user.User.UserID) && (_cp.Account.User.OrgId == user.User.OrgId))只要是同一个组织，就可以用缓存
                       if (_cp.Account.User.OrgId == user.User.OrgId)
                        {
                            return _CacheObject; ;
                        }
                    }
                }
                else
                {
                    return _CacheObject;
                }
            }
            */
            IEntityProviderOP op = null;
            var opType = CustomOPType; 

            try
            {
                op = (IEntityProviderOP)Activator.CreateInstance(opType, user, datasource);
            }
            catch(Exception ex)
            {
                LogManager.LogException(ex);
            }

            _CacheObject = op;
            return op;
        }
#endif
    }

    public class EntityMetaManager
    {
        static EntityMetaManager _Default;

        public static EntityMetaManager Default
        {
            get
            {
                if (_Default == null)
                {
                    _Default = new EntityMetaManager();

#if SILVERLIGHT
#else
                    _Default.Load();
#endif
                }
                return _Default;
            }
        }

        public EntityMetaManager()
        {
            EntityMetas = new Dictionary<Type, TableMeta>();
        }

        Dictionary<Type, TableMeta> EntityMetas;

        public TableMeta GetMetaByTypeName(string type)
        {
            Type entityType = Type.GetType(type);
            if (entityType != null)
                return this[entityType];
            foreach (var item in EntityMetas)
            {
                if (item.Key.FullName == type)
                    return item.Value;
            }
            throw new ApplicationException(string.Format("非有效类型{0}", type));
        }

        public TableMeta this[Type entityType]
        {
            get
            {
                if (!entityType.IsSubclassOf(typeof(EntityBase)))
                    throw new ApplicationException(string.Format("非有效类型{0},实体类型必须继承自EntityBase ", entityType.Name));
                if (!EntityMetas.ContainsKey(entityType))
                    EntityMetas.Add(entityType, CollectionEntityMeta(entityType));
                return EntityMetas[entityType];
            }
        }

#if SILVERLIGHT
#else

        void Load()
        {
            foreach (ServiceProvider item in ServiceProviderConfigSection.GetEntityProvider())
            {
                if (item.Disabled) break;
                if (!string.IsNullOrEmpty(item.AssemblyName))
                {
                    string rootPath = string.Empty;
                    try
                    {
                        //if web application
                        rootPath = HttpRuntime.BinDirectory;
                    }
                    catch
                    { //if remoting application
                        rootPath = string.Empty;
                    }
                    try
                    {
                        string fileName = rootPath + item.AssemblyName + ".dll";
                        if (File.Exists(fileName))
                        {
                            Assembly assembly = Assembly.LoadFrom(fileName);
                            CollectionEntityMeta(assembly);
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }

        void CollectionEntityMeta(Assembly assembly)
        {
            var entityBaseType = typeof(EntityBase);
            foreach (Type item in assembly.GetTypes())
            {
                if (item.IsSubclassOf(entityBaseType))
                {
                  EntityMetas.Add(item,CollectionEntityMeta(item));
                }
            }
        }

#endif

        TableMeta CollectionEntityMeta(Type entityType)
        {
            var meta = new TableMeta() { TableName = entityType.Name, EntityType = entityType };

            var tableAttrs = entityType.GetCustomAttributes(typeof(EntityTableAttribute), false);
            if (tableAttrs.Length > 0)
            {
                var tableAttr = tableAttrs[0] as EntityTableAttribute;
                meta.TableName = tableAttr.TableName;
                meta.SetKeys(tableAttr.Keys);
                meta.IdentityFieldName = tableAttr.IdentityFieldName;
            }

            foreach (var proInfo in entityType.GetProperties())
            {  
                if (proInfo.PropertyType.IsPrimitive 
                    || Type.GetTypeCode(proInfo.PropertyType)== TypeCode.String
                    || Type.GetTypeCode(proInfo.PropertyType) == TypeCode.DateTime)
                {
                    FieldMeta fm = new FieldMeta(proInfo.Name, Type.GetTypeCode(proInfo.PropertyType));
                    meta.Fields.Add(fm);
                    var propertyAttr = proInfo.GetCustomAttributes(typeof(EntityFieldAttribute), false);
                    if (propertyAttr.Length > 0)
                    {
                        var fAttr = propertyAttr[0] as EntityFieldAttribute;
                        fm.AutoNoSeed = fAttr.AutoNoSeedName;
                        continue;
                    }

                    propertyAttr = proInfo.GetCustomAttributes(typeof(IdentityKeyAttribute), false);
                    if (propertyAttr.Length > 0)
                    {
                        meta.IdentityFieldName = proInfo.Name;
                    }
                }
                else
                {
                    var propertyAttr = proInfo.GetCustomAttributes(typeof(ForeignKeyAttribute), false);
                    if (propertyAttr.Length > 0)
                    {
                        var fkAttr = propertyAttr[0] as ForeignKeyAttribute;
                        var pMeta = new ForeignPropertyMeta(proInfo.Name) { Property = proInfo, ForeignKey = fkAttr.ForeignKey, MasterKey = fkAttr.MasterKey };
                        meta.ReferProperties.Add(pMeta);
                        if (proInfo.PropertyType.IsSubclassOf(typeof(EntityBase)))
                            pMeta.DataType = proInfo.PropertyType;
                        else if (proInfo.PropertyType.IsSubclassOf(typeof(EntityList<>)))
                        {
                            foreach (var t in proInfo.PropertyType.GetGenericArguments())
                            {
                                if (t.IsSubclassOf(typeof(EntityBase)))
                                {
                                    pMeta.DataType = t;
                                    pMeta.ForgeignType = ForgeignType.Children;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // throw new ApplicationException("非有效子对象数据类型");
                        }
                    }
                }
            }
            return meta;
        }

        public override string ToString()
        { 
            StringBuilder sb = new StringBuilder();
            foreach (var item in EntityMetas)
            {
                sb.AppendLine(string.Format("{0}",item.Key.FullName));
            }
            return sb.ToString();
        }
    }
}
