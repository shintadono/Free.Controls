using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Free.Controls
{
	public class DataGridViewNumericUpDownColumn : DataGridViewColumn
	{
		DataGridViewNumericUpDownCell Template;

		public DataGridViewNumericUpDownColumn() : base(new DataGridViewNumericUpDownCell())
		{
			Template=CellTemplate as DataGridViewNumericUpDownCell;
		}

		public override object Clone()
		{
			object o=base.Clone();
			DataGridViewNumericUpDownColumn c=o as DataGridViewNumericUpDownColumn;
			if(c==null) return o;

			c.DecimalPlaces=DecimalPlaces;
			c.Hexadecimal=Hexadecimal;
			c.Increment=Increment;
			c.Minimum=Minimum;
			c.Maximum=Maximum;

			return c;
		}

		[Category("NumericUpDown"), Description("Decimal places.")]
		[DefaultValue(0)]
		public int DecimalPlaces
		{
			get { return Template.DecimalPlaces; }
			set { Template.DecimalPlaces=value; }
		}

		[Category("NumericUpDown"), Description("Show value hexadecimal.")]
		[DefaultValue(false)]
		public bool Hexadecimal
		{
			get { return Template.Hexadecimal; }
			set { Template.Hexadecimal=value; }
		}

		[Category("NumericUpDown"), Description("Increment value.")]
		[DefaultValue(typeof(decimal), "1")]
		public decimal Increment
		{
			get { return Template.Increment; }
			set { Template.Increment=value; }
		}

		[Category("NumericUpDown"), Description("Minimum value.")]
		[DefaultValue(typeof(decimal), "0")]
		public decimal Minimum
		{
			get { return Template.Minimum; }
			set { Template.Minimum=value; }
		}

		[Category("NumericUpDown"), Description("Maximum value.")]
		[DefaultValue(typeof(decimal), "100")]
		public decimal Maximum
		{
			get { return Template.Maximum; }
			set { Template.Maximum=value; }
		}
	}

	public class DataGridViewNumericUpDownCell : DataGridViewTextBoxCell
	{
		public DataGridViewNumericUpDownCell() : base()
		{
			DecimalPlaces=0;
			Hexadecimal=false;
			Increment=1;
			Minimum=0;
			Maximum=100;
		}

		public override object Clone()
		{
			object o=base.Clone();
			DataGridViewNumericUpDownCell c=o as DataGridViewNumericUpDownCell;
			if(c==null) return o;

			c.DecimalPlaces=DecimalPlaces;
			c.Hexadecimal=Hexadecimal;
			c.Increment=Increment;
			c.Minimum=Minimum;
			c.Maximum=Maximum;

			return c;
		}

		public override Type EditType { get { return typeof(DataGridViewNumericUpDownControl); } }

		public override Type ValueType { get { return typeof(decimal); } }

		public override object DefaultNewRowValue { get { return 0; } }

		public override Type FormattedValueType { get { return typeof(decimal); } }

		protected override object GetFormattedValue(object value, int rowIndex, ref DataGridViewCellStyle cellStyle,
			TypeConverter valueTypeConverter, TypeConverter formattedValueTypeConverter, DataGridViewDataErrorContexts context)
		{
			return value.ToString();
		}

		public int DecimalPlaces { get; set; }
		public bool Hexadecimal { get; set; }
		public decimal Increment { get; set; }
		public decimal Minimum { get; set; }
		public decimal Maximum { get; set; }

		public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
		{
			base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);

			DataGridViewNumericUpDownControl nud=(DataGridViewNumericUpDownControl)this.DataGridView.EditingControl;

			if(Maximum<Minimum)
			{
				decimal tmp=Maximum;
				Maximum=Minimum;
				Minimum=tmp;
			}

			nud.BeginInit();
			nud.DecimalPlaces=DecimalPlaces;
			nud.Hexadecimal=Hexadecimal;
			nud.Maximum=Maximum;
			nud.Minimum=Minimum;
			switch(dataGridViewCellStyle.Alignment)
			{
				case DataGridViewContentAlignment.BottomLeft:
				case DataGridViewContentAlignment.MiddleLeft:
				case DataGridViewContentAlignment.TopLeft:
					nud.TextAlign=HorizontalAlignment.Left;break;
				case DataGridViewContentAlignment.BottomCenter:
				case DataGridViewContentAlignment.MiddleCenter:
				case DataGridViewContentAlignment.TopCenter:
					nud.TextAlign=HorizontalAlignment.Center; break;
				case DataGridViewContentAlignment.BottomRight:
				case DataGridViewContentAlignment.MiddleRight:
				case DataGridViewContentAlignment.TopRight:
				case DataGridViewContentAlignment.NotSet:
				default:
					nud.TextAlign=HorizontalAlignment.Right; break;
			}
			nud.EndInit();

			decimal v=Minimum;
			if(!decimal.TryParse(this.Value.ToString(), out v)) v=Minimum;
			if(v<Minimum) v=Minimum;
			if(v>Maximum) v=Maximum;
			nud.Value=v;
		}
	}

	public class DataGridViewNumericUpDownControl : NumericUpDown, IDataGridViewEditingControl
	{
		private DataGridView _dataGridView;
		private int _rowIndex;
		private bool _valueChanged=false;

		#region IDataGridViewEditingControl Members
		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
		{
			this.Font=dataGridViewCellStyle.Font;
			this.ForeColor=dataGridViewCellStyle.ForeColor;
			this.BackColor=dataGridViewCellStyle.BackColor;
		}

		public DataGridView EditingControlDataGridView
		{
			get
			{
				return this._dataGridView;
			}
			set
			{
				this._dataGridView=value;
			}
		}

		public object EditingControlFormattedValue
		{
			get
			{
				return this.Value;
			}
			set
			{
				this.Value=decimal.Parse(value.ToString());
			}
		}

		public int EditingControlRowIndex
		{
			get
			{
				return this._rowIndex;
			}
			set
			{
				this._rowIndex=value;
			}
		}

		public bool EditingControlValueChanged
		{
			get
			{
				return this._valueChanged;
			}
			set
			{
				this._valueChanged=value;
			}
		}

		public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
		{
			switch(keyData)
			{
				case Keys.Up:
				case Keys.Down: return true;
				default: return false;
			}
		}

		public Cursor EditingPanelCursor { get { return base.Cursor; } }

		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
		{
			return this.EditingControlFormattedValue;
		}

		public void PrepareEditingControlForEdit(bool selectAll) { }

		public bool RepositionEditingControlOnValueChange { get { return false; } }

		protected override void OnValueChanged(EventArgs e)
		{
			this._valueChanged=true;
			this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
			base.OnValueChanged(e);
		}
		#endregion
	}
}
