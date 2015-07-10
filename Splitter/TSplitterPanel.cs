using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Free.Controls.Splitter
{
	[ToolboxItem(false), Designer(typeof(TSplitterPanelDesigner)), ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch), Docking(DockingBehavior.Never)]
	public sealed class TSplitterPanel : Panel
	{
		// Fields
		private bool collapsed;
		private TSplitContainer owner;

		// Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler AutoSizeChanged
		{
			add
			{
				base.AutoSizeChanged+=value;
			}
			remove
			{
				base.AutoSizeChanged-=value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler DockChanged
		{
			add
			{
				base.DockChanged+=value;
			}
			remove
			{
				base.DockChanged-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new event EventHandler LocationChanged
		{
			add
			{
				base.LocationChanged+=value;
			}
			remove
			{
				base.LocationChanged-=value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new event EventHandler TabIndexChanged
		{
			add
			{
				base.TabIndexChanged+=value;
			}
			remove
			{
				base.TabIndexChanged-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new event EventHandler TabStopChanged
		{
			add
			{
				base.TabStopChanged+=value;
			}
			remove
			{
				base.TabStopChanged-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new event EventHandler VisibleChanged
		{
			add
			{
				base.VisibleChanged+=value;
			}
			remove
			{
				base.VisibleChanged-=value;
			}
		}

		// Methods
		public TSplitterPanel(TSplitContainer owner)
		{
			this.owner=owner;
			base.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		// Properties
		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new AnchorStyles Anchor
		{
			get
			{
				return base.Anchor;
			}
			set
			{
				base.Anchor=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool AutoSize
		{
			get
			{
				return base.AutoSize;
			}
			set
			{
				base.AutoSize=value;
			}
		}

		[Localizable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override AutoSizeMode AutoSizeMode
		{
			get
			{
				return AutoSizeMode.GrowOnly;
			}
			set
			{
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle=value;
			}
		}

		internal bool Collapsed
		{
			get
			{
				return this.collapsed;
			}
			set
			{
				this.collapsed=value;
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0, 0, 0, 0);
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DockStyle Dock
		{
			get
			{
				return base.Dock;
			}
			set
			{
				base.Dock=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ScrollableControl.DockPaddingEdges DockPadding
		{
			get
			{
				return base.DockPadding;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Always), Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Control.Height")]
		public new int Height
		{
			get
			{
				if(this.Collapsed)
				{
					return 0;
				}
				return base.Height;
			}
			set
			{
				throw new NotSupportedException("TSplitterPanel.Height");
			}
		}

		internal int HeightInternal
		{
			get
			{
				return base.Height;
			}
			set
			{
				base.Height=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point Location
		{
			get
			{
				return base.Location;
			}
			set
			{
				base.Location=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size MaximumSize
		{
			get
			{
				return base.MaximumSize;
			}
			set
			{
				base.MaximumSize=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Size MinimumSize
		{
			get
			{
				return base.MinimumSize;
			}
			set
			{
				base.MinimumSize=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new string Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name=value;
			}
		}

		internal TSplitContainer Owner
		{
			get
			{
				return this.owner;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Control Parent
		{
			get
			{
				return base.Parent;
			}
			set
			{
				base.Parent=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Size Size
		{
			get
			{
				if(this.Collapsed)
				{
					return Size.Empty;
				}
				return base.Size;
			}
			set
			{
				base.Size=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int TabIndex
		{
			get
			{
				return base.TabIndex;
			}
			set
			{
				base.TabIndex=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new bool TabStop
		{
			get
			{
				return base.TabStop;
			}
			set
			{
				base.TabStop=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new bool Visible
		{
			get
			{
				return base.Visible;
			}
			set
			{
				base.Visible=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Description("Control.Width"), EditorBrowsable(EditorBrowsableState.Always), Category("Layout")]
		public new int Width
		{
			get
			{
				if(this.Collapsed)
				{
					return 0;
				}
				return base.Width;
			}
			set
			{
				throw new NotSupportedException("TSplitterPanel.Width");
			}
		}

		internal int WidthInternal
		{
			get
			{
				return base.Width;
			}
			set
			{
				base.Width=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnControlRemoved(ControlEventArgs e)
		{
			base.OnControlRemoved(e);
			if((e.Control is TSplitContainer)&&(e.Control.Dock==DockStyle.Fill))
			{
				owner.SetInnerMostBorder(owner);
			}
		}
	}
}
