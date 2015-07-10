using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Free.Controls
{
	/// <summary>
	/// Represents a Windows control that displays a frame around
	/// a group of controls with an optional caption and checkbox.
	/// </summary>
	[DefaultEvent("CheckedChanged")]
	public class CheckGroupBox : GroupBox
	{
		private CheckBox m_checkBox;
		private bool m_contentsEnabled=true;
		private CheckGroupBoxCheckAction m_checkAction=CheckGroupBoxCheckAction.Enable;

		/// <summary>
		/// Initializes a new instance of CheckGroupBox class.
		/// </summary>
		public CheckGroupBox()
		{
			SuspendLayout();

			//base.Text="";

			m_checkBox=new CheckBox();
			m_checkBox.AutoSize=true;
			m_checkBox.Location=new Point(8, 0);
			m_checkBox.Padding=new Padding(3, 0, 0, 0);
			m_checkBox.Checked=true;
			m_checkBox.TextAlign=ContentAlignment.MiddleLeft;
			m_checkBox.CheckedChanged+=new EventHandler(CheckBox_CheckedChanged);
			Controls.Add(m_checkBox);

			ResumeLayout(true);
		}

		#region Public Properties
		/// <summary>
		/// Gets or sets a value indicating whether the
		/// CheckGroupBox is in the checked state.
		/// </summary>
		[Category("Appearance")]
		[DefaultValue(true)]
		public bool Checked
		{
			get { return m_checkBox.Checked; }
			set { m_checkBox.Checked=value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the controls
		/// contained inside this container can respond to user
		/// interaction.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(true)]
		public bool ContentsEnabled
		{
			get { return m_contentsEnabled; }
			set
			{
				m_contentsEnabled=value;
				OnContentsEnabledChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// The text associated with the control.
		/// </summary>
		public override string Text
		{
			get { return m_checkBox!=null?m_checkBox.Text:""; }
			set { if(m_checkBox!=null) m_checkBox.Text=value; }
		}

		/// <summary>
		/// Gets or sets a value indicating how a CheckGroupBox
		/// should behave when its CheckBox is in the checked state.
		/// </summary>
		[Category("Behavior")]
		[DefaultValue(CheckGroupBoxCheckAction.Enable)]
		public CheckGroupBoxCheckAction CheckAction
		{
			get { return m_checkAction; }
			set
			{
				m_checkAction=value;
				OnCheckedChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets the underlying CheckBox control contained
		/// in the CheckGroupBox control.
		/// </summary>
		[Category("Misc")]
		public CheckBox CheckBox
		{
			get { return m_checkBox; }
		}
		#endregion

		public event EventHandler CheckedChanged;

		#region Event Handling
		/// <summary>
		/// CheckGroupBox.CheckBox CheckedChanged event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnCheckedChanged(EventArgs e)
		{
			if(m_checkAction==CheckGroupBoxCheckAction.None) return;

			// Toggle action depending on the value of checkAction.
			ContentsEnabled=(m_checkAction==CheckGroupBoxCheckAction.Disable)^m_checkBox.Checked;
		}

		/// <summary>
		/// ContentsEnabled Changed event.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnContentsEnabledChanged(EventArgs e)
		{
			SuspendLayout();
			foreach(Control control in Controls)
			{
				if(control==m_checkBox) continue;

				// Set action for every control, except for
				// the CheckBox, which should remain intact.
				control.Enabled=m_contentsEnabled;
			}
			ResumeLayout(true);
		}

		private void CheckBox_CheckedChanged(object sender, EventArgs e)
		{
			OnCheckedChanged(e);

			// Raise event for user of control
			if(CheckedChanged!=null) CheckedChanged(this, e);
		}
		#endregion
	}
}
