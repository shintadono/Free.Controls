using System.Collections.Generic;

namespace Free.Controls.Toolbox
{
	public class ToolboxGroup : ToolboxItemBase
	{
		private List<ToolboxItem> _items;
		private bool _expanded;

		public ToolboxGroup(string caption)
		{
			_items=new List<ToolboxItem>();
			_caption=caption;
			_expanded=false;
		}

		public List<ToolboxItem> Items
		{
			get
			{
				return _items;
			}
		}

		public int ItemHeight
		{
			get
			{
				return 19*_items.Count;
			}
		}

		public bool Expanded
		{
			get
			{
				return _expanded;
			}
			set
			{
				_expanded=value;
			}
		}
	}
}
