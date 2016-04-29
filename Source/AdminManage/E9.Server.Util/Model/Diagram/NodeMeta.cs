using System;

namespace NM.Diagram.Render
{
    /// <summary>
    /// Business entity used to model a NodeMeta
    /// </summary>
    public class NodeMeta : LinkElementMeta
    {
        /// <summary>
        /// Default constructor 
        /// </summary>
        public NodeMeta() { }

		//public NodeMeta(LinkData data)
		//{
		//    SId = data.ObjectID;
		//    DataType = data.DataType;          
		//    Text = data.Text;
		//    ReadOnly = data.ReadOnly;
		//    //Data = data;
		//    ImageUrl = data.ImageUrl;
		//    LinkUrl = data.LinkUrl;
		//}

        // Properties
		public int Left
		{
			get;
			set;
		}
		public int Top
		{
			get;
			set;
		}
		public int SId
		{
			get;
			set;
		}
		public int KId
		{
			get;
			set;
		}
		public string Text
		{
			get;
			set;
		}
		public int Level
		{
			get;
			set;
		}
		public int LinkTo
		{
			get;
			set;
		}
		public ObjectType DataType
		{
			get;
			set;
		}
		public NodeDataStatus DataStatus
		{
			get;
			set;
		}
		public int ParentId
		{
			get;
			set;
		}
		public ObjectType ParentType
		{
			get;
			set;
		}
		public NodeType ShowTypes
		{
			get;
			set;
		}
		public NodeAttribute Att
		{
			get;
			set;
		}
		public int Access
		{
			get;
			set;
		}
		public string ImageUrl
		{
			get;
			set;
		}
		public string LinkUrl
		{
			get;
			set;
		}
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

        public Action<NodeMeta> OnClick;

        //[IgnoreDataMember]
        //public LinkData Data { get; set; }
        public NodeMeta Clone()
        {
            NodeMeta result = new NodeMeta();
            result.SId = SId;
            result.Text = Text;
            result.Left = Left;
            result.Top = Top;
            result.Left = Level;
            result.LineColor = LineColor;
            result.LinkTo = LinkTo;
            result.ParentId = ParentId;
            result.ParentType = ParentType;
            result.ReadOnly = ReadOnly;
            result.ShowTypes = ShowTypes;
            result.State = State;
            result.DataStatus = DataStatus;
			result.ImageUrl = ImageUrl;
			result.LinkUrl = LinkUrl;
           // result.Data = Data;
            result.DataType = DataType;
            result.Opacity = Opacity;
            result.Visible = Visible;

            return result;
        }
    }
}
