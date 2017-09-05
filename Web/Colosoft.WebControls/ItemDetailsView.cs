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
using System.Web.UI.WebControls;
using System.Web.UI;
using System.ComponentModel;
using System.Web.UI.Design.WebControls;
using System.Collections.Specialized;
using System.Collections;

namespace Colosoft.WebControls
{
	class ItemDetailsViewDesigner : DataBoundControlDesigner
	{
		protected override bool UsePreviewControl
		{
			get
			{
				return true;
			}
		}

		protected override int SampleRowCount
		{
			get
			{
				return 1;
			}
		}
	}
	/// <summary>
	/// Exibe os detalhes de um item.
	/// </summary>
	[Designer(typeof(ItemDetailsViewDesigner))]
	public class ItemDetailsView : DataBoundControl, INamingContainer
	{
		/// <summary>
		/// Classe para auxiliar a recuperar os dados.
		/// </summary>
		public class ItemDetailsViewDataItem : WebControl, IDataItemContainer, INamingContainer
		{
			private object _dataItem;

			public virtual object DataItem
			{
				get
				{
					return _dataItem;
				}
				set
				{
					_dataItem = value;
				}
			}

			public virtual int DataItemIndex
			{
				get;
				private set;
			}

			public virtual int DisplayIndex
			{
				get;
				private set;
			}

			public ItemDetailsViewDataItem()
			{
			}

			/// <summary>
			/// Método usado para montar o HTML do controle.
			/// </summary>
			/// <param name="writer"></param>
			protected override void Render(HtmlTextWriter writer)
			{
				foreach (Control c in Controls)
					c.RenderControl(writer);
			}
		}

		private ITemplate _itemTemplate;

		private ITemplate _emptyDataTemplate;

		private string[] _dataKeyNames;

		private OrderedDictionary _keyTable;

		private DataKey _dataKey;

		private System.Globalization.CultureInfo _cultureInfo = System.Globalization.CultureInfo.CurrentCulture;

		private ItemDetailsViewDataItem _dataItemContainer;

		/// <summary>
		/// Valores dos campo ligados.
		/// </summary>
		private IOrderedDictionary _boundFieldValues;

		/// <summary>
		/// Item que será visualizado.
		/// </summary>
		public object DataItem
		{
			get
			{
				if(DataSource is System.Collections.IEnumerable)
					foreach (var i in (System.Collections.IEnumerable)DataSource)
						return i;
				return null;
			}
			set
			{
				if(value == null)
					DataSource = null;
				else
					DataSource = new object[] {
						value
					};
			}
		}

		/// <summary>
		/// Cultura usada na instancia.
		/// </summary>
		public System.Globalization.CultureInfo CultureInfo
		{
			get
			{
				return _cultureInfo;
			}
			set
			{
				_cultureInfo = value;
			}
		}

		/// <summary>
		/// Template dos itens.
		/// </summary>
		[TemplateContainer(typeof(ItemDetailsViewDataItem), BindingDirection.TwoWay), DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate ItemTemplate
		{
			get
			{
				return _itemTemplate;
			}
			set
			{
				_itemTemplate = value;
			}
		}

		/// <summary>
		/// Template exibido quando não houver dados.
		/// </summary>
		[TemplateContainer(typeof(FormView)), Browsable(false), DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate EmptyDataTemplate
		{
			get
			{
				return this._emptyDataTemplate;
			}
			set
			{
				this._emptyDataTemplate = value;
			}
		}

		/// <summary>
		/// Nomes das chaves dos dados.
		/// </summary>
		[Editor("System.Web.UI.Design.WebControls.DataFieldEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), DefaultValue((string)null), Category("Data"), TypeConverter(typeof(StringArrayConverter)), Description("DataKeyNames")]
		public virtual string[] DataKeyNames
		{
			get
			{
				object obj2 = this._dataKeyNames;
				if(obj2 != null)
				{
					return (string[])((string[])obj2).Clone();
				}
				return new string[0];
			}
			set
			{
				if(!DataBoundControlHelper.CompareStringArrays(value, this.DataKeyNamesInternal))
				{
					if(value != null)
					{
						this._dataKeyNames = (string[])value.Clone();
					}
					else
					{
						this._dataKeyNames = null;
					}
					this._keyTable = null;
					if(base.Initialized)
					{
						base.RequiresDataBinding = true;
					}
				}
			}
		}

		/// <summary>
		/// Nomes das chaves dos dados.
		/// </summary>
		private string[] DataKeyNamesInternal
		{
			get
			{
				object obj2 = this._dataKeyNames;
				if(obj2 != null)
				{
					return (string[])obj2;
				}
				return new string[0];
			}
		}

		/// <summary>
		/// Lista das chaves dad tabela da instancia.
		/// </summary>
		private OrderedDictionary KeyTable
		{
			get
			{
				if(this._keyTable == null)
				{
					this._keyTable = new OrderedDictionary(this.DataKeyNamesInternal.Length);
				}
				return this._keyTable;
			}
		}

		/// <summary>
		/// Chave de dados da tabela.
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual DataKey DataKey
		{
			get
			{
				if(this._dataKey == null)
					this._dataKey = new DataKey(this.KeyTable);
				return this._dataKey;
			}
		}

		/// <summary>
		/// Valores dos campos ligados.
		/// </summary>
		private IOrderedDictionary BoundFieldValues
		{
			get
			{
				if(this._boundFieldValues == null)
				{
					int capacity = 0x19;
					this._boundFieldValues = new OrderedDictionary(capacity);
				}
				return this._boundFieldValues;
			}
		}

		public ItemDetailsView() : base()
		{
			_dataItemContainer = new ItemDetailsViewDataItem();
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			this.Controls.Add(_dataItemContainer);
		}

		protected override void PerformDataBinding(System.Collections.IEnumerable data)
		{
			base.PerformDataBinding(data);
			if(data == null && this.DataItem != null)
				data = new object[] {
					this.DataItem
				};
			else if(data == null)
				return;
			foreach (var i in data)
			{
				_dataItemContainer.DataItem = i;
				if(i == null && EmptyDataTemplate != null)
					EmptyDataTemplate.InstantiateIn(_dataItemContainer);
				else
					ItemTemplate.InstantiateIn(_dataItemContainer);
				_dataItemContainer.DataBind();
				if(i != null)
				{
					OrderedDictionary keyTable = this.KeyTable;
					if(keyTable != null)
					{
						keyTable.Clear();
						string[] dataKeyNamesInternal = this.DataKeyNamesInternal;
						if(dataKeyNamesInternal != null && dataKeyNamesInternal.Length != 0)
						{
							foreach (string str in dataKeyNamesInternal)
							{
								object propertyValue = DataBinder.GetPropertyValue(i, str);
								keyTable.Add(str, propertyValue);
							}
							_dataKey = new DataKey(keyTable);
						}
					}
				}
			}
			this.ExtractRowValues(this.BoundFieldValues, false);
		}

		/// <summary>
		/// Extrai os valores da linha da instancia.
		/// </summary>
		/// <param name="fieldValues"></param>
		/// <param name="includeKeys"></param>
		protected virtual void ExtractRowValues(IOrderedDictionary fieldValues, bool includeKeys)
		{
			if(fieldValues != null)
			{
				DataBoundControlHelper.ExtractValuesFromBindableControls(fieldValues, this);
				IBindableTemplate itemTemplate = null;
				itemTemplate = this.ItemTemplate as IBindableTemplate;
				string[] dataKeyNamesInternal = this.DataKeyNamesInternal;
				if(itemTemplate != null)
				{
					if((_dataItemContainer != null) && (itemTemplate != null))
					{
						foreach (DictionaryEntry entry in itemTemplate.ExtractValues(_dataItemContainer))
						{
							if(includeKeys || (Array.IndexOf(dataKeyNamesInternal, entry.Key) == -1))
							{
								fieldValues[entry.Key] = entry.Value;
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Método usado para montar o HTML do controle.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Render(HtmlTextWriter writer)
		{
			foreach (Control c in Controls)
				c.RenderControl(writer);
		}

		private static bool IsNullableType(Type type)
		{
			return (type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(Nullable<>)));
		}

		/// <summary>
		/// Recupera os novos valores que foram alterados.
		/// </summary>
		/// <returns></returns>
		public IOrderedDictionary GetNewValues()
		{
			var newValues = new OrderedDictionary();
			this.ExtractRowValues(newValues, true);
			foreach (DictionaryEntry entry2 in this.DataKey.Values)
				newValues.Add(entry2.Key, entry2.Value);
			return newValues;
		}

		/// <summary>
		/// Aplica as modificações no item de dados.
		/// </summary>
		public void ApplyDataItemChanges()
		{
			if(DataItem == null)
				return;
			var instanceProperties = DataItem.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
			foreach (DictionaryEntry i in GetNewValues())
			{
				var property = Array.Find(instanceProperties, f => f.CanWrite && f.Name == (string)i.Key);
				if(property != null)
				{
					if(i.Value == null || (i.Value is string && string.IsNullOrEmpty(i.Value as string)))
						property.SetValue(DataItem, null, null);
					else if(IsNullableType(property.PropertyType) && (i.Value == null || (i.Value is string && string.IsNullOrEmpty(i.Value as string))))
						property.SetValue(DataItem, null, null);
					else
						property.SetValue(DataItem, Convert.ChangeType(i.Value, IsNullableType(property.PropertyType) ? Nullable.GetUnderlyingType(property.PropertyType) : property.PropertyType, this.CultureInfo), null);
				}
			}
		}
	}
}
