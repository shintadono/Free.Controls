using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Free.Controls.Collapsible
{
	class CollapsibleContainerDesigner : ParentControlDesigner, ICollapsibleContainerDesigner
	{
		// Fields
		private IDesignerHost designerHost;
		private bool disableDrawGrid;
		private static int numberOfCollapsibleContainerPanels=1;
		private const string panelName="Panel";
		private CollapsibleContainerPanel selectedPanel;
		private CollapsibleContainer collapsibleContainer;
		private bool collapsibleContainerSelected;
		private bool sizeChangedException=false;
		private CollapsibleContainerPanel collapsibleContainerPanel;

		// Methods
		public override bool CanParent(Control control)
		{
			return false;
		}

		protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
		{
			if(this.Selected==null)
			{
				this.Selected=this.collapsibleContainerPanel;
			}
			CollapsibleContainerPanelDesigner toInvoke=(CollapsibleContainerPanelDesigner)this.designerHost.GetDesigner(this.Selected);
			ParentControlDesigner.InvokeCreateTool(toInvoke, tool);
			return null;
		}

		protected override void Dispose(bool disposing)
		{
			ISelectionService service=(ISelectionService)this.GetService(typeof(ISelectionService));
			if(service!=null)
			{
				service.SelectionChanged-=new EventHandler(this.OnSelectionChanged);
			}
			this.collapsibleContainer.MouseDown-=new MouseEventHandler(this.OnCollapsibleContainerClick);
			this.collapsibleContainer.DoubleClick-=new EventHandler(this.OnCollapsibleContainerDoubleClick);
			base.Dispose(disposing);
		}

		protected override bool GetHitTest(Point point)
		{
			return ((this.InheritanceAttribute!=InheritanceAttribute.InheritedReadOnly)&&this.collapsibleContainerSelected);
		}

		protected override Control GetParentForComponent(IComponent component)
		{
			return this.collapsibleContainerPanel;
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			base.AutoResizeHandles=true;
			this.collapsibleContainer=component as CollapsibleContainer;
			this.collapsibleContainerPanel=this.collapsibleContainer.Panel;
			base.EnableDesignMode(this.collapsibleContainer.Panel, "Panel");
			this.designerHost=(IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			if(this.selectedPanel==null)
			{
				this.Selected=this.collapsibleContainerPanel;
			}
			this.collapsibleContainer.MouseDown+=new MouseEventHandler(this.OnCollapsibleContainerClick);
			this.collapsibleContainer.DoubleClick+=new EventHandler(this.OnCollapsibleContainerDoubleClick);
			this.collapsibleContainer.SizeChanged+=new EventHandler(this.OnSizeChanged);
			ISelectionService service=(ISelectionService)this.GetService(typeof(ISelectionService));
			if(service!=null)
			{
				service.SelectionChanged+=new EventHandler(this.OnSelectionChanged);
			}
		}

		public override ControlDesigner InternalControlDesigner(int internalControlIndex)
		{
			CollapsibleContainerPanel panel;
			switch(internalControlIndex)
			{
				case 0:
					panel=this.collapsibleContainerPanel;
					break;

				default:
					return null;
			}
			return (this.designerHost.GetDesigner(panel) as ControlDesigner);
		}

		public override int NumberOfInternalControlDesigners()
		{
			return numberOfCollapsibleContainerPanels;
		}

		protected override void OnDragEnter(DragEventArgs de)
		{
			de.Effect=DragDropEffects.None;
		}

		protected override void OnPaintAdornments(PaintEventArgs pe)
		{
			try
			{
				disableDrawGrid=true;
				base.OnPaintAdornments(pe);
			}
			finally
			{
				disableDrawGrid=false;
			}
		}

		private void OnSelectionChanged(object sender, EventArgs e)
		{
			ISelectionService service=(ISelectionService)this.GetService(typeof(ISelectionService));
			this.collapsibleContainerSelected=false;
			if(service!=null)
			{
				foreach(object obj2 in service.GetSelectedComponents())
				{
					CollapsibleContainerPanel panel=obj2 as CollapsibleContainerPanel;
					if((panel!=null)&&(panel.Parent==this.collapsibleContainer))
					{
						this.collapsibleContainerSelected=false;
						this.Selected=panel;
						break;
					}
					this.Selected=null;
					if(obj2==this.collapsibleContainer)
					{
						this.collapsibleContainerSelected=true;
						break;
					}
				}
			}
		}

		private void OnCollapsibleContainerClick(object sender, MouseEventArgs e)
		{
			((ISelectionService)this.GetService(typeof(ISelectionService))).SetSelectedComponents(new object[] { this.Control });
		}

		private void OnCollapsibleContainerDoubleClick(object sender, EventArgs e)
		{
			if(this.collapsibleContainerSelected)
			{
				try
				{
					this.DoDefaultAction();
				}
				catch(Exception exception)
				{
					if(ClientUtils.IsCriticalException(exception))
					{
						throw;
					}
					base.DisplayError(exception);
				}
			}
		}

		private void OnSizeChanged(object sender, EventArgs e)
		{
			if((this.InheritanceAttribute!=InheritanceAttribute.InheritedReadOnly)&&!sizeChangedException)
			{
				try
				{
					if(!this.collapsibleContainer.Collapsed)
					{
						this.collapsibleContainer.UncollapsedHeight=this.collapsibleContainer.Size.Height;
					}
					else this.collapsibleContainer.Height=CollapsibleContainer.HeaderHeight;

					base.RaiseComponentChanging(TypeDescriptor.GetProperties(this.collapsibleContainer)["UncollapsedHeight"]);
					base.RaiseComponentChanged(TypeDescriptor.GetProperties(this.collapsibleContainer)["UncollapsedHeight"], null, null);
				}
				catch(InvalidOperationException exception)
				{
					((IUIService)base.Component.Site.GetService(typeof(IUIService))).ShowError(exception.Message);
				}
				catch(CheckoutException exception2)
				{
					if(exception2==CheckoutException.Canceled)
					{
						try
						{
							this.sizeChangedException=true;
							if(!this.collapsibleContainer.Collapsed)
							{
								this.collapsibleContainer.UncollapsedHeight=this.collapsibleContainer.Size.Height;
							}
							return;
						}
						finally
						{
							this.sizeChangedException=false;
						}
					}
					throw;
				}
			}
		}

		void ICollapsibleContainerDesigner.CollapsibleContainerPanelHover()
		{
			this.OnMouseHover();
		}

		// Properties
		public override DesignerActionListCollection ActionLists
		{
			get
			{
				DesignerActionListCollection lists=new DesignerActionListCollection();
				lists.Add(new CollapsibleContainerActionList(this.Control));
				return lists;
			}
		}

		protected override bool AllowControlLasso { get { return false; } }

		public override ICollection AssociatedComponents
		{
			get
			{
				ArrayList list=new ArrayList();
				foreach(object obj in this.collapsibleContainer.Controls)
				{
					if(obj is CollapsibleContainerPanel)
					{
						CollapsibleContainerPanel panel=(CollapsibleContainerPanel)obj;
						foreach(Control control in panel.Controls)
						{
							list.Add(control);
						}
					}
				}

				return list;
			}
		}

		protected override bool DrawGrid { get { return disableDrawGrid?false:base.DrawGrid; } }

		internal CollapsibleContainerPanel Selected
		{
			get
			{
				return this.selectedPanel;
			}
			set
			{
				if(this.selectedPanel!=null)
				{
					CollapsibleContainerPanelDesigner designer=(CollapsibleContainerPanelDesigner)this.designerHost.GetDesigner(this.selectedPanel);
					designer.Selected=false;
				}
				if(value!=null)
				{
					CollapsibleContainerPanelDesigner designer2=(CollapsibleContainerPanelDesigner)this.designerHost.GetDesigner(value);
					this.selectedPanel=value;
					designer2.Selected=true;
				}
				else if(this.selectedPanel!=null)
				{
					CollapsibleContainerPanelDesigner designer3=(CollapsibleContainerPanelDesigner)this.designerHost.GetDesigner(this.selectedPanel);
					this.selectedPanel=null;
					designer3.Selected=false;
				}
			}
		}

		public override IList SnapLines { get { return base.SnapLines as ArrayList; } }

		// Nested Types
		public class CollapsibleContainerActionList : DesignerActionList
		{
			public CollapsibleContainerActionList(IComponent component) : base(component) { }

			public DockStyle Dock
			{
				get { return ((CollapsibleContainer)this.Component).Dock; }
				set { TypeDescriptor.GetProperties(this.Component)["Dock"].SetValue(this.Component, value); }
			}

			public Collapsible.CollapsibleContainerHeader HeaderType
			{
				get { return ((CollapsibleContainer)this.Component).HeaderType; }
				set { TypeDescriptor.GetProperties(this.Component)["HeaderType"].SetValue(this.Component, value); }
			}

			public string HeaderText
			{
				get { return ((CollapsibleContainer)this.Component).HeaderText; }
				set { TypeDescriptor.GetProperties(this.Component)["HeaderText"].SetValue(this.Component, value); }
			}

			public bool Checked
			{
				get { return ((CollapsibleContainer)this.Component).Checked; }
				set { TypeDescriptor.GetProperties(this.Component)["Checked"].SetValue(this.Component, value); }
			}

			public bool Collapsed
			{
				get { return ((CollapsibleContainer)this.Component).Collapsed; }
				set { TypeDescriptor.GetProperties(this.Component)["Collapsed"].SetValue(this.Component, value); }
			}

			public int UncollapsedHeight
			{
				get { return ((CollapsibleContainer)this.Component).UncollapsedHeight; }
				set { TypeDescriptor.GetProperties(this.Component)["UncollapsedHeight"].SetValue(this.Component, value); }
			}

			public override DesignerActionItemCollection GetSortedActionItems()
			{
				DesignerActionItemCollection items=new DesignerActionItemCollection();
				items.Add(new DesignerActionHeaderItem("Control Parameters", "Control Parameters"));
				items.Add(new DesignerActionPropertyItem("Dock", "Dock", "Control Parameters"));
				items.Add(new DesignerActionHeaderItem("Header Parameters", "Header Parameters"));
				items.Add(new DesignerActionPropertyItem("HeaderType", "Header type", "Header Parameters"));
				items.Add(new DesignerActionPropertyItem("HeaderText", "Header text", "Header Parameters"));
				items.Add(new DesignerActionPropertyItem("Checked", "Checked", "Header Parameters"));
				items.Add(new DesignerActionHeaderItem("Body Parameters", "Body Parameters"));
				items.Add(new DesignerActionPropertyItem("Collapsed", "Collapsed", "Body Parameters"));
				items.Add(new DesignerActionPropertyItem("UncollapsedHeight", "Uncollapsed height", "Body Parameters"));
				return items;
			}
		}
	}
}
