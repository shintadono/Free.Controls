using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free.Controls.CrumbBar
{
	[DefaultEvent("CrumbClicked")]
	public class CrumbBar : Control
	{
		class Crumb
		{
			public bool Clicked { get; set; }
			public bool Hovered { get; set; }
			public string Text { get; private set; }

			public Crumb(string text)
			{
				Text=text;
			}
		}

		#region Setup Images
		static Image imgLeftEdge=global::Free.Controls.Properties.Resources.crumb_left_end;
		static Image imgBody=global::Free.Controls.Properties.Resources.crumb_body;
		static Image imgRightEdge=global::Free.Controls.Properties.Resources.crumb_right_end;
		static Image imgRightTriangle=global::Free.Controls.Properties.Resources.crumb_right_point;

		static Image imgSelectedLeftEdge=global::Free.Controls.Properties.Resources.selected_crumb_left_end;
		static Image imgSelectedBody=global::Free.Controls.Properties.Resources.selected_crumb_body;
		static Image imgSelectedRightEdge=global::Free.Controls.Properties.Resources.selected_crumb_right_end;
		static Image imgSelectedRightTriangle=global::Free.Controls.Properties.Resources.selected_crumb_right_point;

		static Image imgHoveredLeftEdge=global::Free.Controls.Properties.Resources.hovered_crumb_left_end;
		static Image imgHoveredBody=global::Free.Controls.Properties.Resources.hovered_crumb_body;
		static Image imgHoveredRightEdge=global::Free.Controls.Properties.Resources.hovered_crumb_right_end;
		static Image imgHoveredRightTriangle=global::Free.Controls.Properties.Resources.hovered_crumb_right_point;

		static Image imgClickedLeftEdge=global::Free.Controls.Properties.Resources.clicked_crumb_left_end;
		static Image imgClickedBody=global::Free.Controls.Properties.Resources.clicked_crumb_body;
		static Image imgClickedRightEdge=global::Free.Controls.Properties.Resources.clicked_crumb_right_end;
		static Image imgClickedRightTriangle=global::Free.Controls.Properties.Resources.clicked_crumb_right_point;
		#endregion

		List<Crumb> crumbList;
		int?[,] idMap;

		public CrumbBar()
		{
			DoubleBuffered=true;

			MinimumSize=new Size(3, 24);
			MaximumSize=new Size(524288, 24);

			crumbList=new List<Crumb>();
			ClearAndRebuildIDMap();
		}

		private void ClearAndRebuildIDMap()
		{
			idMap=new int?[Width, Height];
		}

		private int? GetIDFromIDMap(int x, int y)
		{
			if(x<idMap.GetLength(0))
			{
				if(y<idMap.GetLength(1))
				{
					return idMap[x, y];
				}
			}

			return null;
		}

		protected override void OnResize(EventArgs e)
		{
			ClearAndRebuildIDMap();
			base.OnResize(e);
		}

		public void Add(string text)
		{
			ClearAndRebuildIDMap();
			crumbList.Add(new Crumb(text));
			Invalidate();
		}

		public void Clear()
		{
			ClearAndRebuildIDMap();
			crumbList.Clear();
			Invalidate();
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);

			foreach(Crumb crumb in crumbList)
			{
				crumb.Hovered=false;
			}

			int? id=GetIDFromIDMap(e.X, e.Y);
			if(id!=null)
			{
				for(int i=0; i<crumbList.Count; i++)
				{
					if(i!=id)
					{
						crumbList[i].Clicked=false;
					}
				}

				crumbList[(int)id].Hovered=true;
			}
			else
			{
				foreach(Crumb crumb in crumbList)
				{
					crumb.Clicked=false;
				}
			}

			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseLeave(e);

			foreach(Crumb crumb in crumbList)
			{
				crumb.Clicked=false;
				crumb.Hovered=false;
			}

			Invalidate();
		}

		int? downID=null;

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			int? id=GetIDFromIDMap(e.X, e.Y);
			if(id!=null)
			{
				downID=(int)id;
				crumbList[(int)id].Clicked=true;
			}

			Invalidate();
		}

		public event EventHandler<CrumbBarClickEventArgs> CrumbClicked;

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			int? id=GetIDFromIDMap(e.X, e.Y);
			if(id!=null)
			{
				if(downID==id)
				{
					crumbList[(int)id].Clicked=false;
					var ea=new CrumbBarClickEventArgs((int)id);
					if(CrumbClicked!=null) { CrumbClicked.Invoke(this, ea); }
				}
			}

			downID=null;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if(crumbList.Count>0)
			{
				int uiOverheadLeft=3;
				int uiOverheadRight=13;
				int uiOverhead=uiOverheadLeft+uiOverheadRight;

				// Calc Control length
				int controlWidth=0;

				for(int i=0; i<crumbList.Count; i++)
				{
					int crumbIndex=crumbList.Count-1-i;
					Crumb crumb=crumbList[crumbIndex];
					int textWidth=(int)e.Graphics.MeasureString(crumb.Text, Font).Width;
					controlWidth+=textWidth+uiOverhead; //Vorne und hinten
				}

				int controlWidthUsed=controlWidth-uiOverheadRight; //Damit das erste Element bei X:0 gezeichnet wird

				for(int i=0; i<crumbList.Count; i++)
				{
					int crumbIndex=crumbList.Count-1-i;
					Crumb crumb=crumbList[crumbIndex];
					int textWidth=(int)e.Graphics.MeasureString(crumb.Text, Font).Width;

					if(crumbIndex==0)
					{
						controlWidthUsed-=textWidth+uiOverhead;
						DrawControl(e, crumbIndex, controlWidthUsed+uiOverheadRight, textWidth+uiOverhead, crumb.Clicked, crumb.Hovered);
						DrawText(e, controlWidthUsed+uiOverhead, crumb.Text);
					}
					else
					{
						controlWidthUsed-=textWidth+uiOverhead;
						DrawControl(e, crumbIndex, controlWidthUsed, textWidth+uiOverhead+uiOverheadRight, crumb.Clicked, crumb.Hovered);
						DrawText(e, controlWidthUsed+uiOverhead, crumb.Text);
					}
				}
			}
			
			base.OnPaint(e);
		}

		public float DrawControl(PaintEventArgs e, int index, float x, float width, bool clicked, bool hovered)
		{
			// IndexMap
			for(int yIndexMap=0; yIndexMap<24; yIndexMap++)
			{
				for(int xIndexMap=(int)x; xIndexMap<x+width; xIndexMap++)
				{
					if(xIndexMap<idMap.GetUpperBound(0)&&yIndexMap<idMap.GetUpperBound(1))
					{
						idMap[xIndexMap, yIndexMap]=index;
					}
				}
			}

			if(clicked)
			{
				e.Graphics.DrawImage(imgClickedLeftEdge, x, 0);
				
				for(int i=(int)x+imgClickedLeftEdge.Width; i<=x+width-(imgClickedRightTriangle).Width; i++)
				{
					e.Graphics.DrawImage(imgClickedBody, i, 0);
				}

				e.Graphics.DrawImage(imgClickedRightTriangle, x+width-(imgClickedRightTriangle).Width, 0);
			}
			else if(hovered)
			{
				e.Graphics.DrawImage(imgHoveredLeftEdge, x, 0);

				for(int i=(int)x+imgHoveredLeftEdge.Width; i<=x+width-(imgHoveredRightTriangle).Width; i++)
				{
					e.Graphics.DrawImage(imgHoveredBody, i, 0);
				}

				e.Graphics.DrawImage((imgHoveredRightTriangle), x+width-(imgHoveredRightTriangle).Width, 0);
			}
			else
			{
				e.Graphics.DrawImage(imgLeftEdge, x, 0);
				

				for(int i=(int)x+imgLeftEdge.Width; i<=x+width-(imgRightTriangle).Width; i++)
				{
					e.Graphics.DrawImage(imgBody, i, 0);
				}

				e.Graphics.DrawImage((imgRightTriangle), x+width-(imgRightTriangle).Width, 0);
			}

			return width;
		}

		private void DrawText(PaintEventArgs e, float x=0f, string text="")
		{
			if(!string.IsNullOrEmpty(text))
			{
				PointF p=new PointF();

				var s=e.Graphics.MeasureString(text, Font);

				p=new PointF(x, (24-s.Height)/2);

				using(Brush b=new SolidBrush(ForeColor))
				{
					e.Graphics.DrawString(text, Font, b, p);
				}
			}
		}
	}
}
