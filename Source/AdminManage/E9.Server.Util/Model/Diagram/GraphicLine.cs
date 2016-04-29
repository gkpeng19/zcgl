

#if SILVERLIGHT
using System.Windows.Media;
#else 
using System.Drawing;
#endif

namespace NM.Diagram.Render
{
    public class GraphicLine
    {
        public GraphicLine()
        {
            // Color = Colors.Gray;
        }

        public int Index { get; set; }
        public string Text { get; set; }
        public Color Color { get; set; }
        public double X1 { get; set; }
        public double Y1 { get; set; }
        public double X2 { get; set; }
        public double Y2 { get; set; }

        public GraphicLine NextLine()
        {
            GraphicLine result = new GraphicLine();
            result.Index = Index + 1;
            result.Text = result.Index.ToString();
            result.Color = Color;
            result.X1 = X1;
            result.X2 = X2;
            result.Y1 = Y2;
            result.Y2 = Y2 + (Y2 - Y1);
            return result;
        }
    }
}