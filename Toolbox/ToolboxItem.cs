namespace Free.Controls.Toolbox
{
	public class ToolboxItem : ToolboxItemBase
	{
		private int _iconIndex;
		private ToolboxType _type;

		public ToolboxItem(string caption, int iconIndex, ToolboxType type)
		{
			_caption=caption;
			_iconIndex=iconIndex;
			_type=type;
		}

		public int IconIndex
		{
			get
			{
				return _iconIndex;
			}
			set
			{
				_iconIndex=value;
			}
		}

		public ToolboxType TypeInfo
		{
			get
			{
				return _type;
			}
		}
	}
}
