using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Windows.Forms;

namespace Free.Controls.Splitter
{
	[DefaultEvent("SplitterMoved"), Designer(typeof(TSplitContainerDesigner)), ComVisible(true), ClassInterface(ClassInterfaceType.AutoDispatch), Description("TSplitContainer"), Docking(DockingBehavior.AutoDock)]
	public partial class TSplitContainer : ContainerControl, ISupportInitialize
	{
		#region Fields
		private Point anchor=Point.Empty;
		private int BORDERSIZE;
		private BorderStyle borderStyle;
		private bool callBaseVersion;
		private const int DRAW_END=3;
		private const int DRAW_MOVE=2;
		private const int DRAW_START=1;
		private static readonly object EVENT_MOVED=new object();
		private static readonly object EVENT_MOVING=new object();
		private FixedPanel fixedPanel;
		private bool initializing;
		private int initialSplitterDistance;
		private Rectangle initialSplitterRectangle;
		private int lastDrawSplit=1;
		private const int leftBorder=2;
		private int newPanel1MinSize=25;
		private int newPanel2MinSize=25;
		private int newSplitterWidth=4;
		private Control nextActiveControl;
		private Orientation orientation=Orientation.Horizontal;
		private Cursor overrideCursor;
		private TSplitterPanel panel1;
		private int panel1MinSize=25;
		private TSplitterPanel panel2;
		private int panel2MinSize=25;
		private int panelSize;
		private double ratioHeight;
		private double ratioWidth;
		private bool resizeCalled;
		private const int rightBorder=5;
		private bool selectNextControl;
		private bool setSplitterDistance;
		private bool splitBegin;
		private bool splitBreak;
		private TSplitContainerMessageFilter splitContainerMessageFilter;
		private bool splitContainerScaling;
		private int splitDistance=50;
		private bool splitMove;
		private bool splitterClick;
		private int splitterDistance=50;
		private bool splitterDrag;
		private bool splitterFixed;
		private bool splitterFocused;
		private int splitterInc=1;
		private Rectangle splitterRect;
		private int splitterWidth=4;
		private bool tabStop=true;
		#endregion

		#region Events
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

		[Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
		public new event EventHandler BackgroundImageChanged
		{
			add
			{
				base.BackgroundImageChanged+=value;
			}
			remove
			{
				base.BackgroundImageChanged-=value;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
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

		[Category("Behavior"), Description("SplitterSplitterMovedDescr")]
		public event SplitterEventHandler SplitterMoved
		{
			add
			{
				base.Events.AddHandler(EVENT_MOVED, value);
			}
			remove
			{
				base.Events.RemoveHandler(EVENT_MOVED, value);
			}
		}

		[Category("Behavior"), Description("SplitterSplitterMovingDescr")]
		public event SplitterCancelEventHandler SplitterMoving
		{
			add
			{
				base.Events.AddHandler(EVENT_MOVING, value);
			}
			remove
			{
				base.Events.RemoveHandler(EVENT_MOVING, value);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public new event EventHandler TextChanged
		{
			add
			{
				base.TextChanged+=value;
			}
			remove
			{
				base.TextChanged-=value;
			}
		}
		#endregion

		public TSplitContainer()
		{
			this.panel1=new TSplitterPanel(this);
			this.panel2=new TSplitterPanel(this);
			this.splitterRect=new Rectangle();
			base.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
			base.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			((TypedControlCollection)this.Controls).AddInternal(this.panel1);
			((TypedControlCollection)this.Controls).AddInternal(this.panel2);
			this.UpdateSplitter();
		}

		#region Methods
		// Done that in TSplitterPanel.OnControlRemoved since override not possible.
		//internal override void AfterControlRemoved(Control control, Control oldParent)
		//{
		//    base.AfterControlRemoved(control, oldParent);
		//    if((control is TSplitContainer)&&(control.Dock==DockStyle.Fill))
		//    {
		//        this.SetInnerMostBorder(this);
		//    }
		//}

		private void ApplyPanel1MinSize(int value)
		{
			if(value<0)
			{
				throw new ArgumentOutOfRangeException("Panel1MinSize", "Invalid low bound argument. Panel1MinSize must not be lower than 0.");
			}
			if(this.Orientation==Orientation.Vertical)
			{
				if((base.DesignMode&&(base.Width!=this.DefaultSize.Width))&&(((value+this.Panel2MinSize)+this.SplitterWidth)>base.Width))
				{
					throw new ArgumentOutOfRangeException("Panel1MinSize", "Invalid argument in Panel1MinSize.");
				}
			}
			else if(((this.Orientation==Orientation.Horizontal)&&base.DesignMode)&&((base.Height!=this.DefaultSize.Height)&&(((value+this.Panel2MinSize)+this.SplitterWidth)>base.Height)))
			{
				throw new ArgumentOutOfRangeException("Panel1MinSize", "Invalid argument in Panel1MinSize.");
			}
			this.panel1MinSize=value;
			if(value>this.SplitterDistanceInternal)
			{
				this.SplitterDistanceInternal=value;
			}
		}

		private void ApplyPanel2MinSize(int value)
		{
			if(value<0)
			{
				throw new ArgumentOutOfRangeException("Panel2MinSize", "Invalid low bound argument. Panel2MinSize must not be lower than 0.");
			}
			if(this.Orientation==Orientation.Vertical)
			{
				if((base.DesignMode&&(base.Width!=this.DefaultSize.Width))&&(((value+this.Panel1MinSize)+this.SplitterWidth)>base.Width))
				{
					throw new ArgumentOutOfRangeException("Panel2MinSize", "Invalid argument in Panel2MinSize.");
				}
			}
			else if(((this.Orientation==Orientation.Horizontal)&&base.DesignMode)&&((base.Height!=this.DefaultSize.Height)&&(((value+this.Panel1MinSize)+this.SplitterWidth)>base.Height)))
			{
				throw new ArgumentOutOfRangeException("Panel2MinSize", "Invalid argument in Panel2MinSize.");
			}
			this.panel2MinSize=value;
			if(value>this.Panel2.Width)
			{
				this.SplitterDistanceInternal=this.Panel2.Width+this.SplitterWidthInternal;
			}
		}

		private void ApplySplitterDistance()
		{
			this.SuspendLayout();
			this.SplitterDistanceInternal=this.splitterDistance;
			this.ResumeLayout(false);

			if(this.BackColor==Color.Transparent)
			{
				base.Invalidate();
			}
			if(this.Orientation==Orientation.Vertical)
			{
				if(this.RightToLeft==RightToLeft.No)
				{
					this.splitterRect.X=base.Location.X+this.SplitterDistanceInternal;
				}
				else
				{
					this.splitterRect.X=(base.Right-this.SplitterDistanceInternal)-this.SplitterWidthInternal;
				}
			}
			else
			{
				this.splitterRect.Y=base.Location.Y+this.SplitterDistanceInternal;
			}
		}

		private void ApplySplitterWidth(int value)
		{
			if(value<1)
			{
				throw new ArgumentOutOfRangeException("SplitterWidth", "Invalid low bound argument. SplitterWidth must not be lower than 1.");
			}
			if(this.Orientation==Orientation.Vertical)
			{
				if(base.DesignMode&&(((value+this.Panel1MinSize)+this.Panel2MinSize)>base.Width))
				{
					throw new ArgumentOutOfRangeException("SplitterWidth", "Invalid argument in SplitterWidth.");
				}
			}
			else if(((this.Orientation==Orientation.Horizontal)&&base.DesignMode)&&(((value+this.Panel1MinSize)+this.Panel2MinSize)>base.Height))
			{
				throw new ArgumentOutOfRangeException("SplitterWidth", "Invalid argument in SplitterWidth.");
			}
			this.splitterWidth=value;
			this.UpdateSplitter();
		}

		public void BeginInit()
		{
			this.initializing=true;
		}

		private Rectangle CalcSplitLine(int splitSize, int minWeight)
		{
			Rectangle rectangle=new Rectangle();
			switch(this.Orientation)
			{
				case Orientation.Horizontal:
					rectangle.Width=base.Width;
					rectangle.Height=this.SplitterWidthInternal;
					if(rectangle.Width<minWeight)
					{
						rectangle.Width=minWeight;
					}
					rectangle.Y=this.panel1.Location.Y+splitSize;
					return rectangle;

				case Orientation.Vertical:
					rectangle.Width=this.SplitterWidthInternal;
					rectangle.Height=base.Height;
					if(rectangle.Width<minWeight)
					{
						rectangle.Width=minWeight;
					}
					if(this.RightToLeft==RightToLeft.No)
					{
						rectangle.X=this.panel1.Location.X+splitSize;
						return rectangle;
					}
					rectangle.X=(base.Width-splitSize)-this.SplitterWidthInternal;
					return rectangle;
			}
			return rectangle;
		}

		private void CollapsePanel(TSplitterPanel p, bool collapsing)
		{
			p.Collapsed=collapsing;
			if(collapsing)
			{
				p.Visible=false;
			}
			else
			{
				p.Visible=true;
			}
			this.UpdateSplitter();
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override Control.ControlCollection CreateControlsInstance()
		{
			return new TSplitContainerTypedControlCollection(this, typeof(TSplitterPanel), true);
		}

		private void DrawFocus(Graphics g, Rectangle r)
		{
			r.Inflate(-1, -1);
			ControlPaint.DrawFocusRectangle(g, r, this.ForeColor, this.BackColor);
		}

		private void DrawSplitBar(int mode)
		{
			if((mode!=1)&&(this.lastDrawSplit!=-1))
			{
				this.DrawSplitHelper(this.lastDrawSplit);
				this.lastDrawSplit=-1;
			}
			else if((mode!=1)&&(this.lastDrawSplit==-1))
			{
				return;
			}
			if(mode!=3)
			{
				if(this.splitMove||this.splitBegin)
				{
					this.DrawSplitHelper(this.splitterDistance);
					this.lastDrawSplit=this.splitterDistance;
				}
				else
				{
					this.DrawSplitHelper(this.splitterDistance);
					this.lastDrawSplit=this.splitterDistance;
				}
			}
			else
			{
				if(this.lastDrawSplit!=-1)
				{
					this.DrawSplitHelper(this.lastDrawSplit);
				}
				this.lastDrawSplit=-1;
			}
		}

		[StructLayout(LayoutKind.Sequential)]
		public class LOGBRUSH
		{
			public int lbStyle;
			public int lbColor;
			public IntPtr lbHatch;
		}

		[DllImport("gdi32.dll", EntryPoint="CreateBitmap", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
		private static extern IntPtr CreateBitmap(int nWidth, int nHeight, int nPlanes, int nBitsPerPixel, short[] lpvBits);

		[DllImport("gdi32.dll", EntryPoint="CreateBrushIndirect", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
		private static extern IntPtr CreateBrushIndirect(LOGBRUSH lb);

		[DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
		public static extern IntPtr SelectObject(HandleRef hDC, HandleRef hObject);

		[DllImport("gdi32.dll", EntryPoint="DeleteObject", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
		internal static extern bool DeleteObject(HandleRef hObject);

		[DllImport("gdi32.dll", CharSet=CharSet.Auto, SetLastError=true, ExactSpelling=true)]
		public static extern bool PatBlt(HandleRef hdc, int left, int top, int width, int height, int rop);

		const int DCX_CACHE=0x2;
		const int DCX_LOCKWINDOWUPDATE=0x400;

		[DllImport("user32.dll", EntryPoint="GetDCEx", CharSet=CharSet.Auto, ExactSpelling=true)]
		private static extern IntPtr GetDCEx(HandleRef hWnd, HandleRef hrgnClip, int flags);

		[DllImport("user32.dll", EntryPoint="ReleaseDC", CharSet=CharSet.Auto, ExactSpelling=true)]
		private static extern int ReleaseDC(HandleRef hWnd, HandleRef hDC);

		internal static IntPtr CreateHalftoneHBRUSH()
		{
			short[] lpvBits=new short[8];
			for(int i=0; i<8; i++)
			{
				lpvBits[i]=(short)(((int)0x5555)<<(i&1));
			}
			IntPtr handle=CreateBitmap(8, 8, 1, 1, lpvBits);
			LOGBRUSH lb=new LOGBRUSH();
			lb.lbColor=ColorTranslator.ToWin32(Color.Black);
			lb.lbStyle=3;
			lb.lbHatch=handle;
			IntPtr ptr2=CreateBrushIndirect(lb);
			DeleteObject(new HandleRef(null, handle));
			return ptr2;
		}

		private void DrawSplitHelper(int splitSize)
		{
			// Oben die störische Version ohne Maus-Tracking, unten mit Maus-Tracking aber Native.
			/*
			Rectangle rectangle=this.CalcSplitLine(splitSize, 3);
			using(Graphics g=this.CreateGraphics())
			{
				Brush hatch=new HatchBrush(HatchStyle.Percent50, Color.Gray);
				g.FillRectangle(hatch, rectangle);
			}
			/*/
			Rectangle rectangle=this.CalcSplitLine(splitSize, 3);
			IntPtr handle=base.Handle;
			IntPtr ptr2=GetDCEx(new HandleRef(this, handle), new HandleRef(null, IntPtr.Zero), DCX_CACHE|DCX_LOCKWINDOWUPDATE);
			IntPtr ptr3=CreateHalftoneHBRUSH();
			IntPtr ptr4=SelectObject(new HandleRef(this, ptr2), new HandleRef(null, ptr3));
			PatBlt(new HandleRef(this, ptr2), rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, 0x5a0049);
			SelectObject(new HandleRef(this, ptr2), new HandleRef(null, ptr4));
			DeleteObject(new HandleRef(null, ptr3));
			ReleaseDC(new HandleRef(this, handle), new HandleRef(null, ptr2));
			/**/
		}

		public void EndInit()
		{
			this.initializing=false;
			if(this.newPanel1MinSize!=this.panel1MinSize)
			{
				this.ApplyPanel1MinSize(this.newPanel1MinSize);
			}
			if(this.newPanel2MinSize!=this.panel2MinSize)
			{
				this.ApplyPanel2MinSize(this.newPanel2MinSize);
			}
			if(this.newSplitterWidth!=this.splitterWidth)
			{
				this.ApplySplitterWidth(this.newSplitterWidth);
			}
		}

		private int GetSplitterDistance(int x, int y)
		{
			int num;
			if(this.Orientation==Orientation.Vertical)
			{
				num=x-this.anchor.X;
			}
			else
			{
				num=y-this.anchor.Y;
			}
			int num2=0;
			switch(this.Orientation)
			{
				case Orientation.Horizontal:
					num2=Math.Max(this.panel1.Height+num, this.BORDERSIZE);
					break;

				case Orientation.Vertical:
					if(this.RightToLeft!=RightToLeft.No)
					{
						num2=Math.Max(this.panel1.Width-num, this.BORDERSIZE);
						break;
					}
					num2=Math.Max(this.panel1.Width+num, this.BORDERSIZE);
					break;
			}
			if(this.Orientation==Orientation.Vertical)
			{
				return Math.Max(Math.Min(num2, base.Width-this.Panel2MinSize), this.Panel1MinSize);
			}
			return Math.Max(Math.Min(num2, base.Height-this.Panel2MinSize), this.Panel1MinSize);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			base.Invalidate();
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);
			if(this.IsSplitterMovable&&!this.IsSplitterFixed)
			{
				if((e.KeyData==Keys.Escape)&&this.splitBegin)
				{
					this.splitBegin=false;
					this.splitBreak=true;
				}
				else if(((e.KeyData==Keys.Right)||(e.KeyData==Keys.Down))||((e.KeyData==Keys.Left)||((e.KeyData==Keys.Up)&&this.splitterFocused)))
				{
					if(this.splitBegin)
					{
						this.splitMove=true;
					}
					if((e.KeyData==Keys.Left)||((e.KeyData==Keys.Up)&&this.splitterFocused))
					{
						this.splitterDistance-=this.SplitterIncrement;
						this.splitterDistance=(this.splitterDistance<this.Panel1MinSize)?(this.splitterDistance+this.SplitterIncrement):Math.Max(this.splitterDistance, this.BORDERSIZE);
					}
					if((e.KeyData==Keys.Right)||((e.KeyData==Keys.Down)&&this.splitterFocused))
					{
						this.splitterDistance+=this.SplitterIncrement;
						if(this.Orientation==Orientation.Vertical)
						{
							this.splitterDistance=((this.splitterDistance+this.SplitterWidth)>((base.Width-this.Panel2MinSize)-this.BORDERSIZE))?(this.splitterDistance-this.SplitterIncrement):this.splitterDistance;
						}
						else
						{
							this.splitterDistance=((this.splitterDistance+this.SplitterWidth)>((base.Height-this.Panel2MinSize)-this.BORDERSIZE))?(this.splitterDistance-this.SplitterIncrement):this.splitterDistance;
						}
					}
					if(!this.splitBegin)
					{
						this.splitBegin=true;
					}
					if(this.splitBegin&&!this.splitMove)
					{
						this.initialSplitterDistance=this.SplitterDistanceInternal;
						this.DrawSplitBar(1);
					}
					else
					{
						this.DrawSplitBar(2);
						Rectangle rectangle=this.CalcSplitLine(this.splitterDistance, 0);
						int x=rectangle.X;
						int y=rectangle.Y;
						SplitterCancelEventArgs args=new SplitterCancelEventArgs((base.Left+this.SplitterRectangle.X)+(this.SplitterRectangle.Width/2), (base.Top+this.SplitterRectangle.Y)+(this.SplitterRectangle.Height/2), x, y);
						this.OnSplitterMoving(args);
						if(args.Cancel)
						{
							this.SplitEnd(false);
						}
					}
				}
			}
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);
			if((this.splitBegin&&this.IsSplitterMovable)&&(((e.KeyData==Keys.Right)||(e.KeyData==Keys.Down))||((e.KeyData==Keys.Left)||((e.KeyData==Keys.Up)&&this.splitterFocused))))
			{
				this.DrawSplitBar(3);
				this.ApplySplitterDistance();
				this.splitBegin=false;
				this.splitMove=false;
			}
			if(this.splitBreak)
			{
				this.splitBreak=false;
				this.SplitEnd(false);
			}
			using(Graphics graphics=base.CreateGraphics())
			{
				if(this.BackgroundImage==null)
				{
					using(SolidBrush brush=new SolidBrush(this.BackColor))
					{
						graphics.FillRectangle(brush, this.SplitterRectangle);
					}
				}
				this.DrawFocus(graphics, this.SplitterRectangle);
			}
		}

		protected override void OnLayout(LayoutEventArgs e)
		{
			this.SetInnerMostBorder(this);
			if(this.IsSplitterMovable&&!this.setSplitterDistance)
			{
				this.ResizeSplitContainer();
			}
			base.OnLayout(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			base.Invalidate();
		}

		protected override void OnMouseCaptureChanged(EventArgs e)
		{
			base.OnMouseCaptureChanged(e);
			if(this.splitContainerMessageFilter!=null)
			{
				Application.RemoveMessageFilter(this.splitContainerMessageFilter);
				this.splitContainerMessageFilter=null;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if(((this.IsSplitterMovable&&this.SplitterRectangle.Contains(e.Location))&&base.Enabled)&&(((e.Button==MouseButtons.Left)&&(e.Clicks==1))&&!this.IsSplitterFixed))
			{
				this.splitterFocused=true;
				IContainerControl containerControlInternal=this.Parent.GetContainerControl();
				if(containerControlInternal!=null)
				{
					ContainerControl control2=containerControlInternal as ContainerControl;
					if(control2==null)
					{
						containerControlInternal.ActiveControl=this;
					}
					else
					{
						control2.ActiveControl=this;
					}
				}
				base.ActiveControl=null;
				this.nextActiveControl=this.panel2;
				this.SplitBegin(e.X, e.Y);
				this.splitterClick=true;
			}
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);
			if(base.Enabled)
			{
				this.OverrideCursor=null;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			if(!this.IsSplitterFixed&&this.IsSplitterMovable)
			{
				if((this.Cursor==this.DefaultCursor)&&this.SplitterRectangle.Contains(e.Location))
				{
					if(this.Orientation==Orientation.Vertical)
					{
						this.OverrideCursor=Cursors.VSplit;
					}
					else
					{
						this.OverrideCursor=Cursors.HSplit;
					}
				}
				else
				{
					this.OverrideCursor=null;
				}
				if(this.splitterClick)
				{
					int x=e.X;
					int y=e.Y;
					this.splitterDrag=true;
					this.SplitMove(x, y);
					if(this.Orientation==Orientation.Vertical)
					{
						x=Math.Max(Math.Min(x, base.Width-this.Panel2MinSize), this.Panel1MinSize);
						y=Math.Max(y, 0);
					}
					else
					{
						y=Math.Max(Math.Min(y, base.Height-this.Panel2MinSize), this.Panel1MinSize);
						x=Math.Max(x, 0);
					}
					Rectangle rectangle=this.CalcSplitLine(this.GetSplitterDistance(e.X, e.Y), 0);
					int splitX=rectangle.X;
					int splitY=rectangle.Y;
					SplitterCancelEventArgs args=new SplitterCancelEventArgs(x, y, splitX, splitY);
					this.OnSplitterMoving(args);
					if(args.Cancel)
					{
						this.SplitEnd(false);
					}
				}
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			if(base.Enabled&&((!this.IsSplitterFixed&&this.IsSplitterMovable)&&this.splitterClick))
			{
				base.Capture=false;
				if(this.splitterDrag)
				{
					this.CalcSplitLine(this.GetSplitterDistance(e.X, e.Y), 0);
					this.SplitEnd(true);
				}
				else
				{
					this.SplitEnd(false);
				}
				this.splitterClick=false;
				this.splitterDrag=false;
			}
		}

		protected override void OnMove(EventArgs e)
		{
			base.OnMove(e);
			this.SetSplitterRect(this.Orientation==Orientation.Vertical);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if(this.Focused)
			{
				this.DrawFocus(e.Graphics, this.SplitterRectangle);
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void OnRightToLeftChanged(EventArgs e)
		{
			base.OnRightToLeftChanged(e);
			this.panel1.RightToLeft=this.RightToLeft;
			this.panel2.RightToLeft=this.RightToLeft;
			this.UpdateSplitter();
		}

		public void OnSplitterMoved(SplitterEventArgs e)
		{
			SplitterEventHandler handler=(SplitterEventHandler)base.Events[EVENT_MOVED];
			if(handler!=null)
			{
				handler(this, e);
			}
		}

		public void OnSplitterMoving(SplitterCancelEventArgs e)
		{
			SplitterCancelEventHandler handler=(SplitterCancelEventHandler)base.Events[EVENT_MOVING];
			if(handler!=null)
			{
				handler(this, e);
			}
		}

		private bool ProcessArrowKey(bool forward)
		{
			Control parentInternal=this;
			if(base.ActiveControl!=null)
			{
				parentInternal=base.ActiveControl.Parent;
			}
			return parentInternal.SelectNextControl(base.ActiveControl, forward, false, false, true);
		}

		[UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if((keyData&(Keys.Alt|Keys.Control))==Keys.None)
			{
				Keys keys=keyData&Keys.KeyCode;
				switch(keys)
				{
					case Keys.Left:
					case Keys.Up:
					case Keys.Right:
					case Keys.Down:
						if(this.splitterFocused)
						{
							return false;
						}
						if(!this.ProcessArrowKey((keys==Keys.Right)||(keys==Keys.Down)))
						{
							break;
						}
						return true;

					case Keys.Tab:
						if(this.ProcessTabKey((keyData&Keys.Shift)==Keys.None))
						{
							return true;
						}
						break;
				}
			}
			return base.ProcessDialogKey(keyData);
		}

		[UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
		protected override bool ProcessTabKey(bool forward)
		{
			if(!this.TabStop||this.IsSplitterFixed)
			{
				return base.ProcessTabKey(forward);
			}
			if(this.nextActiveControl!=null)
			{
				base.ActiveControl=this.nextActiveControl;
				this.nextActiveControl=null;
			}
			if(this.SelectNextControlInPanel(base.ActiveControl, forward, true, true, true))
			{
				this.nextActiveControl=null;
				this.splitterFocused=false;
				return true;
			}
			if(this.callBaseVersion)
			{
				this.callBaseVersion=false;
				return base.ProcessTabKey(forward);
			}
			this.splitterFocused=true;
			IContainerControl containerControlInternal=this.Parent.GetContainerControl();
			if(containerControlInternal!=null)
			{
				ContainerControl control2=containerControlInternal as ContainerControl;
				if(control2==null)
				{
					containerControlInternal.ActiveControl=this;
				}
				else
				{
					control2.ActiveControl=this;
				}
			}
			base.ActiveControl=null;
			return true;
		}

		private void RepaintSplitterRect()
		{
			if(!base.IsHandleCreated)
			{
				return;
			}
			Graphics graphics=base.CreateGraphics();
			if(this.BackgroundImage!=null)
			{
				using(TextureBrush brush=new TextureBrush(this.BackgroundImage, WrapMode.Tile))
				{
					graphics.FillRectangle(brush, base.ClientRectangle);
					goto Label_0062;
				}
			}
			using(SolidBrush brush2=new SolidBrush(this.BackColor))
			{
				graphics.FillRectangle(brush2, this.splitterRect);
			}
Label_0062:
			graphics.Dispose();
		}

		private void ResizeSplitContainer()
		{
			if(!this.splitContainerScaling)
			{
				this.panel1.SuspendLayout();
				this.panel2.SuspendLayout();
				if(base.Width==0)
				{
					this.panel1.Size=new Size(0, this.panel1.Height);
					this.panel2.Size=new Size(0, this.panel2.Height);
				}
				else if(base.Height==0)
				{
					this.panel1.Size=new Size(this.panel1.Width, 0);
					this.panel2.Size=new Size(this.panel2.Width, 0);
				}
				else
				{
					if(this.Orientation==Orientation.Vertical)
					{
						if(!this.CollapsedMode)
						{
							if(this.FixedPanel==FixedPanel.Panel1)
							{
								this.panel1.Size=new Size(this.panelSize, base.Height);
								this.panel2.Size=new Size(Math.Max((base.Width-this.panelSize)-this.SplitterWidthInternal, this.Panel2MinSize), base.Height);
							}
							if(this.FixedPanel==FixedPanel.Panel2)
							{
								this.panel2.Size=new Size(this.panelSize, base.Height);
								this.splitterDistance=Math.Max((base.Width-this.panelSize)-this.SplitterWidthInternal, this.Panel1MinSize);
								this.panel1.WidthInternal=this.splitterDistance;
								this.panel1.HeightInternal=base.Height;
							}
							if(this.FixedPanel==FixedPanel.None)
							{
								if(this.ratioWidth!=0.0)
								{
									this.splitterDistance=Math.Max((int)Math.Floor((double)(((double)base.Width)/this.ratioWidth)), this.Panel1MinSize);
								}
								this.panel1.WidthInternal=this.splitterDistance;
								this.panel1.HeightInternal=base.Height;
								this.panel2.Size=new Size(Math.Max((base.Width-this.splitterDistance)-this.SplitterWidthInternal, this.Panel2MinSize), base.Height);
							}
							if(this.RightToLeft==RightToLeft.No)
							{
								this.panel2.Location=new Point(this.panel1.WidthInternal+this.SplitterWidthInternal, 0);
							}
							else
							{
								this.panel1.Location=new Point(base.Width-this.panel1.WidthInternal, 0);
							}
							this.RepaintSplitterRect();
							this.SetSplitterRect(true);
						}
						else if(this.Panel1Collapsed)
						{
							this.panel2.Size=base.Size;
							this.panel2.Location=new Point(0, 0);
						}
						else if(this.Panel2Collapsed)
						{
							this.panel1.Size=base.Size;
							this.panel1.Location=new Point(0, 0);
						}
					}
					else if(this.Orientation==Orientation.Horizontal)
					{
						if(!this.CollapsedMode)
						{
							if(this.FixedPanel==FixedPanel.Panel1)
							{
								this.panel1.Size=new Size(base.Width, this.panelSize);
								int y=this.panelSize+this.SplitterWidthInternal;
								this.panel2.Size=new Size(base.Width, Math.Max(base.Height-y, this.Panel2MinSize));
								this.panel2.Location=new Point(0, y);
							}
							if(this.FixedPanel==FixedPanel.Panel2)
							{
								this.panel2.Size=new Size(base.Width, this.panelSize);
								this.splitterDistance=Math.Max((base.Height-this.Panel2.Height)-this.SplitterWidthInternal, this.Panel1MinSize);
								this.panel1.HeightInternal=this.splitterDistance;
								this.panel1.WidthInternal=base.Width;
								int num2=this.splitterDistance+this.SplitterWidthInternal;
								this.panel2.Location=new Point(0, num2);
							}
							if(this.FixedPanel==FixedPanel.None)
							{
								if(this.ratioHeight!=0.0)
								{
									this.splitterDistance=Math.Max((int)Math.Floor((double)(((double)base.Height)/this.ratioHeight)), this.Panel1MinSize);
								}
								this.panel1.HeightInternal=this.splitterDistance;
								this.panel1.WidthInternal=base.Width;
								int num3=this.splitterDistance+this.SplitterWidthInternal;
								this.panel2.Size=new Size(base.Width, Math.Max(base.Height-num3, this.Panel2MinSize));
								this.panel2.Location=new Point(0, num3);
							}
							this.RepaintSplitterRect();
							this.SetSplitterRect(false);
						}
						else if(this.Panel1Collapsed)
						{
							this.panel2.Size=base.Size;
							this.panel2.Location=new Point(0, 0);
						}
						else if(this.Panel2Collapsed)
						{
							this.panel1.Size=base.Size;
							this.panel1.Location=new Point(0, 0);
						}
					}
					try
					{
						this.resizeCalled=true;
						this.ApplySplitterDistance();
					}
					finally
					{
						this.resizeCalled=false;
					}
				}
				this.panel1.ResumeLayout();
				this.panel2.ResumeLayout();
			}
		}

		[EditorBrowsable(EditorBrowsableState.Advanced)]
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
		{
			try
			{
				float width;
				this.splitContainerScaling=true;
				base.ScaleControl(factor, specified);
				if(this.orientation==Orientation.Vertical)
				{
					width=factor.Width;
				}
				else
				{
					width=factor.Height;
				}
				this.SplitterWidth=(int)Math.Round((double)(this.SplitterWidth*width));
			}
			finally
			{
				this.splitContainerScaling=false;
			}
		}

		protected override void Select(bool directed, bool forward)
		{
			if(!this.selectNextControl) // re-entrant by parentInternal.SelectNextControl (see below)
			{
				if(((this.Panel1.Controls.Count>0)||(this.Panel2.Controls.Count>0))||this.TabStop)
				{
					this.SelectNextControlInContainer(this, forward, true, true, false);
				}
				else
				{
					try
					{
						Control parentInternal=this.Parent;
						this.selectNextControl=true;
						while(parentInternal!=null)
						{
							if(parentInternal.SelectNextControl(this, forward, true, true, parentInternal.Parent==null))
							{
								return;
							}
							parentInternal=parentInternal.Parent;
						}
					}
					finally
					{
						this.selectNextControl=false;
					}
				}
			}
		}

		private static void SelectNextActiveControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
		{
			ContainerControl control=ctl as ContainerControl;
			if(control!=null)
			{
				bool flag=true;
				if(control.Parent!=null)
				{
					IContainerControl containerControlInternal=control.Parent.GetContainerControl();
					if(containerControlInternal!=null)
					{
						containerControlInternal.ActiveControl=control;
						flag=containerControlInternal.ActiveControl==control;
					}
				}
				if(flag)
				{
					ctl.SelectNextControl(null, forward, tabStopOnly, nested, wrap);
				}
			}
			else
			{
				ctl.Select();
			}
		}

		private bool SelectNextControlInContainer(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
		{
			if(!base.Contains(ctl)||(!nested&&(ctl.Parent!=this)))
			{
				ctl=null;
			}
			TSplitterPanel panel=null;
			do
			{
				ctl=base.GetNextControl(ctl, forward);
				TSplitterPanel panel2=ctl as TSplitterPanel;
				if((panel2!=null)&&panel2.Visible)
				{
					if(panel!=null)
					{
						break;
					}
					panel=panel2;
				}
				if((!forward&&(panel!=null))&&(ctl.Parent!=panel))
				{
					ctl=panel;
					break;
				}
				if(ctl==null)
				{
					break;
				}
				if(ctl.CanSelect&&ctl.TabStop)
				{
					if(ctl is TSplitContainer)
					{
						((TSplitContainer)ctl).Select(forward, forward);
					}
					else
					{
						SelectNextActiveControl(ctl, forward, tabStopOnly, nested, wrap);
					}
					return true;
				}
			}
			while(ctl!=null);
			if((ctl!=null)&&this.TabStop)
			{
				this.splitterFocused=true;
				IContainerControl containerControlInternal=this.Parent.GetContainerControl();
				if(containerControlInternal!=null)
				{
					ContainerControl control2=containerControlInternal as ContainerControl;
					if(control2==null)
					{
						containerControlInternal.ActiveControl=this;
					}
					else
					{
						control2.ActiveControl=this;
					}
				}
				base.ActiveControl=null;
				this.nextActiveControl=ctl;
				return true;
			}
			if(!this.SelectNextControlInPanel(ctl, forward, tabStopOnly, nested, wrap))
			{
				Control parentInternal=this.Parent;
				if(parentInternal!=null)
				{
					try
					{
						this.selectNextControl=true;
						parentInternal.SelectNextControl(this, forward, true, true, true);
					}
					finally
					{
						this.selectNextControl=false;
					}
				}
			}
			return false;
		}

		private bool SelectNextControlInPanel(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
		{
			if(!base.Contains(ctl)||(!nested&&(ctl.Parent!=this)))
			{
				ctl=null;
			}
			do
			{
				ctl=base.GetNextControl(ctl, forward);
				if((ctl==null)||((ctl is TSplitterPanel)&&ctl.Visible))
				{
					break;
				}
				if(ctl.CanSelect&&(!tabStopOnly||ctl.TabStop))
				{
					if(ctl is TSplitContainer)
					{
						((TSplitContainer)ctl).Select(forward, forward);
					}
					else
					{
						SelectNextActiveControl(ctl, forward, tabStopOnly, nested, wrap);
					}
					return true;
				}
			}
			while(ctl!=null);
			if((ctl==null)||((ctl is TSplitterPanel)&&!ctl.Visible))
			{
				this.callBaseVersion=true;
			}
			else
			{
				ctl=base.GetNextControl(ctl, forward);
				if(forward)
				{
					this.nextActiveControl=this.panel2;
				}
				else if((ctl==null)||!ctl.Parent.Visible)
				{
					this.callBaseVersion=true;
				}
				else
				{
					this.nextActiveControl=this.panel2;
				}
			}
			return false;
		}

		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
		{
			if((((specified&BoundsSpecified.Height)!=BoundsSpecified.None)&&(this.Orientation==Orientation.Horizontal))&&(height<((this.Panel1MinSize+this.SplitterWidthInternal)+this.Panel2MinSize)))
			{
				height=(this.Panel1MinSize+this.SplitterWidthInternal)+this.Panel2MinSize;
			}
			if((((specified&BoundsSpecified.Width)!=BoundsSpecified.None)&&(this.Orientation==Orientation.Vertical))&&(width<((this.Panel1MinSize+this.SplitterWidthInternal)+this.Panel2MinSize)))
			{
				width=(this.Panel1MinSize+this.SplitterWidthInternal)+this.Panel2MinSize;
			}
			base.SetBoundsCore(x, y, width, height, specified);
			this.SetSplitterRect(this.Orientation==Orientation.Vertical);
		}

		internal void SetInnerMostBorder(TSplitContainer sc)
		{
			foreach(Control control in sc.Controls)
			{
				bool flag=false;
				if(control is TSplitterPanel)
				{
					foreach(Control control2 in control.Controls)
					{
						TSplitContainer container=control2 as TSplitContainer;
						if((container!=null)&&(container.Dock==DockStyle.Fill))
						{
							if(container.BorderStyle!=this.BorderStyle)
							{
								break;
							}
							((TSplitterPanel)control).BorderStyle=BorderStyle.None;
							this.SetInnerMostBorder(container);
							flag=true;
						}
					}
					if(!flag)
					{
						((TSplitterPanel)control).BorderStyle=this.BorderStyle;
					}
				}
			}
		}

		private void SetSplitterRect(bool vertical)
		{
			if(vertical)
			{
				this.splitterRect.X=(this.RightToLeft==RightToLeft.Yes)?((base.Width-this.splitterDistance)-this.SplitterWidthInternal):(base.Location.X+this.splitterDistance);
				this.splitterRect.Y=base.Location.Y;
				this.splitterRect.Width=this.SplitterWidthInternal;
				this.splitterRect.Height=base.Height;
			}
			else
			{
				this.splitterRect.X=base.Location.X;
				this.splitterRect.Y=base.Location.Y+this.SplitterDistanceInternal;
				this.splitterRect.Width=base.Width;
				this.splitterRect.Height=this.SplitterWidthInternal;
			}
		}

		private void SplitBegin(int x, int y)
		{
			this.anchor=new Point(x, y);
			this.splitterDistance=this.GetSplitterDistance(x, y);
			this.initialSplitterDistance=this.splitterDistance;
			this.initialSplitterRectangle=this.SplitterRectangle;
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
			try
			{
				if(this.splitContainerMessageFilter==null)
				{
					this.splitContainerMessageFilter=new TSplitContainerMessageFilter(this);
				}
				Application.AddMessageFilter(this.splitContainerMessageFilter);
			}
			finally
			{
				CodeAccessPermission.RevertAssert();
			}
			base.Capture=true;
			this.DrawSplitBar(1);
		}

		private void SplitEnd(bool accept)
		{
			this.DrawSplitBar(3);
			if(this.splitContainerMessageFilter!=null)
			{
				Application.RemoveMessageFilter(this.splitContainerMessageFilter);
				this.splitContainerMessageFilter=null;
			}
			if(accept)
			{
				this.ApplySplitterDistance();
			}
			else if(this.splitterDistance!=this.initialSplitterDistance)
			{
				this.splitterClick=false;
				this.splitterDistance=this.SplitterDistanceInternal=this.initialSplitterDistance;
			}
			this.anchor=Point.Empty;
		}

		private void SplitMove(int x, int y)
		{
			int splitterDistance=this.GetSplitterDistance(x, y);
			int num2=splitterDistance-this.initialSplitterDistance;
			int num3=num2%this.SplitterIncrement;
			if(this.splitterDistance!=splitterDistance)
			{
				if(this.Orientation==Orientation.Vertical)
				{
					if((splitterDistance+this.SplitterWidthInternal)<=((base.Width-this.Panel2MinSize)-this.BORDERSIZE))
					{
						this.splitterDistance=splitterDistance-num3;
					}
				}
				else if((splitterDistance+this.SplitterWidthInternal)<=((base.Height-this.Panel2MinSize)-this.BORDERSIZE))
				{
					this.splitterDistance=splitterDistance-num3;
				}
			}
			this.DrawSplitBar(2);
		}

		private void UpdateSplitter()
		{
			if(!this.splitContainerScaling)
			{
				this.panel1.SuspendLayout();
				this.panel2.SuspendLayout();
				if(this.Orientation==Orientation.Vertical)
				{
					bool flag=this.RightToLeft==RightToLeft.Yes;
					if(!this.CollapsedMode)
					{
						this.panel1.HeightInternal=base.Height;
						this.panel1.WidthInternal=this.splitterDistance;
						this.panel2.Size=new Size((base.Width-this.splitterDistance)-this.SplitterWidthInternal, base.Height);
						if(!flag)
						{
							this.panel1.Location=new Point(0, 0);
							this.panel2.Location=new Point(this.splitterDistance+this.SplitterWidthInternal, 0);
						}
						else
						{
							this.panel1.Location=new Point(base.Width-this.splitterDistance, 0);
							this.panel2.Location=new Point(0, 0);
						}
						this.RepaintSplitterRect();
						this.SetSplitterRect(true);
						if(!this.resizeCalled)
						{
							this.ratioWidth=((((double)base.Width)/((double)this.panel1.Width))>0.0)?(((double)base.Width)/((double)this.panel1.Width)):this.ratioWidth;
						}
					}
					else
					{
						if(this.Panel1Collapsed)
						{
							this.panel2.Size=base.Size;
							this.panel2.Location=new Point(0, 0);
						}
						else if(this.Panel2Collapsed)
						{
							this.panel1.Size=base.Size;
							this.panel1.Location=new Point(0, 0);
						}
						if(!this.resizeCalled)
						{
							this.ratioWidth=((((double)base.Width)/((double)this.splitterDistance))>0.0)?(((double)base.Width)/((double)this.splitterDistance)):this.ratioWidth;
						}
					}
				}
				else if(!this.CollapsedMode)
				{
					this.panel1.Location=new Point(0, 0);
					this.panel1.WidthInternal=base.Width;
					this.panel1.HeightInternal=this.SplitterDistanceInternal;
					int y=this.splitterDistance+this.SplitterWidthInternal;
					this.panel2.Size=new Size(base.Width, base.Height-y);
					this.panel2.Location=new Point(0, y);
					this.RepaintSplitterRect();
					this.SetSplitterRect(false);
					if(!this.resizeCalled)
					{
						this.ratioHeight=((((double)base.Height)/((double)this.panel1.Height))>0.0)?(((double)base.Height)/((double)this.panel1.Height)):this.ratioHeight;
					}
				}
				else
				{
					if(this.Panel1Collapsed)
					{
						this.panel2.Size=base.Size;
						this.panel2.Location=new Point(0, 0);
					}
					else if(this.Panel2Collapsed)
					{
						this.panel1.Size=base.Size;
						this.panel1.Location=new Point(0, 0);
					}
					if(!this.resizeCalled)
					{
						this.ratioHeight=((((double)base.Height)/((double)this.splitterDistance))>0.0)?(((double)base.Height)/((double)this.splitterDistance)):this.ratioHeight;
					}
				}
				this.panel1.ResumeLayout();
				this.panel2.ResumeLayout();
			}
		}

		private void WmSetCursor(ref Message m)
		{
			if((m.WParam==base.Handle)&&((((int)m.LParam)&0xffff)==1))
			{
				if(this.OverrideCursor!=null)
				{
					Cursor.Current=this.OverrideCursor;
				}
				else
				{
					Cursor.Current=this.Cursor;
				}
			}
			else
			{
				this.DefWndProc(ref m);
			}
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
		protected override void WndProc(ref Message msg)
		{
			switch(msg.Msg)
			{
				case 7: // WM_SETFOCUS
					this.splitterFocused=true;
					base.WndProc(ref msg);
					return;
				case 8: // WM_KILLFOCUS
					this.splitterFocused=false;
					base.WndProc(ref msg);
					return;
				case 0x20: // WM_SETCURSOR
					this.WmSetCursor(ref msg);
					return;
				default:
					base.WndProc(ref msg);
					return;
			}
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

		[Browsable(false), Description("ContainerControl.BindingContext")]
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

		[DispId(-504), Category("Appearance"), Description("BorderStyle"), DefaultValue(0)]
		public BorderStyle BorderStyle
		{
			get
			{
				return this.borderStyle;
			}
			set
			{
				if(!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(BorderStyle));
				}
				if(this.borderStyle!=value)
				{
					this.borderStyle=value;
					base.Invalidate();
					this.SetInnerMostBorder(this);
					if((this.Parent!=null)&&(this.Parent is TSplitterPanel))
					{
						TSplitContainer owner=((TSplitterPanel)this.Parent).Owner;
						owner.SetInnerMostBorder(owner);
					}
				}
				switch(this.BorderStyle)
				{
					case BorderStyle.None:
						this.BORDERSIZE=0;
						return;

					case BorderStyle.FixedSingle:
						this.BORDERSIZE=1;
						return;

					case BorderStyle.Fixed3D:
						this.BORDERSIZE=4;
						return;
				}
			}
		}

		private bool CollapsedMode
		{
			get
			{
				if(!this.Panel1Collapsed)
				{
					return this.Panel2Collapsed;
				}
				return true;
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
				if((this.Parent!=null)&&(this.Parent is TSplitterPanel))
				{
					TSplitContainer owner=((TSplitterPanel)this.Parent).Owner;
					owner.SetInnerMostBorder(owner);
				}
				this.ResizeSplitContainer();
			}
		}

		[Description("FixedPanel"), DefaultValue(0), Category("Layout")]
		public FixedPanel FixedPanel
		{
			get
			{
				return this.fixedPanel;
			}
			set
			{
				if(!ClientUtils.IsEnumValid(value, (int)value, 0, 2))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(FixedPanel));
				}
				if(this.fixedPanel!=value)
				{
					this.fixedPanel=value;
					if(this.fixedPanel==FixedPanel.Panel2)
					{
						if(this.Orientation==Orientation.Vertical)
						{
							this.panelSize=(base.Width-this.SplitterDistanceInternal)-this.SplitterWidthInternal;
						}
						else
						{
							this.panelSize=(base.Height-this.SplitterDistanceInternal)-this.SplitterWidthInternal;
						}
					}
					else
					{
						this.panelSize=this.SplitterDistanceInternal;
					}
				}
			}
		}

		// TODO Welche Auswirkungen hat das Fehlen dieser Überladung
		//internal override bool IsContainerControl
		//{
		//    get
		//    {
		//        return true;
		//    }
		//}

		[Category("Layout"), DefaultValue(false), Localizable(true), Description("IsSplitterFixed")]
		public bool IsSplitterFixed
		{
			get
			{
				return this.splitterFixed;
			}
			set
			{
				this.splitterFixed=value;
			}
		}

		private bool IsSplitterMovable
		{
			get
			{
				if(this.Orientation==Orientation.Vertical)
				{
					return (base.Width>=((this.Panel1MinSize+this.SplitterWidthInternal)+this.Panel2MinSize));
				}
				return (base.Height>=((this.Panel1MinSize+this.SplitterWidthInternal)+this.Panel2MinSize));
			}
		}

		[Description("Orientation"), Category("Behavior"), Localizable(true), DefaultValue(1)]
		public Orientation Orientation
		{
			get
			{
				return this.orientation;
			}
			set
			{
				if(!ClientUtils.IsEnumValid(value, (int)value, 0, 1))
				{
					throw new InvalidEnumArgumentException("value", (int)value, typeof(Orientation));
				}
				if(this.orientation!=value)
				{
					this.orientation=value;
					this.splitDistance=0;
					this.SplitterDistance=this.SplitterDistanceInternal;
					this.UpdateSplitter();
				}
			}
		}

		[DllImport("user32.dll", CharSet=CharSet.Auto)]
		public static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);

		private Cursor OverrideCursor
		{
			get
			{
				return this.overrideCursor;
			}
			set
			{
				if(this.overrideCursor!=value)
				{
					this.overrideCursor=value;
					if(base.IsHandleCreated)
					{
						if(ClientRectangle.Contains(Cursor.Position)||Capture)
						{
							//base.SendMessage(0x20, base.Handle, 1);
							SendMessage(new HandleRef(this, base.Handle), 0x20, base.Handle, (IntPtr)1); // 0x20 WM_SETCURSOR

							//Message msg=new Message();
							//msg.HWnd=this.Handle;
							//msg.WParam=base.Handle;
							//msg.Msg=0x20;
							//msg.LParam=(IntPtr)1;
							//WndProc(ref msg);
						}
					}
				}
			}
		}

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

		[Localizable(false), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), Description("Panel1")]
		public TSplitterPanel Panel1
		{
			get
			{
				return this.panel1;
			}
		}

		[Description("Panel1Collapsed"), DefaultValue(false), Category("Layout")]
		public bool Panel1Collapsed
		{
			get
			{
				return this.panel1.Collapsed;
			}
			set
			{
				if(value!=this.panel1.Collapsed)
				{
					if(value&&this.panel2.Collapsed)
					{
						this.CollapsePanel(this.panel2, false);
					}
					this.CollapsePanel(this.panel1, value);
				}
			}
		}

		[Category("Layout"), RefreshProperties(RefreshProperties.All), DefaultValue(25), Localizable(true), Description("Panel1MinSize")]
		public int Panel1MinSize
		{
			get
			{
				return this.panel1MinSize;
			}
			set
			{
				this.newPanel1MinSize=value;
				if((value!=this.Panel1MinSize)&&!this.initializing)
				{
					this.ApplyPanel1MinSize(value);
				}
			}
		}

		[Description("Panel2"), Category("Appearance"), Localizable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TSplitterPanel Panel2
		{
			get
			{
				return this.panel2;
			}
		}

		[Category("Layout"), Description("Panel2Collapsed"), DefaultValue(false)]
		public bool Panel2Collapsed
		{
			get
			{
				return this.panel2.Collapsed;
			}
			set
			{
				if(value!=this.panel2.Collapsed)
				{
					if(value&&this.panel1.Collapsed)
					{
						this.CollapsePanel(this.panel1, false);
					}
					this.CollapsePanel(this.panel2, value);
				}
			}
		}

		[Description("Panel2MinSize"), DefaultValue(25), Localizable(true), Category("Layout"), RefreshProperties(RefreshProperties.All)]
		public int Panel2MinSize
		{
			get
			{
				return this.panel2MinSize;
			}
			set
			{
				this.newPanel2MinSize=value;
				if((value!=this.Panel2MinSize)&&!this.initializing)
				{
					this.ApplyPanel2MinSize(value);
				}
			}
		}

		[Description("SplitterDistance"), Localizable(true), SettingsBindable(true), Category("Layout"), DefaultValue(50)]
		public int SplitterDistance
		{
			get
			{
				return this.splitDistance;
			}
			set
			{
				if(value!=this.SplitterDistance)
				{
					if(value<0)
					{
						throw new ArgumentOutOfRangeException("SplitterDistance", "Invalid low bound argument. SplitterDistance must not be lower than 0.");
					}
					try
					{
						this.setSplitterDistance=true;
						if(this.Orientation==Orientation.Vertical)
						{
							if(value<this.Panel1MinSize)
							{
								value=this.Panel1MinSize;
							}
							if((value+this.SplitterWidthInternal)>(base.Width-this.Panel2MinSize))
							{
								value=(base.Width-this.Panel2MinSize)-this.SplitterWidthInternal;
							}
							if(value<0)
							{
								throw new InvalidOperationException("SplitterDistance not allowed.");
							}
							this.splitDistance=value;
							this.splitterDistance=value;
							this.panel1.WidthInternal=this.SplitterDistance;
						}
						else
						{
							if(value<this.Panel1MinSize)
							{
								value=this.Panel1MinSize;
							}
							if((value+this.SplitterWidthInternal)>(base.Height-this.Panel2MinSize))
							{
								value=(base.Height-this.Panel2MinSize)-this.SplitterWidthInternal;
							}
							if(value<0)
							{
								throw new InvalidOperationException("SplitterDistance not allowed.");
							}
							this.splitDistance=value;
							this.splitterDistance=value;
							this.panel1.HeightInternal=this.SplitterDistance;
						}

						switch(this.fixedPanel)
						{
							case FixedPanel.Panel1:
								this.panelSize=this.SplitterDistance;
								break;
							case FixedPanel.Panel2:
								if(this.Orientation!=Orientation.Vertical)
									this.panelSize=(base.Height-this.SplitterDistance)-this.SplitterWidthInternal;
								else
									this.panelSize=(base.Width-this.SplitterDistance)-this.SplitterWidthInternal;
								break;
						}

						this.UpdateSplitter();
					}
					finally
					{
						this.setSplitterDistance=false;
					}
					this.OnSplitterMoved(new SplitterEventArgs(this.SplitterRectangle.X+(this.SplitterRectangle.Width/2), this.SplitterRectangle.Y+(this.SplitterRectangle.Height/2), this.SplitterRectangle.X, this.SplitterRectangle.Y));
				}
			}
		}

		private int SplitterDistanceInternal
		{
			get
			{
				return this.splitterDistance;
			}
			set
			{
				this.SplitterDistance=value;
			}
		}

		[Category("Layout"), DefaultValue(1), Localizable(true), Description("SplitterIncrement")]
		public int SplitterIncrement
		{
			get
			{
				return this.splitterInc;
			}
			set
			{
				if(value<1)
				{
					throw new ArgumentOutOfRangeException("SplitterIncrement", "Invalid low bound argument. SplitterIncrement must not be lower than 1.");
				}
				this.splitterInc=value;
			}
		}

		[Browsable(false), Category("Layout"), Description("SplitterRectangle")]
		public Rectangle SplitterRectangle
		{
			get
			{
				Rectangle splitterRect=this.splitterRect;
				splitterRect.X=this.splitterRect.X-base.Left;
				splitterRect.Y=this.splitterRect.Y-base.Top;
				return splitterRect;
			}
		}

		[DefaultValue(4), Category("Layout"), Description("SplitterWidth"), Localizable(true)]
		public int SplitterWidth
		{
			get
			{
				return this.splitterWidth;
			}
			set
			{
				this.newSplitterWidth=value;
				if((value!=this.SplitterWidth)&&!this.initializing)
				{
					this.ApplySplitterWidth(value);
				}
			}
		}

		private int SplitterWidthInternal
		{
			get
			{
				if(!this.CollapsedMode)
				{
					return this.splitterWidth;
				}
				return 0;
			}
		}

		[DefaultValue(true), Description("Control.TabStop"), Category("Behavior"), DispId(-516)]
		public new bool TabStop
		{
			get
			{
				return this.tabStop;
			}
			set
			{
				if(this.TabStop!=value)
				{
					this.tabStop=value;
					this.OnTabStopChanged(EventArgs.Empty);
				}
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
		#endregion

		#region Nested Types
		private class TSplitContainerMessageFilter : IMessageFilter
		{
			// Fields
			private TSplitContainer owner;

			// Methods
			public TSplitContainerMessageFilter(TSplitContainer collapsibleContainer)
			{
				this.owner=collapsibleContainer;
			}

			[SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
			bool IMessageFilter.PreFilterMessage(ref Message m)
			{
				// 0x100 256 WM_KEYDOWN
				// 0x100 256 WM_KEYFIRST
				// 0x101 257 WM_KEYUP
				// 0x102 258 WM_CHAR
				// 0x103 259 WM_DEADCHAR
				// 0x104 260 WM_SYSKEYDOWN
				// 0x105 261 WM_SYSKEYUP
				// 0x106 262 WM_SYSCHAR
				// 0x107 263 WM_SYSDEADCHAR
				// 0x108 264 WM_KEYLAST
				if((m.Msg<0x100)||(m.Msg>0x108))
				{
					return false;
				}
				if(((m.Msg==0x100)&&(((int)m.WParam)==0x1b))||(m.Msg==260)) // [Esc] || WM_SYSKEYDOWN
				{
					this.owner.splitBegin=false;
					this.owner.SplitEnd(false);
					this.owner.splitterClick=false;
					this.owner.splitterDrag=false;
				}
				return true;
			}
		}

		internal class TSplitContainerTypedControlCollection : TypedControlCollection
		{
			// Fields
			private TSplitContainer owner;

			// Methods
			public TSplitContainerTypedControlCollection(Control c, Type type, bool isReadOnly)
				: base(c, type, isReadOnly)
			{
				this.owner=c as TSplitContainer;
			}

			public override void Remove(Control value)
			{
				if(((value is TSplitterPanel)&&!this.owner.DesignMode)&&this.IsReadOnly)
				{
					throw new NotSupportedException("Collection is read only.");
				}
				base.Remove(value);
			}

			public override void SetChildIndex(Control child, int newIndex)
			{
				if(child is TSplitterPanel)
				{
					if(this.owner.DesignMode)
					{
						return;
					}
					if(this.IsReadOnly)
					{
						throw new NotSupportedException("Collection is read only.");
					}
				}
				base.SetChildIndex(child, newIndex);
			}
		}
		#endregion
	}
}
