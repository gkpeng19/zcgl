using System;
using System.Collections;
using System.Collections.Generic;

using NM.Util;
using System.Runtime.Serialization;

namespace NM.Model
{
    public class ProcResult : EntityBase
    {
        [IgnoreDataMember]
        public Int32 ResultID
        {
            get { return GetInt32("ResultID"); }
            set
            {
                SetInt32("ResultID", value);
            }
        }

        [IgnoreDataMember]
        public String ErrorMsg
        {
            get { return GetString("ErrorMsg"); }
            set { SetString("ErrorMsg", value); }
        }
    }

    public class CommandResult : TJson
    {
        public CommandResult() { }

        public bool Result { get; set; }
        public string Message { get; set; }
        public int IntResult { get; set; }

        private LookupData _ReturnValue;
        public LookupData ReturnValue
        {
            get
            {
                if (_ReturnValue == null)
                {
                    _ReturnValue = new LookupData();
                }
                return _ReturnValue;
            }
            set
            {
                _ReturnValue = value;
            }
        }

        public override string ToString()
        {
            return Message;
        }
    }

    public class CommandResult<T> : CommandResult where T:EntityBase,new()
    {
        public CommandResult()
            : base()
        {
            Entity = new T();
        }
        public T Entity { get; set; }
    }

    public class GCommandResult<T1,T2> : CommandResult where T1:EntityBase where T2:EntityBase
    {
        public EntityList<T1> DatasOne { get; set; }
        public EntityList<T2> DatasTwo { get; set; }
    }
}
