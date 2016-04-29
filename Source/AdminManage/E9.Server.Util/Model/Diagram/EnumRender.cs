using System;

namespace NM.Diagram.Render
{
    public enum DialogResult
    {
        None,
        OK,
        Cancel,
        Abort,
        Retry,
        Ignore,
        Yes,
        No
    }

    public enum MouseAction
    {
        DrawLine,
        DrawElement,
        Nothing
    }

    public enum NodeDataStatus
    {
        Original,
        New,
        Deleted
    }

    public enum ElementColor:int
    {
        Nothing = 0,
        Expand = 1,
        Delete = 2,
        Open = 4,
        Detail = 8,
        Collapse = 16,
        All = 31
    }

    public enum PageType : int
    {
        LinkAnalysis = 0,
        LinkChart = 1,
        ChartReader = 2,
        GangUI = 3,
		ChartPage = 4
    }

    public enum ObjectType : int
    {
        Nothing = 0,
        Case = 1,
        Contact = 2,
        Staff = 3,
        Vehicle = 4,
        Address = 5,
        Phone = 6,
        Gang = 7,
        GangMember = 8,
        SubSet = 9,
        Establishment = 10,
        Activity = 11,
        Article = 21,
        PawnTicket = 22,
        PawnShop = 23,
        Image = 100
    }

    [Flags]
    public enum NodeType : int
    {
        Nothing = 0,
        Contact = 1,
        Vehicle = 2,
        Address = 4,
        Phone = 8,
        Establishment = 16,
        Case = 32,
        Activity = 64,
        Gang = 128,
        Subset = 256,
        Article = 512,
        PawnTicket = 1024,
        PawnShop = 2048,
        AllObject = 2047
    }

    [Flags]
    public enum NodeAttribute : int
    {
        Nothing = 0,
        NoExpand = 1,
        NoHyperlink = 2,
        NoMovable = 4,
        NoTip = 8
    }

    public enum ShowEnum
    {
        Nothing = 0,
        ShowImage = 1,
        ShowAssociator = 2,
        ShowMainContactIcon = 4,
        ShowSubContactIcon = 8,
        ShowSameLevelAssocLine = 16,
        ShowLastLevelAssocLine = 32
    }

    public enum ViewType
    {
        Rectangle,
        Round
    }

    [Flags]
    public enum MetaState
    {
        Normal = 0,
        New = 1,
        Deleted = 2,
        ModifyAssoc = 4,
        ChangeFrom = 8,
        ChangeTo = 16,
        ChangeLevel = 32,
        Modify = 64,
        NoRelocal = 127,
        Relocal = 128,
        All = 255
    }

    public enum MetaVisible
    {
        Visible = 0,
        Collapse = 1
    }

    public enum PageFormat
    {
        Letter,
        Letter_Landscape,
        Legal,
        Legal_Landscape,
        Custom,
    }

    public enum PrintForm
    {
        Pdf,
        Bmp
    }

    [Flags]
    public enum ElementOperator
    {
        Nothing = 0,
        Expand = 1,
        Delete = 2,
        Open = 4,
        Detail = 8,
        Collapse = 16,
        All = 31
    }

    public enum GraphicLoad
    {
        Generate,
        Database
    }

    public enum LineStyle
    {
        Curly,
        Straight
    } 
    public enum LineMode : int
    {
        None =0,
        Retrorse = -1,
        Antrorse = 1,
        Both = 2
    }

    public enum StrokePattern
    {
        Solid,
        Dotted,
        Dashed,
        DashDot
    }

    public enum LineColor
    {
        Black,
        Blue,
        Brown,
        Cyan,
        DarkGray,
        Gray,
        Green,
        LightGray,
        Magenta,
        Orange,
        Purple,
        Red,
        Yellow
    }
}
