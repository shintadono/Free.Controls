using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free.Controls.Collapsible
{
	[DefaultProperty("Collapsed"), DefaultEvent("CollapsedChanged"), Designer(typeof(CollapsibleContainerBaseDesigner))]
	[Description("CollapsibleContainerBase"), Docking(DockingBehavior.Never)]
	[ToolboxBitmap(typeof(CollapsibleContainerBase), "Free.Controls.CollapsibleContainer.bmp")]
	public partial class CollapsibleContainerBase : ContainerControl, ISupportInitialize
	{
		internal const int HeaderHeight=25;

		#region Fields
		private bool initializing;
		private bool collapsed;
		private CollapsibleContainerPanel panel1;
		private CollapsibleContainerPanel panel2;
		private int uncollapsedHeight=100;
		#endregion

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

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler BackgroundImageLayoutChanged
		{
			add
			{
				base.BackgroundImageLayoutChanged+=value;
			}
			remove
			{
				base.BackgroundImageLayoutChanged-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event ControlEventHandler ControlAdded
		{
			add
			{
				base.ControlAdded+=value;
			}
			remove
			{
				base.ControlAdded-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event ControlEventHandler ControlRemoved
		{
			add
			{
				base.ControlRemoved+=value;
			}
			remove
			{
				base.ControlRemoved-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler PaddingChanged
		{
			add
			{
				base.PaddingChanged+=value;
			}
			remove
			{
				base.PaddingChanged-=value;
			}
		}
		#endregion

		#region Event
		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public event EventHandler CollapsedChanged;
		#endregion

		public CollapsibleContainerBase()
		{
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			panel1=new CollapsibleContainerPanel();
			panel2=new CollapsibleContainerPanel();
			((TypedControlCollection)Controls).AddInternal(panel1);
			((TypedControlCollection)Controls).AddInternal(panel2);
			CollapseOrExpand(false);
		}

		#region Methods
		public void BeginInit()
		{
			initializing=true;
		}

		public void EndInit()
		{
			initializing=false;
			panel1.Height=HeaderHeight;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new CollapsibleContainerTypedControlCollection(this, typeof(CollapsibleContainerPanel), true);
		}

		private void CollapseOrExpand(bool setHeight)
		{
			panel1.SuspendLayout();
			panel2.SuspendLayout();

			panel1.Location=new Point(0, 0);
			panel2.Location=new Point(0, HeaderHeight);
			panel1.Width=panel2.Width=Width;
			panel1.Height=HeaderHeight;

			if(setHeight)
			{
				if(collapsed)
				{
					Height=HeaderHeight;
				}
				else
				{
					Height=uncollapsedHeight;
				}
			}

			panel2.Height=uncollapsedHeight-HeaderHeight;

			panel1.ResumeLayout();
			panel2.ResumeLayout();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			CollapseOrExpand(false);
			Refresh();
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if(height<HeaderHeight+(Collapsed?0:1)) height=HeaderHeight+(Collapsed?0:1);

			if(Collapsed) height=HeaderHeight;

			base.SetBoundsCore(x, y, width, height, specified);
		}

		public virtual void UpdateCollapsedState()
		{
			CollapseOrExpand(true);
		}
		#endregion

		#region Properties
		[DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never), Description("Form.AutoScroll"), Browsable(false), Category("Layout"), Localizable(true)]
		public override bool AutoScroll
		{
			get
			{
				return false;
			}
			set
			{
				base.AutoScroll=value;
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollMargin
		{
			get
			{
				return base.AutoScrollMargin;
			}
			set
			{
				base.AutoScrollMargin=value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Size AutoScrollMinSize
		{
			get
			{
				return base.AutoScrollMinSize;
			}
			set
			{
				base.AutoScrollMinSize=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), DefaultValue(typeof(Point), "0, 0"), Browsable(false)]
		public override Point AutoScrollOffset
		{
			get
			{
				return base.AutoScrollOffset;
			}
			set
			{
				base.AutoScrollOffset=value;
			}
		}

		[Category("Layout"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Description("Form.AutoScrollPosition"), EditorBrowsable(EditorBrowsableState.Never)]
		public new Point AutoScrollPosition
		{
			get
			{
				return base.AutoScrollPosition;
			}
			set
			{
				base.AutoScrollPosition=value;
			}
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public override bool AutoSize
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

		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public override Image BackgroundImage
		{
			get
			{
				return base.BackgroundImage;
			}
			set
			{
				base.BackgroundImage=value;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override ImageLayout BackgroundImageLayout
		{
			get
			{
				return base.BackgroundImageLayout;
			}
			set
			{
				base.BackgroundImageLayout=value;
			}
		}

		[Browsable(false), Description("BindingContext")]
		public override BindingContext BindingContext
		{
			get
			{
				return base.BindingContext;
			}
			set
			{
				base.BindingContext=value;
			}
		}

		[Category("Appearance"), Description("BodyBorderStyle"), DefaultValue(BorderStyle.None)]
		public BorderStyle BodyBorderStyle
		{
			get
			{
				return panel2.BorderStyle;
			}
			set
			{
				panel2.BorderStyle=value;
			}
		}

		[Category("Appearance"), Description("HeaderBorderStyle"), DefaultValue(BorderStyle.None)]
		public BorderStyle HeaderBorderStyle
		{
			get
			{
				return panel1.BorderStyle;
			}
			set
			{
				panel1.BorderStyle=value;
			}
		}

		[Category("Appearance"), Description("BodyBackColor")]
		public Color BodyBackColor
		{
			get
			{
				return panel2.BackColor;
			}
			set
			{
				panel2.BackColor=value;
			}
		}

		[Category("Appearance"), Description("HeaderBackColor")]
		public Color HeaderBackColor
		{
			get
			{
				return panel1.BackColor;
			}
			set
			{
				panel1.BackColor=value;
			}
		}

		[Category("Layout"), Description("Collapsed"), DefaultValue(false)]
		public bool Collapsed
		{
			get
			{
				return collapsed;
			}
			set
			{
				if(value!=collapsed)
				{
					collapsed=value;
					panel2.Visible=!value;
					CollapseOrExpand(true);

					EventHandler e=CollapsedChanged;
					if(e!=null) e(this, EventArgs.Empty);
				}
			}
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public new Control.ControlCollection Controls
		{
			get
			{
				return base.Controls;
			}
		}

		protected override Size DefaultSize
		{
			get
			{
				return new Size(150, 100);
			}
		}

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

		public bool Initializing { get { return initializing; } }

		[EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public new Padding Padding
		{
			get
			{
				return base.Padding;
			}
			set
			{
				base.Padding=value;
			}
		}

		[Category("Appearance"), Description("Panel1"), Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CollapsibleContainerPanel Panel1
		{
			get
			{
				return panel1;
			}
		}

		[Category("Appearance"), Description("Panel2"), Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CollapsibleContainerPanel Panel2
		{
			get
			{
				return panel2;
			}
		}

		[Bindable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				base.Text=value;
			}
		}

		[Category("Layout"), Description("Uncollapsed Height")]
		public int UncollapsedHeight
		{
			get
			{
				return uncollapsedHeight;
			}
			set
			{
				if(value!=uncollapsedHeight&&value>HeaderHeight)
				{
					uncollapsedHeight=value;
					CollapseOrExpand(true);
				}
			}
		}
		#endregion

		#region Nested Types (Nested um an die protected CollapsibleContainerBase.DesignMode-Varable zu kommen)
		internal class CollapsibleContainerTypedControlCollection : TypedControlCollection
		{
			private CollapsibleContainerBase owner;

			public CollapsibleContainerTypedControlCollection(Control c, Type type, bool isReadOnly)
				: base(c, type, isReadOnly)
			{
				owner=c as CollapsibleContainerBase;
			}

			public override void Remove(Control value)
			{
				if((value is CollapsibleContainerPanel)&&!owner.DesignMode&&IsReadOnly)
					throw new NotSupportedException("Collection is read only.");

				base.Remove(value);
			}

			public override void SetChildIndex(Control child, int newIndex)
			{
				if(child is CollapsibleContainerPanel)
				{
					if(owner.DesignMode)
						return;

					if(IsReadOnly)
						throw new NotSupportedException("Collection is read only.");
				}
				base.SetChildIndex(child, newIndex);
			}
		}
		#endregion
	}
}
