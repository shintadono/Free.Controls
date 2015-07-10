namespace Free.Controls.Toolbox
{
	public class ToolboxItemBase
	{
		protected int _top;
		protected bool _mouseOver;
		protected string _caption;
		protected bool _selected;

		public int Top
		{
			get
			{
				return _top;
			}
			set
			{
				_top=value;
			}
		}

		public bool MouseOver
		{
			get
			{
				return _mouseOver;
			}
			set
			{
				_mouseOver=value;
			}
		}

		public string Caption
		{
			get
			{
				return _caption;
			}
			set
			{
				_caption=value;
			}
		}

		public bool Selected
		{
			get
			{
				return _selected;
			}
			set
			{
				_selected=value;
			}
		}
	}
}
