using System;

namespace Free.Controls.CrumbBar
{
	public class CrumbBarClickEventArgs : EventArgs
	{
		/// <summary>
		/// Gets the index of the clicked crumb in the nesting.
		/// </summary>
		public int Index { get; private set; }

		/// <summary>
		/// Creates a new instance of the CrumbClickEventArgs class with the necessary parameters.
		/// </summary>
		/// <param name="index">The index of the clicked crumb.</param>
		/// <param name="checkd">Was the crumb checked before it was clicked?</param>
		/// <param name="checksonclick">Is the crumb supposed to change it's checked state when clicked?</param>
		/// <param name="sender">The clicked crumb.</param>
		internal CrumbBarClickEventArgs(int index)
		{
			Index=index;
		}
	}
}
