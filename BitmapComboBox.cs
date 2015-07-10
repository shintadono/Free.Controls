using System.Drawing;
using System.Windows.Forms;

namespace Free.Controls
{
	public class BitmapComboBox : ComboBox
	{
		public BitmapComboBox()
		{
			DrawMode=DrawMode.OwnerDrawVariable;
			DropDownStyle=ComboBoxStyle.DropDownList;

			DrawItem+=OnDrawItem;
			MeasureItem+=OnMeasureItem;
		}

		void OnMeasureItem(object sender, MeasureItemEventArgs e)
		{
			if(e.Index<0||e.Index>=Items.Count) return;

			Bitmap bm=Items[e.Index] as Bitmap;
			if(bm!=null)
			{
				e.ItemWidth=bm.Width;
				e.ItemHeight=bm.Height;
			}
		}

		void OnDrawItem(object sender, DrawItemEventArgs e)
		{
			e.DrawBackground();

			if(e.Index<0||e.Index>=Items.Count) return;

			Bitmap bm=Items[e.Index] as Bitmap;
			if(bm!=null) e.Graphics.DrawImage(bm, e.Bounds.X, e.Bounds.Y, e.Bounds.Width, e.Bounds.Height);

			e.DrawFocusRectangle();
		}
	}
}
