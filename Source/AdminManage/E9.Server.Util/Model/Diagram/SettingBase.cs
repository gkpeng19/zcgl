using System;
using NM.Util;

namespace NM.Diagram.Render
{
    public class SettingBase :  TJson
    {
        public SettingBase()
        {
            IsAdjustment = true;
        }

        public SettingBase(int width, int height)
            : this()
        {
            if (width <= 0 || height <= 0)
                throw new Exception("Width or Height must be grater than zero.");

            this.Width = width;
            this.Height = height;
        }

        public SettingBase(int width, int height, int left, int top, int right, int bottom)
            : this(width, height)
        {
            if (left < 0 || top < 0 || right < 0 || bottom < 0)
                throw new Exception("Margin parameters must equal or be grater than zero.");

            this.LeftMargin = left;
            this.TopMargin = top;
            this.RightMargin = right;
            this.BottomMargin = bottom;
        }

        public LineStyle LineStyle { get; set; }
        public bool IsAdjustment { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public double LineThick { get; set; }
        public int NodeWidth { get; set; }
        public int NodeHeight { get; set; }
        public int LeftMargin { get; set; }
        public int TopMargin { get; set; }
        public int RightMargin { get; set; }
        public int BottomMargin { get; set; }
        public bool ReadOnly { get; set; }
    }
}
