using System;
using System.Drawing;
using System.Windows.Forms;
using System.Security.Permissions;
using System.ComponentModel;

namespace Free.Controls
{
	/// <summary>
	/// The RangeBar class describes a slide control with two buttons.
	/// A number range is assigned to the control and with the two slide
	/// buttons you can select an interval inside the range. This control can
	/// p.e. used for threshold setting in an image processing tool.
	/// If you push with left mouse button on a slide button it will marked and
	/// while mouse button is pressed you can move the slider left and right.
	/// Otherwise you can use the arrow keys to manipulate the slider position.
	/// The control will throw two events. While left mouse button is pressed and the
	/// position of one slider has changed the event OnRangeChanging will generate and
	/// if you release mouse button, the event OnRangeChanged signals program that
	/// a new range was selected.
	/// </summary>
	[DefaultEvent("RangeChanged")]
	public class RangeBar : UserControl
	{
		/// <summary>
		/// Designer variable
		/// </summary>
		private System.ComponentModel.Container components=null;

		public RangeBar()
		{
			// Dieser Aufruf ist für den Windows Form-Designer erforderlich.
			InitializeComponent();
		}

		/// <summary>
		/// Die verwendeten Ressourcen bereinigen.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if(disposing)
			{
				if(components!=null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code
		/// <summary>
		/// Erforderliche Methode für die Designerunterstützung.
		/// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
		/// </summary>
		private void InitializeComponent()
		{
			this.SuspendLayout();
			this.Name="RangeBar";
			this.Size=new System.Drawing.Size(344, 64);
			this.Load+=new System.EventHandler(this.OnLoad);
			this.SizeChanged+=new System.EventHandler(this.OnSizeChanged);
			this.Paint+=new System.Windows.Forms.PaintEventHandler(this.OnPaint);
			this.Leave+=new System.EventHandler(this.OnLeave);
			this.MouseDown+=new System.Windows.Forms.MouseEventHandler(this.OnMouseDown);
			this.MouseMove+=new System.Windows.Forms.MouseEventHandler(this.OnMouseMove);
			this.MouseUp+=new System.Windows.Forms.MouseEventHandler(this.OnMouseUp);
			this.Resize+=new System.EventHandler(this.OnResize);
			this.ResumeLayout(false);
		}
		#endregion

		public enum ActiveGrabberEnum { None, Min, Max, Both };
		public enum RangeBarOrientation { LeftRight, TopDown, BottomUp };
		public enum TickStyleEnum { None, Top, Bottom, Both };

		#region Variables
		// The values
		int limitMin=0, limitMax=10;
		int rangeMin=3, rangeMax=5;
		int giantStepSize=1;

		// The style stuff
		RangeBarOrientation orientationBar=RangeBarOrientation.LeftRight; // orientation of range bar
		int barSize=8;

		int grabberWidth=8;
		int grabberSize=24;

		TickStyleEnum tickStyle=TickStyleEnum.Bottom;
		int tickSize=3;
		int divisions=10;

		int shadowSize=1;

		Color rangeColor=SystemColors.ActiveCaption;
		Color rangeColorInactive=SystemColors.InactiveCaption;

		// The temp values
		int oldRangeMin=0, oldRangeMax=0; // used to save values for changing-event while mouse-drag.
		int posRangeMin=0, posRangeMax=0; // Pixel-Position of grabber
		int posLimitMin=0, posLimitMax=0; // Pixel-Position of the limits

		// used while mouse gragging a grabber
		bool moveLGrabber=false, moveRGrabber=false, moveBGrabber=false;
		ActiveGrabberEnum activeGrabber=ActiveGrabberEnum.None;
		int mouseGrabOriginX=0, mouseGrabOriginY=0;
		int oldPosRangeMin=0, oldPosRangeMax=0;

		// Hit-Boxes
		Point[] lGrabberPoints=new Point[4];
		Point[] rGrabberPoints=new Point[4];
		Rectangle bGrabberRect=new Rectangle();
		#endregion

		#region Properties
		/// <summary>
		/// Gets and sets the lower value of the range.
		/// </summary>
		public int RangeMinimum
		{
			get
			{
				return rangeMin;
			}
			set
			{
				rangeMin=value;
				if(rangeMin<limitMin) rangeMin=limitMin;
				else if(rangeMin>limitMax) rangeMin=limitMax;
				if(rangeMin>rangeMax) rangeMin=rangeMax;
				Range2Pos();
				Invalidate(true);
			}
		}

		/// <summary>
		/// Gets and sets the upper value of the range.
		/// </summary>
		public int RangeMaximum
		{
			get { return rangeMax; }
			set
			{
				rangeMax=value;
				if(rangeMax<limitMin) rangeMax=limitMin;
				else if(rangeMax>limitMax) rangeMax=limitMax;
				if(rangeMax<rangeMin) rangeMax=rangeMin;
				Range2Pos();
				Invalidate(true);
			}
		}

		/// <summary>
		/// Gets and sets the lower value of the limits.
		/// </summary>
		[DefaultValue(0)]
		public int LimitsMinimum
		{
			get { return limitMin; }
			set
			{
				limitMin=value;
				if(rangeMin<limitMin) rangeMin=limitMin;
				Range2Pos();
				Invalidate(true);
			}
		}

		/// <summary>
		/// Gets and sets the upper value of the limits.
		/// </summary>
		[DefaultValue(10)]
		public int LimitsMaximum
		{
			get { return limitMax; }
			set
			{
				limitMax=value;
				if(rangeMax>limitMax) rangeMax=limitMax;
				Range2Pos();
				Invalidate(true);
			}
		}

		/// <summary>
		/// Gets and sets the step size for [Shift]+Arrow-Key.
		/// </summary>
		[DefaultValue(3)]
		public int GiantStepSize
		{
			get { return giantStepSize; }
			set
			{
				giantStepSize=value;
				if(giantStepSize<2) giantStepSize=2;
			}
		}

		/// <summary>
		/// Gets and sets the range bar orientation.
		/// </summary>
		[DefaultValue(RangeBarOrientation.LeftRight)]
		public RangeBarOrientation Orientation
		{
			get
			{
				return orientationBar;
			}
			set
			{
				orientationBar=value;
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the bar size.
		/// </summary>
		[DefaultValue(8)]
		public int BarSize
		{
			get
			{
				return barSize;
			}
			set
			{
				barSize=Math.Max(Math.Min(value, grabberSize-2), 1);
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the grabber width.
		/// </summary>
		[DefaultValue(8)]
		public int GrabberWidth
		{
			get
			{
				return grabberWidth;
			}
			set
			{
				grabberWidth=Math.Max(3, value);
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the grabber size.
		/// </summary>
		[DefaultValue(24)]
		public int GrabberSize
		{
			get
			{
				return grabberSize;
			}
			set
			{
				grabberSize=Math.Max(Math.Max(barSize+2, 3), value);
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the scale orientation.
		/// </summary>
		[DefaultValue(TickStyleEnum.Bottom)]
		public TickStyleEnum TickStyle
		{
			get
			{
				return tickStyle;
			}
			set
			{
				tickStyle=value;
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the tick size.
		/// </summary>
		[DefaultValue(3)]
		public int TickSize
		{
			get
			{
				return tickSize;
			}
			set
			{
				tickSize=Math.Min(Math.Max(2, value), barSize);
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the number of divisions.
		/// </summary>
		[DefaultValue(10)]
		public int Divisions
		{
			get { return divisions; }
			set
			{
				divisions=value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets and sets the shadow size.
		/// </summary>
		[DefaultValue(1)]
		public int ShadowSize
		{
			get
			{
				return shadowSize;
			}
			set
			{
				shadowSize=Math.Min(Math.Max(1, value), barSize);
				Invalidate();
				Update();
			}
		}

		/// <summary>
		/// Gets and sets the color of the range.
		/// </summary>
		public Color RangeColor
		{
			get { return rangeColor; }
			set
			{
				rangeColor=value;
				Refresh();
			}
		}

		/// <summary>
		/// Gets and sets the color of the range, if control is disabled.
		/// </summary>
		public Color RangeColorInactive
		{
			get { return rangeColorInactive; }
			set
			{
				rangeColorInactive=value;
				Refresh();
			}
		}
		#endregion

		#region Methods
		/// <summary>
		/// Sets the selected range.
		/// </summary>
		/// <param name="min">Minimun value of the range.</param>
		/// <param name="max">Maximum value of the range.</param>
		public void SetSelectRange(int min, int max)
		{
			RangeMinimum=min;
			RangeMaximum=max;
			Range2Pos();
			Invalidate(true);
		}

		/// <summary>
		/// Sets the limits.
		/// </summary>
		/// <param name="min">Minimum value of the limits.</param>
		/// <param name="max">Maximum value of the limits.</param>
		public void SetLimits(int min, int max)
		{
			limitMin=min;
			limitMax=max;
			Range2Pos();
			Invalidate(true);
		}

		/// <summary>
		/// Transforms pixel positions to range values.
		/// </summary>
		private void Pos2Range()
		{
			int w;

			if(orientationBar==RangeBarOrientation.LeftRight) w=Width;
			else w=Height;

			double posw=w-2*grabberWidth-2;
			double factor=(limitMax-limitMin)/posw;

			if(orientationBar==RangeBarOrientation.LeftRight||orientationBar==RangeBarOrientation.TopDown)
			{
				rangeMin=limitMin+(int)Math.Round(factor*(posRangeMin-posLimitMin));
				rangeMax=limitMin+(int)Math.Round(factor*(posRangeMax-posLimitMin));
			}
			else
			{
				rangeMax=limitMin+(int)Math.Round(factor*(posLimitMax-posRangeMin));
				rangeMin=limitMin+(int)Math.Round(factor*(posLimitMax-posRangeMax));
			}

			// keep the limits
			if(rangeMax>=limitMax&&rangeMin<=limitMin)
			{
				rangeMax=limitMax;
				rangeMin=limitMin;
			}
			else if(rangeMax>limitMax)
			{
				int d=rangeMax-limitMax;
				rangeMax-=d;
				rangeMin-=d;
				if(rangeMin<limitMin) rangeMin=limitMin;
			}
			else if(rangeMin<limitMin)
			{
				int d=rangeMin-limitMax;
				rangeMax-=d;
				rangeMin-=d;
				if(rangeMax>limitMax) rangeMax=limitMax;
			}
		}

		/// <summary>
		/// Transforms range values to pixel positions.
		/// </summary>
		private void Range2Pos()
		{
			int w;

			if(orientationBar==RangeBarOrientation.LeftRight) w=Width;
			else w=Height;

			double posw=w-2*grabberWidth-2;
			double factor=posw/(limitMax-limitMin);

			if(orientationBar==RangeBarOrientation.LeftRight||orientationBar==RangeBarOrientation.TopDown)
			{
				posRangeMin=posLimitMin+(int)Math.Round(factor*(rangeMin-limitMin));
				posRangeMax=posLimitMin+(int)Math.Round(factor*(rangeMax-limitMin));
			}
			else
			{
				posRangeMax=posLimitMax-(int)Math.Round(factor*(rangeMin-limitMin));
				posRangeMin=posLimitMax-(int)Math.Round(factor*(rangeMax-limitMin));
			}
		}
		#endregion

		#region Overridden Methods
		[UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
		protected override bool ProcessDialogKey(Keys keyData)
		{
			if(!Enabled) return base.ProcessDialogKey(keyData);

			if((keyData&(Keys.Alt|Keys.Control))==Keys.None)
			{
				Keys keys=keyData&Keys.KeyCode;
				ActiveGrabberEnum grabber=activeGrabber;

				if(orientationBar==RangeBarOrientation.BottomUp)
				{
					if(grabber==ActiveGrabberEnum.Min) grabber=ActiveGrabberEnum.Max;
					else if(grabber==ActiveGrabberEnum.Max) grabber=ActiveGrabberEnum.Min;

					switch(keys)
					{
						case Keys.Left:
						case Keys.Up: keys=Keys.Down; break;
						case Keys.Right:
						case Keys.Down: keys=Keys.Up; break;
					}
				}

				int steps=(keyData&Keys.Shift)==Keys.Shift?giantStepSize:1;

				switch(keys)
				{
					case Keys.Left:
					case Keys.Up:
						if(grabber==ActiveGrabberEnum.Min)
						{
							rangeMin-=steps;
							if(rangeMin<limitMin) rangeMin=limitMin;
							OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
							Invalidate(true);
						}
						else if(grabber==ActiveGrabberEnum.Max)
						{
							rangeMax-=steps;
							if(rangeMax<limitMin) rangeMax=limitMin;
							if(rangeMax<rangeMin) rangeMin=rangeMax;
							OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
							Invalidate(true);
						}
						else if(grabber==ActiveGrabberEnum.Both)
						{
							if(rangeMin-steps<limitMin) steps=rangeMin-limitMin;
							rangeMin-=steps;
							rangeMax-=steps;
							OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
							Invalidate(true);
						}
						return true;
					case Keys.Right:
					case Keys.Down:
						if(grabber==ActiveGrabberEnum.Max)
						{
							rangeMax+=steps;
							if(rangeMax>limitMax) rangeMax=limitMax;
							OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
							Invalidate(true);
						}
						else if(grabber==ActiveGrabberEnum.Min)
						{
							rangeMin+=steps;
							if(rangeMin>limitMax) rangeMin=limitMax;
							if(rangeMax<rangeMin) rangeMax=rangeMin;
							OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
							Invalidate(true);
						}
						else if(grabber==ActiveGrabberEnum.Both)
						{
							if(rangeMax+steps>limitMax) steps=limitMax-rangeMax;
							rangeMin+=steps;
							rangeMax+=steps;
							OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
							Invalidate(true);
						}
						return true;
					case Keys.Tab:
						if(this.ProcessTabKey((keyData&Keys.Shift)==Keys.None)) return true;
						break;
				}
			}

			return base.ProcessDialogKey(keyData);
		}

		protected override bool ProcessTabKey(bool forward)
		{
			if(!this.TabStop) return base.ProcessTabKey(forward);

			if(!forward&&activeGrabber==ActiveGrabberEnum.Max)
			{
				activeGrabber=ActiveGrabberEnum.Both;
				Invalidate(true);
				return true;
			}

			if(!forward&&activeGrabber==ActiveGrabberEnum.Both)
			{
				activeGrabber=ActiveGrabberEnum.Min;
				Invalidate(true);
				return true;
			}

			if(forward&&activeGrabber==ActiveGrabberEnum.Min)
			{
				activeGrabber=ActiveGrabberEnum.Both;
				Invalidate(true);
				return true;
			}

			if(forward&&activeGrabber==ActiveGrabberEnum.Both)
			{
				activeGrabber=ActiveGrabberEnum.Max;
				Invalidate(true);
				return true;
			}

			return base.ProcessTabKey(forward);
		}

		protected override void Select(bool directed, bool forward)
		{
			if(directed&&!forward)
			{
				activeGrabber=ActiveGrabberEnum.Max;
				Invalidate(true);
			}
			else
			{
				activeGrabber=ActiveGrabberEnum.Min;
				Invalidate(true);
			}

			base.Select(directed, forward);
		}
		#endregion

		#region Event handlers
		/// <summary>
		/// Handles Paint-event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnPaint(object sender, PaintEventArgs e)
		{
			Pen penRange=SystemPens.Control;
			Pen penShadowLight=SystemPens.ControlLightLight;
			Pen penShadowDark=SystemPens.ControlDarkDark;

			Brush brushRange=SystemBrushes.Control;
			Brush brushShadowLight=SystemBrushes.ControlLightLight;
			Brush brushShadowDark=SystemBrushes.ControlDarkDark;

			SolidBrush brushInner=new SolidBrush(Enabled?rangeColor:rangeColorInactive);

			// Range
			posLimitMin=grabberWidth;
			if(this.orientationBar==RangeBarOrientation.LeftRight) posLimitMax=Width-grabberWidth-1;
			else posLimitMax=Height-grabberWidth-1;

			// Range check
			if(posRangeMin<posLimitMin) posRangeMin=posLimitMin;
			if(posRangeMin>posLimitMax) posRangeMin=posLimitMax;
			if(posRangeMax>posLimitMax) posRangeMax=posLimitMax;
			if(posRangeMax<posLimitMin) posRangeMax=posLimitMin;

			Range2Pos();

			if(orientationBar==RangeBarOrientation.LeftRight)
			{
				#region LeftRight
				int barOffset=(Height-barSize)/2;
				int grabberOffset=(Height-grabberSize)/2;
				int width=Width;

				// total range bar frame
				e.Graphics.FillRectangle(brushShadowDark, grabberWidth/2, barOffset, shadowSize, barSize-shadowSize); // left
				e.Graphics.FillRectangle(brushShadowDark, grabberWidth/2, barOffset, width-shadowSize-grabberWidth, shadowSize); // top
				e.Graphics.FillRectangle(brushShadowLight, grabberWidth/2, barOffset+barSize-shadowSize, width-grabberWidth, shadowSize); // bottom
				e.Graphics.FillRectangle(brushShadowLight, width-shadowSize-grabberWidth/2, barOffset, shadowSize, barSize-shadowSize); // right

				// inner region
				e.Graphics.FillRectangle(brushInner, posRangeMin, barOffset+shadowSize, posRangeMax-posRangeMin, barSize-2*shadowSize);

				// ticks
				if(divisions>1)
				{
					int tickOffsetBottom=barOffset+barSize+2;
					int tickOffsetTop=barOffset-2-tickSize;

					double dtick=(double)(posLimitMax-posLimitMin)/(double)divisions;
					for(int i=0; i<divisions+1; i++)
					{
						int tickpos=(int)Math.Round((double)i*dtick);
						if(tickStyle==TickStyleEnum.Bottom||tickStyle==TickStyleEnum.Both)
							e.Graphics.DrawLine(penShadowDark, grabberWidth+tickpos, tickOffsetBottom, grabberWidth+tickpos, tickOffsetBottom+tickSize-1);
						if(tickStyle==TickStyleEnum.Top||tickStyle==TickStyleEnum.Both)
							e.Graphics.DrawLine(penShadowDark, grabberWidth+tickpos, tickOffsetTop, grabberWidth+tickpos, tickOffsetTop+tickSize-1);
					}
				}

				// left grabber
				lGrabberPoints[0].X=posRangeMin-grabberWidth+1; lGrabberPoints[0].Y=grabberOffset+grabberSize/3;
				lGrabberPoints[1].X=posRangeMin+1; lGrabberPoints[1].Y=grabberOffset;
				lGrabberPoints[2].X=posRangeMin+1; lGrabberPoints[2].Y=grabberOffset+grabberSize;
				lGrabberPoints[3].X=posRangeMin-grabberWidth+1; lGrabberPoints[3].Y=grabberOffset+2*grabberSize/3;

				e.Graphics.FillPolygon(brushRange, lGrabberPoints);

				// right grabber
				rGrabberPoints[0].X=posRangeMax+grabberWidth; rGrabberPoints[0].Y=grabberOffset+grabberSize/3-1;
				rGrabberPoints[1].X=posRangeMax; rGrabberPoints[1].Y=grabberOffset-1;
				rGrabberPoints[2].X=posRangeMax; rGrabberPoints[2].Y=grabberOffset+grabberSize;
				rGrabberPoints[3].X=posRangeMax+grabberWidth; rGrabberPoints[3].Y=grabberOffset+2*grabberSize/3;

				e.Graphics.FillPolygon(brushRange, rGrabberPoints);

				e.Graphics.DrawLine(penShadowDark, lGrabberPoints[3].X, lGrabberPoints[3].Y, lGrabberPoints[1].X-1, lGrabberPoints[2].Y-1); // bottom
				e.Graphics.DrawLine(penShadowLight, lGrabberPoints[0].X, lGrabberPoints[0].Y-1, lGrabberPoints[0].X, lGrabberPoints[3].Y); // left
				e.Graphics.DrawLine(penShadowLight, lGrabberPoints[0].X+1, lGrabberPoints[0].Y-2, lGrabberPoints[1].X-1, lGrabberPoints[1].Y); // upper
				if(posRangeMin<posRangeMax) e.Graphics.DrawLine(penShadowDark, lGrabberPoints[1].X, lGrabberPoints[1].Y, lGrabberPoints[1].X, lGrabberPoints[2].Y-1); // right

				if(activeGrabber==ActiveGrabberEnum.Min||activeGrabber==ActiveGrabberEnum.Both)
				{
					e.Graphics.DrawLine(penShadowLight, posRangeMin-grabberWidth/2, grabberOffset+grabberSize/3-1, posRangeMin-grabberWidth/2, grabberOffset+2*grabberSize/3); // active mark
					e.Graphics.DrawLine(penShadowDark, posRangeMin-grabberWidth/2+1, grabberOffset+grabberSize/3-1, posRangeMin-grabberWidth/2+1, grabberOffset+2*grabberSize/3); // active mark
				}

				e.Graphics.DrawLine(penShadowDark, rGrabberPoints[2].X+1, rGrabberPoints[2].Y-1, rGrabberPoints[3].X, rGrabberPoints[3].Y); // bottom
				e.Graphics.DrawLine(penShadowDark, rGrabberPoints[0].X, rGrabberPoints[0].Y, rGrabberPoints[3].X, rGrabberPoints[3].Y-1); // right
				e.Graphics.DrawLine(penShadowDark, rGrabberPoints[0].X-1, rGrabberPoints[0].Y-1, rGrabberPoints[1].X+1, rGrabberPoints[1].Y+1); // upper
				if(posRangeMin<posRangeMax) e.Graphics.DrawLine(penShadowLight, rGrabberPoints[1].X, rGrabberPoints[1].Y+1, rGrabberPoints[2].X, rGrabberPoints[2].Y-1); // left

				if(activeGrabber==ActiveGrabberEnum.Max||activeGrabber==ActiveGrabberEnum.Both)
				{
					e.Graphics.DrawLine(penShadowLight, posRangeMax+grabberWidth/2-1, grabberOffset+grabberSize/3-1, posRangeMax+grabberWidth/2-1, grabberOffset+2*grabberSize/3); // active mark
					e.Graphics.DrawLine(penShadowDark, posRangeMax+grabberWidth/2, grabberOffset+grabberSize/3-1, posRangeMax+grabberWidth/2, grabberOffset+2*grabberSize/3); // active mark
				}

				bGrabberRect.X=posRangeMin+1;
				bGrabberRect.Y=barOffset+shadowSize;
				bGrabberRect.Width=Math.Max(1, posRangeMax-posRangeMin-1);
				bGrabberRect.Height=barSize;
				#endregion
			}
			else // vertical bar
			{
				#region Vertical
				int barOffset=(Width-barSize)/2;
				int grabberOffset=(Width-grabberSize)/2;
				int height=Height;

				// total range bar frame
				e.Graphics.FillRectangle(brushShadowDark, barOffset, grabberWidth/2, barSize-shadowSize, shadowSize); // top
				e.Graphics.FillRectangle(brushShadowDark, barOffset, grabberWidth/2, shadowSize, height-shadowSize-grabberWidth); // left
				e.Graphics.FillRectangle(brushShadowLight, barOffset+barSize-shadowSize, grabberWidth/2, shadowSize, height-grabberWidth); // right
				e.Graphics.FillRectangle(brushShadowLight, barOffset, height-shadowSize-grabberWidth/2, barSize-shadowSize, shadowSize); // bottom

				// inner region
				e.Graphics.FillRectangle(brushInner, barOffset+shadowSize, posRangeMin, barSize-2*shadowSize, posRangeMax-posRangeMin);

				// ticks
				if(divisions>1)
				{
					int tickOffsetBottom=barOffset+barSize+2;
					int tickOffsetTop=barOffset-2-tickSize;

					double dtick=(double)(posLimitMax-posLimitMin)/divisions;
					for(int i=0; i<=divisions; i++)
					{
						int tickpos=(int)Math.Round(i*dtick);
						if(tickStyle==TickStyleEnum.Bottom||tickStyle==TickStyleEnum.Both)
							e.Graphics.DrawLine(penShadowDark, tickOffsetBottom, grabberWidth+tickpos, tickOffsetBottom+tickSize-1, grabberWidth+tickpos);
						if(tickStyle==TickStyleEnum.Top||tickStyle==TickStyleEnum.Both)
							e.Graphics.DrawLine(penShadowDark, tickOffsetTop, grabberWidth+tickpos, tickOffsetTop+tickSize-1, grabberWidth+tickpos);
					}
				}

				// upper grabber
				lGrabberPoints[0].Y=posRangeMin-grabberWidth+1; lGrabberPoints[0].X=grabberOffset+grabberSize/3;
				lGrabberPoints[1].Y=posRangeMin+1; lGrabberPoints[1].X=grabberOffset;
				lGrabberPoints[2].Y=posRangeMin+1; lGrabberPoints[2].X=grabberOffset+grabberSize;
				lGrabberPoints[3].Y=posRangeMin-grabberWidth+1; lGrabberPoints[3].X=grabberOffset+2*grabberSize/3;

				e.Graphics.FillPolygon(brushRange, lGrabberPoints);

				// lower grabber
				rGrabberPoints[0].Y=posRangeMax+grabberWidth; rGrabberPoints[0].X=grabberOffset+grabberSize/3;
				rGrabberPoints[1].Y=posRangeMax; rGrabberPoints[1].X=grabberOffset;
				rGrabberPoints[2].Y=posRangeMax; rGrabberPoints[2].X=grabberOffset+grabberSize;
				rGrabberPoints[3].Y=posRangeMax+grabberWidth; rGrabberPoints[3].X=grabberOffset+2*grabberSize/3;

				e.Graphics.FillPolygon(brushRange, rGrabberPoints);
	
				e.Graphics.DrawLine(penShadowDark, lGrabberPoints[3].X, lGrabberPoints[3].Y, lGrabberPoints[2].X-1, lGrabberPoints[2].Y-1); // right
				e.Graphics.DrawLine(penShadowLight, lGrabberPoints[0].X-1, lGrabberPoints[0].Y, lGrabberPoints[3].X, lGrabberPoints[3].Y); // top
				e.Graphics.DrawLine(penShadowLight, lGrabberPoints[0].X-2, lGrabberPoints[0].Y+1, lGrabberPoints[1].X, lGrabberPoints[1].Y-1); // left
				if(posRangeMin<posRangeMax) e.Graphics.DrawLine(penShadowDark, lGrabberPoints[1].X, lGrabberPoints[1].Y, lGrabberPoints[2].X-1, lGrabberPoints[2].Y); // bottom

				if(activeGrabber==ActiveGrabberEnum.Min||activeGrabber==ActiveGrabberEnum.Both)
				{
					e.Graphics.DrawLine(penShadowLight, grabberOffset+grabberSize/3-1, posRangeMin-grabberWidth/2, grabberOffset+2*grabberSize/3, posRangeMin-grabberWidth/2); // active mark
					e.Graphics.DrawLine(penShadowDark, grabberOffset+grabberSize/3-1, posRangeMin-grabberWidth/2+1, grabberOffset+2*grabberSize/3, posRangeMin-grabberWidth/2+1); // active mark
				}

				e.Graphics.DrawLine(penShadowDark, rGrabberPoints[2].X-1, rGrabberPoints[2].Y+1, rGrabberPoints[3].X, rGrabberPoints[3].Y); // right
				e.Graphics.DrawLine(penShadowDark, rGrabberPoints[0].X-1, rGrabberPoints[0].Y, rGrabberPoints[3].X-1, rGrabberPoints[3].Y); // bottom
				e.Graphics.DrawLine(penShadowDark, rGrabberPoints[0].X-2, rGrabberPoints[0].Y-1, rGrabberPoints[1].X, rGrabberPoints[1].Y+1); // left
				if(posRangeMin<posRangeMax) e.Graphics.DrawLine(penShadowLight, rGrabberPoints[1].X, rGrabberPoints[1].Y, rGrabberPoints[2].X-1, rGrabberPoints[2].Y); // top

				if(activeGrabber==ActiveGrabberEnum.Max||activeGrabber==ActiveGrabberEnum.Both)
				{
					e.Graphics.DrawLine(penShadowLight, grabberOffset+grabberSize/3-1, posRangeMax+grabberWidth/2-1, grabberOffset+2*grabberSize/3, posRangeMax+grabberWidth/2-1); // active mark
					e.Graphics.DrawLine(penShadowDark, grabberOffset+grabberSize/3-1, posRangeMax+grabberWidth/2, grabberOffset+2*grabberSize/3, posRangeMax+grabberWidth/2); // active mark
				}

				bGrabberRect.X=barOffset+shadowSize;
				bGrabberRect.Y=posRangeMin+1;
				bGrabberRect.Width=barSize;
				bGrabberRect.Height=Math.Max(1, posRangeMax-posRangeMin-1);
				#endregion
			}
		}

		#region Mouse-Events
		/// <summary>
		/// Handles MouseDown-event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			if(!Enabled) return;

			Rectangle lMarkRect=new Rectangle(Math.Min(lGrabberPoints[0].X, lGrabberPoints[1].X), Math.Min(lGrabberPoints[0].Y, lGrabberPoints[3].Y),
				Math.Abs(lGrabberPoints[2].X-lGrabberPoints[0].X), Math.Max(Math.Abs(lGrabberPoints[0].Y-lGrabberPoints[3].Y), Math.Abs(lGrabberPoints[0].Y-lGrabberPoints[1].Y)));

			Rectangle rMarkRect=new Rectangle(Math.Min(rGrabberPoints[0].X, rGrabberPoints[2].X), Math.Min(rGrabberPoints[0].Y, rGrabberPoints[1].Y),
				Math.Abs(rGrabberPoints[0].X-rGrabberPoints[2].X), Math.Max(Math.Abs(rGrabberPoints[2].Y-rGrabberPoints[0].Y), Math.Abs(rGrabberPoints[1].Y-rGrabberPoints[0].Y)));

			// save current value, even if not necessary
			oldRangeMin=rangeMin;
			oldRangeMax=rangeMax;

			if(lMarkRect.Contains(e.X, e.Y))
			{
				Capture=true;
				moveLGrabber=true;
				activeGrabber=ActiveGrabberEnum.Min;
				Invalidate(true);
			}

			if(rMarkRect.Contains(e.X, e.Y))
			{
				Capture=true;
				moveRGrabber=true;
				activeGrabber=ActiveGrabberEnum.Max;
				Invalidate(true);
			}

			if(bGrabberRect.Contains(e.X, e.Y))
			{
				mouseGrabOriginX=e.X;
				mouseGrabOriginY=e.Y;

				oldPosRangeMin=posRangeMin;
				oldPosRangeMax=posRangeMax;

				Capture=true;
				moveBGrabber=true;
				activeGrabber=ActiveGrabberEnum.Both;
				Invalidate(true);
			}
		}

		/// <summary>
		/// Handles MouseUp-event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			if(!Enabled) return;

			Capture=false;

			moveLGrabber=false;
			moveRGrabber=false;
			moveBGrabber=false;

			Invalidate();
			OnRangeChanged(new RangeChangedEventArgs(rangeMin, rangeMax));
		}

		/// <summary>
		/// Handles MouseMove-event.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMouseMove(object sender, MouseEventArgs e)
		{
			if(!Enabled) return;

			Rectangle lMarkRect=new Rectangle(Math.Min(lGrabberPoints[0].X, lGrabberPoints[1].X), Math.Min(lGrabberPoints[0].Y, lGrabberPoints[3].Y),
				Math.Abs(lGrabberPoints[2].X-lGrabberPoints[0].X), Math.Max(Math.Abs(lGrabberPoints[0].Y-lGrabberPoints[3].Y), Math.Abs(lGrabberPoints[0].Y-lGrabberPoints[1].Y)));

			Rectangle rMarkRect=new Rectangle(Math.Min(rGrabberPoints[0].X, rGrabberPoints[2].X), Math.Min(rGrabberPoints[0].Y, rGrabberPoints[1].Y),
				Math.Abs(rGrabberPoints[0].X-rGrabberPoints[2].X), Math.Max(Math.Abs(rGrabberPoints[2].Y-rGrabberPoints[0].Y), Math.Abs(rGrabberPoints[1].Y-rGrabberPoints[0].Y)));

			if(lMarkRect.Contains(e.X, e.Y)||rMarkRect.Contains(e.X, e.Y)||bGrabberRect.Contains(e.X, e.Y))
			{
				if(orientationBar==RangeBarOrientation.LeftRight) Cursor=Cursors.SizeWE;
				else Cursor=Cursors.SizeNS;
			}
			else Cursor=Cursors.Arrow;

			if(moveLGrabber)
			{
				if(orientationBar==RangeBarOrientation.LeftRight)
				{
					posRangeMin=e.X;
					Cursor=Cursors.SizeWE;
				}
				else
				{
					posRangeMin=e.Y;
					Cursor=Cursors.SizeNS;
				}

				if(posRangeMin<posLimitMin) posRangeMin=posLimitMin;
				if(posRangeMin>posLimitMax) posRangeMin=posLimitMax;
				if(posRangeMax<posRangeMin) posRangeMax=posRangeMin;
				Pos2Range();
				activeGrabber=ActiveGrabberEnum.Min;
				Invalidate(true);

				OnRangeChanging(new RangeChangingEventArgs(rangeMin, rangeMax, oldRangeMin, oldRangeMax));
			}
			else if(moveRGrabber)
			{
				if(orientationBar==RangeBarOrientation.LeftRight)
				{
					posRangeMax=e.X;
					Cursor=Cursors.SizeWE;
				}
				else
				{
					posRangeMax=e.Y;
					Cursor=Cursors.SizeNS;
				}

				if(posRangeMax>posLimitMax) posRangeMax=posLimitMax;
				if(posRangeMax<posLimitMin) posRangeMax=posLimitMin;
				if(posRangeMin>posRangeMax) posRangeMin=posRangeMax;
				Pos2Range();
				activeGrabber=ActiveGrabberEnum.Max;
				Invalidate(true);

				OnRangeChanging(new RangeChangingEventArgs(rangeMin, rangeMax, oldRangeMin, oldRangeMax));
			}
			else if(moveBGrabber)
			{
				int delta=0;
				if(orientationBar==RangeBarOrientation.LeftRight)
				{
					delta=e.X-mouseGrabOriginX;
					Cursor=Cursors.SizeWE;
				}
				else
				{
					delta=e.Y-mouseGrabOriginY;
					Cursor=Cursors.SizeNS;
				}

				if(oldPosRangeMin+delta<posLimitMin) delta=posLimitMin-oldPosRangeMin;
				else if(oldPosRangeMax+delta>posLimitMax) delta=posLimitMax-oldPosRangeMax;

				posRangeMin=oldPosRangeMin+delta;
				posRangeMax=oldPosRangeMax+delta;

				Pos2Range();
				activeGrabber=ActiveGrabberEnum.Both;
				Invalidate(true);

				OnRangeChanging(new RangeChangingEventArgs(rangeMin, rangeMax, oldRangeMin, oldRangeMax));
			}
		}
		#endregion

		private void OnLeave(object sender, EventArgs e)
		{
			activeGrabber=ActiveGrabberEnum.None;
			Invalidate(true);
		}

		private void OnLoad(object sender, EventArgs e)
		{
			// use double buffering
			SetStyle(ControlStyles.DoubleBuffer, true);
			SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			SetStyle(ControlStyles.UserPaint, true);
		}

		private void OnResize(object sender, EventArgs e)
		{
			Invalidate(true);
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			Invalidate(true);
			Update();
		}
		#endregion

		#region Change/Changing Events
		public class RangeChangedEventArgs : EventArgs
		{
			public int RangeMinimum;
			public int RangeMaximum;

			public RangeChangedEventArgs(int min, int max)
			{
				RangeMinimum=min;
				RangeMaximum=max;
			}
		}

		public class RangeChangingEventArgs : EventArgs
		{
			public int RangeMinimum;
			public int RangeMaximum;

			public int OldRangeMinimum;
			public int OldRangeMaximum;

			public RangeChangingEventArgs(int min, int max, int oldMin, int oldMax)
			{
				RangeMinimum=min;
				RangeMaximum=max;
				OldRangeMinimum=oldMin;
				OldRangeMaximum=oldMax;
			}
		}

		/// <summary>
		/// Delegate to handle range changed.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void RangeChangedEventHandler(object sender, RangeChangedEventArgs e);

		/// <summary>
		/// Delegate to handle range is changing.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public delegate void RangeChangingEventHandler(object sender, RangeChangingEventArgs e);

		public event RangeChangedEventHandler RangeChanged; // event handler for range changed
		public event RangeChangingEventHandler RangeChanging; // event handler for range is changing

		public virtual void OnRangeChanged(RangeChangedEventArgs e)
		{
			if(RangeChanged!=null)
				RangeChanged(this, e);
		}

		public virtual void OnRangeChanging(RangeChangingEventArgs e)
		{
			if(RangeChanging!=null)
				RangeChanging(this, e);
		}
		#endregion
	}
}
