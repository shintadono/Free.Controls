using System;
using System.Globalization;
using System.Windows.Forms;

namespace Free.Controls
{
	internal class TypedControlCollection : ReadOnlyControlCollection
	{
		// Fields
		private Control ownerControl;
		private Type typeOfControl;

		// Methods
		public TypedControlCollection(Control owner, Type typeOfControl, bool isReadOnly=false)
			: base(owner, isReadOnly)
		{
			this.typeOfControl=typeOfControl;
			this.ownerControl=owner;
		}

		public override void Add(Control value)
		{
			if(value==null) throw new ArgumentNullException("value");

			if(IsReadOnly) throw new NotSupportedException("Collection is read only.");

			if(!typeOfControl.IsAssignableFrom(value.GetType()))
				throw new ArgumentException(string.Format("Controls added to this collection must be of type '{0}'.", typeOfControl.Name));

			base.Add(value);
		}
	}
}
