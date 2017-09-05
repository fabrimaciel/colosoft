/* 
 * Colosoft Framework - generic framework to assist in development on the .NET platform
 * Copyright (C) 2013  <http://www.colosoft.com.br/framework> - support@colosoft.com.br
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.WebControls;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Security.Permissions;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Implementação de uma DropDownList customizada.
	/// </summary>
	[DefaultEvent("SelectedIndexChanged"), ParseChildren(true, "Items"), ControlValueProperty("SelectedValue"), AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class CustomListControl : DataBoundControl, IEditableTextControl, ITextControl
	{
		private bool _stateLoaded;

		private int cachedSelectedIndex;

		private ArrayList cachedSelectedIndices;

		private string cachedSelectedValue;

		private static readonly object EventSelectedIndexChanged;

		private static readonly object EventTextChanged;

		private ListItemCollection items;

		/// <summary>
		/// Evento acioando quando o texto do controle sofrer alguma alteração.
		/// </summary>
		public event EventHandler TextChanged {
			add
			{
				base.Events.AddHandler(EventTextChanged, value);
			}
			remove {
				base.Events.RemoveHandler(EventTextChanged, value);
			}
		}

		/// <summary>
		/// Tag que representa o controle.
		/// </summary>
		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Select;
			}
		}

		/// <summary>
		/// Nome do campo que será exibido como texto.
		/// </summary>
		[DefaultValue(""), Themeable(false)]
		public virtual string DataTextField
		{
			get
			{
				object obj2 = this.ViewState["DataTextField"];
				if(obj2 != null)
					return (string)obj2;
				return string.Empty;
			}
			set
			{
				this.ViewState["DataTextField"] = value;
				if(base.Initialized)
					base.RequiresDataBinding = true;
			}
		}

		/// <summary>
		/// Formatação para a exibição do texto.
		/// </summary>
		[DefaultValue(""), Themeable(false)]
		public virtual string DataTextFormatString
		{
			get
			{
				object obj2 = this.ViewState["DataTextFormatString"];
				if(obj2 != null)
				{
					return (string)obj2;
				}
				return string.Empty;
			}
			set
			{
				this.ViewState["DataTextFormatString"] = value;
				if(base.Initialized)
				{
					base.RequiresDataBinding = true;
				}
			}
		}

		/// <summary>
		/// Nome do campo que será usado como valor.
		/// </summary>
		[Themeable(false), DefaultValue("")]
		public virtual string DataValueField
		{
			get
			{
				object obj2 = this.ViewState["DataValueField"];
				if(obj2 != null)
					return (string)obj2;
				return string.Empty;
			}
			set
			{
				this.ViewState["DataValueField"] = value;
				if(base.Initialized)
					base.RequiresDataBinding = true;
			}
		}

		/// <summary>
		/// Itens da lista.
		/// </summary>
		[DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerDefaultProperty), Editor("System.Web.UI.Design.WebControls.ListItemsCollectionEditor,System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), MergableProperty(false)]
		public virtual ListItemCollection Items
		{
			get
			{
				if(items == null)
					items = new ListItemCollection();
				if(base.IsTrackingViewState)
					((IStateManager)items).TrackViewState();
				return items;
			}
		}

		/// <summary>
		/// Identifica se a lista suporta multi seleção.
		/// </summary>
		internal virtual bool IsMultiSelectInternal
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Nome do controle no cliente.
		/// </summary>
		[Themeable(false), DefaultValue("")]
		public virtual string ClientControlName
		{
			get
			{
				object obj2 = this.ViewState["ClientControlName"];
				if(obj2 != null)
					return (string)obj2;
				return string.Empty;
			}
			set
			{
				this.ViewState["ClientControlName"] = value;
				if(base.Initialized)
					base.RequiresDataBinding = true;
			}
		}

		/// <summary>
		/// Instancia do item selecionado.
		/// </summary>
		[DefaultValue((string)null), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual ListItem SelectedItem
		{
			get
			{
				int selectedIndex = this.SelectedIndex;
				if(selectedIndex >= 0)
					return this.Items[selectedIndex];
				return null;
			}
		}

		/// <summary>
		/// Valor do item selecionado.
		/// </summary>
		[Themeable(false), Bindable(true, BindingDirection.TwoWay), Browsable(false), DefaultValue(""), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string SelectedValue
		{
			get
			{
				int selectedIndex = this.SelectedIndex;
				if(selectedIndex >= 0)
					return this.Items[selectedIndex].Value;
				return string.Empty;
			}
			set
			{
				if(this.Items.Count != 0)
				{
					if((value == null) || (base.DesignMode && (value.Length == 0)))
					{
						this.ClearSelection();
						return;
					}
					ListItem item = this.Items.FindByValue(value);
					if((((this.Page != null) && this.Page.IsPostBack) && this._stateLoaded) && (item == null))
						throw new ArgumentOutOfRangeException("value", string.Format("The '{0}' and '{1}' attributes are mutually exclusive.", this.ID, "SelectedValue"));
					if(item != null)
					{
						this.ClearSelection();
						item.Selected = true;
					}
				}
				this.cachedSelectedValue = value;
			}
		}

		/// <summary>
		/// Index do item selecionado.
		/// </summary>
		[Browsable(false), Themeable(false), Bindable(true), DefaultValue(0), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual int SelectedIndex
		{
			get
			{
				for(int i = 0; i < this.Items.Count; i++)
				{
					if(this.Items[i].Selected)
					{
						return i;
					}
				}
				return -1;
			}
			set
			{
				if(value < -1)
				{
					if(this.Items.Count != 0)
						throw new ArgumentOutOfRangeException("value", string.Format("The '{0}' and '{1}' attributes are mutually exclusive.", this.ID, "SelectedIndex"));
					value = -1;
				}
				if(((this.Items.Count != 0) && (value < this.Items.Count)) || (value == -1))
				{
					this.ClearSelection();
					if(value >= 0)
					{
						this.Items[value].Selected = true;
					}
				}
				else if(this._stateLoaded)
					throw new ArgumentOutOfRangeException("value", string.Format("The '{0}' and '{1}' attributes are mutually exclusive.", this.ID, "SelectedIndex"));
				this.cachedSelectedIndex = value;
			}
		}

		/// <summary>
		/// Indices selecionados internamente.
		/// </summary>
		internal virtual ArrayList SelectedIndicesInternal
		{
			get
			{
				this.cachedSelectedIndices = new ArrayList(3);
				for(int i = 0; i < this.Items.Count; i++)
				{
					if(this.Items[i].Selected)
						this.cachedSelectedIndices.Add(i);
				}
				return this.cachedSelectedIndices;
			}
		}

		/// <summary>
		/// Texto controle.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(""), Themeable(false)]
		public virtual string Text
		{
			get
			{
				return this.SelectedValue;
			}
			set
			{
				this.SelectedValue = value;
			}
		}

		/// <summary>
		/// Construtor geral.
		/// </summary>
		static CustomListControl()
		{
			EventSelectedIndexChanged = new object();
			EventTextChanged = new object();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public CustomListControl()
		{
			this.cachedSelectedIndex = -1;
		}

		/// <summary>
		/// Recupera pelo valor na coleção informada.
		/// </summary>
		/// <param name="collection"></param>
		/// <param name="value"></param>
		/// <param name="includeDisabled"></param>
		/// <returns></returns>
		protected static int FindByValue(ListItemCollection collection, string value, bool includeDisabled)
		{
			int num = 0;
			foreach (ListItem item in collection)
			{
				if(item.Value.Equals(value) && (includeDisabled || item.Enabled))
					return num;
				num++;
			}
			return -1;
		}

		/// <summary>
		/// Identifica se é para salvar o estado dos indices.
		/// </summary>
		internal bool SaveSelectedIndicesViewState
		{
			get
			{
				if((((base.Events[EventSelectedIndexChanged] != null) || (base.Events[EventTextChanged] != null)) || (!base.IsEnabled || !this.Visible)))
					return true;
				foreach (ListItem item in this.Items)
					if(!item.Enabled)
						return true;
				Type type = base.GetType();
				return (((type != typeof(DropDownList)) && (type != typeof(ListBox))) && ((type != typeof(CheckBoxList)) && (type != typeof(RadioButtonList))));
			}
		}

		/// <summary>
		/// Identifica se é para anexa os itens.
		/// </summary>
		[DefaultValue(false), Themeable(false)]
		public virtual bool AppendDataBoundItems
		{
			get
			{
				object obj2 = this.ViewState["AppendDataBoundItems"];
				return ((obj2 != null) && ((bool)obj2));
			}
			set
			{
				this.ViewState["AppendDataBoundItems"] = value;
				if(base.Initialized)
					base.RequiresDataBinding = true;
			}
		}

		private static readonly System.Reflection.FieldInfo _writerAttrListFieldInfo = typeof(HtmlTextWriter).GetField("_attrList", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

		/// <summary>
		/// Aplica os atributos 
		/// </summary>
		/// <param name="writer"></param>
		protected override void AddAttributesToRender(HtmlTextWriter writer)
		{
			if(this.IsMultiSelectInternal)
				writer.AddAttribute(HtmlTextWriterAttribute.Multiple, "multiple");
			if(this.Enabled && !base.IsEnabled)
				writer.AddAttribute(HtmlTextWriterAttribute.Disabled, "disabled");
			base.AddAttributesToRender(writer);
		}

		public override string ClientID
		{
			get
			{
				this.EnsureID();
				string uniqueID = base.UniqueID;
				if((uniqueID != null) && (uniqueID.IndexOf(this.IdSeparator) >= 0))
					return uniqueID.Replace(this.IdSeparator, '_');
				return uniqueID;
			}
		}

		/// <summary>
		/// Identificador unico do controle.
		/// </summary>
		public override string UniqueID
		{
			get
			{
				if(!string.IsNullOrEmpty(ClientControlName))
					return ClientControlName;
				return base.UniqueID;
			}
		}

		/// <summary>
		/// Carrega o estado do controle.
		/// </summary>
		/// <param name="savedState"></param>
		protected override void LoadViewState(object savedState)
		{
			if(savedState != null)
			{
				Triplet triplet = (Triplet)savedState;
				base.LoadViewState(triplet.First);
				((IStateManager)this.Items).LoadViewState(triplet.Second);
				ArrayList third = triplet.Third as ArrayList;
				if(third != null)
					this.SelectInternal(third);
			}
			else
			{
				base.LoadViewState(null);
			}
			this._stateLoaded = true;
		}

		private static System.Reflection.MethodInfo _dataSourceViewExecuteSelect = typeof(DataSourceView).GetMethod("ExecuteSelect", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

		/// <summary>
		/// Realiza o binding dos dados
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			IEnumerable data = (IEnumerable)_dataSourceViewExecuteSelect.Invoke(this.GetData(), new object[] {
				DataSourceSelectArguments.Empty
			});
			this.PerformDataBinding(data);
		}

		/// <summary>
		/// Evento acionado quando o texto sofrer alteração.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnTextChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)base.Events[EventTextChanged];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando o index for selecionado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnSelectedIndexChanged(EventArgs e)
		{
			EventHandler handler = (EventHandler)base.Events[EventSelectedIndexChanged];
			if(handler != null)
				handler(this, e);
			this.OnTextChanged(e);
		}

		/// <summary>
		/// Executa o binding.
		/// </summary>
		/// <param name="dataSource"></param>
		protected override void PerformDataBinding(IEnumerable dataSource)
		{
			base.PerformDataBinding(dataSource);
			if(dataSource != null)
			{
				bool flag = false;
				bool flag2 = false;
				string dataTextField = this.DataTextField;
				string dataValueField = this.DataValueField;
				string dataTextFormatString = this.DataTextFormatString;
				if(!this.AppendDataBoundItems)
				{
					this.Items.Clear();
				}
				ICollection is2 = dataSource as ICollection;
				if(is2 != null)
				{
					this.Items.Capacity = is2.Count + this.Items.Count;
				}
				if((dataTextField.Length != 0) || (dataValueField.Length != 0))
				{
					flag = true;
				}
				if(dataTextFormatString.Length != 0)
				{
					flag2 = true;
				}
				foreach (object obj2 in dataSource)
				{
					ListItem item = new ListItem();
					if(flag)
					{
						if(dataTextField.Length > 0)
						{
							item.Text = DataBinder.GetPropertyValue(obj2, dataTextField, dataTextFormatString);
						}
						if(dataValueField.Length > 0)
						{
							item.Value = DataBinder.GetPropertyValue(obj2, dataValueField, null);
						}
					}
					else
					{
						if(flag2)
						{
							item.Text = string.Format(CultureInfo.CurrentCulture, dataTextFormatString, new object[] {
								obj2
							});
						}
						else
						{
							item.Text = obj2.ToString();
						}
						item.Value = obj2.ToString();
					}
					this.Items.Add(item);
				}
			}
			if(this.cachedSelectedValue != null)
			{
				int num = -1;
				num = FindByValue(this.Items, this.cachedSelectedValue, true);
				if(-1 == num)
					throw new ArgumentOutOfRangeException("value", string.Format("'{0}' has a {1} which is invalid because it does not exist in the list of items.", this.ID, "SelectedValue"));
				if((this.cachedSelectedIndex != -1) && (this.cachedSelectedIndex != num))
					throw new ArgumentException(string.Format("The '{0}' and '{1}' attributes are mutually exclusive.", "SelectedIndex", "SelectedValue"));
				this.SelectedIndex = num;
				this.cachedSelectedValue = null;
				this.cachedSelectedIndex = -1;
			}
			else if(this.cachedSelectedIndex != -1)
			{
				this.SelectedIndex = this.cachedSelectedIndex;
				this.cachedSelectedIndex = -1;
			}
		}

		/// <summary>
		/// Executa a consulta dos dados.
		/// </summary>
		protected override void PerformSelect()
		{
			this.OnDataBinding(EventArgs.Empty);
			base.RequiresDataBinding = false;
			base.MarkAsDataBound();
			this.OnDataBound(EventArgs.Empty);
		}

		/// <summary>
		/// Verifica multipla seleção.
		/// </summary>
		protected virtual void VerifyMultiSelect()
		{
			if(!this.IsMultiSelectInternal)
				throw new HttpException("Cant multiselect in single mode");
		}

		/// <summary>
		/// Define seleção dos dados postados.
		/// </summary>
		/// <param name="selectedIndex"></param>
		protected void SetPostDataSelection(int selectedIndex)
		{
			if((this.Items.Count != 0) && (selectedIndex < this.Items.Count))
			{
				this.ClearSelection();
				if(selectedIndex >= 0)
					this.Items[selectedIndex].Selected = true;
			}
		}

		/// <summary>
		/// Renderiza o conteúdo.
		/// </summary>
		/// <param name="writer"></param>
		protected override void RenderContents(HtmlTextWriter writer)
		{
			ListItemCollection items = this.Items;
			int count = items.Count;
			if(count > 0)
			{
				bool flag = false;
				for(int i = 0; i < count; i++)
				{
					ListItem item = items[i];
					if(item.Enabled)
					{
						writer.WriteBeginTag("option");
						if(item.Selected)
						{
							if(flag)
							{
								this.VerifyMultiSelect();
							}
							flag = true;
							writer.WriteAttribute("selected", "selected");
						}
						writer.WriteAttribute("value", item.Value, true);
						if(item.Attributes.Count > 0)
							item.Attributes.Render(writer);
						if(this.Page != null)
						{
							this.Page.ClientScript.RegisterForEventValidation(this.UniqueID, item.Value);
						}
						writer.Write('>');
						HttpUtility.HtmlEncode(item.Text, writer);
						writer.WriteEndTag("option");
						writer.WriteLine();
					}
				}
			}
		}

		/// <summary>
		/// Salva o view state do controle.
		/// </summary>
		/// <returns></returns>
		protected override object SaveViewState()
		{
			object x = base.SaveViewState();
			object y = ((IStateManager)this.Items).SaveViewState();
			object z = null;
			if(this.SaveSelectedIndicesViewState)
			{
				z = this.SelectedIndicesInternal;
			}
			if(((z == null) && (y == null)) && (x == null))
			{
				return null;
			}
			return new Triplet(x, y, z);
		}

		protected override void TrackViewState()
		{
			base.TrackViewState();
			((IStateManager)this.Items).TrackViewState();
		}

		/// <summary>
		/// Seleciona o indices informados.
		/// </summary>
		/// <param name="selectedIndices"></param>
		internal void SelectInternal(ArrayList selectedIndices)
		{
			this.ClearSelection();
			for(int i = 0; i < selectedIndices.Count; i++)
			{
				int num2 = (int)selectedIndices[i];
				if((num2 >= 0) && (num2 < this.Items.Count))
					Items[num2].Selected = true;
			}
			cachedSelectedIndices = selectedIndices;
		}

		/// <summary>
		/// Limpa as seleção da lista.
		/// </summary>
		public virtual void ClearSelection()
		{
			for(int i = 0; i < Items.Count; i++)
				Items[i].Selected = false;
		}
	}
}
