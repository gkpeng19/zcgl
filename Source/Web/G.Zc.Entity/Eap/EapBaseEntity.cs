using GOMFrameWork.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace G.Zc.Entity.Eap
{
    public class EapBaseEntity : EntityBase { }

    public class EapSearchEntity : SearchEntity
    {
        public EapSearchEntity() { }
        public EapSearchEntity(string searchId)
            : base(searchId)
        {
        }
    }
    public class EapOracleProcEntity : ProcEntity
    {
        public EapOracleProcEntity() { }
        public EapOracleProcEntity(string procName)
            : base(procName)
        {
        }
    }

    [OracleOutParam("p_cursor")]
    public class EapOracleListProcEntity:EapOracleProcEntity
    {
        public EapOracleListProcEntity() { }
        public EapOracleListProcEntity(string procName)
            : base(procName)
        {
        }
    }
}
