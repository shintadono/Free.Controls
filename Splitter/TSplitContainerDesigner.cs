using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Behavior;

namespace Free.Controls.Splitter
{
	class TSplitContainerDesigner : ParentControlDesigner
	{
		// Fields
		private IDesignerHost designerHost;
		private bool disabledGlyphs;
		private bool disableDrawGrid;
		private int initialSplitterDist;
		private static int numberOfSplitterPanels=2;
		private const string panel1Name="Panel1";
		private const string panel2Name="Panel2";
		private TSplitterPanel selectedPanel;
		private TSplitContainer splitContainer;
		private bool splitContainerSelected;
		private bool splitterDistanceException;
		private TSplitterPanel splitterPanel1;
		private TSplitterPanel splitterPanel2;

		// Methods
		public override bool CanParent(Control control)
		{
			return false;
		}

		protected override IComponent[] CreateToolCore(ToolboxItem tool, int x, int y, int width, int height, bool hasLocation, bool hasSize)
		{
			if(this.Selected==null)
			{
				this.Selected=this.splitterPanel1;
			}
			TSplitterPanelDesigner toInvoke=(TSplitterPanelDesigner)this.designerHost.GetDesigner(this.Selected);
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
			this.splitContainer.MouseDown-=new MouseEventHandler(this.OnSplitContainer);
			this.splitContainer.SplitterMoved-=new SplitterEventHandler(this.OnSplitterMoved);
			this.splitContainer.SplitterMoving-=new SplitterCancelEventHandler(this.OnSplitterMoving);
			this.splitContainer.DoubleClick-=new EventHandler(this.OnSplitContainerDoubleClick);
			base.Dispose(disposing);
		}

		//protected override ControlBodyGlyph GetControlGlyph(GlyphSelectionType selectionType)
		//{
		//    ControlBodyGlyph glyph=null;
		//    SelectionManager service=(SelectionManager)this.GetService(typeof(SelectionManager));
		//    if(service!=null)
		//    {
		//        Rectangle bounds=base.BehaviorService.ControlRectInAdornerWindow(this.splitterPanel1);
		//        TSplitterPanelDesigner designer=this.designerHost.GetDesigner(this.splitterPanel1) as TSplitterPanelDesigner;
		//        this.OnSetCursor();
		//        if(designer!=null)
		//        {
		//            glyph=new ControlBodyGlyph(bounds, Cursor.Current, this.splitterPanel1, designer);
		//            service.BodyGlyphAdorner.Glyphs.Add(glyph);
		//        }
		//        bounds=base.BehaviorService.ControlRectInAdornerWindow(this.splitterPanel2);
		//        designer=this.designerHost.GetDesigner(this.splitterPanel2) as TSplitterPanelDesigner;
		//        if(designer!=null)
		//        {
		//            glyph=new ControlBodyGlyph(bounds, Cursor.Current, this.splitterPanel2, designer);
		//            service.BodyGlyphAdorner.Glyphs.Add(glyph);
		//        }
		//    }
		//    return base.GetControlGlyph(selectionType);
		//}

		protected override bool GetHitTest(Point point)
		{
			return ((this.InheritanceAttribute!=InheritanceAttribute.InheritedReadOnly)&&this.splitContainerSelected);
		}

		protected override Control GetParentForComponent(IComponent component)
		{
			return this.splitterPanel1;
		}

		public override void Initialize(IComponent component)
		{
			base.Initialize(component);
			base.AutoResizeHandles=true;
			this.splitContainer=component as TSplitContainer;
			this.splitterPanel1=this.splitContainer.Panel1;
			this.splitterPanel2=this.splitContainer.Panel2;
			base.EnableDesignMode(this.splitContainer.Panel1, "Panel1");
			base.EnableDesignMode(this.splitContainer.Panel2, "Panel2");
			this.designerHost=(IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			if(this.selectedPanel==null)
			{
				this.Selected=this.splitterPanel1;
			}
			this.splitContainer.MouseDown+=new MouseEventHandler(this.OnSplitContainer);
			this.splitContainer.SplitterMoved+=new SplitterEventHandler(this.OnSplitterMoved);
			this.splitContainer.SplitterMoving+=new SplitterCancelEventHandler(this.OnSplitterMoving);
			this.splitContainer.DoubleClick+=new EventHandler(this.OnSplitContainerDoubleClick);
			ISelectionService service=(ISelectionService)this.GetService(typeof(ISelectionService));
			if(service!=null)
			{
				service.SelectionChanged+=new EventHandler(this.OnSelectionChanged);
			}
		}

		public override ControlDesigner InternalControlDesigner(int internalControlIndex)
		{
			TSplitterPanel panel;
			switch(internalControlIndex)
			{
				case 0:
					panel=this.splitterPanel1;
					break;

				case 1:
					panel=this.splitterPanel2;
					break;

				default:
					return null;
			}
			return (this.designerHost.GetDesigner(panel) as ControlDesigner);
		}

		public override int NumberOfInternalControlDesigners()
		{
			return numberOfSplitterPanels;
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
			this.splitContainerSelected=false;
			if(service!=null)
			{
				foreach(object obj2 in service.GetSelectedComponents())
				{
					TSplitterPanel panel=obj2 as TSplitterPanel;
					if((panel!=null)&&(panel.Parent==this.splitContainer))
					{
						this.splitContainerSelected=false;
						this.Selected=panel;
						break;
					}
					this.Selected=null;
					if(obj2==this.splitContainer)
					{
						this.splitContainerSelected=true;
						break;
					}
				}
			}
		}

		private void OnSplitContainer(object sender, MouseEventArgs e)
		{
			((ISelectionService)this.GetService(typeof(ISelectionService))).SetSelectedComponents(new object[] { this.Control });
		}

		private void OnSplitContainerDoubleClick(object sender, EventArgs e)
		{
			if(this.splitContainerSelected)
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

		private void OnSplitterMoved(object sender, SplitterEventArgs e)
		{
			if((this.InheritanceAttribute!=InheritanceAttribute.InheritedReadOnly)&&!this.splitterDistanceException)
			{
				try
				{
					base.RaiseComponentChanging(TypeDescriptor.GetProperties(this.splitContainer)["SplitterDistance"]);
					base.RaiseComponentChanged(TypeDescriptor.GetProperties(this.splitContainer)["SplitterDistance"], null, null);
					if(this.disabledGlyphs)
					{
						BehaviorServiceAdornerCollectionEnumerator enumerator=base.BehaviorService.Adorners.GetEnumerator();
						while(enumerator.MoveNext())
						{
							enumerator.Current.Enabled=true;
						}

						//SelectionManager service=(SelectionManager)this.GetService(typeof(SelectionManager));
						//if(service!=null)
						//{
						//    service.Refresh();
						//}
						this.disabledGlyphs=false;
					}
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
							this.splitterDistanceException=true;
							this.splitContainer.SplitterDistance=this.initialSplitterDist;
							return;
						}
						finally
						{
							this.splitterDistanceException=false;
						}
					}
					throw;
				}
			}
		}

		private void OnSplitterMoving(object sender, SplitterCancelEventArgs e)
		{
			this.initialSplitterDist=this.splitContainer.SplitterDistance;
			if(this.InheritanceAttribute!=InheritanceAttribute.InheritedReadOnly)
			{
				this.disabledGlyphs=true;
				//Adorner bodyGlyphAdorner=null;
				//SelectionManager service=(SelectionManager)this.GetService(typeof(SelectionManager));
				//if(service!=null)
				//{
				//    bodyGlyphAdorner=service.BodyGlyphAdorner;
				//}
				//foreach(Adorner adorner2 in base.BehaviorService.Adorners)
				//{
				//    if((bodyGlyphAdorner==null)||!adorner2.Equals(bodyGlyphAdorner))
				//    {
				//        adorner2.Enabled=false;
				//    }
				//}
				//ArrayList list=new ArrayList();
				//foreach(ControlBodyGlyph glyph in bodyGlyphAdorner.Glyphs)
				//{
				//    if(!(glyph.RelatedComponent is TSplitterPanel))
				//    {
				//        list.Add(glyph);
				//    }
				//}
				//foreach(Glyph glyph2 in list)
				//{
				//    bodyGlyphAdorner.Glyphs.Remove(glyph2);
				//}
			}
		}

		internal void SplitterPanelHover()
		{
			this.OnMouseHover();
		}

		// Properties
		public override DesignerActionListCollection ActionLists
		{
			get
			{
				DesignerActionListCollection lists=new DesignerActionListCollection();
				OrientationActionList list=new OrientationActionList(this);
				lists.Add(list);
				return lists;
			}
		}

		protected override bool AllowControlLasso { get { return false; } }

		public override ICollection AssociatedComponents
		{
			get
			{
				ArrayList list=new ArrayList();
				foreach(TSplitterPanel panel in this.splitContainer.Controls)
				{
					foreach(Control control in panel.Controls)
					{
						list.Add(control);
					}
				}
				return list;
			}
		}

		protected override bool DrawGrid { get { return disableDrawGrid?false:base.DrawGrid; } }

		internal TSplitterPanel Selected
		{
			get
			{
				return this.selectedPanel;
			}
			set
			{
				if(this.selectedPanel!=null)
				{
					TSplitterPanelDesigner designer=(TSplitterPanelDesigner)this.designerHost.GetDesigner(this.selectedPanel);
					designer.Selected=false;
				}
				if(value!=null)
				{
					TSplitterPanelDesigner designer2=(TSplitterPanelDesigner)this.designerHost.GetDesigner(value);
					this.selectedPanel=value;
					designer2.Selected=true;
				}
				else if(this.selectedPanel!=null)
				{
					TSplitterPanelDesigner designer3=(TSplitterPanelDesigner)this.designerHost.GetDesigner(this.selectedPanel);
					this.selectedPanel=null;
					designer3.Selected=false;
				}
			}
		}

		public override IList SnapLines { get { return base.SnapLines as ArrayList; } }

		// Nested Types
		private class OrientationActionList : DesignerActionList
		{
			// Fields
			private string actionName;
			private TSplitContainerDesigner owner;
			private Component ownerComponent;

			// Methods
			public OrientationActionList(TSplitContainerDesigner owner)
				: base(owner.Component)
			{
				this.owner=owner;
				this.ownerComponent=owner.Component as Component;
				if(this.ownerComponent!=null)
				{
					PropertyDescriptor descriptor=TypeDescriptor.GetProperties(this.ownerComponent)["Orientation"];
					if(descriptor!=null)
					{
						bool flag=((Orientation)descriptor.GetValue(this.ownerComponent))==Orientation.Horizontal;
						this.actionName=flag?"Vertical":"Horizontal";
					}
				}
			}

			public override DesignerActionItemCollection GetSortedActionItems()
			{
				DesignerActionItemCollection items=new DesignerActionItemCollection();
				items.Add(new DesignerActionEventHandlerItem(this.actionName, new EventHandler(this.OnOrientationActionClick)));
				//items.Add(new DesignerActionMethodItem(this,
				//                  "InvertColors", "Invert Colors",
				//                  "Appearance",
				//                  "Inverts the fore and background colors.",
				//                   false));
				return items;
			}

			private void OnOrientationActionClick(object sender, EventArgs e)
			{
				DesignerActionItem verb=sender as DesignerActionItem;
				if(verb!=null)
				{
					Orientation orientation=verb.DisplayName.Equals("Horizontal")?Orientation.Horizontal:Orientation.Vertical;
					this.actionName=orientation==Orientation.Horizontal?"Vertical":"Horizontal";
					PropertyDescriptor descriptor=TypeDescriptor.GetProperties(this.ownerComponent)["Orientation"];
					if((descriptor!=null)&&(((Orientation)descriptor.GetValue(this.ownerComponent))!=orientation))
					{
						descriptor.SetValue(this.ownerComponent, orientation);
					}
					DesignerActionUIService service=(DesignerActionUIService)this.owner.GetService(typeof(DesignerActionUIService));
					if(service!=null)
					{
						service.Refresh(this.ownerComponent);
					}
				}
			}

			internal class DesignerActionEventHandlerItem : DesignerActionMethodItem
			{
				// Fields
				private EventHandler execHandler;

				// Methods
				public DesignerActionEventHandlerItem(string text, EventHandler handler) : base(null, null, text, "Verbs", "", false)
				{
					execHandler=handler;
				}

				public override void Invoke()
				{
					if(execHandler!=null)
					{
						try
						{
							execHandler(this, EventArgs.Empty);
						}
						catch(CheckoutException exception)
						{
							if(exception!=CheckoutException.Canceled)
							{
								throw;
							}
						}
					}
				}
			}
		}
	}
}
