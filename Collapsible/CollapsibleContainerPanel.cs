using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free.Controls.Collapsible
{
	[ToolboxItem(false), Designer(typeof(CollapsibleContainerPanelDesigner)), Docking(DockingBehavior.Never)]
	public sealed class CollapsibleContainerPanel : Panel
	{
		#region Events (Hauptsächlich verstecken)
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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
		#endregion

		// Methods
		public CollapsibleContainerPanel()
		{
			base.SetStyle(ControlStyles.ResizeRedraw, true);
		}

		[EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		[Category("Layout"), Description("Control.Width")]
		public new int Width
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

		[EditorBrowsable(EditorBrowsableState.Always), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		[Category("Layout"), Description("Control.Height")]
		public new int Height
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Size Size
		{
			get
			{
				return base.Size;
			}
			set
			{
				base.Size=value;
			}
		}

		protected override Padding DefaultMargin
		{
			get
			{
				return new Padding(0, 0, 0, 0);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		[Localizable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new ScrollableControl.DockPaddingEdges DockPadding
		{
			get
			{
				return base.DockPadding;
			}
		}

		#region Properties (Hauptsächlich verstecken)
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
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
		#endregion
	}
}
