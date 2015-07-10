using System;

namespace Free.Controls.Toolbox
{
	[Serializable]
	public class ToolboxType
	{
		private Type _type;
		private int _defaultWidth;
		private int _defaultHeight;

		public ToolboxType(Type type, int defaultWidth, int defaultHeight)
		{
			_type=type;
			_defaultWidth=defaultWidth;
			_defaultHeight=defaultHeight;
		}

		public int DefaultWidth
		{
			get
			{
				return _defaultWidth;
			}
		}

		public int DefaultHeight
		{
			get
			{
				return _defaultHeight;
			}
		}

		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
}
