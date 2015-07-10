using System;
using System.Windows.Forms;

namespace Free.Controls
{
	/// <summary>
	/// ComboBox, die es ermöglicht, die darin enthaltenen
	/// Einträge zu suchen, jedoch keine neuen anzulegen.
	/// </summary>
	public class SearchableComboBox : ComboBox
	{
		/// <summary>
		/// Initialisiert eine durchsuchbare ComboBox.
		/// </summary>
		public SearchableComboBox()
		{
			// Autovervollständigung einschalten
			this.AutoCompleteMode=AutoCompleteMode.SuggestAppend;
			this.AutoCompleteSource=AutoCompleteSource.ListItems;

			// Einfügen von Text mittels Kontextmenüs verhindern
			this.ContextMenu=new ContextMenu();

			// Einzelne Tastendrücke abfangen
			this.KeyPress+=new KeyPressEventHandler(SearchableComboBox_KeyPress);

			//Leave Event abfangen (um Autocomplete zu aktivieren)
			this.Leave+=new EventHandler(SearchableComboBox_Leave);
		}

		/// <summary>
		/// Wird aufgerufen, wenn eine Taste gedrückt wird.
		/// Überprüft, ob es sich um eine gültige Taste handelt.
		/// </summary>
		private void SearchableComboBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			// Bei der Löschtaste die Methode verlassen
			if(e.KeyChar=='\b') return;

			string searchString=String.Concat(this.SelectedText!=""?this.Text.Replace(this.SelectedText, ""):this.Text, e.KeyChar.ToString());
			bool success=false;

			// Alle Einträge überprüfen, ob ein passender gefunden werden kann
			foreach(object item in this.Items)
			{
				if(item.ToString().StartsWith(searchString))
				{
					success=true;
					break;
				}
			}

			// Wenn es nicht erfolgreich war, den Tastendruck verhindern
			if(!success) e.Handled=true;
		}

		/// <summary>
		/// Sorgt dafür das beim verlassen der Box ein gültiger Eintrag ausgewählt wird.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SearchableComboBox_Leave(object sender, System.EventArgs e)
		{
			string searchString=this.Text;

			foreach(object item in this.Items)
			{
				if(item.ToString().StartsWith(searchString))
				{
					//success=true;
					this.SelectedItem=item;
					break;
				}
			}
		}

		/// <summary>
		/// Verhindert das Einfügen von Text mithilfe von Strg + V.
		/// </summary>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(keyData==(Keys.Control|Keys.V))
			{
				return true;
			}
			else
			{
				return base.ProcessCmdKey(ref msg, keyData);
			}
		}
	}
}
