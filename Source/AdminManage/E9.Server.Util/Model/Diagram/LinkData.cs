using System;
using System.Collections.Generic;
using NM.Util;

namespace NM.Diagram.Render
{
    /// <summary>
    /// Business entity used to model a LinkData
    /// It is a base modle class 
    /// </summary>
    public class LinkData : SerializableData
    {
     
        public LinkData()
        {
            Images = new List<ObjectImage>();
        }

        public virtual int ObjectID { get; set; }
        //{
        //    get { return 0; }
        //    set { }
        //}

        public virtual int KeyID { get; set; }
        //{
        //    get { return ObjectID; }
        //    set { }
        //}

        public virtual string NameID
        {
            get
            {
                return DataType.ToString() + "_" + ObjectID.ToString();
            }
        }
        public string Text{get;set;} 
        public ObjectType DataType { get; set; }
        public virtual string CustomAttibutes { get; set; }
        public string ImageUrl { get; set; }
		public string LinkUrl { get; set; }
        public string associate { get; set; }
        public int associate_id { get; set; }
        public string Attribute { get; set; }
        public bool ReadOnly { get; set; }
        public List<ObjectImage> Images { get; set; }
        public ObjectIcon Icon { get; set; }
        public Uri ImageUri
        {
            get
            {
                if (!string.IsNullOrEmpty(ImageUrl))
                {
                    return new Uri(ImageUrl);
                }
                else
                    return new Uri("../../images/noImage.jpg", UriKind.Relative);
            }
        }
        public override string ToString()
        {
            return DataType.ToString() + "  " + Text;
        } 
    }
}