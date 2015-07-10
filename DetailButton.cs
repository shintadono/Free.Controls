using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;

namespace Free.Controls
{
	public class DetailButton : Control, IButtonControl
	{
		Font boldFont;

		#region States
		bool defaultButton=false;
		bool kbPressed=false;
		bool mousePressed=false;
		bool mouseHot=false;
		#endregion

		#region Public Properties
		string caption="";

		[Category("Appearance"), Localizable(true)]
		public string Caption
		{
			get { return caption; }
			set { caption=value; Invalidate(); }
		}

		string description="";

		[Category("Appearance"), Editor(typeof(MultilineStringEditor), typeof(UITypeEditor)), Localizable(true)]
		public string Description
		{
			get { return description; }
			set { description=value; Invalidate(); }
		}

		Image image;

		[Category("Appearance")]
		public Image Image
		{
			get { return image; }
			set { image=value; Invalidate(); }
		}

		int imagePadding=16;

		[Category("Appearance"), DefaultValue(16)]
		public int ImagePadding
		{
			get { return imagePadding; }
			set { imagePadding=value; Invalidate(); }
		}

		ContentAlignment textAlign=ContentAlignment.MiddleCenter;

		[Category("Appearance"), DefaultValue(ContentAlignment.MiddleCenter)]
		public ContentAlignment TextAlign
		{
			get { return textAlign; }
			set { textAlign=value; Invalidate(); }
		}

		int textDistance=0;

		[Category("Appearance"), DefaultValue(0)]
		public int TextDistance
		{
			get { return textDistance; }
			set { textDistance=value; Invalidate(); }
		}

		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override string Text
		{
			get { return base.Text; }
			set { base.Text=value; }
		}
		#endregion

		public DetailButton()
		{
			Caption="Caption";
			Description="Description";

			boldFont=new Font(Font, FontStyle.Bold);

			// Double buffering
			DoubleBuffered=true;
			SetStyle(ControlStyles.ResizeRedraw, true);
		}

		protected override Size DefaultSize { get { return new Size(200, 50); } }

		protected override void OnEnabledChanged(EventArgs e)
		{
			base.OnEnabledChanged(e);

			if(!Enabled) kbPressed=mousePressed=false;
			Invalidate();
		}

		protected override void OnFontChanged(EventArgs e)
		{
			base.OnFontChanged(e);
			boldFont=new Font(Font, FontStyle.Bold);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);

			DrawButton(e, kbPressed||mousePressed);
			DrawText(e, kbPressed||mousePressed);
		}

		private void DrawButton(PaintEventArgs e, bool pressed=false)
		{
			// Set the background color to the parent if visual styles
			// are disabled, because DrawParentBackground will only paint
			// over the control background if visual styles are enabled.
			BackColor=Application.RenderWithVisualStyles?Color.Azure:Parent.BackColor;

			// If you comment out the call to DrawParentBackground,
			// the background of the control will still be visible
			// outside the pressed button, if visual styles are enabled.
			ButtonRenderer.DrawParentBackground(e.Graphics, ClientRectangle, this);

			System.Windows.Forms.VisualStyles.PushButtonState state=defaultButton?System.Windows.Forms.VisualStyles.PushButtonState.Default:System.Windows.Forms.VisualStyles.PushButtonState.Normal;
			if(mouseHot) state=System.Windows.Forms.VisualStyles.PushButtonState.Hot;
			if(pressed) state=System.Windows.Forms.VisualStyles.PushButtonState.Pressed;
			if(!Enabled) state=System.Windows.Forms.VisualStyles.PushButtonState.Disabled;

			if(Image==null) ButtonRenderer.DrawButton(e.Graphics, ClientRectangle, Focused, state);
			else
			{
				Rectangle imageRectangle=new Rectangle(imagePadding+Padding.Left, (Height-Image.Height)/2, Image.Width, Image.Height);
				ButtonRenderer.DrawButton(e.Graphics, ClientRectangle, Image, imageRectangle, Focused, state);
			}
		}

		private void DrawText(PaintEventArgs e, bool pressed=false)
		{
			int offset=(pressed&&!Application.RenderWithVisualStyles)?1:0;

			Rectangle imageRectangle=new Rectangle();
			int imageTotalWith=0;

			if(Image!=null)
			{
				imageRectangle=new Rectangle(imagePadding, (Height-Image.Height)/2, Image.Width, Image.Height);
				imageTotalWith=2*imagePadding+Image.Width;
			}

			int maxWidth=Width-Padding.Horizontal-imageTotalWith-(Application.RenderWithVisualStyles?0:1);

			StringFormat sf=new StringFormat();
			switch(TextAlign)
			{
				case ContentAlignment.TopLeft:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.BottomLeft: sf.Alignment=StringAlignment.Near; break;
				case ContentAlignment.TopCenter:
				default:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.BottomCenter: sf.Alignment=StringAlignment.Center; break;
				case ContentAlignment.TopRight:
				case ContentAlignment.MiddleRight:
				case ContentAlignment.BottomRight: sf.Alignment=StringAlignment.Far; break;
			}

			SizeF sizeCaption=e.Graphics.MeasureString(Caption, boldFont, maxWidth, sf);
			SizeF sizeDescription=e.Graphics.MeasureString(Description, Font, maxWidth, sf);

			float textHeight=sizeCaption.Height+textDistance+sizeDescription.Height;

			float yCapt;

			switch(TextAlign)
			{
				case ContentAlignment.TopLeft:
				case ContentAlignment.TopCenter:
				case ContentAlignment.TopRight:
					yCapt=Padding.Top;
					break;
				default:
				case ContentAlignment.MiddleLeft:
				case ContentAlignment.MiddleCenter:
				case ContentAlignment.MiddleRight:
					yCapt=Padding.Top+(Height-Padding.Vertical-textHeight)/2;
					break;
				case ContentAlignment.BottomLeft:
				case ContentAlignment.BottomCenter:
				case ContentAlignment.BottomRight:
					yCapt=Height-Padding.Bottom-textHeight-(Application.RenderWithVisualStyles?0:1);
					break;
			}

			float yDesc=yCapt+sizeCaption.Height+textDistance;

			RectangleF rectCapt=new RectangleF(Padding.Left+imageTotalWith+offset, yCapt+offset, maxWidth, Height-yCapt-Padding.Bottom);
			e.Graphics.DrawString(Caption, boldFont, Enabled?SystemBrushes.ControlText:SystemBrushes.GrayText, rectCapt, sf);

			RectangleF rectDesc=new RectangleF(Padding.Left+imageTotalWith+offset, yDesc+offset, maxWidth, Height-yDesc-Padding.Bottom);
			e.Graphics.DrawString(Description, Font, Enabled?SystemBrushes.ControlText:SystemBrushes.GrayText, rectDesc, sf);
		}

		#region Focus
		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);
			Invalidate();
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			Invalidate();
		}
		#endregion

		#region Keyboard
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			if(e.KeyCode!=Keys.Space) return;

			kbPressed=true;
			Focus();
			Invalidate();

			OnClick(e);
		}

		protected override void OnKeyUp(KeyEventArgs e)
		{
			base.OnKeyUp(e);

			if(e.KeyCode!=Keys.Space) return;

			kbPressed=false;
			Focus();
			Invalidate();
		}
		#endregion

		#region Mouse Event
		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);

			mousePressed=true;
			Focus();
			Invalidate();
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);

			mousePressed=false;
			Focus();
			Invalidate();
		}

		protected override void OnMouseEnter(EventArgs e)
		{
			base.OnMouseEnter(e);
			mouseHot=true;
			Invalidate();
		}

		protected override void OnMouseLeave(EventArgs e)
		{
			base.OnMouseEnter(e);
			mouseHot=false;
			Invalidate();
		}
		#endregion

		#region IButtonControl Implementation
		[Category("Behavior")]
		[DefaultValue(DialogResult.None)]
		public DialogResult DialogResult { get; set; }

		public void NotifyDefault(bool value)
		{
			defaultButton=value;
			Invalidate();
		}

		public void PerformClick()
		{
			if(!CanSelect) return;
			if(ValidateActiveControl()) OnClick(EventArgs.Empty);
		}

		protected override void OnClick(EventArgs e)
		{
			Form form=FindForm();
			if(form!=null) form.DialogResult=DialogResult;

			AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
			AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

			base.OnClick(e);
		}

		protected bool ValidateActiveControl()
		{
			IContainerControl c=GetContainerControl();
			if(c==null) return true;

			ContainerControl container=c as ContainerControl;
			if(container==null) return true;

			while(container.ActiveControl==null)
			{
				Control parent=container.Parent;
				if(parent==null) break;
				
				ContainerControl cc=parent.GetContainerControl() as ContainerControl;
				if(cc==null) break;

				container=cc;
			}

			return container.Validate(true);
		}
		#endregion
	}
}
