using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace App.Admin.Core
{

    /// <summary>
    /// 为构造树定义的类
    /// </summary>
    public class TreeBase
    {
        public TreeBase()
        {
            children = new List<TreeBase>();
        }
        public int id { get; set; }
        public string text { get; set; }
        public string state { get; set; }

       [XmlElement(ElementName = "checked")]
        public bool ischeck { get; set; }
        public List<TreeBase> children { get; set; }


    }
}