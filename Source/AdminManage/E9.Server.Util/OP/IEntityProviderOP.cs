using System;
using NM.Util;
namespace NM.OP
{
   public interface IEntityProviderOP
    {
        int Save(NM.Util.SerializableData data);
        IJson Load(NM.Model.SearchCriteria searchCriteria);

        void BeforeSave(NM.Util.SerializableData data);
        void AfterSave(NM.Util.SerializableData data);

        bool blCoustomTran { get; set; }
        void BeginTran();
        void CommitTran();
        void RollBackTran();

    }
}
