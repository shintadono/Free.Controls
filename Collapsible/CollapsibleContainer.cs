using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free.Controls.Collapsible
{
	[DefaultProperty("Collapsed"), DefaultEvent("CollapsedChanged"), Designer(typeof(CollapsibleContainerDesigner))]
	[Description("CollapsibleContainer"), Docking(DockingBehavior.Never)]
	[ToolboxBitmap(typeof(CollapsibleContainer), "Free.Controls.CollapsibleContainer.bmp")]
	public partial class CollapsibleContainer : ContainerControl, ISupportInitialize
	{
		internal const int HeaderHeight=27;

		#region Fields
		private bool initializing;
		private bool collapsed;
		private CollapsibleContainerPanel panel;
		private int uncollapsedHeight=100;
		private CollapsibleContainerHeader headerType=CollapsibleContainerHeader.Label;

		private Label headerLabel=new Label();
		private TableLayoutPanel headerTableLayoutPanel=new TableLayoutPanel();
		private CheckBox headerCheckbox=new CheckBox();
		private Button headerImage=new Button();
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

		#region Events
		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public event EventHandler CheckedChanged
		{
			add
			{
				headerCheckbox.CheckedChanged+=value;
			}
			remove
			{
				headerCheckbox.CheckedChanged-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public event EventHandler CollapsedChanged;
		#endregion

		#region Eigenschaften
		[Localizable(false), DefaultValue(CollapsibleContainerHeader.Label), Category("Appearance")]
		public CollapsibleContainerHeader HeaderType
		{
			get
			{
				return headerType;
			}
			set
			{
				headerType=value;

				if(headerType==CollapsibleContainerHeader.Label)
				{
					headerCheckbox.Visible=false;
					headerLabel.Visible=true;
					panel.Enabled=true;
				}
				else if(headerType==CollapsibleContainerHeader.Checkbox)
				{
					headerCheckbox.Visible=true;
					headerLabel.Visible=false;
					panel.Enabled=headerCheckbox.Checked;
				}
			}
		}

		[Localizable(true), DefaultValue("Header Text"), Category("Appearance")]
		public string HeaderText
		{
			get
			{
				return headerLabel.Text;
			}
			set
			{
				headerCheckbox.Text=value;
				headerLabel.Text=value;
			}
		}

		[DefaultValue(false), Category("Appearance")]
		public bool Checked
		{
			get
			{
				return headerCheckbox.Checked;
			}
			set
			{
				headerCheckbox.Checked=value;
			}
		}
		#endregion

		public CollapsibleContainer()
		{
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			panel=new CollapsibleContainerPanel();
			panel.TabIndex=2;
			((TypedControlCollection)Controls).AddInternal(panel);
			CollapseOrExpand(false);

			headerTableLayoutPanel.SuspendLayout();

			headerTableLayoutPanel.ColumnCount=2;
			headerTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			headerTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 26F));
			headerTableLayoutPanel.Controls.Add(headerLabel, 0, 0);
			headerTableLayoutPanel.Controls.Add(headerCheckbox, 0, 0);
			headerTableLayoutPanel.Controls.Add(headerImage, 1, 0);
			headerTableLayoutPanel.Dock=DockStyle.Top;
			headerTableLayoutPanel.Location=new Point(0, 0);
			headerTableLayoutPanel.Height=HeaderHeight;
			headerTableLayoutPanel.Name="headerTableLayoutPanel";
			headerTableLayoutPanel.RowCount=1;
			headerTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
			headerTableLayoutPanel.TabIndex=0;

			// Header Label
			headerLabel.Dock=DockStyle.Fill;
			headerLabel.Text="Header Text";
			headerLabel.AutoSize=false;
			headerLabel.TextAlign=ContentAlignment.MiddleLeft;
			headerLabel.TabIndex=0;
			headerLabel.Click+=delegate(object s, EventArgs e) { Collapsed=!Collapsed; };

			// Header Checkbox
			headerCheckbox.Dock=DockStyle.Fill;
			headerCheckbox.Text="Header Text";
			headerCheckbox.Name="headerCheckbox";
			headerCheckbox.Padding=new Padding(2, 2, 0, 0);
			headerCheckbox.TextAlign=ContentAlignment.BottomLeft;
			headerCheckbox.TabIndex=1;
			headerCheckbox.UseVisualStyleBackColor=true;
			headerCheckbox.CheckedChanged+=delegate { panel.Enabled=headerCheckbox.Checked; if(headerCheckbox.Checked) Collapsed=false; };
			headerCheckbox.Visible=false;

			// Image
			headerImage.Dock=DockStyle.Fill;
			headerImage.FlatAppearance.BorderSize=0;
			headerImage.FlatStyle=System.Windows.Forms.FlatStyle.Flat;
			headerImage.Image=Free.Controls.Properties.Resources.Collapse;
			headerImage.Name="headerImage";
			headerImage.ImageAlign=ContentAlignment.TopLeft;
			headerImage.Margin=new Padding(0);
			headerImage.TabIndex=0;
			headerImage.UseVisualStyleBackColor=true;
			headerImage.Click+=delegate(object s, EventArgs e) { Collapsed=!Collapsed; };

			headerTableLayoutPanel.ResumeLayout(false);

			((TypedControlCollection)Controls).AddInternal(headerTableLayoutPanel);
		}

		#region Methods
		public void BeginInit()
		{
			initializing=true;
		}

		public void EndInit()
		{
			initializing=false;
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new CollapsibleContainerTypedControlCollection(this, typeof(CollapsibleContainerPanel), true);
		}

		private void CollapseOrExpand(bool setHeight)
		{
			panel.SuspendLayout();

			panel.Location=new Point(0, HeaderHeight);
			panel.Width=Width;

			if(setHeight)
			{
				if(collapsed)
				{
					Height=HeaderHeight;
					headerImage.Image=Free.Controls.Properties.Resources.Expand;
				}
				else
				{
					Height=uncollapsedHeight;
					headerImage.Image=Free.Controls.Properties.Resources.Collapse;
				}
			}

			panel.Height=uncollapsedHeight-HeaderHeight;

			panel.ResumeLayout();
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
				return panel.BorderStyle;
			}
			set
			{
				panel.BorderStyle=value;
			}
		}

		[Category("Appearance"), Description("HeaderBorderStyle"), DefaultValue(BorderStyle.None)]
		public BorderStyle HeaderBorderStyle
		{
			get
			{
				return headerTableLayoutPanel.BorderStyle;
			}
			set
			{
				headerTableLayoutPanel.BorderStyle=value;
			}
		}

		[Category("Appearance"), Description("HeaderBackColor")]
		public Color HeaderBackColor
		{
			get
			{
				return headerTableLayoutPanel.BackColor;
			}
			set
			{
				headerTableLayoutPanel.BackColor=value;
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
					panel.Visible=!value;
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

		[Category("Appearance"), Description("Panel"), Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public CollapsibleContainerPanel Panel
		{
			get
			{
				return panel;
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

		#region Nested Types (Nested um an die protected CollapsibleContainer.DesignMode-Varable zu kommen)
		internal class CollapsibleContainerTypedControlCollection : TypedControlCollection
		{
			private CollapsibleContainer owner;

			public CollapsibleContainerTypedControlCollection(Control c, Type type, bool isReadOnly)
				: base(c, type, isReadOnly)
			{
				owner=c as CollapsibleContainer;
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
