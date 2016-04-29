using System.Runtime.Serialization;
using NM.Model;

namespace NM.Diagram.Render
{
    /// <summary>
    /// Business entity used to model a LineMeta
    /// to keep the Association of two Nodes as a line
    /// </summary>
    public class LineMeta : LinkElementMeta
    {
        public LineMeta() { }

 
        public LineMeta(int from, int to)
        {           
            FromId = from;          
            ToId = to;
            Visible = MetaVisible.Visible;
        } 

        //Properties 
        public int FromId { get; set; }
        public ObjectType FromType { get; set; }
        public int ToId { get; set; }
        public ObjectType ToType { get; set; }
        public string Association { get; set; }
        public int Associate_ID { get; set; }
        public LineMode LineMode { get; set; }
        public StrokePattern StrokePattern { get; set; }
        [IgnoreDataMember]
        public NodeMeta From { get; set; }
        [IgnoreDataMember]
        public NodeMeta To { get; set; }
    }
}