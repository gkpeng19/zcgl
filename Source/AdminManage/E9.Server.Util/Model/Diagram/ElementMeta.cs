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
	public class LinkElementMeta : TJson
	{
		public LinkElementMeta() { }

		public MetaState State
		{
			get;
			set;
		}
		public bool ReadOnly
		{
			get;
			set;
		}
		public MetaVisible Visible
		{
			get;
			set;
		}
		public double Opacity
		{
			get;
			set;
		}
		[IgnoreDataMember]
		public Color LineColor
		{
			get;
			set;
		}
		public byte[] ColorArray
		{
			get
			{
				byte[] array = new byte[4] 
                { 
                    LineColor.A, LineColor.R, LineColor.G, LineColor.B 
                };
				return array;
			}
			set
			{
				if (null != value && value.Length == 4
					&& !(value[0] != 0 && value[1] != 0 && value[2] != 0 && value[3] != 0))
				{
					LineColor = Color.FromArgb(value[0], value[1], value[2], value[3]);
				}
				else
				{
					LineColor = Color.FromArgb(0xFF, 0x00, 0x00, 0x8B);
				}
			}
		}
	}
}