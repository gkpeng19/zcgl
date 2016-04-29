using System;
using NM.Util;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NM.OP;
using System.Collections;
using System.Reflection;

#if SILVERLIGHT
using NM.Service;
#endif 

namespace NM.Model
{
    public interface IEntity
    {
        //[IgnoreDataMember]
        //EntityStatus S { get; set; }

        void ResetValue(); // todo
        void SaveStatus();
        void LoadStatus();
        void ResetStatus(EntityStatus s);
        void RemoveDataItem(EntityStatus s);
        void Lighten();//
        void SetValue(string key, object value);
        void LoadIDandStatus(IEntity e);

#if  SILVERLIGHT
#else
        void Save(EntityBase parent, LoginInfo user, DataProvider datasource);
        void Save(EntityBase parent, LoginInfo user, DataProvider datasource,bool isInTrans);
#endif

        EntityStatus GetChildrenStatus();
    }

    public class EntityBase : SerializableData, IEntity// where T : EntityBase//<T>, new()
    {
        public EntityBase()
        {
            GID = Guid.NewGuid();
        }


        private bool _Checked;
        [IgnoreDataMember]
        public bool Checked
        {
            get { return _Checked; }
            set
            {
                if (_Checked != value)
                {
                    _Checked = value;
                    OnPropertyChanged("Checked");
                }
            }
        }

        public Guid GID;// { get; set; }


        [IgnoreDataMember]
        public Int32 ID //{ get; set;}
        {
            get { return GetInt32("ID"); }
            set
            {
                SetInt32("ID", value);
            }
        }

        #region Status

        public EntityStatus _S ;//{ get; set; }
        public EntityStatus GetChildrenStatus()
        {
            var result = S;
            foreach (var p in TJson.LoadTypeProperty(this.GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                    result = MergerStatus(result, ((IEntity)o).GetChildrenStatus());
            }
            return result;
        }

        public void SaveStatus()
        {
            _S = S;
            Items.ForEach(e => e.SaveStatus());
            foreach (var p in LoadTypeProperty(this.GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                    ((IEntity)o).SaveStatus();
            }
        }

        public void LoadStatus()
        {
            S = _S;
            Items.ForEach(e => e.LoadStatus());
            foreach (var p in LoadTypeProperty(this.GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                    ((IEntity)o).LoadStatus();
            }
        }

        public void ResetStatus(EntityStatus s)
        {
            S = s;
            Items.ForEach(e =>
            {
                e.S = s;
            });

            foreach (var p in LoadTypeProperty(this.GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                    ((IEntity)o).ResetStatus(s);
            }
        }

        public void ResetStatus(EntityStatus oldES, EntityStatus newES)
        {
            Items.ForEach(e =>
            {
                if (e.S == oldES)
                {
                    e.S = newES;
                }
            }); 
        }

        public void Lighten()
        {
            var identityName= EntityMetaManager.Default[this.GetType()].IdentityFieldName;
            for (int index = Items.Count - 1; index >= 0; index--)
            {
                if (Items[index].S == EntityStatus.Original && Items[index].K != identityName)
                    Items.RemoveAt(index);
            }

            foreach (var p in LoadTypeProperty(this.GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                    ((IEntity)o).Lighten();
            }
        }

       
        public void LoadIDandStatus(IEntity e)
        {
            if (e is EntityBase)
            {
                EntityBase s = e as EntityBase;
                S = s.S;
                var identityName = EntityMetaManager.Default[GetType()].IdentityFieldName;
                if (!string.IsNullOrEmpty(identityName))
                    this[identityName] = s[identityName];
                //todo 删除对象
                foreach (var p in LoadTypeProperty(this.GetType(), typeof(IEntity)))
                {
                    var op = p.GetValue(this, null) as IEntity;
                    var sp = p.GetValue(e, null) as IEntity;
                    if (op != null && sp != null)
                        op.LoadIDandStatus(sp);
                }
            }
        }

        public void Delete()
        {
            S = EntityStatus.Delete;
        }

        public void SetValue(string key, object value)
        {
            PropertyInfo   _p= SerializableData.GetObjPro(key, this.GetType());
            if (_p != null)
            {
                TypeCode tc = Type.GetTypeCode(_p.PropertyType);
                SetValue(key, value, tc);
            }
            else
            {
                SetValue(key, value, TypeCode.Empty);//默认 不处理
            }

        }
#if SILVERLIGHT

        public void Save<T>(object sender,bool isEAP, EventHandler<TEventArgs<T>> del) where T :EntityBase
        { 
            var searchOP = new EntityOP<T>(isEAP ? AppContext.EAPRequestContent : AppContext.RequestContent);
            searchOP.Save(sender, (T)this, del  );
        }
#else

        public void Save(EntityBase parent, LoginInfo user, DataProvider datasource)
        {
            //datasource.BeginTran();
            var op = EntityMetaManager.Default[this.GetType()].GetOP(user, datasource);
            op.BeforeSave(this);
            op.blCoustomTran = true;
            op.BeginTran();
            try
            {
                op.Save(this);

                foreach (var p in LoadTypeProperty(GetType(), typeof(IEntity)))
                {
                    var o = p.GetValue(this, null);
                    if (o != null)
                    {
                        ForeignPropertyMeta fp = EntityMetaManager.Default[this.GetType()].GetForeignProperty(p);
                        if (fp != null && !string.IsNullOrEmpty(fp.MasterKey) && !string.IsNullOrEmpty(fp.ForeignKey))
                        {
                            if (this[fp.MasterKey] != null)
                                ((IEntity)o).SetValue(fp.ForeignKey, this[fp.MasterKey]);
                        }

                        ((IEntity)o).Save(this, user, datasource,true);
                    }
                }
                ResetStatus(EntityStatus.Original);
                op.CommitTran();
                
            }
            catch 
            {
                op.RollBackTran();
                throw;
            }
            finally
            {
                op.blCoustomTran = false;
            }
            op.AfterSave(this);
        }

        public void Save(EntityBase parent, LoginInfo user, DataProvider datasource, bool isInTrans)
        {
            //datasource.BeginTran();
            var op = EntityMetaManager.Default[this.GetType()].GetOP(user, datasource);
            op.BeforeSave(this);

            op.blCoustomTran = isInTrans;

            op.Save(this);

            foreach (var p in LoadTypeProperty(GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                {
                    ForeignPropertyMeta fp = EntityMetaManager.Default[this.GetType()].GetForeignProperty(p);
                    if (fp != null && !string.IsNullOrEmpty(fp.MasterKey) && !string.IsNullOrEmpty(fp.ForeignKey))
                    {
                        if (this[fp.MasterKey] != null)
                            ((IEntity)o).SetValue(fp.ForeignKey, this[fp.MasterKey]);
                    }

                    ((IEntity)o).Save(this, user, datasource,isInTrans );
                }
            }
            ResetStatus(EntityStatus.Original);

            op.AfterSave(this);
        }
#endif

        public void RemoveDataItem(EntityStatus s)
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                var e = Items[i];
                if (e.K != "ID" && (e.S & s) == e.S) Items.RemoveAt(i);
            }

            foreach (var p in LoadTypeProperty(GetType(), typeof(IEntity)))
            {
                var o = p.GetValue(this, null);
                if (o != null)
                    ((IEntity)o).RemoveDataItem(s);
            }
        }

        #endregion
        protected override void BeforeSerialize(TJson obj)
        {
            base.BeforeSerialize(obj);
            SaveStatus();
        }

        protected override void AfterDeserialize(TJson obj)
        {
            base.AfterDeserialize(obj);
            LoadStatus();
        }
    }

    public class EntityList<T> : TJsonList<T>, IEntity where T : EntityBase
    {
        public bool IsChildren { get; set; }

        #region ISerialize Members

        public EntityStatus GetChildrenStatus()
        {
            var result = EntityStatus.Original;
            foreach (IEntity s in this)
            {
                result = SerializableData.MergerStatus(result, s.GetChildrenStatus());
            }
            return result;
        }

        public void SetValue(string key, object value)
        {
            ForEach(e => e.SetValue(key, value));
        }

        public void ResetValue()
        {
            ForEach(e => e.ResetValue());
        }

        public void SaveStatus()
        {
            ForEach(e => e.SaveStatus());
        }

        public void LoadStatus()
        {
            ForEach(e => e.LoadStatus());
        }

        public void ResetStatus(EntityStatus s)
        {
            ForEach(e => e.ResetStatus(s));
        }

        public void RemoveDataItem(EntityStatus s)
        {
            ForEach(e => e.RemoveDataItem(s));
        }

        public void Lighten()
        {
            for (int index = Count - 1; index >= 0; index--)
            {
                if (this[index].S == EntityStatus.Original)
                    RemoveAt(index);
                else
                    this[index].Lighten();
            }
        }

        public void LoadIDandStatus(IEntity e)
        {
            if (e is EntityList<T>)
            {
                var s = e as EntityList<T>;
                if (s != null)
                {
                    for(int index=Count-1;index>=0;index--) 
                    {
                        var item=this[index];
                        if (item.S == EntityStatus.Delete)
                        {
                            RemoveAt(index);
                            continue;
                        }

                        EntityBase v = s.FirstOrDefault(a => { return a.GID == item.GID; });

                        if (v != null)
                        {                            
                            item.LoadIDandStatus(v);
                        }
                    }
                }
            }
        }

#if SILVERLIGHT
#else
        public void Save(EntityBase parent, LoginInfo user, DataProvider datasource)
        {
            if (parent.S == EntityStatus.Delete)
                ForEach(e => e.S = EntityStatus.Delete);
            ForEach(e => e.Save(parent, user, datasource));
        }

        public void Save(EntityBase parent, LoginInfo user, DataProvider datasource, bool isInTrans)
        {
            if (parent.S == EntityStatus.Delete)
                ForEach(e => e.S = EntityStatus.Delete);
            ForEach(e => e.Save(parent, user, datasource,isInTrans));
        }
#endif
        #endregion
    }
    
  
}
