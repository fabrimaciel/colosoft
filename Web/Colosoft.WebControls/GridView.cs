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
using System.Web;
using System.Security.Permissions;
using System.Web.UI.WebControls;
using Colosoft.WebControls.GridView;
using System.Web.Script.Serialization;
using System.ComponentModel;
using System.Collections;
using System.Data;
using System.Collections.Specialized;
using System.Reflection;

namespace Colosoft.WebControls.GridView
{
	[ToolboxData("<{0}:GridView runat=server></{0}:GridView>")]
	public class GridView : CompositeDataBoundControl, IPostBackDataHandler, IPostBackEventHandler, INamingContainer
	{
		public delegate void CellBindEventHandler (object sender, CellBindEventArgs e);

		public delegate void DataRequestedEventHandler (object sender, DataRequestedEventArgs e);

		public delegate void DataRequestEventHandler (object sender, DataRequestEventArgs e);

		public delegate void RowAddEventHandler (object sender, RowAddEventArgs e);

		public delegate void RowDeleteEventHandler (object sender, RowDeleteEventArgs e);

		public delegate void RowEditEventHandler (object sender, RowEditEventArgs e);

		public delegate void RowSelectEventHandler (object sender, RowSelectEventArgs e);

		public delegate void SearchEventHandler (object sender, SearchEventArgs e);

		public delegate void SortEventHandler (object sender, SortEventArgs e);

		public delegate void DataLinkEventHandler (object sender, DataLinkEventArgs e);

		private AddDialogSettings _addDialogSettings;

		private AjaxCallBackMode _ajaxCallBackMode;

		private bool _ajaxCallModeResolved;

		private AppearanceSettings _appearanceSettings;

		private ClientSideEvents _clientSideEvents;

		private ColumnCollection _columnCollection;

		private bool _dataSourceSorted;

		private DeleteDialogSettings _deleteDialogSettings;

		private EditDialogSettings _editDialogSettings;

		private HierarchySettings _hierarchySettings;

		private PagerSettings _pagerSettings;

		private SearchDialogSettings _searchDialogSettings;

		private DataSourceSelectArguments _selectArguments;

		private SortSettings _sortSettings;

		private GroupSettings _groupSettings;

		private JavaScriptSerializer _sr;

		private ToolBarSettings _toolBarSettings;

		private int _totalRows;

		private IEnumerable _dataLink;

		private static readonly object EventCellBinding;

		private static readonly object EventCellBound;

		private static readonly object EventDataRequested;

		private static readonly object EventDataRequesting;

		private static readonly object EventRowAdded;

		private static readonly object EventRowAdding;

		private static readonly object EventRowDeleted;

		private static readonly object EventRowDeleting;

		private static readonly object EventRowEdited;

		private static readonly object EventRowEditing;

		private static readonly object EventRowSelecting;

		private static readonly object EventSearched;

		private static readonly object EventSearching;

		private static readonly object EventSorted;

		private static readonly object EventSorting;

		private static readonly object EventDataLinkChanged;

		[Category("Action"), Description("GridView_OnSearching")]
		public event CellBindEventHandler CellBinding {
			add
			{
				base.Events.AddHandler(EventCellBinding, value);
			}
			remove {
				base.Events.RemoveHandler(EventCellBinding, value);
			}
		}

		[Description("GridView_OnSearched"), Category("Action")]
		public event CellBindEventHandler CellBound {
			add
			{
				base.Events.AddHandler(EventCellBound, value);
			}
			remove {
				base.Events.RemoveHandler(EventCellBound, value);
			}
		}

		[Description("GridView_OnSearched"), Category("Action")]
		public event DataRequestedEventHandler DataRequested {
			add
			{
				base.Events.AddHandler(EventDataRequested, value);
			}
			remove {
				base.Events.RemoveHandler(EventDataRequested, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearched")]
		public event DataRequestEventHandler DataRequesting {
			add
			{
				base.Events.AddHandler(EventDataRequesting, value);
			}
			remove {
				base.Events.RemoveHandler(EventDataRequesting, value);
			}
		}

		[Description("GridView_OnSearched"), Category("Action")]
		public event EventHandler RowAdded {
			add
			{
				base.Events.AddHandler(EventRowAdded, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowAdded, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearching")]
		public event RowAddEventHandler RowAdding {
			add
			{
				base.Events.AddHandler(EventRowAdding, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowAdding, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearched")]
		public event EventHandler RowDeleted {
			add
			{
				base.Events.AddHandler(EventRowDeleted, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowDeleted, value);
			}
		}

		[Description("GridView_OnSearching"), Category("Action")]
		public event RowDeleteEventHandler RowDeleting {
			add
			{
				base.Events.AddHandler(EventRowDeleting, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowDeleting, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearched")]
		public event EventHandler RowEdited {
			add
			{
				base.Events.AddHandler(EventRowEdited, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowEdited, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearching")]
		public event RowEditEventHandler RowEditing {
			add
			{
				base.Events.AddHandler(EventRowEditing, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowEditing, value);
			}
		}

		[Description("GridView_OnSearching"), Category("Action")]
		public event RowSelectEventHandler RowSelecting {
			add
			{
				base.Events.AddHandler(EventRowSelecting, value);
			}
			remove {
				base.Events.RemoveHandler(EventRowSelecting, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearched")]
		public event EventHandler Searched {
			add
			{
				base.Events.AddHandler(EventSearched, value);
			}
			remove {
				base.Events.RemoveHandler(EventSearched, value);
			}
		}

		[Category("Action"), Description("GridView_OnSearching")]
		public event SearchEventHandler Searching {
			add
			{
				base.Events.AddHandler(EventSearching, value);
			}
			remove {
				base.Events.RemoveHandler(EventSearching, value);
			}
		}

		[Category("Action"), Description("GridView_OnSorted")]
		public event EventHandler Sorted {
			add
			{
				base.Events.AddHandler(EventSorted, value);
			}
			remove {
				base.Events.RemoveHandler(EventSorted, value);
			}
		}

		[Description("GridView_OnSorting"), Category("Action")]
		public event SortEventHandler Sorting {
			add
			{
				base.Events.AddHandler(EventSorting, value);
			}
			remove {
				base.Events.RemoveHandler(EventSorting, value);
			}
		}

		[Description("GridView_OnDataLinkChanged"), Category("Action")]
		public event DataLinkEventHandler DataLinkChanged {
			add
			{
				base.Events.AddHandler(EventDataLinkChanged, value);
			}
			remove {
				base.Events.RemoveHandler(EventDataLinkChanged, value);
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string AccessKey
		{
			get
			{
				return base.AccessKey;
			}
			set
			{
				base.AccessKey = value;
			}
		}

		/// <summary>
		/// Configuração da interface Add Dialog no jqGrid
		/// </summary>
		[Description("Settings related to UI/Functionality of the Add Dialog in jqGrid"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AddDialogSettings AddDialogSettings
		{
			get
			{
				if(_addDialogSettings == null)
				{
					_addDialogSettings = new AddDialogSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_addDialogSettings).TrackViewState();
					}
				}
				return _addDialogSettings;
			}
		}

		/// <summary>
		/// Modo de chamada ajax.
		/// </summary>
		public AjaxCallBackMode AjaxCallBackMode
		{
			get
			{
				if(!_ajaxCallModeResolved)
				{
					ResolveAjaxCallBackMode();
				}
				return _ajaxCallBackMode;
			}
		}

		/// <summary>
		/// Configurações da aparencia do GridView.
		/// </summary>
		[Category("Settings"), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description("Settings related to appearance in jqGrid"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceSettings AppearanceSettings
		{
			get
			{
				if(_appearanceSettings == null)
				{
					_appearanceSettings = new AppearanceSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_appearanceSettings).TrackViewState();
					}
				}
				return _appearanceSettings;
			}
		}

		/// <summary>
		/// Obtém e define se a largura do GridView será definida automaticamente.
		/// </summary>
		[DefaultValue(false)]
		public bool AutoWidth
		{
			get
			{
				object obj2 = this.ViewState["AutoWidth"];
				if(obj2 == null)
					return false;
				return (bool)obj2;
			}
			set
			{
				ViewState["AutoWidth"] = value;
			}
		}

		/// <summary>
		/// Cor de fundo do GridView.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color BackColor
		{
			get
			{
				return base.BackColor;
			}
			set
			{
				base.BackColor = value;
			}
		}

		/// <summary>
		/// Cor da borda do GridView.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override System.Drawing.Color BorderColor
		{
			get
			{
				return base.BorderColor;
			}
			set
			{
				base.BorderColor = value;
			}
		}

		/// <summary>
		/// Estilo da borda do GridView.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override BorderStyle BorderStyle
		{
			get
			{
				return base.BorderStyle;
			}
			set
			{
				base.BorderStyle = value;
			}
		}

		/// <summary>
		/// Largura da borda do GridView.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public override Unit BorderWidth
		{
			get
			{
				return base.BorderWidth;
			}
			set
			{
				base.BorderWidth = value;
			}
		}

		/// <summary>
		/// Informação sobre os eventos que irão rodar no lado do cliente.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), Category("Client-side events"), PersistenceMode(PersistenceMode.InnerProperty), Description("Defines client-side events in jqGrid")]
		public virtual ClientSideEvents ClientSideEvents
		{
			get
			{
				if(_clientSideEvents == null)
				{
					_clientSideEvents = new ClientSideEvents();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_clientSideEvents).TrackViewState();
					}
				}
				return _clientSideEvents;
			}
		}

		/// <summary>
		/// Obtém e definie se o Grid irá suportar reordenação das colunas pelo cliente.
		/// </summary>
		[DefaultValue(false), Description("Whether or not columns re-ordering is enabled. Default is false.")]
		public bool ColumnReordering
		{
			get
			{
				object obj2 = this.ViewState["ColumnReordering"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["ColumnReordering"] = value;
			}
		}

		/// <summary>
		/// Lista das colunas que compõem o Grid.
		/// </summary>
		[DefaultValue((string)null), Category("Default"), PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), Description("DataControls_Columns")]
		public virtual ColumnCollection Columns
		{
			get
			{
				if(_columnCollection == null)
				{
					_columnCollection = new ColumnCollection();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_columnCollection).TrackViewState();
					}
				}
				return _columnCollection;
			}
		}

		/// <summary>
		/// Nome do estilo Css que será aplicado ao Grid.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string CssClass
		{
			get
			{
				return base.CssClass;
			}
			set
			{
				base.CssClass = value;
			}
		}

		/// <summary>
		/// Url que será usada para a recuperação dos dados.
		/// </summary>
		[DefaultValue(""), Category("")]
		public string DataUrl
		{
			get
			{
				object obj2 = this.ViewState["DataUrl"];
				if(obj2 != null)
				{
					return (string)obj2;
				}
				if(base.DesignMode)
				{
					return "";
				}
				return HttpContext.Current.Request.FilePath;
			}
			set
			{
				this.ViewState["DataUrl"] = value;
			}
		}

		/// <summary>
		/// Configurações relacionadas com a interface Delete Dialog no jqGrid.
		/// </summary>
		[Description("Settings related to UI/Functionality of the Delete Dialog in jqGrid"), Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual DeleteDialogSettings DeleteDialogSettings
		{
			get
			{
				if(_deleteDialogSettings == null)
				{
					_deleteDialogSettings = new DeleteDialogSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_deleteDialogSettings).TrackViewState();
					}
				}
				return _deleteDialogSettings;
			}
		}

		/// <summary>
		/// Configurações relacionadas com a interface Edit Dialog no jqGrid.
		/// </summary>
		[Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description("Settings related to UI/Functionality of the Edit Dialog in jqGrid")]
		public virtual EditDialogSettings EditDialogSettings
		{
			get
			{
				if(_editDialogSettings == null)
				{
					_editDialogSettings = new EditDialogSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_editDialogSettings).TrackViewState();
					}
				}
				return _editDialogSettings;
			}
		}

		/// <summary>
		/// Url que será usada para editar os dados do GridView.
		/// </summary>
		[Category(""), DefaultValue("")]
		public string EditUrl
		{
			get
			{
				object obj2 = this.ViewState["EditUrl"];
				if(obj2 != null)
				{
					return (string)obj2;
				}
				if(base.DesignMode)
				{
					return "";
				}
				return HttpContext.Current.Request.FilePath;
			}
			set
			{
				this.ViewState["EditUrl"] = value;
			}
		}

		/// <summary>
		/// Fonte aplicada ao GridView.
		/// </summary>
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override FontInfo Font
		{
			get
			{
				return base.Font;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override System.Drawing.Color ForeColor
		{
			get
			{
				return base.ForeColor;
			}
			set
			{
				base.ForeColor = value;
			}
		}

		[Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Description("Settings related to Hierarchy in jqGrid")]
		public virtual HierarchySettings HierarchySettings
		{
			get
			{
				if(_hierarchySettings == null)
				{
					_hierarchySettings = new HierarchySettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_hierarchySettings).TrackViewState();
					}
				}
				return _hierarchySettings;
			}
		}

		/// <summary>
		/// Obtém e define se o GridView irá suportar seleção multipla.
		/// </summary>
		[DefaultValue(false)]
		public bool MultiSelect
		{
			get
			{
				object obj2 = this.ViewState["MultiSelect"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["MultiSelect"] = value;
			}
		}

		/// <summary>
		/// Tecla que será usada para auxiliar a multiseleção.
		/// </summary>
		[DefaultValue(0)]
		public MultiSelectKey MultiSelectKey
		{
			get
			{
				object obj2 = this.ViewState["MultiSelectKey"];
				if(obj2 == null)
				{
					return MultiSelectKey.None;
				}
				return (MultiSelectKey)obj2;
			}
			set
			{
				this.ViewState["MultiSelectKey"] = value;
			}
		}

		/// <summary>
		/// Modo que será usado na multiseleção.
		/// </summary>
		[DefaultValue(1)]
		public MultiSelectMode MultiSelectMode
		{
			get
			{
				object obj2 = this.ViewState["MultiSelectMode"];
				if(obj2 == null)
				{
					return MultiSelectMode.SelectOnRowClick;
				}
				return (MultiSelectMode)obj2;
			}
			set
			{
				this.ViewState["MultiSelectMode"] = value;
			}
		}

		/// <summary>
		/// Configuração da paginação.
		/// </summary>
		[Description("Settings related to UI/Functionality of paging in jqGrid"), Category("Settings"), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual PagerSettings PagerSettings
		{
			get
			{
				if(_pagerSettings == null)
				{
					_pagerSettings = new PagerSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_pagerSettings).TrackViewState();
					}
				}
				return _pagerSettings;
			}
		}

		[Description("The rendering mode of the grid. Default is 'Default'. Optimized may speed the grid up, but does not support all functionality."), DefaultValue(0)]
		public RenderingMode RenderingMode
		{
			get
			{
				object obj2 = this.ViewState["RenderingMode"];
				if(obj2 == null)
				{
					return RenderingMode.Default;
				}
				return (RenderingMode)obj2;
			}
			set
			{
				this.ViewState["RenderingMode"] = value;
			}
		}

		/// <summary>
		/// Configurações da interface de pesquisa.
		/// </summary>
		[NotifyParentProperty(true), Category("Settings"), PersistenceMode(PersistenceMode.InnerProperty), Description("Settings related to UI/Functionality of the Search Dialog in jqGrid"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual SearchDialogSettings SearchDialogSettings
		{
			get
			{
				if(_searchDialogSettings == null)
				{
					_searchDialogSettings = new SearchDialogSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_searchDialogSettings).TrackViewState();
					}
				}
				return _searchDialogSettings;
			}
		}

		/// <summary>
		/// Obtém e define a linha selecionada.
		/// </summary>
		[Category("ToolBar"), DefaultValue("")]
		public string SelectedRow
		{
			get
			{
				object obj2 = this.ViewState["SelectedRow"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["SelectedRow"] = value;
			}
		}

		/// <summary>
		/// Lista das linhas selecionadas.
		/// </summary>
		[Category("ToolBar")]
		public RowCollection SelectedRows
		{
			get
			{
				object obj2 = this.ViewState["SelectedRows"];
				if(obj2 == null)
				{
					obj2 = new RowCollection();
					this.ViewState["SelectedRows"] = obj2;
				}
				return (RowCollection)obj2;
			}
			set
			{
				this.ViewState["SelectedRows"] = value;
			}
		}

		/// <summary>
		/// Propriedade usada para vincular algum dados ao Grid.
		/// </summary>
		public IEnumerable DataLink
		{
			get
			{
				return _dataLink;
			}
			set
			{
				if(_dataLink != value)
				{
					_dataLink = value;
					OnDataLinkChanged(new DataLinkEventArgs {
						DataLink = _dataLink
					});
				}
			}
		}

		/// <summary>
		/// Identifise se o DataLink será usado para vincular os dados selecionados
		/// no Grid.
		/// </summary>
		[Category("Data"), DefaultValue(true)]
		public bool UseSelectedRowsDataLink
		{
			get
			{
				object obj2 = this.ViewState["UseSelectedRowsDataLink"];
				if(obj2 == null)
				{
					return false;
				}
				return (bool)obj2;
			}
			set
			{
				this.ViewState["UseSelectedRowsDataLink"] = value;
			}
		}

		/// <summary>
		/// Propriedade que serão carregada alem das colunas
		/// </summary>
		[Category("Data"), DefaultValue(true)]
		public string PropertiesInclude
		{
			get
			{
				object obj2 = this.ViewState["PropertiesInclude"];
				if(obj2 == null)
				{
					return "";
				}
				return (string)obj2;
			}
			set
			{
				this.ViewState["PropertiesInclude"] = value;
			}
		}

		/// <summary>
		/// Recupera se o toolbar está sendo exibido ou não.
		/// </summary>
		private bool ShowToolBar
		{
			get
			{
				if(((!this.ToolBarSettings.ShowAddButton && !this.ToolBarSettings.ShowDeleteButton) && (!this.ToolBarSettings.ShowEditButton && !this.ToolBarSettings.ShowRefreshButton)) && !this.ToolBarSettings.ShowSearchButton)
					return this.ToolBarSettings.ShowViewRowDetailsButton;
				return true;
			}
		}

		/// <summary>
		/// Configuração das ordenação no Grid.
		/// </summary>
		[Description("Settings related to UI/Functionality of sorting in jqGrid"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true), PersistenceMode(PersistenceMode.InnerProperty), Category("Settings")]
		public virtual SortSettings SortSettings
		{
			get
			{
				if(_sortSettings == null)
				{
					_sortSettings = new SortSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_sortSettings).TrackViewState();
					}
				}
				return _sortSettings;
			}
		}

		/// <summary>
		/// Configuração associadas com o agrupamento.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty), NotifyParentProperty(true), Description("Settings related to Grouping"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DefaultValue((string)null), Category("Settings"), MergableProperty(false)]
		public virtual GroupSettings GroupSettings
		{
			get
			{
				if(_groupSettings == null)
				{
					_groupSettings = new GroupSettings();
					if(base.IsTrackingViewState)
						((IStateManager)_groupSettings).TrackViewState();
				}
				return _groupSettings;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override short TabIndex
		{
			get
			{
				return base.TabIndex;
			}
			set
			{
				base.TabIndex = value;
			}
		}

		/// <summary>
		/// Configuração da barra de ferramentas.
		/// </summary>
		[NotifyParentProperty(true), Description("Settings related to UI/Functionality of the ToolBar in jqGrid"), Category("Settings"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public virtual ToolBarSettings ToolBarSettings
		{
			get
			{
				if(_toolBarSettings == null)
				{
					_toolBarSettings = new ToolBarSettings();
					if(base.IsTrackingViewState)
					{
						((IStateManager)_toolBarSettings).TrackViewState();
					}
				}
				return _toolBarSettings;
			}
		}

		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override string ToolTip
		{
			get
			{
				return base.ToolTip;
			}
			set
			{
				base.ToolTip = value;
			}
		}

		static GridView()
		{
			EventSorted = new object();
			EventSorting = new object();
			EventSearched = new object();
			EventSearching = new object();
			EventRowAdding = new object();
			EventRowAdded = new object();
			EventRowEditing = new object();
			EventRowEdited = new object();
			EventRowDeleting = new object();
			EventRowDeleted = new object();
			EventRowSelecting = new object();
			EventCellBinding = new object();
			EventCellBound = new object();
			EventDataRequesting = new object();
			EventDataRequested = new object();
			EventDataLinkChanged = new object();
		}

		public GridView()
		{
		}

		/// <summary>
		/// Envia a respota.
		/// </summary>
		/// <param name="responseText"></param>
		private void EmitResponse(string responseText)
		{
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.Write(responseText);
			HttpContext.Current.Response.Flush();
			HttpContext.Current.Response.End();
		}

		private void EnforceColumnContraints()
		{
			if(this.Columns.Count == 0)
				throw new Exception("Columns should be explicitly defined with at least DataField set to the respective datasource field, e.g. \r\n                                        <colosoft:GridView ...>\r\n                                             <Columns>\r\n                                                <colosoft:Column DataField='OrderID' PrimaryKey='True' />\r\n                                                <colosoft:Column DataField='Freight' Editable='true' Sortable='false'  />\r\n                                                <colosoft:Column DataField='OrderDate' DataFormatString='{0:d}' />\r\n                                                <colosoft:Column DataField='ShipCity' />\r\n                                            </Columns>\r\n                                            ...");
		}

		/// <summary>
		/// Recupera as opções para adicionar um novo item.
		/// </summary>
		/// <returns></returns>
		private string GetAddOptions()
		{
			JsonAddDialog dialog = new JsonAddDialog(this);
			return dialog.Process();
		}

		/// <summary>
		/// Recupera as opções para apagar os itens.
		/// </summary>
		/// <returns></returns>
		private string GetDelOptions()
		{
			var dialog = new JsonDelDialog(this);
			dialog.Process();
			return dialog.RemoveQuotesForJavaScriptMethods(_sr.Serialize(dialog.JsonValues));
		}

		private string GetEditOptions()
		{
			var dialog = new JsonEditDialog(this);
			return dialog.Process();
		}

		private string GetSearchOptions()
		{
			JsonSearchDialog dialog = new JsonSearchDialog(this);
			dialog.Process();
			return _sr.Serialize(dialog.JsonValues);
		}

		/// <summary>
		/// Carrega as opções usadas na inicialização do Grid.
		/// </summary>
		/// <param name="subGrid"></param>
		/// <returns></returns>
		private string GetStartupOptions(bool subGrid)
		{
			StringBuilder builder = new StringBuilder();
			string str = subGrid ? "jQuery('#' + subgrid_table_id)" : string.Format("jQuery('#{0}')", this.ClientID);
			string str2 = subGrid ? "jQuery('#' + pager_id)" : string.Format("jQuery('#{0}')", this.ClientID + "_pager");
			string str3 = subGrid ? "&parentRowID=' + row_id + '" : string.Empty;
			string str4 = (this.DataUrl.IndexOf("?") > 0) ? "&" : "?";
			string str5 = (this.EditUrl.IndexOf("?") > 0) ? "&" : "?";
			string str6 = string.Format("{0}{1}jqGridID={2}{3}", new object[] {
				this.DataUrl,
				str4,
				this.ClientID,
				str3
			});
			string str7 = string.Format("{0}{1}jqGridID={2}&editMode=1{3}", new object[] {
				this.EditUrl,
				str5,
				this.ClientID,
				str3
			});
			builder.AppendFormat("{0}.jqGrid({{\n", str);
			builder.AppendFormat("url: '{0}'\n", str6);
			builder.AppendFormat(",editurl: '{0}'\n", str7);
			builder.AppendFormat(",datatype: 'json'\n", new object[0]);
			builder.AppendFormat(",page: {0}", this.PagerSettings.CurrentPage);
			builder.AppendFormat(",colNames: {0}\n", this.GetColNames());
			builder.AppendFormat(",colModel: {0}\n", this.GetColModel());
			builder.AppendFormat(",viewrecords: true\n", new object[0]);
			builder.AppendFormat(",scrollrows: true\n", new object[0]);
			if((this.ToolBarSettings.ToolBarPosition == ToolBarPosition.Bottom) || (this.ToolBarSettings.ToolBarPosition == ToolBarPosition.TopAndBottom))
			{
				builder.AppendFormat(",pager: {0}\n", str2);
			}
			if((this.ToolBarSettings.ToolBarPosition == ToolBarPosition.Top) || (this.ToolBarSettings.ToolBarPosition == ToolBarPosition.TopAndBottom))
				builder.Append(",toppager: true\n");
			if(this.AutoWidth)
				builder.Append(",autowidth: true\n");
			if(this.AppearanceSettings.ShowFooter)
			{
				builder.Append(",footerrow: true\n");
				builder.Append(",userDataOnFooter: true\n");
			}
			if(this.ColumnReordering)
				builder.Append(",sortable: true\n");
			if(this.AppearanceSettings.ScrollBarOffset != 0x12)
				builder.AppendFormat(",scrollOffset: {0}\n", this.AppearanceSettings.ScrollBarOffset);
			if(this.AppearanceSettings.RightToLeft)
				builder.Append(",direction: 'rtl'\n");
			if(this.RenderingMode == RenderingMode.Optimized)
				builder.Append(",gridview: true\n");
			if((this.HierarchySettings.HierarchyMode == HierarchyMode.Parent) || (this.HierarchySettings.HierarchyMode == HierarchyMode.ParentAndChild))
				builder.Append(",subGrid: true\n");
			if(!string.IsNullOrEmpty(this.ClientSideEvents.SubGridRowExpanded))
				builder.AppendFormat(",subGridRowExpanded: {0}\n", this.ClientSideEvents.SubGridRowExpanded);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.ServerError))
				builder.AppendFormat(",errorCell: {0}\n", this.ClientSideEvents.ServerError);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.RowSelect))
				builder.AppendFormat(",onSelectRow: {0}\n", this.ClientSideEvents.RowSelect);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.ColumnSort))
				builder.AppendFormat(",onSortCol: {0}\n", this.ClientSideEvents.ColumnSort);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.RowDoubleClick))
				builder.AppendFormat(",ondblClickRow: {0}\n", this.ClientSideEvents.RowDoubleClick);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.RowRightClick))
				builder.AppendFormat(",onRightClickRow: {0}\n", this.ClientSideEvents.RowRightClick);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.LoadDataError))
				builder.AppendFormat(",loadError: {0}\n", this.ClientSideEvents.LoadDataError);
			else
				builder.AppendFormat(",loadError: {0}\n", "jqGrid_aspnet_loadErrorHandler");
			if(!string.IsNullOrEmpty(this.ClientSideEvents.GridInitialized))
				builder.AppendFormat(",gridComplete: {0}\n", this.ClientSideEvents.GridInitialized);
			if(!string.IsNullOrEmpty(this.ClientSideEvents.BeforeAjaxRequest))
				builder.AppendFormat(",gridComplete: {0}\n", this.ClientSideEvents.BeforeAjaxRequest);
			if(!this.AppearanceSettings.HighlightRowsOnHover)
				builder.Append(",hoverrows: false\n");
			if(this.AppearanceSettings.AlternateRowBackground)
				builder.Append(",altRows: true\n");
			if(this.AppearanceSettings.ShowRowNumbers)
				builder.Append(",rownumbers: true\n");
			if(this.AppearanceSettings.RowNumbersColumnWidth != 0x19)
				builder.AppendFormat(",rownumWidth: {0}\n", this.AppearanceSettings.RowNumbersColumnWidth.ToString());
			if(this.PagerSettings.ScrollBarPaging)
				builder.AppendFormat(",scroll: 1\n", new object[0]);
			builder.AppendFormat(",rowNum: {0}\n", this.PagerSettings.PageSize.ToString());
			builder.AppendFormat(",rowList: {0}\n", this.PagerSettings.PageSizeOptions.ToString());
			if(!string.IsNullOrEmpty(this.PagerSettings.NoRowsMessage))
				builder.AppendFormat(",emptyrecords: '{0}'\n", this.PagerSettings.NoRowsMessage.ToString());
			builder.AppendFormat(",postBackUrl: \"{0}\"", this.Page.ClientScript.GetPostBackEventReference(this, "jqGridParams"));
			builder.AppendFormat(",editDialogOptions: {0}", this.GetEditOptions());
			builder.AppendFormat(",addDialogOptions: {0}", this.GetAddOptions());
			builder.AppendFormat(",delDialogOptions: {0}", this.GetDelOptions());
			builder.AppendFormat(",searchDialogOptions: {0}", this.GetSearchOptions());
			if(!string.IsNullOrEmpty(this.SortSettings.InitialSortColumn))
			{
				builder.AppendFormat(",sortname: '{0}'\n", this.SortSettings.InitialSortColumn);
			}
			builder.AppendFormat(",sortorder: '{0}'\n", this.SortSettings.InitialSortDirection.ToString().ToLower());
			if(this.MultiSelect)
			{
				builder.Append(",multiselect: true\n");
				if(this.MultiSelectMode == MultiSelectMode.SelectOnCheckBoxClickOnly)
				{
					builder.AppendFormat(",multiboxonly: true\n", this.MultiSelect.ToString().ToLower());
				}
				if(this.MultiSelectKey != MultiSelectKey.None)
				{
					builder.AppendFormat(",multikey: '{0}'\n", this.GetMultiKeyString(this.MultiSelectKey));
				}
			}
			if(this.GroupSettings.GroupFields.Count > 0)
			{
				builder.Append(",grouping:true");
				builder.Append(",groupingView: {");
				builder.AppendFormat("groupField: ['{0}']", this.GroupSettings.GroupFields[0].DataField);
				builder.AppendFormat(",groupColumnShow: [{0}]", this.GroupSettings.GroupFields[0].ShowGroupColumn.ToString().ToLower());
				builder.AppendFormat(",groupText: ['{0}']", this.GroupSettings.GroupFields[0].HeaderText);
				builder.AppendFormat(",groupOrder: ['{0}']", this.GroupSettings.GroupFields[0].GroupSortDirection.ToString().ToLower());
				builder.AppendFormat(",groupSummary: [{0}]", this.GroupSettings.GroupFields[0].ShowGroupSummary.ToString().ToLower());
				builder.AppendFormat(",groupCollapse: {0}", this.GroupSettings.CollapseGroups.ToString().ToLower());
				builder.AppendFormat(",groupDataSorted: true", new object[0]);
				if(this.GroupSettings.ShowSummaryOnHide)
					builder.Append(",showSummaryOnHide: true");
				builder.Append("}");
			}
			if(!string.IsNullOrEmpty(this.AppearanceSettings.Caption))
			{
				builder.AppendFormat(",caption: '{0}'\n", this.AppearanceSettings.Caption);
			}
			builder.AppendFormat(",hidegrid: {0}\n", this.AppearanceSettings.ShowCaptionGridToggleButton.ToString().ToLower());
			if(!this.Width.IsEmpty)
			{
				builder.AppendFormat(",width: '{0}'\n", this.Width.ToString().Replace("px", ""));
			}
			if(!this.Height.IsEmpty)
			{
				builder.AppendFormat(",height: '{0}'\n", this.Height.ToString().Replace("px", ""));
			}
			builder.AppendFormat(",viewsortcols: [{0},'{1}',{2}]", "false", this.SortSettings.SortIconsPosition.ToString().ToLower(), (this.SortSettings.SortAction == SortAction.ClickOnHeader) ? "true" : "false");
			builder.AppendFormat("}})\n\r", new object[0]);
			builder.Append(this.GetToolBarOptions(subGrid));
			builder.Append(this.GetLoadErrorHandler());
			if(!subGrid)
			{
				builder.Append(this.GetJQuerySubmit());
				builder.Append(this.GetSelectedRowPostBack());
				builder.Append(this.RestoreSelectedState());
				builder.Append(this.GetTooltipScript());
			}
			if(this.ToolBarSettings.ShowSearchToolBar)
			{
				builder.AppendFormat("jQuery('#{0}').filterToolbar();", this.ClientID);
			}
			return builder.ToString();
		}

		/// <summary>
		/// Carrega as opções da barra de ferramentas do Grid.
		/// </summary>
		/// <param name="subGrid"></param>
		/// <returns></returns>
		private string GetToolBarOptions(bool subGrid)
		{
			StringBuilder builder = new StringBuilder();
			if(!this.ShowToolBar)
			{
				return string.Empty;
			}
			var bar = new JsonToolBar(this.ToolBarSettings);
			if(!subGrid)
				builder.AppendFormat(".navGrid('#{0}',{1},{2},{3},{4},{5} );", new object[] {
					this.ClientID + "_pager",
					_sr.Serialize(bar),
					string.Format("jQuery('#{0}').getGridParam('editDialogOptions')", this.ClientID),
					string.Format("jQuery('#{0}').getGridParam('addDialogOptions')", this.ClientID),
					string.Format("jQuery('#{0}').getGridParam('delDialogOptions')", this.ClientID),
					string.Format("jQuery('#{0}').getGridParam('searchDialogOptions')", this.ClientID)
				});
			else
				builder.AppendFormat(".navGrid('#' + pager_id,{0},{1},{2},{3},{4} );", new object[] {
					_sr.Serialize(bar),
					"jQuery('#' + subgrid_table_id).getGridParam('editDialogOptions')",
					"jQuery('#' + subgrid_table_id).getGridParam('addDialogOptions')",
					"jQuery('#' + subgrid_table_id).getGridParam('delDialogOptions')",
					"jQuery('#' + subgrid_table_id).getGridParam('searchDialogOptions')"
				});
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o script usado para recupera a sub grid se existir.
		/// </summary>
		/// <returns></returns>
		private string GetChildSubGridJavaScript()
		{
			var builder = new StringBuilder();
			builder.Append("<script type='text/javascript'>\n");
			builder.AppendFormat("function showSubGrid_{0}(subgrid_id, row_id, message, suffix) {{", this.ID);
			builder.Append("var subgrid_table_id, pager_id;\r\n\t\t                subgrid_table_id = subgrid_id+'_t';\r\n\t\t                pager_id = 'p_'+ subgrid_table_id;\r\n                        if (suffix) { subgrid_table_id += suffix; pager_id += suffix;  }\r\n                        if (message) jQuery('#'+subgrid_id).append(message);                        \r\n\t\t                jQuery('#'+subgrid_id).append('<table id=' + subgrid_table_id + ' class=scroll></table><div id=' + pager_id + ' class=scroll></div>');\r\n                ");
			builder.Append(this.GetStartupOptions(true));
			builder.Append("}");
			builder.Append("</script>");
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o texto com o modelo das colunas do GridView.
		/// </summary>
		/// <returns></returns>
		private string GetColModel()
		{
			Hashtable[] hashtableArray = new Hashtable[this.Columns.Count];
			for(int i = 0; i < this.Columns.Count; i++)
			{
				JsonColModel model = new JsonColModel(this.Columns[i], this);
				hashtableArray[i] = model.JsonValues;
			}
			return JsonColModel.RemoveQuotesForJavaScriptMethods(_sr.Serialize(hashtableArray), this);
		}

		/// <summary>
		/// Recupera um texto com os nomes da colunas no formato correto.
		/// </summary>
		/// <returns></returns>
		private string GetColNames()
		{
			var strArray = new string[Columns.Count];
			for(int i = 0; i < Columns.Count; i++)
			{
				var column = Columns[i];
				strArray[i] = column.HeaderText;
			}
			return _sr.Serialize(strArray);
		}

		/// <summary>
		/// Recupera um texto com os nomes da colunas no formato correto.
		/// </summary>
		/// <returns></returns>
		private string GetColTooltips()
		{
			var strArray = new string[Columns.Count];
			for(int i = 0; i < Columns.Count; i++)
			{
				var column = Columns[i];
				strArray[i] = column.Tooltip;
			}
			return _sr.Serialize(strArray);
		}

		/// <summary>
		/// Recupera a coluna pelo nome informado.
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		private Column GetColumnFromDataTable(string columnName)
		{
			foreach (Column column in this.Columns)
			{
				if(column.QualifiedName == columnName)
					return column;
			}
			return null;
		}

		class ColumnPropertyPath
		{
			/// <summary>
			/// Caminho para se chegar a propriedade.
			/// </summary>
			public string[] Path;

			public string Name;

			public Column Column;

			private PropertyInfo[] _propertyPath;

			/// <summary>
			/// Recupera o caminho das propriedades.
			/// </summary>
			/// <returns></returns>
			public PropertyInfo[] GetPropertyPath(Type baseType)
			{
				if(_propertyPath != null)
					return _propertyPath;
				_propertyPath = new PropertyInfo[Path.Length];
				var currentType = baseType;
				for(var i = 0; i < Path.Length; i++)
				{
					var property = currentType.GetProperty(Path[i]);
					if(property == null)
						throw new InvalidOperationException(string.Format("Property \"{0}\" not found in \"{1}\"", Path[i], currentType.FullName + (i > 0 ? "." + string.Join(".", Path, 0, i) : "")));
					currentType = property.PropertyType;
					_propertyPath[i] = property;
				}
				return _propertyPath;
			}

			/// <summary>
			/// Recupera os valor da coluna.
			/// </summary>
			/// <param name="instance"></param>
			/// <returns></returns>
			public object GetValue(object instance)
			{
				if(Path.Length == 0)
				{
					if(Column != null && !string.IsNullOrEmpty(Column.ValuesFormatter))
						return Column.GetValuesFormatterInstance().Format(instance, Column.FieldName, instance);
					return null;
				}
				if(_propertyPath == null)
					GetPropertyPath(instance.GetType());
				var currentInstance = instance;
				for(var i = 0; i < Path.Length; i++)
				{
					if(currentInstance == null)
						throw new NullReferenceException(string.Format("O value of property \"{0}.{1}\" is null", instance.GetType().FullName, string.Join(".", Path, 0, i)));
					try
					{
						currentInstance = _propertyPath[i].GetValue(currentInstance, null);
					}
					catch(TargetInvocationException ex)
					{
						throw new Exception(string.Format("Fail on get property value from \"{0}.{1}\"", instance.GetType().FullName, string.Join(".", Path, 0, i)), ex);
					}
				}
				if(Column != null && !string.IsNullOrEmpty(Column.ValuesFormatter))
					currentInstance = Column.GetValuesFormatterInstance().Format(instance, Column.DataField, currentInstance);
				return currentInstance;
			}
		}

		/// <summary>
		/// Obtém um DataTable com base no dados contidos no IEnumerable.
		/// </summary>
		/// <param name="ien"></param>
		/// <returns></returns>
		private DataTable ObtainDataTableFromIEnumerable(IEnumerable ien)
		{
			var table = new DataTable();
			var columnProperties = new List<ColumnPropertyPath>();
			foreach (var i in PropertiesInclude.Split(','))
				if(!string.IsNullOrEmpty(i))
				{
					columnProperties.Add(new ColumnPropertyPath {
						Name = i,
						Path = i.Split('.')
					});
				}
			foreach (Column i in this.Columns)
			{
				columnProperties.Add(new ColumnPropertyPath {
					Name = i.QualifiedName,
					Path = string.IsNullOrEmpty(i.DataField) ? new string[0] : i.DataField.Split('.'),
					Column = i
				});
			}
			Type objBaseType = null;
			foreach (object obj2 in ien)
			{
				if(table.Columns.Count == 0)
				{
					objBaseType = obj2.GetType();
					foreach (var property in columnProperties)
					{
						Type columnType = null;
						var path = property.GetPropertyPath(obj2.GetType());
						if(path.Length == 0)
							columnType = typeof(string);
						else
						{
							var info = path[path.Length - 1];
							columnType = info.PropertyType;
							if(columnType.IsGenericType && (columnType.GetGenericTypeDefinition() == typeof(Nullable<>)))
								columnType = Nullable.GetUnderlyingType(columnType);
						}
						var columnName = property.Column == null ? property.Name : property.Column.QualifiedName;
						if(!table.Columns.Contains(columnName))
							table.Columns.Add(columnName, property.Column != null && !string.IsNullOrEmpty(property.Column.ValuesFormatter) ? typeof(string) : columnType);
					}
				}
				DataRow row = table.NewRow();
				foreach (var property in columnProperties)
				{
					object obj3 = property.GetValue(obj2);
					if(obj3 != null)
					{
						row[property.Name] = obj3;
					}
					else
						row[property.Name] = DBNull.Value;
				}
				table.Rows.Add(row);
			}
			return table;
		}

		/// <summary>
		/// Recupera um DataTable com base no dados contidos no IEnumerable.
		/// </summary>
		/// <param name="en"></param>
		/// <returns></returns>
		private DataTable GetDataTableFromIEnumerable(IEnumerable en)
		{
			var table = new DataTable();
			var view = en as DataView;
			if(view != null)
				return view.ToTable();
			if(en != null)
				table = this.ObtainDataTableFromIEnumerable(en);
			return table;
		}

		/// <summary>
		/// Recupera as informações sobre o rodapé do Grid.
		/// </summary>
		/// <returns></returns>
		private Hashtable GetFooterInfo()
		{
			Hashtable hashtable = new Hashtable();
			if(this.AppearanceSettings.ShowFooter)
			{
				foreach (Column column in Columns)
					hashtable[column.QualifiedName] = column.FooterValue;
			}
			return hashtable;
		}

		/// <summary>
		/// Recupera o script usado para enviar os dados para o servidor.
		/// </summary>
		/// <returns></returns>
		private string GetJQuerySubmit()
		{
			StringBuilder builder = new StringBuilder();
			if(MultiSelect)
				builder.AppendFormat("\r\n                        var _theForm = document.getElementsByTagName('FORM')[0];\r\n                        var oldSubmit = _theForm.onsubmit;\r\n                        _theForm.onsubmit = function() \r\n                        {{  \r\n                            jQuery('#{0}').attr('value', '[' + jQuery('#{1}').getGridParam('selarrrow').toString() + ']');\r\n                            if (oldSubmit != undefined && oldSubmit != null) oldSubmit();\r\n                        }};\r\n                       ", this.ClientID + "_SelectedRow", this.ClientID, this.ClientID + "_CurrentPage");
			else
				builder.AppendFormat("\r\n                        var _theForm = document.getElementsByTagName('FORM')[0];\r\n                        var oldSubmit = _theForm.onsubmit;\r\n                        _theForm.onsubmit = function() \r\n                        {{  \r\n                            jQuery('#{0}').attr('value', '[' + jQuery('#{1}').getGridParam('selrow') + ']');\r\n                            if (oldSubmit != undefined && oldSubmit != null) oldSubmit();\r\n                        }};\r\n                       ", this.ClientID + "_SelectedRow", this.ClientID, this.ClientID + "_CurrentPage");
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o script usado para carregar o tooltip.
		/// </summary>
		/// <returns></returns>
		private string GetTooltipScript()
		{
			return "var tooltips = " + GetColTooltips() + "\r\nvar grid = jQuery('#" + ClientID + "');\r\n\t\t\rvar tHeaders = grid.parent().parent().parent().find('table').find('th');\r\n                  tHeaders.each(function(index) {\r\n                 var e = $(tHeaders[index]);\r\n                 e.attr('title', tooltips[index]);\r\n});\r\n";
		}

		/// <summary>
		/// Recupera o scritp usado para carregar um erro ocorrido.
		/// </summary>
		/// <returns></returns>
		private string GetLoadErrorHandler()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("function jqGrid_aspnet_loadErrorHandler(xht, st, handler) {");
			builder.Append("if (xht.status == 404) { return; } jQuery(document.body).css('font-size','100%'); jQuery(document.body).html(xht.responseText);");
			builder.Append("}");
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o texto que representa a chave de multiseleção.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		private string GetMultiKeyString(MultiSelectKey key)
		{
			switch(key)
			{
			case MultiSelectKey.Shift:
				return "shiftKey";
			case MultiSelectKey.Ctrl:
				return "ctrlKey";
			case MultiSelectKey.Alt:
				return "altKey";
			}
			throw new Exception("Should not be here.");
		}

		/// <summary>
		/// Recupera os dados de uma página especifica.
		/// </summary>
		/// <param name="startIndex"></param>
		/// <param name="endIndex"></param>
		/// <param name="dt"></param>
		/// <returns></returns>
		private DataTable GetPagedDataTable(int startIndex, int endIndex, DataTable dt)
		{
			var table = new DataTable();
			foreach (DataColumn column in dt.Columns)
				table.Columns.Add(column.ColumnName, column.DataType);
			for(int i = startIndex; i < endIndex; i++)
			{
				DataRow row = table.NewRow();
				for(int j = 0; j < dt.Columns.Count; j++)
					row[j] = dt.Rows[i][j];
				table.Rows.InsertAt(row, 0);
			}
			return table;
		}

		/// <summary>
		/// Recupera a posição da coluna que contém a chave primária.
		/// </summary>
		/// <returns></returns>
		private int GetPrimaryKeyIndex()
		{
			foreach (Column column in this.Columns)
			{
				if(column.PrimaryKey)
				{
					return this.Columns.IndexOf(column);
				}
			}
			return 0;
		}

		/// <summary>
		/// Recupera o script usado para definir a linha selecionada e enviar os dados para o servidor.
		/// </summary>
		/// <returns></returns>
		private string GetSelectedRowPostBack()
		{
			if(((RowSelectEventHandler)base.Events[EventRowSelecting]) != null)
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("\r\n                    jQuery('#{0}').bind('click', function() {{\r\n                        var grid = jQuery('#{0}');\r\n                        var rowKey = grid.getGridParam('selrow');\r\n                        var postBackScript = grid.getGridParam('postBackUrl').replace(/jqGridParams/g, rowKey);\r\n                        jQuery('#{0}_CurrentPage').attr('value', grid.getGridParam('page'));\r\n                        jQuery('#{0}_SelectedRow').attr('value', grid.getGridParam('selrow'));\r\n                        eval(postBackScript);\r\n                    }});\r\n                ", this.ClientID);
				return builder.ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// Recupera script para restaurar o estado selecionado.
		/// </summary>
		/// <returns></returns>
		private string RestoreSelectedState()
		{
			if(MultiSelect && this.SelectedRows.Count > 0)
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("\r\n                        jQuery('#{0}').setGridParam( {{ gridComplete: function() \r\n                        {{                            \r\n                            var grid = jQuery('#{0}');\r\n", this.ClientID);
				foreach (var i in SelectedRows)
					builder.AppendFormat("                            grid.setSelection('{0}',false);\r\n", i);
				builder.Append("                        }\r\n                     });\r\n\r\n                ");
				return builder.ToString();
			}
			else if(!string.IsNullOrEmpty(this.SelectedRow))
			{
				StringBuilder builder = new StringBuilder();
				builder.AppendFormat("\r\n                        jQuery('#{0}').setGridParam( {{ gridComplete: function() \r\n                        {{                            \r\n                            var grid = jQuery('#{0}');\r\n                            grid.setSelection('{1}',false);\r\n                        }}\r\n                     }});\r\n\r\n                ", this.ClientID, this.SelectedRow);
				return builder.ToString();
			}
			return string.Empty;
		}

		/// <summary>
		/// Recupera o script usado para iniciar o Grid.
		/// </summary>
		/// <returns></returns>
		private string GetStartupJavascript()
		{
			StringBuilder builder = new StringBuilder();
			builder.Append("<script type='text/javascript'>\n");
			builder.Append("jQuery(document).ready(function() {");
			builder.Append(this.GetStartupOptions(false));
			builder.Append("});");
			builder.Append("</script>");
			return builder.ToString();
		}

		/// <summary>
		/// Recupera o script para as clausulas de pesquisa.
		/// </summary>
		/// <param name="search"></param>
		/// <param name="isLinq"></param>
		/// <returns></returns>
		private string GetWhereClause(string search, bool isLinq)
		{
			string whereClause = string.Empty;
			if((!string.IsNullOrEmpty(search) || Convert.ToBoolean(search)) && this.ToolBarSettings.ShowSearchToolBar)
				whereClause = new ToolBarSearching(this).GetWhereClause(null, isLinq);
			return whereClause;
		}

		/// <summary>
		/// Executa a exclusão da linha.
		/// </summary>
		/// <param name="e"></param>
		private void HandleDelete(RowDeleteEventArgs e)
		{
			if(base.IsBoundUsingDataSourceID)
			{
				var keys = new OrderedDictionary();
				var oldValues = new OrderedDictionary();
				DataSourceView data = this.GetData();
				if(data == null)
					throw new HttpException("DataSource is null in edit mode (on update)");
				foreach (Column column in this.Columns)
				{
					if(column.PrimaryKey)
						keys.Add(column.QualifiedName, e.RowKey);
				}
				data.Delete(keys, oldValues, new DataSourceViewOperationCallback(this.HandleDeleteCallback));
			}
		}

		private bool HandleDeleteCallback(int affectedRows, Exception ex)
		{
			if(ex != null)
				throw ex;
			return true;
		}

		/// <summary>
		/// Executa a inserção da linha.
		/// </summary>
		/// <param name="e"></param>
		private void HandleInsert(RowAddEventArgs e)
		{
			if(base.IsBoundUsingDataSourceID)
			{
				var values = new OrderedDictionary();
				DataSourceView data = this.GetData();
				if(data == null)
					throw new HttpException("DataSource is null in edit mode (on update)");
				foreach (string str in e.RowData.Keys)
					values.Add(str, e.RowData[str]);
				data.Insert(values, new DataSourceViewOperationCallback(this.HandleInsertCallback));
			}
		}

		private bool HandleInsertCallback(int affectedRows, Exception ex)
		{
			if(ex != null)
				throw ex;
			return true;
		}

		/// <summary>
		/// Executa a atualiza dos dados.
		/// </summary>
		private void HandleUpdate()
		{
			if(base.IsBoundUsingDataSourceID && (this.GetData() == null))
				throw new HttpException("DataSource is null in edit mode (on update)");
		}

		/// <summary>
		/// Executa a atualiza da linha.
		/// </summary>
		/// <param name="e"></param>
		private void HandleUpdate(RowEditEventArgs e)
		{
			if(base.IsBoundUsingDataSourceID)
			{
				var keys = new OrderedDictionary();
				var values = new OrderedDictionary();
				var oldValues = new OrderedDictionary();
				var data = this.GetData();
				if(data == null)
					throw new HttpException("DataSource is null in edit mode (on update)");
				foreach (Column column in this.Columns)
				{
					if(column.PrimaryKey)
						keys.Add(column.QualifiedName, e.RowData[column.QualifiedName]);
					if(column.Editable)
						values.Add(column.QualifiedName, e.RowData[column.QualifiedName]);
				}
				data.Update(keys, values, oldValues, new DataSourceViewOperationCallback(this.HandleUpdateCallback));
			}
		}

		private bool HandleUpdateCallback(int affectedRows, Exception ex)
		{
			if(ex != null)
				throw ex;
			return true;
		}

		/// <summary>
		/// Verifica se foi feita uma requisição para o Grid.
		/// </summary>
		/// <returns></returns>
		private bool IsGridRequest()
		{
			string str = HttpContext.Current.Request.QueryString["jqGridID"];
			return (!string.IsNullOrEmpty(str) && (str == this.ClientID));
		}

		/// <summary>
		/// Recupera a expressão de ordenação.
		/// </summary>
		/// <param name="sortExpression"></param>
		/// <param name="sortDirection"></param>
		/// <returns></returns>
		private string GetSortExpression(string sortExpression, string sortDirection)
		{
			string str = "";
			if(this.GroupSettings.GroupFields.Count > 0)
			{
				string str2 = sortExpression.Split(new char[] {
					' '
				})[0];
				string str3 = sortExpression.Split(new char[] {
					' '
				})[1].Split(new char[] {
					','
				})[0];
				sortExpression = sortExpression.Split(new char[] {
					','
				})[1];
				str = str2 + " " + str3;
			}
			if((sortExpression != null) && (sortExpression == " "))
				sortExpression = "";
			if(string.IsNullOrEmpty(sortExpression))
				return str;
			if((this.GroupSettings.GroupFields.Count > 0) && !str.EndsWith(","))
				str = str + ",";
			return (str + sortExpression + " " + sortDirection);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="retrievedData"></param>
		private void OnDataSourceViewSelectCallback(IEnumerable retrievedData)
		{
			switch(this.AjaxCallBackMode)
			{
			case AjaxCallBackMode.RequestData:
			case AjaxCallBackMode.Search:
				this.PerformRequestData(retrievedData);
				return;
			case AjaxCallBackMode.EditRow:
				this.PerformRowEdit();
				return;
			case AjaxCallBackMode.AddRow:
				this.PerformRowAdd();
				return;
			case AjaxCallBackMode.DeleteRow:
				this.PerformRowDelete();
				return;
			}
		}

		/// <summary>
		/// Processa os dados para a requisição.
		/// </summary>
		/// <param name="retrievedData"></param>
		private void PerformRequestData(IEnumerable retrievedData)
		{
			int num = Convert.ToInt32(HttpContext.Current.Request.QueryString["rows"]);
			int num2 = Convert.ToInt32(HttpContext.Current.Request.QueryString["page"]);
			string sortExpression = HttpContext.Current.Request.QueryString["sidx"];
			string sortDirection = HttpContext.Current.Request.QueryString["sord"];
			string search = HttpContext.Current.Request.QueryString["_search"];
			DataView defaultView = GetDataTableFromIEnumerable(retrievedData).DefaultView;
			PerformSearch(defaultView, search);
			if(!_dataSourceSorted)
			{
				PerformSort(defaultView, sortExpression, sortDirection);
			}
			else
				OnSorted(new EventArgs());
			DataTable dt = defaultView.ToTable();
			if(_selectArguments.TotalRowCount > -1)
				_totalRows = _selectArguments.TotalRowCount;
			int num3 = (_totalRows > 0) ? _totalRows : dt.Rows.Count;
			int num4 = (num3 > 0) ? Convert.ToInt32((int)(num3 / num)) : 1;
			if((num3 % num) != 0)
				num4++;
			int startIndex = (num2 * num) - num;
			int endIndex = (num3 > (num2 * num)) ? (startIndex + num) : num3;
			int num7 = (num3 > (num2 * num)) ? num : (endIndex - startIndex);
			if(_totalRows > 0)
			{
				startIndex = 0;
				endIndex = dt.Rows.Count;
			}
			if(((DataRequestedEventHandler)base.Events[EventDataRequested]) != null)
			{
				DataTable table2 = this.GetPagedDataTable(startIndex, endIndex, dt);
				this.OnDataRequested(new DataRequestedEventArgs(table2));
			}
			JsonResponse response = new JsonResponse();
			response.page = num2;
			response.total = num4;
			response.records = num3;
			response.rows = new JsonRow[num7 < 0 ? 0 : num7];
			response.userdata = this.GetFooterInfo();
			int num8 = 0;
			for(int i = startIndex; i < endIndex; i++)
			{
				string[] strArray = new string[this.Columns.Count];
				for(int j = 0; j < this.Columns.Count; j++)
				{
					Column column = this.Columns[j];
					string str4 = "";
					if(!string.IsNullOrEmpty(column.QualifiedName))
					{
						var colInfo = dt.Columns[column.QualifiedName];
						if(colInfo == null)
							throw new Exception(string.Format("Column {0} not found.", column.QualifiedName));
						int ordinal = colInfo.Ordinal;
						if(string.IsNullOrEmpty(column.DataFormatString) && string.IsNullOrEmpty(column.ValuesFormatter))
							str4 = dt.Rows[i].ItemArray[ordinal].ToString();
						else
							str4 = column.FormatDataValue(dt.Rows[i].ItemArray[ordinal], column.HtmlEncode);
					}
					strArray[j] = str4;
				}
				string rowKey = strArray[this.GetPrimaryKeyIndex()];
				for(int k = 0; k < this.Columns.Count; k++)
				{
					var e = new CellBindEventArgs(strArray[k], k, i, rowKey, dt.Rows[i].ItemArray);
					this.OnCellBinding(e);
					strArray[k] = e.CellHtml;
					this.OnCellBound(e);
				}
				var row = new JsonRow();
				row.id = rowKey;
				row.cell = strArray;
				response.rows[num8++] = row;
			}
			this.EmitResponse(_sr.Serialize(response));
		}

		/// <summary>
		/// Processa os dados para um novo linha.
		/// </summary>
		private void PerformRowAdd()
		{
			NameValueCollection values = new NameValueCollection();
			foreach (string str in HttpContext.Current.Request.Form.Keys)
			{
				values[str] = HttpContext.Current.Request.Form[str];
			}
			var e = new RowAddEventArgs();
			e.RowData = values;
			string str2 = HttpContext.Current.Request.QueryString["parentRowID"];
			if(!string.IsNullOrEmpty(str2))
				e.ParentRowKey = str2;
			OnRowAdding(e);
			if(!e.Cancel)
				this.HandleInsert(e);
			OnRowAdded(new EventArgs());
		}

		/// <summary>
		/// Processa os dados para a linha que está sendo apagada.
		/// </summary>
		private void PerformRowDelete()
		{
			NameValueCollection values = new NameValueCollection();
			foreach (string str in HttpContext.Current.Request.Form.Keys)
				values[str] = HttpContext.Current.Request.Form[str];
			var rows = values["id"].Split(',');
			foreach (var r in rows)
			{
				if(string.IsNullOrEmpty(r))
					continue;
				var e = new RowDeleteEventArgs();
				e.RowKey = r;
				OnRowDeleting(e);
				if(!e.Cancel)
					HandleDelete(e);
				else
					continue;
				OnRowDeleted(new EventArgs());
			}
		}

		/// <summary>
		/// Processa os dados para a linha que está sendo editada.
		/// </summary>
		private void PerformRowEdit()
		{
			var values = new NameValueCollection();
			foreach (string str in HttpContext.Current.Request.Form.Keys)
			{
				if(str != "oper")
				{
					values[str] = HttpContext.Current.Request.Form[str];
				}
			}
			string dataField = string.Empty;
			foreach (Column column in this.Columns)
			{
				if(column.PrimaryKey)
				{
					dataField = column.QualifiedName;
					break;
				}
			}
			if(!string.IsNullOrEmpty(dataField) && !string.IsNullOrEmpty(values["id"]))
			{
				values[dataField] = values["id"];
			}
			var e = new RowEditEventArgs();
			e.RowData = values;
			e.RowKey = values["id"];
			var str3 = HttpContext.Current.Request.QueryString["parentRowID"];
			if(!string.IsNullOrEmpty(str3))
				e.ParentRowKey = str3;
			OnRowEditing(e);
			if(!e.Cancel)
				HandleUpdate(e);
			OnRowEdited(new EventArgs());
		}

		/// <summary>
		/// Processa a pesquisa nos dados.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="search"></param>
		private void PerformSearch(DataView view, string search)
		{
			if(!string.IsNullOrEmpty(search) && Convert.ToBoolean(search))
			{
				string str = HttpContext.Current.Request.QueryString["filters"];
				string str2 = HttpContext.Current.Request.QueryString["searchField"];
				string searchString = HttpContext.Current.Request.QueryString["searchString"];
				string searchOperation = HttpContext.Current.Request.QueryString["searchOper"];
				if(string.IsNullOrEmpty(str) && !string.IsNullOrEmpty(str2))
					new Searching(this, str2, searchString, searchOperation).PerformSearch(view, search);
				else if(!string.IsNullOrEmpty(str))
					new MultipleSearching(this, str).PerformSearch(view);
				else if(this.ToolBarSettings.ShowSearchToolBar)
					new ToolBarSearching(this).PerformSearch(view, false);
			}
		}

		/// <summary>
		/// Processa os dados para a ordenação.
		/// </summary>
		/// <param name="view"></param>
		/// <param name="sortExpression"></param>
		/// <param name="sortDirection"></param>
		private void PerformSort(DataView view, string sortExpression, string sortDirection)
		{
			if(!string.IsNullOrEmpty(sortExpression))
			{
				var e = new SortEventArgs(sortExpression, sortDirection);
				OnSorting(e);
				if(!e.Cancel && (view.Count > 0))
					view.Sort = string.Format("{0} {1}", sortExpression, sortDirection);
				OnSorted(new EventArgs());
			}
		}

		/// <summary>
		/// Processa a chamada de retorno.
		/// </summary>
		private void ProcessCallBack()
		{
			int newPageIndex = Convert.ToInt32(HttpContext.Current.Request.QueryString["page"]);
			int num2 = Convert.ToInt32(HttpContext.Current.Request.QueryString["rows"]);
			string sortExpression = HttpContext.Current.Request.QueryString["sidx"];
			string sortDirection = HttpContext.Current.Request.QueryString["sord"];
			string search = HttpContext.Current.Request.QueryString["_search"];
			string parentRowKey = HttpContext.Current.Request.QueryString["parentRowID"];
			this.PagerSettings.CurrentPage = newPageIndex;
			this.PagerSettings.PageSize = num2;
			if(((DataRequestEventHandler)base.Events[EventDataRequesting]) != null)
			{
				string whereClause = this.GetWhereClause(search, false);
				var e = new DataRequestEventArgs(sortExpression, sortDirection, newPageIndex, whereClause, parentRowKey);
				this.OnDataRequesting(e);
				if(e.TotalRows > 0)
				{
					_totalRows = e.TotalRows;
				}
			}
			if(IsGridRequest())
			{
				_selectArguments = this.CreateDataSourceSelectArguments();
				GetData().Select(_selectArguments, new DataSourceViewSelectCallback(OnDataSourceViewSelectCallback));
			}
		}

		/// <summary>
		/// Renderiza o código HTML do Grid.
		/// </summary>
		/// <param name="writer"></param>
		private void RenderGridHtml(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("table");
			writer.WriteAttribute("id", this.ClientID);
			writer.Write('>');
			writer.WriteEndTag("table");
		}

		private void RenderHidden(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("input");
			writer.WriteAttribute("id", this.ClientID + "_SelectedRow");
			writer.WriteAttribute("name", this.UniqueID);
			writer.WriteAttribute("type", "hidden");
			writer.Write('>');
			writer.WriteEndTag("input");
			writer.WriteBeginTag("input");
			writer.WriteAttribute("id", this.ClientID + "_CurrentPage");
			writer.WriteAttribute("name", this.UniqueID);
			writer.WriteAttribute("type", "hidden");
			writer.Write('>');
			writer.WriteEndTag("input");
		}

		private void RenderPagerHtml(HtmlTextWriter writer)
		{
			writer.WriteBeginTag("div");
			writer.WriteAttribute("id", this.ClientID + "_pager");
			writer.Write('>');
			writer.WriteEndTag("div");
		}

		private void ResolveAjaxCallBackMode()
		{
			_ajaxCallModeResolved = true;
			if(this.IsGridRequest())
			{
				string str4;
				string str = HttpContext.Current.Request.Form["oper"];
				string str2 = HttpContext.Current.Request.QueryString["editMode"];
				string str3 = HttpContext.Current.Request.QueryString["_search"];
				_ajaxCallBackMode = AjaxCallBackMode.RequestData;
				if(!string.IsNullOrEmpty(str) && ((str4 = str) != null))
				{
					if(str4 == "add")
					{
						_ajaxCallBackMode = AjaxCallBackMode.AddRow;
						return;
					}
					if(str4 == "edit")
					{
						_ajaxCallBackMode = AjaxCallBackMode.EditRow;
						return;
					}
					if(str4 == "del")
					{
						_ajaxCallBackMode = AjaxCallBackMode.DeleteRow;
						return;
					}
				}
				if(!string.IsNullOrEmpty(str2))
					_ajaxCallBackMode = AjaxCallBackMode.EditRow;
				if(!string.IsNullOrEmpty(str3) && Convert.ToBoolean(str3))
					_ajaxCallBackMode = AjaxCallBackMode.Search;
			}
		}

		protected override int CreateChildControls(IEnumerable dataSource, bool dataBinding)
		{
			return 0;
		}

		/// <summary>
		/// Cria os argumentos para a seleção dos dados.
		/// </summary>
		/// <returns></returns>
		protected override DataSourceSelectArguments CreateDataSourceSelectArguments()
		{
			var rows = Convert.ToInt32(HttpContext.Current.Request.QueryString["rows"]);
			var page = Convert.ToInt32(HttpContext.Current.Request.QueryString["page"]);
			string sortExpression = HttpContext.Current.Request.QueryString["sidx"];
			string sortDirection = HttpContext.Current.Request.QueryString["sord"];
			string search = HttpContext.Current.Request.QueryString["_search"];
			DataSourceSelectArguments arguments = new DataSourceSelectArguments();
			DataSourceView data = this.GetData();
			if(data.CanSort && (sortExpression != null))
			{
				var e = new SortEventArgs(sortExpression, sortDirection);
				this.OnSorting(e);
				_dataSourceSorted = true;
				if(!e.Cancel && !string.IsNullOrEmpty(sortExpression))
					arguments.SortExpression = sortExpression + " " + sortDirection;
			}
			if(data.CanPage)
			{
				if(data.CanRetrieveTotalRowCount)
				{
					arguments.RetrieveTotalRowCount = true;
					arguments.MaximumRows = this.PagerSettings.PageSize;
				}
				else
				{
					arguments.MaximumRows = -1;
				}
				arguments.StartRowIndex = this.PagerSettings.PageSize * (this.PagerSettings.CurrentPage - 1);
			}
			return arguments;
		}

		/// <summary>
		/// Carrega os dados salvos no ViewState.
		/// </summary>
		/// <param name="savedState"></param>
		protected override void LoadViewState(object savedState)
		{
			if(savedState != null)
			{
				object[] objArray = (object[])savedState;
				base.LoadViewState(objArray[0]);
				if(objArray[1] != null)
					((IStateManager)this.ToolBarSettings).LoadViewState(objArray[1]);
				if(objArray[2] != null)
					((IStateManager)this.EditDialogSettings).LoadViewState(objArray[2]);
				if(objArray[3] != null)
					((IStateManager)this.SearchDialogSettings).LoadViewState(objArray[3]);
				if(objArray[4] != null)
					((IStateManager)this.PagerSettings).LoadViewState(objArray[4]);
				if(objArray[5] != null)
					((IStateManager)this.SortSettings).LoadViewState(objArray[5]);
				if(objArray[6] != null)
					((IStateManager)this.AppearanceSettings).LoadViewState(objArray[6]);
				if(objArray[7] != null)
					((IStateManager)this.AddDialogSettings).LoadViewState(objArray[7]);
				if(objArray[8] != null)
					((IStateManager)this.DeleteDialogSettings).LoadViewState(objArray[8]);
				if(objArray[9] != null)
					((IStateManager)this.GroupSettings).LoadViewState(objArray[10]);
			}
			else
			{
				base.LoadViewState(null);
			}
		}

		/// <summary>
		/// Salva os dados no ViewState.
		/// </summary>
		/// <returns></returns>
		protected override object SaveViewState()
		{
			object obj2 = base.SaveViewState();
			object obj3 = (_toolBarSettings != null) ? ((IStateManager)_toolBarSettings).SaveViewState() : null;
			object obj4 = (_editDialogSettings != null) ? ((IStateManager)_editDialogSettings).SaveViewState() : null;
			object obj5 = (_searchDialogSettings != null) ? ((IStateManager)_searchDialogSettings).SaveViewState() : null;
			object obj6 = (_pagerSettings != null) ? ((IStateManager)_pagerSettings).SaveViewState() : null;
			object obj7 = (_sortSettings != null) ? ((IStateManager)_sortSettings).SaveViewState() : null;
			object obj8 = (_appearanceSettings != null) ? ((IStateManager)_appearanceSettings).SaveViewState() : null;
			object obj9 = (_addDialogSettings != null) ? ((IStateManager)_addDialogSettings).SaveViewState() : null;
			object obj10 = (_deleteDialogSettings != null) ? ((IStateManager)_deleteDialogSettings).SaveViewState() : null;
			object obj11 = (this._groupSettings != null) ? ((IStateManager)this._groupSettings).SaveViewState() : null;
			return new object[] {
				obj2,
				obj3,
				obj4,
				obj5,
				obj6,
				obj7,
				obj8,
				obj9,
				obj10,
				obj11
			};
		}

		protected override void TrackViewState()
		{
			base.TrackViewState();
			if(this._toolBarSettings != null)
				((IStateManager)this._toolBarSettings).TrackViewState();
			if(this._editDialogSettings != null)
				((IStateManager)this._editDialogSettings).TrackViewState();
			if(this._searchDialogSettings != null)
				((IStateManager)this._searchDialogSettings).TrackViewState();
			if(this._pagerSettings != null)
				((IStateManager)this._pagerSettings).TrackViewState();
			if(this._sortSettings != null)
				((IStateManager)this._sortSettings).TrackViewState();
			if(this._appearanceSettings != null)
				((IStateManager)this._appearanceSettings).TrackViewState();
			if(this._addDialogSettings != null)
				((IStateManager)this._addDialogSettings).TrackViewState();
			if(this._deleteDialogSettings != null)
				((IStateManager)this._deleteDialogSettings).TrackViewState();
			if(this._toolBarSettings != null)
				((IStateManager)this._toolBarSettings).TrackViewState();
			if(this._groupSettings != null)
				((IStateManager)this._groupSettings).TrackViewState();
		}

		/// <summary>
		/// Método acionado quando dados são vinculados a célula do Grid.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnCellBinding(CellBindEventArgs e)
		{
			var handler = (CellBindEventHandler)base.Events[EventCellBinding];
			if(handler != null)
				handler(this, e);
		}

		protected internal virtual void OnCellBound(CellBindEventArgs e)
		{
			var handler = (CellBindEventHandler)base.Events[EventCellBound];
			if(handler != null)
				handler(this, e);
		}

		protected internal virtual void OnDataRequested(DataRequestedEventArgs e)
		{
			var handler = (DataRequestedEventHandler)base.Events[EventDataRequested];
			if(handler != null)
				handler(this, e);
		}

		protected internal virtual void OnDataRequesting(DataRequestEventArgs e)
		{
			var handler = (DataRequestEventHandler)base.Events[EventDataRequesting];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando um linha é adicionada.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowAdded(EventArgs e)
		{
			EventHandler handler = (EventHandler)base.Events[EventRowAdded];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando uma linha está sendo adicionada.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowAdding(RowAddEventArgs e)
		{
			var handler = (RowAddEventHandler)base.Events[EventRowAdding];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando uma linha é apagada.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowDeleted(EventArgs e)
		{
			var handler = (EventHandler)base.Events[EventRowDeleted];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando uma linha está sendo apagada.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowDeleting(RowDeleteEventArgs e)
		{
			var handler = (RowDeleteEventHandler)base.Events[EventRowDeleting];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando uma linha é editada.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowEdited(EventArgs e)
		{
			var handler = (EventHandler)base.Events[EventRowEdited];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionando quando uma linha está sendo editada.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowEditing(RowEditEventArgs e)
		{
			var handler = (RowEditEventHandler)base.Events[EventRowEditing];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando uma linha está sendo selecionado.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnRowSelecting(RowSelectEventArgs e)
		{
			var handler = (RowSelectEventHandler)base.Events[EventRowSelecting];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando ocorre um pesquisa.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnSearched(EventArgs e)
		{
			EventHandler handler = (EventHandler)base.Events[EventSearched];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando está ocorrento uma pesquisa.
		/// </summary>
		/// <param name="e"></param>
		protected internal virtual void OnSearching(SearchEventArgs e)
		{
			var handler = (SearchEventHandler)base.Events[EventSearching];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando os dados são ordenados.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnSorted(EventArgs e)
		{
			EventHandler handler = (EventHandler)base.Events[EventSorted];
			if(handler != null)
			{
				handler(this, e);
			}
		}

		/// <summary>
		/// Método acionado quando os dados estão sendo ordenados.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnSorting(SortEventArgs e)
		{
			bool isBoundUsingDataSourceID = base.IsBoundUsingDataSourceID;
			var handler = (SortEventHandler)base.Events[EventSorting];
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando o valor do link de dados é alterado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDataLinkChanged(DataLinkEventArgs e)
		{
			var handler = (DataLinkEventHandler)base.Events[EventDataLinkChanged];
			if(handler != null)
				handler(this, e);
		}

		protected override void OnPagePreLoad(object sender, EventArgs e)
		{
			base.OnPagePreLoad(sender, e);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			_sr = new JavaScriptSerializer();
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			if(this.Page != null)
			{
				if(this.IsGridRequest())
				{
					this.ProcessCallBack();
				}
				else
				{
					EnforceColumnContraints();
					if(UseSelectedRowsDataLink && DataLink != null)
					{
						ColumnPropertyPath propInfo = null;
						foreach (object i in DataLink)
						{
							if(propInfo == null)
							{
								foreach (Column col in this.Columns)
									if(col.PrimaryKey)
									{
										propInfo = new ColumnPropertyPath {
											Name = col.QualifiedName,
											Path = string.IsNullOrEmpty(col.DataField) ? new string[0] : col.DataField.Split('.'),
											Column = col
										};
										break;
									}
								if(propInfo == null)
									break;
							}
							var key = propInfo.GetValue(i.GetType());
							if(key != null)
								this.SelectedRows.Add(key.ToString());
						}
					}
				}
			}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if(!base.DesignMode)
			{
				string script = ((this.HierarchySettings.HierarchyMode == HierarchyMode.Child) || (this.HierarchySettings.HierarchyMode == HierarchyMode.ParentAndChild)) ? this.GetChildSubGridJavaScript() : this.GetStartupJavascript();
				ScriptManager.RegisterStartupScript(this, typeof(GridView), "_jqrid_startup" + this.ClientID, script, false);
				RenderGridHtml(writer);
				if(this.ToolBarSettings.ToolBarPosition != ToolBarPosition.Hidden)
					RenderPagerHtml(writer);
				RenderHidden(writer);
			}
		}

		/// <summary>
		/// Procura um control de maneira recursiva.
		/// </summary>
		/// <param name="root"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		internal Control FindControlRecursive(Control root, string id)
		{
			if(root.ID == id)
				return root;
			foreach (Control control in root.Controls)
			{
				Control control2 = this.FindControlRecursive(control, id);
				if(control2 != null)
				{
					return control2;
				}
			}
			return null;
		}

		public override void DataBind()
		{
		}

		/// <summary>
		/// Exporta os dados do Grid para o Excel.
		/// </summary>
		/// <param name="fileName"></param>
		public void ExportToExcel(string fileName)
		{
			DataGrid child = new DataGrid();
			child.ID = this.ID + "_exportGrid";
			this.Controls.Add(child);
			child.DataSource = this.DataSource;
			child.DataSourceID = this.DataSourceID;
			child.DataBind();
			HttpContext.Current.Response.ClearContent();
			HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + fileName);
			HttpContext.Current.Response.ContentType = "application/excel";
			var writer = new System.IO.StringWriter();
			HtmlTextWriter writer2 = new HtmlTextWriter(writer);
			child.RenderControl(writer2);
			HttpContext.Current.Response.Write(writer.ToString());
			HttpContext.Current.Response.End();
		}

		/// <summary>
		/// Verifica se pode realiza um bind com o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public virtual bool IsBindableType(Type type)
		{
			if(type == null)
			{
				return false;
			}
			Type underlyingType = Nullable.GetUnderlyingType(type);
			if(underlyingType != null)
			{
				type = underlyingType;
			}
			if(((!type.IsPrimitive && (type != typeof(string))) && ((type != typeof(DateTime)) && (type != typeof(decimal)))) && ((type != typeof(Guid)) && (type != typeof(DateTimeOffset))))
			{
				return (type == typeof(TimeSpan));
			}
			return true;
		}

		/// <summary>
		/// Carrega os dados postados.
		/// </summary>
		/// <param name="postDataKey"></param>
		/// <param name="postCollection"></param>
		/// <returns></returns>
		public bool LoadPostData(string postDataKey, NameValueCollection postCollection)
		{
			var data = postCollection[postDataKey];
			var startPos = data.IndexOf('[') + 1;
			var endPos = data.LastIndexOf(']');
			var selecteds = (startPos != -1 && endPos > startPos ? data.Substring(startPos, data.LastIndexOf(']') - startPos).Split(',') : new string[0]);
			string str2 = data.Substring(data.LastIndexOf(',') + 1).Trim();
			if(selecteds.Length > 0)
				if(MultiSelect)
				{
					var collection = new RowCollection();
					foreach (var i in selecteds)
						collection.Add(i);
					this.SelectedRows = collection;
				}
				else
					this.SelectedRow = selecteds[0];
			else
			{
				this.SelectedRows = new RowCollection();
				this.SelectedRow = null;
			}
			if(!string.IsNullOrEmpty(str2))
				this.PagerSettings.CurrentPage = Convert.ToInt32(str2);
			return false;
		}

		public void RaisePostBackEvent(string eventArgument)
		{
			SelectedRow = eventArgument;
			var e = new RowSelectEventArgs();
			e.RowKey = eventArgument;
			OnRowSelecting(e);
		}

		public void RaisePostDataChangedEvent()
		{
		}

		public void ShowEditValidationMessage(string errorMessage)
		{
			HttpContext.Current.Response.Clear();
			HttpContext.Current.Response.StatusCode = 500;
			HttpContext.Current.Response.StatusDescription = errorMessage;
			HttpContext.Current.Response.Write(errorMessage);
			HttpContext.Current.Response.End();
		}
	}
}
