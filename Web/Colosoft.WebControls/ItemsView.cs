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
using System.Web.UI.HtmlControls;
using System.Collections;
using System.Web.UI.Design;
using System.Web.UI.Design.WebControls;

namespace Colosoft.WebControls
{
	class ItemsViewDesigner : DataBoundControlDesigner
	{
		public override string GetDesignTimeHtml()
		{
			var div = new HtmlGenericControl("div");
			div.Style.Add(HtmlTextWriterStyle.Padding, "5px 5px 5px 5px");
			div.Style.Add(HtmlTextWriterStyle.Width, "50px");
			div.Style.Add(HtmlTextWriterStyle.FontFamily, "Arial");
			div.Style.Add(HtmlTextWriterStyle.FontSize, "10pt");
			div.InnerText = "ItemsView";
			var sb = new StringBuilder();
			var writer = new HtmlTextWriter(new System.IO.StringWriter(sb));
			div.RenderControl(writer);
			return sb.ToString();
		}

		protected override bool UsePreviewControl
		{
			get
			{
				return true;
			}
		}
	}
	/// <summary>
	/// Representa uma coluna .
	/// </summary>
	public class ItemsViewColumn : IStateManager
	{
		private TableItemStyle _columnStyle;

		private bool _marked;

		private StateBag _statebag = null;

		private ItemsView _owner;

		/// <summary>
		/// Estilo da coluna.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty), Category("Styles"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string)null)]
		public virtual TableItemStyle ColumnStyle
		{
			get
			{
				if(this._columnStyle == null)
				{
					this._columnStyle = new TableItemStyle();
					if(this.IsTrackingViewState)
					{
						((IStateManager)this._columnStyle).TrackViewState();
					}
				}
				return this._columnStyle;
			}
		}

		/// <summary>
		/// ViewState da coluna.
		/// </summary>
		protected StateBag ViewState
		{
			get
			{
				return _statebag;
			}
			set
			{
				_statebag = new StateBag();
			}
		}

		/// <summary>
		/// Obtém um valor que determina se o objeto ItemsViewColumn está marcado para salvar seu estado.
		/// </summary>
		public bool IsTrackingViewState
		{
			get
			{
				return this._marked;
			}
		}

		/// <summary>
		/// Define o elemento pai da coluna.
		/// </summary>
		/// <param name="owner"></param>
		internal void SetOwner(ItemsView owner)
		{
			_owner = owner;
		}

		/// <summary>
		/// Salva os dados no viewstate.
		/// </summary>
		/// <returns></returns>
		protected virtual object SaveViewState()
		{
			object obj2 = ((IStateManager)this.ViewState).SaveViewState();
			object obj3 = (_columnStyle != null) ? ((IStateManager)_columnStyle).SaveViewState() : null;
			if((obj2 == null) && (obj3 == null))
			{
				return null;
			}
			return new object[] {
				obj2,
				obj3
			};
		}

		/// <summary>
		/// Recupera os dados salvos no ViewState
		/// </summary>
		/// <param name="savedState"></param>
		protected virtual void LoadViewState(object savedState)
		{
			if(savedState != null)
			{
				object[] objArray = (object[])savedState;
				if(objArray[0] != null)
				{
					((IStateManager)this.ViewState).LoadViewState(objArray[0]);
				}
				if(objArray[1] != null)
				{
					((IStateManager)this.ColumnStyle).LoadViewState(objArray[1]);
				}
			}
		}

		protected virtual void TrackViewState()
		{
			this._marked = true;
			((IStateManager)this.ViewState).TrackViewState();
			if(this._columnStyle != null)
			{
				((IStateManager)this._columnStyle).TrackViewState();
			}
		}

		void IStateManager.LoadViewState(object state)
		{
			this.LoadViewState(state);
		}

		object IStateManager.SaveViewState()
		{
			return this.SaveViewState();
		}

		void IStateManager.TrackViewState()
		{
			this.TrackViewState();
		}
	}
	public sealed class ItemsViewColumnCollection : ICollection, IEnumerable, IStateManager
	{
		private ArrayList columns;

		private bool marked;

		private ItemsView owner;

		[Browsable(false)]
		public int Count
		{
			get
			{
				return this.columns.Count;
			}
		}

		[Browsable(false)]
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		[Browsable(false)]
		public ItemsViewColumn this[int index]
		{
			get
			{
				return (ItemsViewColumn)this.columns[index];
			}
		}

		[Browsable(false)]
		public object SyncRoot
		{
			get
			{
				return this;
			}
		}

		bool IStateManager.IsTrackingViewState
		{
			get
			{
				return this.marked;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="columns"></param>
		public ItemsViewColumnCollection(ItemsView owner, ArrayList columns)
		{
			this.owner = owner;
			this.columns = columns;
		}

		public void Add(ItemsViewColumn column)
		{
			this.AddAt(-1, column);
		}

		public void AddAt(int index, ItemsViewColumn column)
		{
			if(column == null)
			{
				throw new ArgumentNullException("column");
			}
			if(index == -1)
			{
				this.columns.Add(column);
			}
			else
			{
				this.columns.Insert(index, column);
			}
			column.SetOwner(this.owner);
			if(this.marked)
			{
				((IStateManager)column).TrackViewState();
			}
			this.OnColumnsChanged();
		}

		public void Clear()
		{
			this.columns.Clear();
			this.OnColumnsChanged();
		}

		public void CopyTo(Array array, int index)
		{
			if(array == null)
			{
				throw new ArgumentNullException("array");
			}
			IEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				array.SetValue(enumerator.Current, index++);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return this.columns.GetEnumerator();
		}

		public int IndexOf(DataGridColumn column)
		{
			if(column != null)
			{
				return this.columns.IndexOf(column);
			}
			return -1;
		}

		public void Remove(DataGridColumn column)
		{
			int index = this.IndexOf(column);
			if(index >= 0)
			{
				this.RemoveAt(index);
			}
		}

		public void RemoveAt(int index)
		{
			if((index < 0) || (index >= this.Count))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			this.columns.RemoveAt(index);
			this.OnColumnsChanged();
		}

		/// <summary>
		/// Método acionado quando ocorre alteração na coleção.
		/// </summary>
		private void OnColumnsChanged()
		{
			if(this.owner != null)
			{
				this.owner.OnColumnsChanged();
			}
		}

		/// <summary>
		/// Recupera os dados do viewState.
		/// </summary>
		/// <param name="savedState"></param>
		void IStateManager.LoadViewState(object savedState)
		{
			if(savedState != null)
			{
				object[] objArray = (object[])savedState;
				if(objArray.Length == this.columns.Count)
				{
					for(int i = 0; i < objArray.Length; i++)
					{
						if(objArray[i] != null)
						{
							((IStateManager)this.columns[i]).LoadViewState(objArray[i]);
						}
					}
				}
			}
		}

		/// <summary>
		/// Salva os dados no viewState.
		/// </summary>
		/// <returns></returns>
		object IStateManager.SaveViewState()
		{
			int count = this.columns.Count;
			object[] objArray = new object[count];
			bool flag = false;
			for(int i = 0; i < count; i++)
			{
				objArray[i] = ((IStateManager)this.columns[i]).SaveViewState();
				if(objArray[i] != null)
				{
					flag = true;
				}
			}
			if(!flag)
			{
				return null;
			}
			return objArray;
		}

		void IStateManager.TrackViewState()
		{
			this.marked = true;
			int count = this.columns.Count;
			for(int i = 0; i < count; i++)
			{
				((IStateManager)this.columns[i]).TrackViewState();
			}
		}
	}
	/// <summary>
	/// Visualiza os itens na horizontal.
	/// </summary>
	[Designer(typeof(ItemsViewDesigner))]
	public class ItemsView : DataBoundControl, INamingContainer
	{
		/// <summary>
		/// Classe para auxiliar na recuperação dos dados.
		/// </summary>
		public class ItemsViewDataItem : WebControl, IDataItemContainer, INamingContainer
		{
			public virtual object DataItem
			{
				get;
				set;
			}

			public virtual int DataItemIndex
			{
				get;
				set;
			}

			public virtual int DisplayIndex
			{
				get;
				set;
			}

			protected override void Render(HtmlTextWriter writer)
			{
				foreach (Control c in Controls)
					c.RenderControl(writer);
			}
		}

		private ITemplate _itemTemplate;

		private ITemplate _emptyDataTemplate;

		private ITemplate _emptyItemTemplate;

		private TableItemStyle _rowStyle;

		/// <summary>
		/// Coleção das colunas.
		/// </summary>
		private ItemsViewColumnCollection _columnCollection;

		protected override void Render(HtmlTextWriter writer)
		{
			if(this.Controls.Count >= 1)
			{
				Table table = (Table)this.Controls[0];
				table.CopyBaseAttributes(this);
				if(base.ControlStyleCreated && !base.ControlStyle.IsEmpty)
				{
					table.ApplyStyle(base.ControlStyle);
				}
				else
				{
					table.GridLines = GridLines.None;
					table.CellSpacing = 0;
				}
				table.Caption = this.Caption;
				table.CaptionAlign = this.CaptionAlign;
				foreach (TableRow row in table.Rows)
				{
					Style s = new TableItemStyle();
					s.CopyFrom(_rowStyle);
					if((s != null) && row.Visible)
						row.MergeStyle(s);
				}
			}
			base.Render(writer);
		}

		protected override void PerformDataBinding(IEnumerable data)
		{
			base.PerformDataBinding(data);
			int num = this.CreateChildControls(data, true);
		}

		/// <summary>
		/// Cria os controles filhos
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="dataBinding"></param>
		/// <returns></returns>
		protected virtual int CreateChildControls(System.Collections.IEnumerable dataSource, bool dataBinding)
		{
			var table = new Table();
			table.CellPadding = CellPadding;
			table.CellSpacing = CellSpacing;
			this.Controls.Add(table);
			TableRow row = null;
			int col = 0, i = 0;
			if(dataSource != null && Columns.Count > 0)
			{
				var enumerator = dataSource.GetEnumerator();
				while (true)
				{
					try
					{
						if(!enumerator.MoveNext())
							break;
					}
					catch(ArgumentOutOfRangeException)
					{
						break;
					}
					object item = enumerator.Current;
					if(i == 0 || col == Columns.Count)
					{
						if(row != null)
							table.Rows.Add(row);
						row = new TableRow();
						col = 0;
					}
					var item2 = new ItemsViewDataItem() {
						DataItem = item,
						DisplayIndex = i,
						DataItemIndex = i
					};
					this.Controls.Add(item2);
					ItemTemplate.InstantiateIn(item2);
					if(dataBinding)
						item2.DataBind();
					var cell = new TableCell();
					if(i == 0)
						cell.ApplyStyle(Columns[i].ColumnStyle);
					cell.Controls.Add(item2);
					row.Cells.Add(cell);
					i++;
					col++;
				}
				if(row.Cells.Count > 0)
				{
					if(row.Cells.Count != Columns.Count)
						while (row.Cells.Count != Columns.Count)
						{
							var item2 = new ItemsViewDataItem() {
								DataItem = null,
								DisplayIndex = i,
								DataItemIndex = i
							};
							EmptyItemTemplate.InstantiateIn(item2);
							var cell = new TableCell();
							if(i == 0)
								cell.ApplyStyle(Columns[i].ColumnStyle);
							cell.Controls.Add(item2);
							row.Cells.Add(cell);
						}
					table.Rows.Add(row);
				}
			}
			if(i == 0)
			{
				this.Controls.Remove(table);
				if(_emptyDataTemplate != null)
					_emptyDataTemplate.InstantiateIn(this);
			}
			return i;
		}

		/// <summary>
		/// Método acionado quando as colunas forem alteradas.
		/// </summary>
		internal void OnColumnsChanged()
		{
			if(base.Initialized)
			{
				base.RequiresDataBinding = true;
			}
		}

		protected override HtmlTextWriterTag TagKey
		{
			get
			{
				return HtmlTextWriterTag.Div;
			}
		}

		/// <summary>
		/// Colunas da instancia.
		/// </summary>
		[MergableProperty(false), DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty), Category("Default")]
		public virtual ItemsViewColumnCollection Columns
		{
			get
			{
				if(this._columnCollection == null)
				{
					this._columnCollection = new ItemsViewColumnCollection(this, new ArrayList());
					if(base.IsTrackingViewState)
					{
						((IStateManager)this._columnCollection).TrackViewState();
					}
				}
				return this._columnCollection;
			}
		}

		[DefaultValue(-1), Category("Layout")]
		public virtual int CellPadding
		{
			get
			{
				object obj2 = this.ViewState["CellPadding"];
				if(obj2 == null)
				{
					return 0;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["CellPadding"] = value;
			}
		}

		/// <summary>
		/// Espaço entre as células.
		/// </summary>
		[DefaultValue(0), Category("Layout")]
		public virtual int CellSpacing
		{
			get
			{
				object obj2 = this.ViewState["CellSpacing"];
				if(obj2 == null)
				{
					return 0;
				}
				return (int)obj2;
			}
			set
			{
				this.ViewState["CellSpacing"] = value;
			}
		}

		/// <summary>
		/// Template dos itens.
		/// </summary>
		[TemplateContainer(typeof(ItemsViewDataItem), BindingDirection.TwoWay), DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), Description("InsertRowStyle")]
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
		/// Template dos items vazios.
		/// </summary>
		[TemplateContainer(typeof(ItemsViewDataItem), BindingDirection.TwoWay), DefaultValue((string)null), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ITemplate EmptyItemTemplate
		{
			get
			{
				return _emptyItemTemplate;
			}
			set
			{
				_emptyItemTemplate = value;
			}
		}

		/// <summary>
		/// Template quando não houver dados para serem processados.
		/// </summary>
		[TemplateContainer(typeof(ItemsViewDataItem)), Browsable(false), DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty), Description("EmptyDataTemplate")]
		public virtual ITemplate EmptyDataTemplate
		{
			get
			{
				return _emptyDataTemplate;
			}
			set
			{
				_emptyDataTemplate = value;
			}
		}

		[NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Category("Styles"), DefaultValue((string)null), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public TableItemStyle RowStyle
		{
			get
			{
				if(_rowStyle == null)
				{
					_rowStyle = new TableItemStyle();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_rowStyle).TrackViewState();
					}
				}
				return _rowStyle;
			}
		}

		[Category("Accessibility"), DefaultValue(""), Localizable(true)]
		public virtual string Caption
		{
			get
			{
				string str = (string)this.ViewState["Caption"];
				if(str == null)
				{
					return string.Empty;
				}
				return str;
			}
			set
			{
				this.ViewState["Caption"] = value;
			}
		}

		[DefaultValue(0), Category("Accessibility")]
		public virtual TableCaptionAlign CaptionAlign
		{
			get
			{
				object obj2 = this.ViewState["CaptionAlign"];
				if(obj2 == null)
				{
					return TableCaptionAlign.NotSet;
				}
				return (TableCaptionAlign)obj2;
			}
			set
			{
				if((value < TableCaptionAlign.NotSet) || (value > TableCaptionAlign.Right))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				this.ViewState["CaptionAlign"] = value;
			}
		}
	}
}
