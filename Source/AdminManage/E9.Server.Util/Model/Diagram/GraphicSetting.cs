using System.Collections.Generic; 
using NM.Util;

namespace NM.Diagram.Render
{
    /// <summary>
    /// Business entity used to model a GraphicSetting
    /// </summary>
    public class GraphicSetting : SettingBase
    {
        #region ** Constructors

        /// <summary>
        /// Default constructor with some specified initial values
        /// </summary>
        public GraphicSetting()
        {
            init();
        }

        public GraphicSetting(int width, int height)
            : base(width, height)
        {
            init();
        }

        public GraphicSetting(int width, int height, int left, int top, int right, int bottom)
            : base(width, height, left, top, right, bottom)
        {
            init();
        }

        #endregion

        private void init()
        {
            ViewDeep = 1;
            NodeWidth = 60;
            NodeHeight = 60;
            ShowEnum = ShowEnum.ShowImage | ShowEnum.ShowAssociator | ShowEnum.ShowSubContactIcon;
        }

        public void CopyFrom(GraphicSetting graphicSetting)
        {
            Associates = graphicSetting.Associates.GetActiveAssociate();
            BeginDeepth = graphicSetting.BeginDeepth;             
            IsHideTitle = graphicSetting.IsHideTitle;
            ImagePath = graphicSetting.ImagePath;
            LineStyle = graphicSetting.LineStyle;
            OffsetX = graphicSetting.OffsetX;
            OffsetY = graphicSetting.OffsetY;
            SaveFilter = graphicSetting.SaveFilter;
            ShowTypes = graphicSetting.ShowTypes;
            ViewType = graphicSetting.ViewType;
            ViewDeep = graphicSetting.ViewDeep;
        }        

        #region ** Properties

        public int ViewDeep { get; set; }
        public int OffsetX { get; set; }
        public int OffsetY { get; set; }
        public bool IsAppend { get; set; }
        public bool IsSub { get; set; }
        public bool IsHideTitle { get; set; }
        public int BeginDeepth { get; set; }
        public string ImagePath { get; set; }
        public ViewType ViewType { get; set; }
        public ShowEnum ShowEnum { get; set; }
        public MetaState SaveFilter { get; set; }
        public NodeType ShowTypes { get; set; }
        private ObjectAssociateSetting _Associates;
        public ObjectAssociateSetting Associates
        {
            get
            {
                if (_Associates == null || _Associates.Items.Count == 0)
                {
                    _Associates = new ObjectAssociateSetting();
                    _Associates.Add(new ObjectAssociate(ObjectType.Case, NodeType.AllObject));
                    _Associates.Add(new ObjectAssociate(ObjectType.Contact, NodeType.AllObject));
                    _Associates.Add(new ObjectAssociate(ObjectType.Address, NodeType.AllObject));
                    _Associates.Add(new ObjectAssociate(ObjectType.Phone, NodeType.AllObject));
                    _Associates.Add(new ObjectAssociate(ObjectType.Vehicle, NodeType.AllObject));
                    _Associates.Add(new ObjectAssociate(ObjectType.Establishment, NodeType.AllObject));
                    //_Associates.Add(new ObjectAssociate(ObjectType.Activity, NodeType.AllObject));
                    _Associates.Add(new ObjectAssociate(ObjectType.Gang, NodeType.Contact));
                    _Associates.Add(new ObjectAssociate(ObjectType.Article, NodeType.AllObject));
                }
                return _Associates;
            }
            set
            {
                _Associates = value;
            }
        }

        //public bool ShowImage
        //{
        //    get
        //    {
        //        return (ShowEnum & ShowEnum.ShowImage) == ShowEnum.ShowImage;
        //    }
        //}
        //public bool ShowAssociator { get { return (ShowEnum & ShowEnum.ShowAssociator) == ShowEnum.ShowAssociator; } }
        //public bool ShowMainContactIcon { get { return (ShowEnum & ShowEnum.ShowMainContactIcon) == ShowEnum.ShowMainContactIcon; } }
        //public bool ShowSubContactIcon { get { return (ShowEnum & ShowEnum.ShowSubContactIcon) == ShowEnum.ShowSubContactIcon; } }
        //public bool ShowSameLevelAssocLine { get { return (ShowEnum & ShowEnum.ShowSameLevelAssocLine) == ShowEnum.ShowSameLevelAssocLine; } }
        //public bool ShowLastLevelAssocLine { get { return (ShowEnum & ShowEnum.ShowLastLevelAssocLine) == ShowEnum.ShowLastLevelAssocLine; } }

        #endregion
    }

    public class ObjectAssociate : TJson
    {
        public ObjectAssociate() { }

        public ObjectAssociate(ObjectType objectType, NodeType nodeType)
        {
            ObjectType = objectType;
            Associate = nodeType;
            Enable = true;
        }

        public ObjectType ObjectType { get; set; }
        public NodeType Associate { get; set; }
        public bool Enable { get; set; }
    }

    public class ObjectAssociateSetting : TJson
    {
        public ObjectAssociateSetting() { }
       
        List<ObjectAssociate> _Items;
        public List<ObjectAssociate> Items
        {
            get
            {
                if (_Items == null)
                    _Items = new List<ObjectAssociate>();
                return _Items;
            }
            set { _Items = value; }
        }

        public void Add(ObjectAssociate newItem)
        {
            Items.Add(newItem);
        }

        public bool Contain(ObjectType objectType)
        {
            foreach (ObjectAssociate item in Items)
            {
                if (item.ObjectType == objectType)
                    return item.Enable;

            }
            return false;
        }

        public ObjectAssociateSetting GetActiveAssociate()
        {
            ObjectAssociateSetting result = new ObjectAssociateSetting();
            foreach (ObjectAssociate item in Items)
            {
                if (item.Enable)
                    result.Add(item);
            }
            return result;
        }
    }
}
