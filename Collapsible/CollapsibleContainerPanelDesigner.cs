using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Free.Controls.Collapsible
{
	internal class CollapsibleContainerPanelDesigner : ScrollableControlDesigner
	{
		// Fields
		private IDesignerHost designerHost;
		private bool selected;
		private ICollapsibleContainerDesigner collapsibleContainerDesigner;
		private CollapsibleContainerPanel collapsibleContainerPanel;

		// Methods
		public CollapsibleContainerPanelDesigner()
		{
			base.AutoResizeHandles=true;
		}

		public override bool CanBeParentedTo(IDesigner parentDesigner)
		{
			return parentDesigner is ICollapsibleContainerDesigner;
		}

		protected override void Dispose(bool disposing)
		{
			IComponentChangeService service=(IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			if(service!=null)
			{
				service.ComponentChanged-=new ComponentChangedEventHandler(this.OnComponentChanged);
			}
			base.Dispose(disposing);
		}

		protected virtual void DrawBorder(Graphics graphics)
		{
			Panel component=(Panel)base.Component;
			if((component!=null)&&component.Visible)
			{
				Rectangle clientRectangle=this.Control.ClientRectangle;
				using(Pen pen=new Pen(BorderPenColor))
				{
					pen.DashStyle=DashStyle.Dash;
					clientRectangle.Width--;
					clientRectangle.Height--;
					graphics.DrawRectangle(pen, clientRectangle);
				}
			}
		}

		internal void DrawSelectedBorder()
		{
			Control control=this.Control;
			using(Graphics graphics=control.CreateGraphics())
			{
				Rectangle clientRectangle=control.ClientRectangle;
				using(Pen pen=new Pen(BorderPenColor))
				{
					pen.DashStyle=DashStyle.Dash;
					clientRectangle.Inflate(-4, -4);
					graphics.DrawRectangle(pen, clientRectangle);
				}
			}
		}

		internal void DrawWaterMark(Graphics g)
		{
			Control control=this.Control;
			Rectangle clientRectangle=control.ClientRectangle;
			string name=control.Name;
			using(Font font=new Font("Arial", 8f))
			{
				int x=(clientRectangle.Width/2)-(((int)g.MeasureString(name, font).Width)/2);
				int y=clientRectangle.Height/2;
				TextRenderer.DrawText(g, name, font, new Point(x, y), Color.Black, TextFormatFlags.GlyphOverhangPadding);
			}
		}

		internal void EraseBorder()
		{
			Control control=this.Control;
			Rectangle clientRectangle=control.ClientRectangle;
			Graphics graphics=control.CreateGraphics();
			Pen pen=new Pen(control.BackColor);
			pen.DashStyle=DashStyle.Dash;
			clientRectangle.Inflate(-4, -4);
			graphics.DrawRectangle(pen, clientRectangle);
			pen.Dispose();
			graphics.Dispose();
			control.Invalidate();
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			this.collapsibleContainerPanel=(CollapsibleContainerPanel)component;
			this.designerHost=(IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			this.collapsibleContainerDesigner=(ICollapsibleContainerDesigner)this.designerHost.GetDesigner(this.collapsibleContainerPanel.Parent);
			IComponentChangeService service=(IComponentChangeService)this.GetService(typeof(IComponentChangeService));
			if(service!=null)
			{
				service.ComponentChanged+=new ComponentChangedEventHandler(this.OnComponentChanged);
			}
			PropertyDescriptor descriptor=TypeDescriptor.GetProperties(component)["Locked"];
			if((descriptor!=null)&&(this.collapsibleContainerPanel.Parent is CollapsibleContainer))
			{
				descriptor.SetValue(component, true);
			}
		}

		private void OnComponentChanged(object sender, ComponentChangedEventArgs e)
		{
			if(this.collapsibleContainerPanel.Parent!=null)
			{
				if(this.collapsibleContainerPanel.Controls.Count==0)
				{
					using(Graphics g=this.collapsibleContainerPanel.CreateGraphics())
					{
						this.DrawWaterMark(g);
					}
				}
				else
				{
					this.collapsibleContainerPanel.Invalidate();
				}
			}
		}

		protected override void OnDragDrop(DragEventArgs de)
		{
			if(this.InheritanceAttribute==InheritanceAttribute.InheritedReadOnly)
			{
				de.Effect=DragDropEffects.None;
			}
			else
			{
				base.OnDragDrop(de);
			}
		}

		protected override void OnDragEnter(DragEventArgs de)
		{
			if(this.InheritanceAttribute==InheritanceAttribute.InheritedReadOnly)
			{
				de.Effect=DragDropEffects.None;
			}
			else
			{
				base.OnDragEnter(de);
			}
		}

		protected override void OnDragLeave(EventArgs e)
		{
			if(this.InheritanceAttribute!=InheritanceAttribute.InheritedReadOnly)
			{
				base.OnDragLeave(e);
			}
		}

		protected override void OnDragOver(DragEventArgs de)
		{
			if(this.InheritanceAttribute==InheritanceAttribute.InheritedReadOnly)
			{
				de.Effect=DragDropEffects.None;
			}
			else
			{
				base.OnDragOver(de);
			}
		}

		protected override void OnMouseHover()
		{
			if(this.collapsibleContainerDesigner!=null)
			{
				this.collapsibleContainerDesigner.CollapsibleContainerPanelHover();
			}
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			base.OnPaintAdornments(pe);
			if(this.collapsibleContainerPanel.BorderStyle==BorderStyle.None)
			{
				this.DrawBorder(pe.Graphics);
			}
			if(this.Selected)
			{
				this.DrawSelectedBorder();
			}
			if(this.collapsibleContainerPanel.Controls.Count==0)
			{
				this.DrawWaterMark(pe.Graphics);
			}
		}

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);
			properties.Remove("Modifiers");
			properties.Remove("Locked");
			properties.Remove("GenerateMember");
			foreach(DictionaryEntry entry in properties)
			{
				PropertyDescriptor oldPropertyDescriptor=(PropertyDescriptor)entry.Value;
				if(oldPropertyDescriptor.Name.Equals("Name")&&oldPropertyDescriptor.DesignTimeOnly)
				{
					properties[entry.Key]=TypeDescriptor.CreateProperty(oldPropertyDescriptor.ComponentType, oldPropertyDescriptor, new Attribute[] { BrowsableAttribute.No, DesignerSerializationVisibilityAttribute.Hidden });
					break;
				}
			}
		}

		// Properties
		protected Color BorderPenColor
		{
			get
			{
				return this.Control.BackColor.GetBrightness()<0.5?ControlPaint.Light(this.Control.BackColor):ControlPaint.Dark(this.Control.BackColor);
			}
		}

		protected override InheritanceAttribute InheritanceAttribute
		{
			get
			{
				if((this.collapsibleContainerPanel!=null)&&(this.collapsibleContainerPanel.Parent!=null))
				{
					return (InheritanceAttribute)TypeDescriptor.GetAttributes(this.collapsibleContainerPanel.Parent)[typeof(InheritanceAttribute)];
				}
				return base.InheritanceAttribute;
			}
		}

		internal bool Selected
		{
			get
			{
				return this.selected;
			}
			set
			{
				this.selected=value;
				if(this.selected)
				{
					this.DrawSelectedBorder();
				}
				else
				{
					this.EraseBorder();
				}
			}
		}

		public override SelectionRules SelectionRules
		{
			get
			{
				SelectionRules none=SelectionRules.None;
				if(this.Control.Parent is CollapsibleContainer)
				{
					none=SelectionRules.None|SelectionRules.Locked;
				}
				return none;
			}
		}

		public override IList SnapLines
		{
			get
			{
				ArrayList snapLines=null;
				base.AddPaddingSnapLines(ref snapLines);
				return snapLines;
			}
		}
	}
}
