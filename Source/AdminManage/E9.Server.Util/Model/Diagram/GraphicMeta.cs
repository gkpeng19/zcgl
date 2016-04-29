using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using NM.Util;
using NM.Model;


#if SILVERLIGHT
using System.Windows.Media;
#else 
using System.Drawing;
#endif

namespace NM.Diagram.Render
{
    /// <summary>
    /// Business entity used to model a GraphicMetaInfo
    /// </summary>
    public class GraphicMetaInfo : TJson
    {
        /// <summary>
        /// Default constructor with some default initial values
        /// </summary>
        public GraphicMetaInfo()
        {
            this.title = "";
            this.note = "";
        }

        public GraphicMetaInfo(GraphicMetaInfo source)
        {
            CloneFrom(source);
        }

        // Properties
        public string title { get; set; }
        public int graphic_id { get; set; }
        public string note { get; set; }
        public int subject_id { get; set; }
        public int subject_type { get; set; }
        public int Version { get; set; }
        public bool IsPublic { get; set; }
        public int addby { get; set; }
        public string addbyName { get; set; }
        public string addon { get; set; }
        public int editby { get; set; }
        public string editon { get; set; }
        public bool ReadOnly { get; set; }
        public string ReadOnlyText { get; set; }
        public GraphicState GraphicState { get; set; }
        public GraphicLoad ComeFrom { get; set; }
        public PageType PageType { get; set; }
        public ObjectType ObjectType
        {
            get
            {
                return (ObjectType)subject_type;
            }
        }

        public string Text
        {
            get
            {
                return string.Format("{0}[{1}]", title, string.IsNullOrEmpty(editon) ? addon : editon);
            }
        }

        public void CloneFrom(GraphicMetaInfo source)
        {
            if (this == source) return;
            title = source.title;
            graphic_id = source.graphic_id;
            note = source.note;
            subject_id = source.subject_id;
            subject_type = source.subject_type;
            Version = source.Version;
            IsPublic = source.IsPublic;
            addby = source.addby;
            addon = source.addon;
            editby = source.editby;
            editon = source.editon;
            ReadOnly = source.ReadOnly;
            ReadOnlyText = source.ReadOnlyText;
            GraphicState = source.GraphicState;
            ComeFrom = source.ComeFrom;
            PageType = source.PageType;
        }
    }

    /// <summary>
    /// Business entity used to model a GraphicMetaBase
    /// </summary>
    public class GraphicMetaBase : GraphicMetaInfo
    {
        // Internal member varivables
        private GraphicSetting _Setting;

        /// <summary>
        /// Default constructor with some default initial values
        /// </summary>
        public GraphicMetaBase()
        {
            Nodes = new List<NodeMeta>();
            Lines = new List<LineMeta>();
            BackLines = new List<GraphicLine>();
        }

        // Properties
        public GraphicSetting Setting
        {
            get
            {
                if (_Setting == null)
                {
                    _Setting = new GraphicSetting();
                }
                return _Setting;
            }
            set
            {
                _Setting = value;
            }
        }
        public List<NodeMeta> Nodes { get; set; }
        public List<LineMeta> Lines { get; set; }
        public List<GraphicLine> BackLines { get; set; }

        /// <summary>
        /// Find and get the specified Node model object By the id and Node type
        /// </summary>
        /// <param name="innerId">NodeMeta Id</param>
        /// <param name="type">Node Type</param>
        /// <returns>NodeMeta object for the specified id and nodetype or null</returns>
        protected NodeMeta GetNoteMetaByInnerId(int innerId, ObjectType type)
        {
            foreach (NodeMeta item in Nodes)
            {
                if (item.SId == innerId && item.DataType == type)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Add a Node(Meta) to the Nodes (a node collection)
        /// </summary>
        /// <param name="node">A NodeMeta</param>
        public void AddNode(NodeMeta node)
        {
            Nodes.Add(node);
        }

        /// <summary>
        /// a virtual method for the subclasses to implement or extend it
        /// </summary>
        public virtual bool AddObject(NodeMeta data)
        {
            return true;
        }


        public LineMeta AddLine(int fromID, ObjectType fromType, int toID, ObjectType toType, string assoc)
        {
            LineMeta line = new LineMeta()
            {
                LineMode = LineMode.Antrorse,
                LineColor =Color.FromArgb(255,0,0,0),
                StrokePattern = StrokePattern.Solid,
                ReadOnly = true
            };
            // GetNoteMeta(LinkData) to initial  
            NodeMeta node = GetNoteMetaByInnerId(fromID, fromType);
            if (node != null)
            {
                line.FromId = node.SId;
                line.FromType = node.DataType;
                line.From = node;
            }

            //
            // GetNodeMeta(LinkData) to end
            node = GetNoteMetaByInnerId(toID, toType);
            if (node != null)
            {
                line.ToType = node.DataType;
                line.ToId = node.SId;
                line.To = node;
            }

            //
            // Appoint the associate for the new Line(Mete)
            line.Association = assoc;
            //
            // Push the new Line(Meta) into the Line collection Lines
            Lines.Add(line);
            return line;
        }


        /// <summary>
        /// Create a Line(Meta) to the Lines (a Line collection)
        /// </summary>
        /// <param name="from">Initial Node</param>
        /// <param name="to">End Node</param>
        /// <param name="assoc">Associate</param>
        /// <returns>return the new line created</returns>
        public LineMeta AddLine(NodeMeta from, NodeMeta to, string assoc)
        {
            if (from != null && to != null)
            {
                return AddLine(from.SId, from.DataType, to.SId, to.DataType, assoc);
            }
            return null;
        }

        public void ResetStatus()
        {
            foreach (NodeMeta item in Nodes)
            {
                item.State = MetaState.Normal;
            }

            foreach (LineMeta line in Lines)
            {
                line.State = MetaState.Normal;
            }
        }

        /// <summary>
        /// Tinker up the coordinate or position based on the offsetX&offsetY
        /// </summary>
        public void ApplyOffset()
        {
            if (Setting.OffsetX > 0 && Setting.OffsetY > 0)
            {
                foreach (NodeMeta node in Nodes)
                {
                    node.Left -= (int)(Setting.Width / 2 - Setting.OffsetX);
                    node.Top -= (int)(Setting.Height / 2 - Setting.OffsetY);
                }
            }
        }

        public void CloneFrom(GraphicMetaBase source)
        {
            if (this == source) return;
            base.CloneFrom(source);
            BackLines.Clear();
            BackLines.AddRange(source.BackLines);
            Lines.Clear();
            Lines.AddRange(source.Lines);
            Nodes.Clear();
            Nodes.AddRange(source.Nodes);
            Setting = source.Setting;
        }

        public void CopyIdAndStatus(GraphicMetaBase source)
        {
            if (this == source) return;
            base.CloneFrom(source);
            foreach (LineMeta line in Lines)
            {
                LineMeta lineMeta = null;
                try
                {
                    lineMeta = source.Lines.First<LineMeta>(c => c.FromId == line.FromId && c.FromType == line.FromType && c.ToId == line.ToId && c.ToType == line.ToType);
                    line.Associate_ID = lineMeta.Associate_ID;
                    line.State = lineMeta.State;
                }
                catch
                { }
            }

            foreach (NodeMeta node in Nodes)
            {
                NodeMeta nodemeta = null;
                try
                {
                    nodemeta = source.Nodes.First<NodeMeta>(c => c.SId == node.SId && c.DataType == node.DataType);
                    node.State = nodemeta.State;
                }
                catch
                { }
            }
        }
    }
 

    /// <summary>
    /// Business entity used to model a GraphicMeta
    /// </summary>
    public sealed class GraphicMeta : GraphicMetaBase
    {
        /// <summary>
        /// Default constructor with some specified initial values
        /// </summary>
        public GraphicMeta()
        {
            // Data = new List<NodeMeta>();
        }

        //Properties
		//public List<NodeMeta> Data { get; set; }

		//protected NodeMeta GetMetaData(NodeMeta nm)
		//{
		//    return Data.FirstOrDefault((c) => { return c.DataType == nm.DataType && c.SId == nm.SId; });
		//}

		//public override bool AddObject(NodeMeta data)
		//{
		//    if (null == data) return false;
		//    _BindTag = false;
		//    bool result = false;
		//    result = Data.IndexOf(data) < 0;
		//    if (result)
		//        Data.Add(data);
		//    return result;
		//}

		//public List<NodeMeta> GetAllMetaData()
		//{
		//    return new List<NodeMeta>(Data);
		//}
        /// <summary>
        /// ?need some desirable detail for the method
        /// </summary>
        /// <param name="meta"></param>
        public void MegerGraphic(GraphicMeta meta)
        {
            if (subject_id == 0)
            {
                subject_id = meta.subject_id;
                subject_type = meta.subject_type;
                title = meta.title;
                Version = meta.Version;
                base.note = meta.note;
            }

            //foreach (NodeMeta node in meta.Nodes)
            for (int index = meta.Nodes.Count - 1; index >= 0; index--)
            {
                NodeMeta node = meta.Nodes[index];
                NodeMeta existNode = GetNoteMetaByInnerId(node.SId, node.DataType);
                if (existNode == null)
                {
                    node.State = MetaState.New;
                    Nodes.Add(node);
                    //AddObject(meta.GetMetaData(node));
                }
                else
                {
                    node.Left = existNode.Left;
                    node.Top = existNode.Top;
                    meta.Nodes.RemoveAt(index);
                }
            }

            //
            //The line collection whose elements are copied to the new list named copyLines.
            //
            List<LineMeta> copyLines = new List<LineMeta>(Lines);
            //foreach (LineMeta line in meta.Lines)
            for (int index = meta.Lines.Count - 1; index >= 0; index--)
            {
                LineMeta line = meta.Lines[index];
                bool lineExists = false;
                foreach (LineMeta l in copyLines)
                {
                    if ((l.FromId == line.FromId && l.FromType == line.FromType
                         && l.ToId == line.ToId && l.ToType == line.ToType)
                        || (l.FromId == line.ToId && l.FromType == line.ToType
                         && l.ToId == line.FromId && l.ToType == line.FromType)
                        )
                    {
                        lineExists = true;
                        break;
                    }
                }
                if (!lineExists)
                {
                    line.State = MetaState.New;
                    Lines.Add(line);
                }
                else
                {
                    meta.Lines.RemoveAt(index);
                }
            }
        }

        public void CloneFrom(GraphicMeta source)
        {
            if (this == source) return;
            base.CloneFrom(source as GraphicMetaBase);
			//foreach (NodeMeta item in source.GetAllMetaData())
			//{
			//    AddObject(item);
			//}
        }

        public void DeleteAppendNode()
        {
            if (!Setting.IsAppend) return;
            List<NodeMeta> deleteNode = new List<NodeMeta>();
            deleteNode.AddRange(Nodes.Where(item => { return item.Level > Setting.BeginDeepth + 1; }));
            deleteNode.ForEach(e =>
            {
                Nodes.Remove(e);
                for (int index = Lines.Count - 1; index >= 0; index--)
                {
                    var line = Lines[index];
                    if ((line.FromId == e.SId && line.FromType == e.DataType)
                        || (line.ToId == e.SId && line.ToType == e.DataType))
                    {
                        Lines.Remove(line);
                        break;
                    }
                }
            });
        }

        [IgnoreDataMember]
        bool _BindTag;
		public void BindTagData()
		{
			if (_BindTag) return;
			foreach (LineMeta line in Lines)
			{
				line.From = GetNoteMetaByInnerId(line.FromId, line.FromType);
				line.To = GetNoteMetaByInnerId(line.ToId, line.ToType);
			}

			//if (GetAllMetaData().Count > 0)
			//    foreach (NodeMeta node in Nodes)
			//    {
			//        node.Data = GetMetaData(node);
			//    }
			_BindTag = true;
		}

        protected override void AfterDeserialize(TJson obj)
        {
            base.AfterDeserialize(obj);
            BindTagData();
        }

        public static int Margin = 50;
        public static int BandHeight = 120;
        
    }
}