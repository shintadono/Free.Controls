using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Free.Controls
{
	public class FocusablePanel : Panel
	{
		public FocusablePanel()
		{
			this.SetStyle(ControlStyles.Selectable, true);
			this.TabStop=true;
		}

		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public new event KeyEventHandler KeyDown
		{
			add
			{
				base.KeyDown+=value;
			}
			remove
			{
				base.KeyDown-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public new event KeyPressEventHandler KeyPress
		{
			add
			{
				base.KeyPress+=value;
			}
			remove
			{
				base.KeyPress-=value;
			}
		}

		[EditorBrowsable(EditorBrowsableState.Always), Browsable(true)]
		public new event KeyEventHandler KeyUp
		{
			add
			{
				base.KeyUp+=value;
			}
			remove
			{
				base.KeyUp-=value;
			}
		}

		protected override bool IsInputKey(Keys keyData)
		{
			//if(keyData==Keys.Up||keyData==Keys.Down||keyData==Keys.Left||keyData==Keys.Right) return true;
			if(keyData==Keys.Enter) return true;
			return base.IsInputKey(keyData);
		}

		protected override void OnEnter(EventArgs e)
		{
			this.Invalidate();
			base.OnEnter(e);
		}

		protected override void OnGotFocus(EventArgs e)
		{
			this.Invalidate();
			base.OnGotFocus(e);
		}

		protected override void OnLeave(EventArgs e)
		{
			this.Invalidate();
			base.OnLeave(e);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			this.Invalidate();
			base.OnLostFocus(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();
			base.OnMouseDown(e);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			if(Focused)
			{
				var rc=ClientRectangle;
				rc.Inflate(-2, -2);
				ControlPaint.DrawFocusRectangle(e.Graphics, rc);
			}
		}
	}
}
