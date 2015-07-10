using System;
using System.Windows.Forms;

namespace Free.Controls
{
	internal class ReadOnlyControlCollection : Control.ControlCollection
	{
		public ReadOnlyControlCollection(Control owner, bool isReadOnly)
			: base(owner)
		{
			_isReadOnly=isReadOnly;
		}

		public override void Add(Control value)
		{
			if(IsReadOnly) throw new NotSupportedException("Collection is read only.");
			AddInternal(value);
		}

		internal virtual void AddInternal(Control value)
		{
			base.Add(value);
		}

		public override void Clear()
		{
			if(IsReadOnly) throw new NotSupportedException("Collection is read only.");
			base.Clear();
		}

		public override void RemoveByKey(string key)
		{
			if(IsReadOnly) throw new NotSupportedException("Collection is read only.");
			base.RemoveByKey(key);
		}

		internal virtual void RemoveInternal(Control value)
		{
			base.Remove(value);
		}

		private readonly bool _isReadOnly;
		public override bool IsReadOnly
		{
			get
			{
				return _isReadOnly;
			}
		}
	}
}
