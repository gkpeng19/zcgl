using System;
using System.Collections.Generic;

namespace NM.Diagram.Render
{
    public class ObjectCaption
    {
        public ObjectCaption()
        {
            Items = new List<ObjectCaption>();
        }

        public string code { get; set; }
        public string description { get; set; }
        public string caption { get; set; }

        //public ObjectType CodeType
        //{
        //    get
        //    {
        //        return (ObjectType)Enum.Parse(typeof(ObjectType), code, true);
        //    }
        //}


        public List<ObjectCaption> Items
        {
            get;
            set;
        }

        public string this[ObjectType type]
        {
            get
            {
                string sReturn = "";

                foreach (var item in Items)
                {
                    if (item.description == ((int)type).ToString() || item.code == type.ToString())
                    {
                        sReturn = item.caption;
                        break;
                    }
                }

                return sReturn;
            }
        }

        public ObjectType this[string sText]
        {
            get {
                ObjectType oType = ObjectType.Nothing;

                foreach (var item in Items)
                {
                    if (item.code == sText || item.description == sText)
                    {
                        oType = (ObjectType)Enum.Parse(typeof(ObjectType), item.code, true);
                    }
                }

                return oType;
            }
        }
    }
}
