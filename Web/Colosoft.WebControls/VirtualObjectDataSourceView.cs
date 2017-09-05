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
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using System.Runtime.InteropServices;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.Compilation;
using System.Globalization;
using System.Security.Permissions;
using System.Linq;

namespace Colosoft.WebControls
{
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class VirtualObjectDataSourceView : DataSourceView, IStateManager
	{
		/// <summary>
		/// Representa os dados de um método.
		/// </summary>
		[StructLayout(LayoutKind.Sequential)]
		struct ObjectDataSourceMethod
		{
			/// <summary>
			/// Operação que será realizada.
			/// </summary>
			internal DataSourceOperation Operation;

			/// <summary>
			/// Tipo onde o método está inserido.
			/// </summary>
			internal Type Type;

			/// <summary>
			/// Parametros que serão repassados para o método.
			/// </summary>
			internal OrderedDictionary Parameters;

			/// <summary>
			/// Informações do método.
			/// </summary>
			internal MethodInfo MethodInfo;

			/// <summary>
			/// Identifica se é para configurar o tamanho da pagina de dados para a lista virtual.
			/// </summary>
			internal bool ConfigureMaximumRows;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="operation"></param>
			/// <param name="type"></param>
			/// <param name="methodInfo"></param>
			/// <param name="configureMaximumRows"></param>
			/// <param name="parameters"></param>
			internal ObjectDataSourceMethod(DataSourceOperation operation, Type type, MethodInfo methodInfo, bool configureMaximumRows, OrderedDictionary parameters)
			{
				this.Operation = operation;
				this.Type = type;
				this.Parameters = parameters;
				this.MethodInfo = methodInfo;
				this.ConfigureMaximumRows = configureMaximumRows;
			}
		}

		/// <summary>
		/// Armazena o resultado de uma datasource.
		/// </summary>
		class ObjectDataSourceResult
		{
			/// <summary>
			/// Quantidade de linhas afetadas.
			/// </summary>
			internal int AffectedRows;

			/// <summary>
			/// Valor de retorno.
			/// </summary>
			internal object ReturnValue;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="returnValue"></param>
			/// <param name="affectedRows"></param>
			internal ObjectDataSourceResult(object returnValue, int affectedRows)
			{
				this.ReturnValue = returnValue;
				this.AffectedRows = affectedRows;
			}
		}

		/// <summary>
		/// Armazena os valores padrões.
		/// </summary>
		static class DefaultValues
		{
			public static string FactoryTypeName;

			public static string FactoryMethod;

			public static string FactoryInstanceMethod = "get_Instance";
		}

		private delegate Return Func<Arg1, Arg2, Return> (Arg1 arg1, Arg2 arg2);

		private ConflictOptions _conflictDetection;

		private HttpContext _context;

		private bool _convertNullToDBNull;

		private string _dataObjectTypeName;

		private string _createDataObjectMethod;

		private ParameterCollection _createDataObjectParameters;

		private string _deleteMethod;

		private ParameterCollection _deleteParameters;

		private bool _enablePaging;

		private string _filterExpression;

		private ParameterCollection _filterParameters;

		private string _insertMethod;

		private ParameterCollection _insertParameters;

		private string _maximumRowsParameterName;

		private string _oldValuesParameterFormatString;

		private VirtualObjectDataSource _owner;

		private string _selectCountMethod;

		private string _selectMethod;

		private ParameterCollection _selectParameters;

		private string _sortParameterName;

		private string _startRowIndexParameterName;

		private string _selectByKeysMethod;

		private ParameterCollection _selectByKeysParameters;

		private bool _tracking;

		private string _typeName;

		private string _factoryTypeName = DefaultValues.FactoryTypeName;

		private string _factoryInstanceMethod = DefaultValues.FactoryInstanceMethod;

		private string _factoryMethod = DefaultValues.FactoryMethod;

		private string _contractName;

		private string _culture;

		private CultureInfo _cultureInfo;

		private string _updateMethod;

		private ParameterCollection _updateParameters;

		private VirtualObjectDataSourceUpdateStrategy _updateStrategy = VirtualObjectDataSourceUpdateStrategy.Normal;

		private VirtualObjectDataSourceDeleteStrategy _deleteStrategy = VirtualObjectDataSourceDeleteStrategy.Normal;

		private static readonly object EventDeleted;

		private static readonly object EventDeleting;

		private static readonly object EventFiltering;

		private static readonly object EventInserted;

		private static readonly object EventInserting;

		private static readonly object EventObjectCreated;

		private static readonly object EventObjectCreating;

		private static readonly object EventObjectDisposing;

		private static readonly object EventSelected;

		private static readonly object EventSelecting;

		private static readonly object EventUpdated;

		private static readonly object EventUpdating;

		private static readonly object EventSelectingByKeys;

		private static readonly object EventSelectedByKeys;

		private static readonly MethodInfo GetParemeterValueMethodInfo;

		/// <summary>
		/// Evento acionado quando o objeto é apagado.
		/// </summary>
		public event VirtualObjectDataSourceStatusEventHandler Deleted {
			add
			{
				base.Events.AddHandler(EventDeleted, value);
			}
			remove {
				base.Events.RemoveHandler(EventDeleted, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o objeto está sendo apagado.
		/// </summary>
		public event VirtualObjectDataSourceMethodEventHandler Deleting {
			add
			{
				base.Events.AddHandler(EventDeleting, value);
			}
			remove {
				base.Events.RemoveHandler(EventDeleting, value);
			}
		}

		/// <summary>
		/// Evento acionado quando os dados estão sendo filtrados.
		/// </summary>
		public event VirtualObjectDataSourceFilteringEventHandler Filtering {
			add
			{
				base.Events.AddHandler(EventFiltering, value);
			}
			remove {
				base.Events.RemoveHandler(EventFiltering, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o item é inserido.
		/// </summary>
		public event VirtualObjectDataSourceStatusEventHandler Inserted {
			add
			{
				base.Events.AddHandler(EventInserted, value);
			}
			remove {
				base.Events.RemoveHandler(EventInserted, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o item está sendo inserido.
		/// </summary>
		public event VirtualObjectDataSourceMethodEventHandler Inserting {
			add
			{
				base.Events.AddHandler(EventInserting, value);
			}
			remove {
				base.Events.RemoveHandler(EventInserting, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o objeto é criado.
		/// </summary>
		public event VirtualObjectDataSourceObjectEventHandler ObjectCreated {
			add
			{
				base.Events.AddHandler(EventObjectCreated, value);
			}
			remove {
				base.Events.RemoveHandler(EventObjectCreated, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o objeto está sendo criado.
		/// </summary>
		public event VirtualObjectDataSourceObjectEventHandler ObjectCreating {
			add
			{
				base.Events.AddHandler(EventObjectCreating, value);
			}
			remove {
				base.Events.RemoveHandler(EventObjectCreating, value);
			}
		}

		public event VirtualObjectDataSourceDisposingEventHandler ObjectDisposing {
			add
			{
				base.Events.AddHandler(EventObjectDisposing, value);
			}
			remove {
				base.Events.RemoveHandler(EventObjectDisposing, value);
			}
		}

		/// <summary>
		/// Evento acionado quando um item é selecionado.
		/// </summary>
		public event VirtualObjectDataSourceStatusEventHandler Selected {
			add
			{
				base.Events.AddHandler(EventSelected, value);
			}
			remove {
				base.Events.RemoveHandler(EventSelected, value);
			}
		}

		/// <summary>
		/// Evento acionado qaundo o item está sendo selecionado.
		/// </summary>
		public event VirtualObjectDataSourceSelectingEventHandler Selecting {
			add
			{
				base.Events.AddHandler(EventSelecting, value);
			}
			remove {
				base.Events.RemoveHandler(EventSelecting, value);
			}
		}

		/// <summary>
		/// Evento acionado quando os itens estiverem sendo selecionados pelas chaves.
		/// </summary>
		public event VirtualObjectDataSourceSeletingByKeysEventHandler SelectingByKeys {
			add
			{
				base.Events.AddHandler(EventSelectingByKeys, value);
			}
			remove {
				base.Events.RemoveHandler(EventSelectingByKeys, value);
			}
		}

		/// <summary>
		/// Evento acionado quando os itens estiverem sendo selecionados pelas chaves.
		/// </summary>
		public event VirtualObjectDataSourceStatusEventHandler SelectedByKeys {
			add
			{
				base.Events.AddHandler(EventSelectedByKeys, value);
			}
			remove {
				base.Events.RemoveHandler(EventSelectedByKeys, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o item é atualizado.
		/// </summary>
		public event VirtualObjectDataSourceStatusEventHandler Updated {
			add
			{
				base.Events.AddHandler(EventUpdated, value);
			}
			remove {
				base.Events.RemoveHandler(EventUpdated, value);
			}
		}

		/// <summary>
		/// Evento acionado quando o item está sendo atualizado.
		/// </summary>
		public event VirtualObjectDataSourceMethodEventHandler Updating {
			add
			{
				base.Events.AddHandler(EventUpdating, value);
			}
			remove {
				base.Events.RemoveHandler(EventUpdating, value);
			}
		}

		/// <summary>
		/// Estratégia para atualização dos dados.
		/// </summary>
		public VirtualObjectDataSourceUpdateStrategy UpdateStrategy
		{
			get
			{
				return _updateStrategy;
			}
			set
			{
				_updateStrategy = value;
			}
		}

		/// <summary>
		/// Estratégia de exclusão dos itens.
		/// </summary>
		public VirtualObjectDataSourceDeleteStrategy DeleteStrategy
		{
			get
			{
				return _deleteStrategy;
			}
			set
			{
				_deleteStrategy = value;
			}
		}

		/// <summary>
		/// Nome do método que será usado para inserir um novo item.
		/// </summary>
		public string InsertMethod
		{
			get
			{
				if(_insertMethod == null)
				{
					return string.Empty;
				}
				return _insertMethod;
			}
			set
			{
				_insertMethod = value;
			}
		}

		/// <summary>
		/// Nome do método de exclusão que será usado pela instancia.
		/// </summary>
		public string DeleteMethod
		{
			get
			{
				if(_deleteMethod == null)
				{
					return string.Empty;
				}
				return _deleteMethod;
			}
			set
			{
				_deleteMethod = value;
			}
		}

		/// <summary>
		/// Nome do método que será usado para recupera os itens.
		/// </summary>
		public string SelectMethod
		{
			get
			{
				if(_selectMethod == null)
					return string.Empty;
				return _selectMethod;
			}
			set
			{
				if(SelectMethod != value)
				{
					_selectMethod = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Nome do método usado para recupera os
		/// dados pelas chaves informadas.
		/// </summary>
		public string SelectByKeysMethod
		{
			get
			{
				if(_selectByKeysMethod == null)
					return string.Empty;
				return _selectByKeysMethod;
			}
			set
			{
				if(_selectByKeysMethod != value)
				{
					_selectByKeysMethod = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Nome do método usado para recuperar a quantidade de itens da seleção.
		/// </summary>
		public string SelectCountMethod
		{
			get
			{
				if(_selectCountMethod == null)
					return string.Empty;
				return _selectCountMethod;
			}
			set
			{
				if(SelectCountMethod != value)
				{
					_selectCountMethod = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Nome do método que será usado para atualiza um item.
		/// </summary>
		public string UpdateMethod
		{
			get
			{
				if(_updateMethod == null)
					return string.Empty;
				return _updateMethod;
			}
			set
			{
				_updateMethod = value;
			}
		}

		/// <summary>
		/// Lista dos parametros que serão usados pelo método Insert.
		/// </summary>
		public ParameterCollection InsertParameters
		{
			get
			{
				if(_insertParameters == null)
					_insertParameters = new ParameterCollection();
				return _insertParameters;
			}
		}

		/// <summary>
		/// Lista dos parametros que serão usados pelo método Delete.
		/// </summary>
		public ParameterCollection DeleteParameters
		{
			get
			{
				if(_deleteParameters == null)
					_deleteParameters = new ParameterCollection();
				return _deleteParameters;
			}
		}

		/// <summary>
		/// Parametros que serão usados pelo método Select.
		/// </summary>
		public ParameterCollection SelectParameters
		{
			get
			{
				if(_selectParameters == null)
				{
					_selectParameters = new ParameterCollection();
					_selectParameters.ParametersChanged += new EventHandler(SelectParametersChangedEventHandler);
					if(this._tracking)
						((IStateManager)_selectParameters).TrackViewState();
				}
				return _selectParameters;
			}
		}

		/// <summary>
		/// Parametros que serão usados pelo método GetByKeys.
		/// </summary>
		public ParameterCollection SelectByKeysParameters
		{
			get
			{
				if(_selectByKeysParameters == null)
				{
					_selectByKeysParameters = new ParameterCollection();
					_selectByKeysParameters.ParametersChanged += SelectByKeysParametersChanged;
					if(this._tracking)
						((IStateManager)_selectByKeysParameters).TrackViewState();
				}
				return _selectByKeysParameters;
			}
		}

		/// <summary>
		/// Parametros que serão usados pelo método de atualização.
		/// </summary>
		public ParameterCollection UpdateParameters
		{
			get
			{
				if(_updateParameters == null)
					_updateParameters = new ParameterCollection();
				return _updateParameters;
			}
		}

		/// <summary>
		/// Recupera e define se a instancia suporta paginação no resultado da seleção.
		/// </summary>
		public bool EnablePaging
		{
			get
			{
				return _enablePaging;
			}
			set
			{
				if(EnablePaging != value)
				{
					_enablePaging = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Identifica se a instancia suporta a operação de exclusão.
		/// </summary>
		public override bool CanDelete
		{
			get
			{
				return (this.DeleteMethod.Length != 0);
			}
		}

		/// <summary>
		/// Identifica se a instancia suporta a operação de inserção
		/// </summary>
		public override bool CanInsert
		{
			get
			{
				return (this.InsertMethod.Length != 0);
			}
		}

		/// <summary>
		/// Identifica se a instancia suporta paginar o resultado.
		/// </summary>
		public override bool CanPage
		{
			get
			{
				return this.EnablePaging;
			}
		}

		/// <summary>
		/// Identifica se a instancia suporta receber a quantidade total de linhas
		/// através de um recurso especifico.
		/// </summary>
		public override bool CanRetrieveTotalRowCount
		{
			get
			{
				if(SelectCountMethod.Length <= 0)
					return EnablePaging;
				return true;
			}
		}

		/// <summary>
		/// Identifica se a instancia suporta ordenação.
		/// </summary>
		public override bool CanSort
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Identifica se a instancia pode atualizar os dados.
		/// </summary>
		public override bool CanUpdate
		{
			get
			{
				return (this.UpdateMethod.Length != 0);
			}
		}

		/// <summary>
		/// Obtém ou define um valor que determina como a instancia 
		/// realiza atualizações e exclusões quando os dados em uma 
		/// linha em que houve alterações de dados foram alterados no 
		/// armazenamento durante o tempo da operação.
		/// </summary>
		public ConflictOptions ConflictDetection
		{
			get
			{
				return _conflictDetection;
			}
			set
			{
				if((value < ConflictOptions.OverwriteChanges) || (value > ConflictOptions.CompareAllValues))
				{
					throw new ArgumentOutOfRangeException("value");
				}
				_conflictDetection = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Identifica se a instancia irá converto os valores nulos para DBNull.
		/// </summary>
		public bool ConvertNullToDBNull
		{
			get
			{
				return _convertNullToDBNull;
			}
			set
			{
				_convertNullToDBNull = value;
			}
		}

		/// <summary>
		/// Obtém ou define o nome de uma classe que a instancia irá usar 
		/// como parâmetro em uma operação de dados. A instancia usa a classe 
		/// especificada em vez de objetos de parâmetro que estão nas coleções 
		/// de vários parâmetros.
		/// </summary>
		public string DataObjectTypeName
		{
			get
			{
				if(_dataObjectTypeName == null)
					return string.Empty;
				return _dataObjectTypeName;
			}
			set
			{
				if(DataObjectTypeName != value)
				{
					_dataObjectTypeName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Método usado para cria o objeto de dados.
		/// </summary>
		public string CreateDataObjectMethod
		{
			get
			{
				if(_createDataObjectMethod == null)
					return string.Empty;
				return _createDataObjectMethod;
			}
			set
			{
				if(CreateDataObjectMethod != value)
				{
					_createDataObjectMethod = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Parametros que serão usados pelo método que cria o objeto de dados.
		/// </summary>
		public ParameterCollection CreateDataObjectParameters
		{
			get
			{
				if(_createDataObjectParameters == null)
				{
					_createDataObjectParameters = new ParameterCollection();
					_createDataObjectParameters.ParametersChanged += CreateObjectParametersChanged;
					if(this._tracking)
						((IStateManager)_createDataObjectParameters).TrackViewState();
				}
				return _createDataObjectParameters;
			}
		}

		/// <summary>
		/// Método acionado quando os parametros do método 
		/// de criação do objeto de dados forem alterados.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CreateObjectParametersChanged(object sender, EventArgs e)
		{
			this.OnDataSourceViewChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Obtém ou define uma expressão de filtragem que é aplicada quando o 
		/// método objeto comercial que é identificado pela propriedade 
		/// SelectMethod é chamado.
		/// </summary>
		public string FilterExpression
		{
			get
			{
				if(_filterExpression == null)
				{
					return string.Empty;
				}
				return _filterExpression;
			}
			set
			{
				if(FilterExpression != value)
				{
					_filterExpression = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Obtém um conjunto de parâmetros que estão associados com qualquer 
		/// parâmetro espaços reservados que estão em FilterExpression.
		/// </summary>
		public ParameterCollection FilterParameters
		{
			get
			{
				if(_filterParameters == null)
				{
					_filterParameters = new ParameterCollection();
					_filterParameters.ParametersChanged += new EventHandler(SelectParametersChangedEventHandler);
					if(_tracking)
					{
						((IStateManager)_filterParameters).TrackViewState();
					}
				}
				return _filterParameters;
			}
		}

		/// <summary>
		/// Obtém ou define o nome do parâmetro do método de recuperação de dados 
		/// que é usado para indicar o número de registros a recuperar para paginação 
		/// de fonte de dados de suporte.
		/// </summary>
		public string MaximumRowsParameterName
		{
			get
			{
				return _maximumRowsParameterName;
			}
			set
			{
				if(MaximumRowsParameterName != value)
				{
					_maximumRowsParameterName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Obtém ou define uma seqüência de formato a aplicar-se os nomes dos parâmetros 
		/// para os valores originais que são passados para os métodos Update ou Delete.
		/// </summary>
		[DefaultValue("{0}"), Category("Data")]
		public string OldValuesParameterFormatString
		{
			get
			{
				if(_oldValuesParameterFormatString == null)
				{
					return "{0}";
				}
				return _oldValuesParameterFormatString;
			}
			set
			{
				_oldValuesParameterFormatString = value;
				OnDataSourceViewChanged(EventArgs.Empty);
			}
		}

		/// <summary>
		/// Obtém ou define o nome do parâmetro método de recuperação de dados que é usado 
		/// para especificar uma expressão de classificação para a classificação da fonte 
		/// de dados de suporte.
		/// </summary>
		public string SortParameterName
		{
			get
			{
				if(_sortParameterName == null)
				{
					return string.Empty;
				}
				return _sortParameterName;
			}
			set
			{
				if(SortParameterName != value)
				{
					_sortParameterName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Obtém ou define o nome do parâmetro do método de recuperação de dados que é 
		/// usado para indicar o índice inteiro do primeiro registro para recuperar a 
		/// partir do conjunto de resultados para a paginação de fonte de dados de suporte.
		/// </summary>
		public string StartRowIndexParameterName
		{
			get
			{
				return _startRowIndexParameterName;
			}
			set
			{
				if(StartRowIndexParameterName != value)
				{
					_startRowIndexParameterName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Obtém ou define o nome da classe que representa a instância de controle.
		/// </summary>
		public string TypeName
		{
			get
			{
				if(_typeName == null)
				{
					return string.Empty;
				}
				return _typeName;
			}
			set
			{
				if(TypeName != value)
				{
					_typeName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Nome do tipo da classe de fabrica abstrata que será usada.
		/// </summary>
		public string FactoryTypeName
		{
			get
			{
				if(_factoryTypeName == null)
					return string.Empty;
				return _factoryTypeName;
			}
			set
			{
				if(FactoryTypeName != value)
				{
					_factoryTypeName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Método da fabrica abstrata usado para recupera a instancia da classe abstrata.
		/// </summary>
		[DefaultValue("get_Instance")]
		public string FactoryInstanceMethod
		{
			get
			{
				if(_factoryInstanceMethod == null)
					return string.Empty;
				return _factoryInstanceMethod;
			}
			set
			{
				if(FactoryInstanceMethod != value)
				{
					_factoryInstanceMethod = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Método usado para criar a instancia do tipo que será trabalhada.
		/// </summary>
		public string FactoryMethod
		{
			get
			{
				if(_factoryMethod == null)
					return string.Empty;
				return _factoryMethod;
			}
			set
			{
				if(FactoryMethod != value)
				{
					_factoryMethod = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Nome do contrato que serão utilizado para recupera 
		/// a instancia do tipo que fornece acesso aos métodos
		/// das operações.
		/// </summary>
		public string ContractName
		{
			get
			{
				return _contractName;
			}
			set
			{
				if(_contractName != value)
				{
					_contractName = value;
					OnDataSourceViewChanged(EventArgs.Empty);
				}
			}
		}

		/// <summary>
		/// Cultura que será utilizada.
		/// </summary>
		public string Culture
		{
			get
			{
				return _culture;
			}
			set
			{
				if(_culture != value)
				{
					_culture = value;
					if(!string.IsNullOrEmpty(value))
						_cultureInfo = System.Globalization.CultureInfo.GetCultureInfo(value);
					else
						_cultureInfo = null;
				}
			}
		}

		/// <summary>
		/// Cultura configurada.
		/// </summary>
		public CultureInfo CultureInfo
		{
			get
			{
				return _cultureInfo;
			}
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static VirtualObjectDataSourceView()
		{
			EventDeleted = new object();
			EventDeleting = new object();
			EventFiltering = new object();
			EventInserted = new object();
			EventInserting = new object();
			EventObjectCreated = new object();
			EventObjectCreating = new object();
			EventObjectDisposing = new object();
			EventSelected = new object();
			EventSelecting = new object();
			EventUpdated = new object();
			EventUpdating = new object();
			EventSelectingByKeys = new object();
			EventSelectedByKeys = new object();
			GetParemeterValueMethodInfo = typeof(Parameter).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic, null, CallingConventions.Standard, new Type[] {
				typeof(object),
				typeof(bool)
			}, null);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="name"></param>
		/// <param name="context"></param>
		public VirtualObjectDataSourceView(VirtualObjectDataSource owner, string name, HttpContext context) : base(owner, name)
		{
			_owner = owner;
			_context = context;
		}

		/// <summary>
		/// Converte o valor do parametro para o objeto do tipo informado.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="type"></param>
		/// <param name="paramName"></param>
		/// <param name="culture">Cultura que será utilizada na conversão.</param>
		/// <returns></returns>
		private static object ConvertType(object value, Type type, string paramName, CultureInfo culture)
		{
			string text = value as string;
			if(text != null)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(type);
				if(converter == null)
					return value;
				try
				{
					if(culture != null)
						value = converter.ConvertFromString(null, culture, text);
					else
						value = converter.ConvertFromInvariantString(text);
				}
				catch(NotSupportedException)
				{
					throw new InvalidOperationException(string.Format("Cannot convert value \"{0}\" of parameter \"{0}\" to \"{1}\"", paramName, typeof(string).FullName, type.FullName));
				}
				catch(Exception ex)
				{
					throw new InvalidOperationException(string.Format("Cannot convert value \"{1}\" of parameter \"{0}\" to \"{2}\"", paramName, text, type.FullName), ex);
				}
			}
			return value;
		}

		/// <summary>
		/// Recuipera o valor do objeto.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="destinationType"></param>
		/// <param name="paramName"></param>
		/// <param name="culture">Cultura que será usada para vincular o valor ao objeto.</param>
		/// <returns></returns>
		private static object BuildObjectValue(object value, Type destinationType, string paramName, CultureInfo culture)
		{
			if((value != null) && !destinationType.IsInstanceOfType(value))
			{
				Type elementType = destinationType;
				bool flag = false;
				if(destinationType.IsGenericType && (destinationType.GetGenericTypeDefinition() == typeof(Nullable<>)))
				{
					elementType = destinationType.GetGenericArguments()[0];
					flag = true;
				}
				else if(destinationType.IsByRef)
				{
					elementType = destinationType.GetElementType();
				}
				value = ConvertType(value, elementType, paramName, culture);
				if(flag)
				{
					Type type = value.GetType();
					if(elementType != type)
					{
						throw new InvalidOperationException(string.Format("Cannot convert value \"{0}\" of parameter \"{0}\" to \"{1}\"", paramName, type.FullName, elementType.FullName));
					}
				}
			}
			else if(value is int[] && destinationType.IsArray && destinationType.GetElementType().IsEnum)
			{
				var elementType = destinationType.GetElementType();
				var intValues = (int[])value;
				var values2 = Array.CreateInstance(elementType, intValues.Length);
				var enumValues = Enum.GetValues(elementType);
				for(var i = 0; i < intValues.Length; i++)
				{
					object enumValue = intValues[i];
					var found = false;
					foreach (var j in enumValues)
						if((int)j == intValues[i])
						{
							enumValue = j;
							found = true;
						}
					if(found)
						values2.SetValue(enumValue, i);
				}
				value = values2;
			}
			return value;
		}

		/// <summary>
		/// Constrói uma instancia do objeto do tipo informado com base nos parametros de entrada.
		/// </summary>
		/// <param name="dataObjectType"></param>
		/// <param name="inputParameters"></param>
		/// <returns></returns>
		private object BuildDataObject(Type dataObjectType, IDictionary inputParameters)
		{
			object component = null;
			if(!string.IsNullOrEmpty(CreateDataObjectMethod))
				component = ExecuteCreateDataObject();
			else
				component = Activator.CreateInstance(dataObjectType);
			var editableObject = component as IEditableObject;
			if(editableObject != null)
				editableObject.BeginEdit();
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
			foreach (DictionaryEntry entry in inputParameters)
			{
				string name = (entry.Key == null) ? string.Empty : entry.Key.ToString();
				PropertyDescriptor descriptor = properties.Find(name, true);
				if(descriptor == null)
					throw new InvalidOperationException(string.Format("Property {0} not found in {1}", name, this._owner.ID));
				if(descriptor.IsReadOnly)
				{
					if(typeof(IEnumerable).IsAssignableFrom(descriptor.PropertyType))
					{
						if(entry.Value == null)
							continue;
						var propertyValue = descriptor.GetValue(component);
						if(propertyValue != null && entry.Value is IEnumerable && typeof(IList).IsAssignableFrom(propertyValue.GetType()))
						{
							var destination = propertyValue as IList;
							var source = (IEnumerable)entry.Value;
							var updated = new List<object>();
							foreach (var i in source)
							{
								if(i != null)
								{
									var found = false;
									for(var j = 0; j < destination.Count; j++)
										if(i.Equals(destination[j]))
										{
											destination[j] = i;
											updated.Add(i);
											found = true;
											continue;
										}
									if(!found)
									{
										destination.Add(i);
										updated.Add(i);
									}
								}
							}
							var removeItems = new Stack<int>();
							for(var i = 0; i < destination.Count; i++)
							{
								var item = destination[i];
								if(!updated.Exists(f => !object.ReferenceEquals(f, null) && f.Equals(item)))
								{
									destination.RemoveAt(i--);
								}
							}
							continue;
						}
					}
					throw new InvalidOperationException(string.Format("Property {0} in {1} is readonly.", name, this._owner.ID));
				}
				object obj3 = BuildObjectValue(entry.Value, descriptor.PropertyType, name, CultureInfo);
				descriptor.SetValue(component, obj3);
			}
			if(editableObject != null)
				editableObject.EndEdit();
			return component;
		}

		/// <summary>
		/// Método usado para preecher os dados da instancia com o parametros informados
		/// </summary>
		/// <param name="component"></param>
		/// <param name="inputParameters"></param>
		/// <returns></returns>
		private object FillDataObject(object component, IDictionary inputParameters)
		{
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(component);
			foreach (DictionaryEntry entry in inputParameters)
			{
				string name = (entry.Key == null) ? string.Empty : entry.Key.ToString();
				PropertyDescriptor descriptor = properties.Find(name, true);
				if(descriptor == null)
					throw new InvalidOperationException(string.Format("Property {0} not found in {1}", name, this._owner.ID));
				if(descriptor.IsReadOnly)
				{
					if(typeof(IEnumerable).IsAssignableFrom(descriptor.PropertyType))
					{
						if(entry.Value == null)
							continue;
						var propertyValue = descriptor.GetValue(component);
						if(propertyValue != null && entry.Value is IEnumerable && typeof(IList).IsAssignableFrom(propertyValue.GetType()))
						{
							var destination = propertyValue as IList;
							var source = (IEnumerable)entry.Value;
							var updated = new List<object>();
							foreach (var i in source)
							{
								if(i != null)
								{
									var found = false;
									for(var j = 0; j < destination.Count; j++)
										if(i.Equals(destination[j]))
										{
											if(!object.ReferenceEquals(i, destination[j]))
												destination[j] = i;
											updated.Add(i);
											found = true;
											continue;
										}
									if(!found)
									{
										destination.Add(i);
										updated.Add(i);
									}
								}
							}
							var removeItems = new Stack<int>();
							for(var i = 0; i < destination.Count; i++)
							{
								var item = destination[i];
								if(!updated.Exists(f => !object.ReferenceEquals(f, null) && f.Equals(item)))
								{
									destination.RemoveAt(i--);
								}
							}
							continue;
						}
					}
					throw new InvalidOperationException(string.Format("Property {0} in {1} is readonly.", name, this._owner.ID));
				}
				object obj3 = BuildObjectValue(entry.Value, descriptor.PropertyType, name, CultureInfo);
				descriptor.SetValue(component, obj3);
			}
			return component;
		}

		/// <summary>
		/// Método acionado quando os parametros do método de seleção forem alterados.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="e"></param>
		private void SelectParametersChangedEventHandler(object o, EventArgs e)
		{
			this.OnDataSourceViewChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Método acionado quando os parametros do método que recupera a
		/// instancia pelas chaves forem alterados.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SelectByKeysParametersChanged(object sender, EventArgs e)
		{
			this.OnDataSourceViewChanged(EventArgs.Empty);
		}

		/// <summary>
		/// Recupera a instancia do tipo om base no nome informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private Type GetType(string typeName)
		{
			if(this.TypeName.Length == 0)
				throw new InvalidOperationException(string.Format("Type not specified for VirtualObjectDataSource {0}", _owner.ID));
			Type type = BuildManager.GetType(typeName, false, true);
			if(type == null)
				throw new InvalidOperationException(string.Format("Type {0} not found for VirtualObjectDataSource {1}.", typeName, _owner.ID));
			return type;
		}

		/// <summary>
		/// Tenta recupera o tipo do objeto que gerencia os dados.
		/// </summary>
		/// <returns></returns>
		private Type TryGetDataObjectType()
		{
			string dataObjectTypeName = this.DataObjectTypeName;
			if(dataObjectTypeName.Length == 0)
				return null;
			Type type = BuildManager.GetType(dataObjectTypeName, false, true);
			if(type == null)
				throw new InvalidOperationException(string.Format("DataObject type not found in {0}", _owner.ID));
			return type;
		}

		/// <summary>
		/// Recupera o valor do parametro.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static object GetParameterValue(Parameter parameter, object value)
		{
			return ((Func<object, bool, object>)Delegate.CreateDelegate(typeof(Func<object, bool, object>), parameter, GetParemeterValueMethodInfo)).Invoke(value, true);
		}

		/// <summary>
		/// Mescla os dicionarios.
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		/// <param name="parameterNameFormatString"></param>
		private static void MergeDictionaries(ParameterCollection reference, IDictionary source, IDictionary destination, string parameterNameFormatString)
		{
			if(source != null)
			{
				foreach (DictionaryEntry entry in source)
				{
					object obj2 = entry.Value;
					Parameter parameter = null;
					string key = (string)entry.Key;
					if(parameterNameFormatString != null)
						key = string.Format(CultureInfo.InvariantCulture, parameterNameFormatString, new object[] {
							key
						});
					foreach (Parameter parameter2 in reference)
					{
						if(string.Equals(parameter2.Name, key, StringComparison.OrdinalIgnoreCase))
						{
							parameter = parameter2;
							break;
						}
					}
					if(parameter != null)
						obj2 = GetParameterValue(parameter, obj2);
					destination[key] = obj2;
				}
			}
		}

		/// <summary>
		/// Mescla os dados dos dicionarios.
		/// </summary>
		/// <param name="reference"></param>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		private static void MergeDictionaries(ParameterCollection reference, IDictionary source, IDictionary destination)
		{
			MergeDictionaries(reference, source, destination, null);
		}

		/// <summary>
		/// Recupera o tipo do método por pelo tipo da operação.
		/// </summary>
		/// <param name="operation"></param>
		/// <returns></returns>
		private static DataObjectMethodType GetMethodTypeFromOperation(DataSourceOperation operation)
		{
			switch(operation)
			{
			case DataSourceOperation.Delete:
				return DataObjectMethodType.Delete;
			case DataSourceOperation.Insert:
				return DataObjectMethodType.Insert;
			case DataSourceOperation.Select:
				return DataObjectMethodType.Select;
			case DataSourceOperation.Update:
				return DataObjectMethodType.Update;
			}
			throw new ArgumentOutOfRangeException("operation");
		}

		/// <summary>
		/// Recupera os dados do método.
		/// </summary>
		/// <param name="type">Tipo onde o método está inserido.</param>
		/// <param name="methodName">Nome do método.</param>
		/// <param name="allParameters">Parametros do método.</param>
		/// <param name="operation"></param>
		/// <returns></returns>
		private ObjectDataSourceMethod GetResolvedMethodData(Type type, string methodName, IDictionary allParameters, DataSourceOperation operation)
		{
			bool flag = operation == DataSourceOperation.SelectCount;
			var select = DataObjectMethodType.Select;
			if(!flag)
				select = GetMethodTypeFromOperation(operation);
			var methods = type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance).Where(f => StringComparer.InvariantCultureIgnoreCase.Equals(methodName, f.Name) && !f.IsGenericMethodDefinition).ToArray();
			MethodInfo methodInfo = null;
			ParameterInfo[] methodParameters = null;
			int num = -1;
			bool multipleOverloads = false;
			int count = allParameters.Count;
			bool configurePageSize = false;
			var originalParameterNames = new List<string>();
			foreach (string i in allParameters.Keys)
				originalParameterNames.Add(i);
			bool existsMaximumRowsParameter = false;
			object maximumRowsParameter = null;
			bool existsSortParameter = false;
			object sortParameter = null;
			if(operation == DataSourceOperation.Select && !string.IsNullOrEmpty(MaximumRowsParameterName) && allParameters.Contains(MaximumRowsParameterName))
			{
				existsMaximumRowsParameter = true;
				maximumRowsParameter = allParameters[MaximumRowsParameterName];
			}
			if(operation == DataSourceOperation.Select && !string.IsNullOrEmpty(SortParameterName) && allParameters.Contains(SortParameterName))
			{
				existsSortParameter = true;
				sortParameter = allParameters[SortParameterName];
			}
			int checkMethodState = 0;
			do
			{
				foreach (MethodInfo info2 in methods)
				{
					ParameterInfo[] infoArray3 = info2.GetParameters();
					if(infoArray3.Length == count)
					{
						bool flag3 = false;
						foreach (ParameterInfo info3 in infoArray3)
						{
							if(!allParameters.Contains(info3.Name))
							{
								flag3 = true;
								break;
							}
						}
						if(!flag3)
						{
							int num4 = 0;
							if(!flag)
							{
								var attribute = Attribute.GetCustomAttribute(info2, typeof(DataObjectMethodAttribute), true) as DataObjectMethodAttribute;
								if((attribute != null) && (attribute.MethodType == select))
								{
									if(attribute.IsDefault)
										num4 = 2;
									else
										num4 = 1;
								}
							}
							if(num4 == num)
								multipleOverloads = true;
							else if(num4 > num)
							{
								num = num4;
								multipleOverloads = false;
								methodInfo = info2;
								methodParameters = infoArray3;
							}
						}
					}
				}
				configurePageSize = true;
				if(methodInfo == null)
				{
					if(existsMaximumRowsParameter && checkMethodState == 0)
					{
						allParameters.Remove(MaximumRowsParameterName);
						checkMethodState = 1;
						count--;
					}
					else if(existsSortParameter && checkMethodState == 1)
					{
						allParameters.Remove(SortParameterName);
						checkMethodState = 2;
						count--;
					}
					else if(existsSortParameter && checkMethodState == 2)
					{
						if(existsMaximumRowsParameter)
						{
							configurePageSize = false;
							allParameters.Add(MaximumRowsParameterName, maximumRowsParameter);
							count++;
						}
						checkMethodState = 3;
					}
					else
						break;
				}
				else
					break;
			}
			while (true);
			if(multipleOverloads)
				throw new InvalidOperationException(string.Format("Multiple overloads in ", _owner.ID));
			if(methodInfo == null)
			{
				if(originalParameterNames.Count == 0)
					throw new InvalidOperationException(string.Format("Method {1} not found.", _owner.ID, methodName));
				string[] array = new string[originalParameterNames.Count];
				originalParameterNames.CopyTo(array, 0);
				string str = string.Join(", ", array);
				throw new InvalidOperationException(string.Format("Method {1} not found with params {2}", _owner.ID, methodName, str));
			}
			OrderedDictionary parameters = null;
			int length = methodParameters.Length;
			if(length > 0)
			{
				parameters = new OrderedDictionary(length, StringComparer.OrdinalIgnoreCase);
				bool convertNullToDBNull = this.ConvertNullToDBNull;
				for(int i = 0; i < methodParameters.Length; i++)
				{
					ParameterInfo info4 = methodParameters[i];
					string name = info4.Name;
					object obj2 = allParameters[name];
					if(convertNullToDBNull && (obj2 == null))
						obj2 = DBNull.Value;
					else
						obj2 = BuildObjectValue(obj2, info4.ParameterType, name, CultureInfo);
					parameters.Add(name, obj2);
				}
			}
			return new ObjectDataSourceMethod(operation, type, methodInfo, configurePageSize, parameters);
		}

		/// <summary>
		/// Recupera os dados do método.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="methodName"></param>
		/// <param name="dataObjectType"></param>
		/// <param name="oldDataObject"></param>
		/// <param name="newDataObject"></param>
		/// <param name="operation"></param>
		/// <returns></returns>
		private ObjectDataSourceMethod GetResolvedMethodData(Type type, string methodName, Type dataObjectType, object oldDataObject, object newDataObject, DataSourceOperation operation)
		{
			int num;
			MethodInfo[] methods = type.GetMethods(BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
			MethodInfo methodInfo = null;
			ParameterInfo[] infoArray2 = null;
			if(oldDataObject == null)
				num = 1;
			else if(newDataObject == null)
				num = 1;
			else
				num = 2;
			foreach (MethodInfo info2 in methods)
			{
				if(string.Equals(methodName, info2.Name, StringComparison.OrdinalIgnoreCase) && !info2.IsGenericMethodDefinition)
				{
					ParameterInfo[] parameters = info2.GetParameters();
					if(parameters.Length == num)
					{
						if((num == 1) && (parameters[0].ParameterType == dataObjectType))
						{
							methodInfo = info2;
							infoArray2 = parameters;
							break;
						}
						if(((num == 2) && (parameters[0].ParameterType == dataObjectType)) && (parameters[1].ParameterType == dataObjectType))
						{
							methodInfo = info2;
							infoArray2 = parameters;
							break;
						}
					}
				}
			}
			if(methodInfo == null)
				throw new InvalidOperationException(string.Format("VirtualObjectDataSourceView {0} method {1} not found for DataObject {2}", new object[] {
					this._owner.ID,
					methodName,
					dataObjectType.FullName
				}));
			OrderedDictionary dictionary = new OrderedDictionary(2, StringComparer.OrdinalIgnoreCase);
			if(oldDataObject == null)
				dictionary.Add(infoArray2[0].Name, newDataObject);
			else if(newDataObject == null)
				dictionary.Add(infoArray2[0].Name, oldDataObject);
			else
			{
				string name = infoArray2[0].Name;
				string a = infoArray2[1].Name;
				string b = string.Format(CultureInfo.InvariantCulture, this.OldValuesParameterFormatString, new object[] {
					name
				});
				if(string.Equals(a, b, StringComparison.OrdinalIgnoreCase))
				{
					dictionary.Add(name, newDataObject);
					dictionary.Add(a, oldDataObject);
				}
				else
				{
					b = string.Format(CultureInfo.InvariantCulture, this.OldValuesParameterFormatString, new object[] {
						a
					});
					if(!string.Equals(name, b, StringComparison.OrdinalIgnoreCase))
						throw new InvalidOperationException(string.Format("VirtualObjectDataSourceView {0} no old values params", new object[] {
							this._owner.ID
						}));
					dictionary.Add(name, oldDataObject);
					dictionary.Add(a, newDataObject);
				}
			}
			return new ObjectDataSourceMethod(operation, type, methodInfo, false, dictionary.AsReadOnly());
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="instance"></param>
		private void ReleaseInstance(object instance)
		{
			var e = new VirtualObjectDataSourceDisposingEventArgs(instance);
			this.OnObjectDisposing(e);
			if(!e.Cancel)
			{
				IDisposable disposable = instance as IDisposable;
				if(disposable != null)
				{
					disposable.Dispose();
				}
			}
		}

		/// <summary>
		/// Recupera o dicionário com os parametros de saída.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		private IDictionary GetOutputParameters(ParameterInfo[] parameters, object[] values)
		{
			IDictionary dictionary = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
			for(int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo info = parameters[i];
				if(info.ParameterType.IsByRef)
				{
					dictionary[info.Name] = values[i];
				}
			}
			return dictionary;
		}

		/// <summary>
		/// Chama um método.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="disposeInstance"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		private ObjectDataSourceResult InvokeMethod(ObjectDataSourceMethod method, bool disposeInstance, ref object instance)
		{
			if(method.MethodInfo.IsStatic)
			{
				if(instance != null)
					this.ReleaseInstance(instance);
				instance = null;
			}
			else if(instance == null)
			{
				var e = new VirtualObjectDataSourceEventArgs(null);
				this.OnObjectCreating(e);
				if(e.ObjectInstance == null)
				{
					System.Reflection.ConstructorInfo methodTypeConstructor = null;
					if(!method.Type.IsAbstract && !method.Type.IsInterface)
					{
						methodTypeConstructor = method.Type.GetConstructors().Where(f => f.GetParameters().Length == 0).FirstOrDefault();
					}
					if((method.Type.IsAbstract || method.Type.IsInterface || methodTypeConstructor == null) && !string.IsNullOrEmpty(FactoryTypeName))
					{
						object factoryInstance = null;
						var type = this.GetType(FactoryTypeName);
						if(type.IsAbstract && !string.IsNullOrEmpty(FactoryInstanceMethod))
						{
							var fiMethod = type.GetMethod(FactoryInstanceMethod, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
							if(fiMethod == null)
								throw new Exception(string.Format("Fail on create factory bacause method \"{0}\" not found in \"{1}\".", FactoryInstanceMethod, FactoryTypeName));
							if(fiMethod.IsStatic)
								try
								{
									factoryInstance = fiMethod.Invoke(null, null);
								}
								catch(Exception ex)
								{
									if(ex is TargetInvocationException && ex.InnerException != null)
										ex = ex.InnerException;
									throw ex;
								}
						}
						if(string.IsNullOrEmpty(FactoryMethod))
							throw new Exception("FactoryMethod not informed.");
						var fMethod = type.GetMethod(FactoryMethod);
						if(fMethod == null)
							throw new Exception(string.Format("Method \"{0}\" not found in \"{1}\".", FactoryMethod, type.FullName));
						var fMethodParameters = fMethod.GetParameters();
						if(fMethodParameters.Length == 0)
							try
							{
								e.ObjectInstance = fMethod.Invoke(factoryInstance, null);
							}
							catch(Exception ex)
							{
								if(ex is TargetInvocationException && ex.InnerException != null)
									ex = ex.InnerException;
								throw new Exception(string.Format("Fail on call method \"{0}\" in \"{1}\"", fMethod.Name, type.FullName), ex);
							}
						else if(fMethodParameters.Length == 2 && fMethodParameters[0].ParameterType == typeof(Type) && fMethodParameters[1].ParameterType == typeof(string))
						{
							try
							{
								e.ObjectInstance = fMethod.Invoke(factoryInstance, new object[] {
									method.Type,
									ContractName
								});
							}
							catch(Exception ex)
							{
								if(ex is TargetInvocationException && ex.InnerException != null)
									ex = ex.InnerException;
								throw new Exception(string.Format("Error call method \"{0}\" in \"{1}\"", fMethod.Name, type.FullName), ex);
							}
						}
						else
						{
							var parameterNames = new string[fMethodParameters.Length];
							for(var i = 0; i < parameterNames.Length; i++)
								parameterNames[i] = string.Format("[{0} {1}]", fMethodParameters[i].ParameterType.Name, fMethodParameters[i].Name);
							throw new InvalidOperationException(string.Format("Invalid factory method {0} with parameters {1}.\r\nExpected method with empty parameters or (Type, string)", FactoryMethod, string.Join(", ", parameterNames)));
						}
					}
					else
						try
						{
							e.ObjectInstance = Activator.CreateInstance(method.Type);
						}
						catch(MissingMethodException ex)
						{
							throw new MissingMethodException(string.Format("Not found public and without parameters constructor for type '{0}'", method.Type.FullName), ex);
						}
					this.OnObjectCreated(e);
				}
				instance = e.ObjectInstance;
			}
			object returnValue = null;
			int affectedRows = -1;
			bool flag = false;
			object[] parameters = null;
			if((method.Parameters != null) && (method.Parameters.Count > 0))
			{
				parameters = new object[method.Parameters.Count];
				var methodInfoParameters = method.MethodInfo.GetParameters();
				for(int i = 0; i < method.Parameters.Count; i++)
				{
					parameters[i] = (i < parameters.Length ? PrepareParameterValue(methodInfoParameters[i].ParameterType, method.Parameters[i]) : method.Parameters[i]);
				}
			}
			try
			{
				returnValue = method.MethodInfo.Invoke(instance, parameters);
			}
			catch(Exception exception)
			{
				if(exception is TargetInvocationException && exception.InnerException != null)
					exception = exception.InnerException;
				if(exception is TargetException)
				{
					if(instance == null && !method.MethodInfo.IsStatic)
					{
						exception = new TargetException(string.Format("Can not access method \"{0}\" to the null instance of \"{1}\"", method.MethodInfo.Name, method.Type.FullName), exception);
					}
					else if(instance != null && !method.Type.IsInstanceOfType(instance))
					{
						exception = new TargetException(string.Format("Type \"{0}\" is not compatible with method \"{1}\" in VirtualObjectDataSource", instance.GetType().FullName, method.Type.FullName + "." + method.MethodInfo.Name), exception);
					}
				}
				IDictionary outputParameters = this.GetOutputParameters(method.MethodInfo.GetParameters(), parameters);
				var args2 = new VirtualObjectDataSourceStatusEventArgs(returnValue, outputParameters, exception);
				flag = true;
				switch(method.Operation)
				{
				case DataSourceOperation.Delete:
					this.OnDeleted(args2);
					break;
				case DataSourceOperation.Insert:
					this.OnInserted(args2);
					break;
				case DataSourceOperation.Select:
					this.OnSelected(args2);
					break;
				case DataSourceOperation.Update:
					this.OnUpdated(args2);
					break;
				case DataSourceOperation.SelectCount:
					this.OnSelected(args2);
					break;
				}
				returnValue = args2.ReturnValue;
				affectedRows = args2.AffectedRows;
				if(!args2.ExceptionHandled)
				{
					throw exception;
				}
			}
			finally
			{
				try
				{
					if(!flag)
					{
						IDictionary dictionary2 = this.GetOutputParameters(method.MethodInfo.GetParameters(), parameters);
						var args3 = new VirtualObjectDataSourceStatusEventArgs(returnValue, dictionary2);
						switch(method.Operation)
						{
						case DataSourceOperation.Delete:
							this.OnDeleted(args3);
							break;
						case DataSourceOperation.Insert:
							this.OnInserted(args3);
							break;
						case DataSourceOperation.Select:
							this.OnSelected(args3);
							break;
						case DataSourceOperation.Update:
							this.OnUpdated(args3);
							break;
						case DataSourceOperation.SelectCount:
							this.OnSelected(args3);
							break;
						}
						returnValue = args3.ReturnValue;
						affectedRows = args3.AffectedRows;
					}
				}
				finally
				{
					if((instance != null) && disposeInstance)
					{
						this.ReleaseInstance(instance);
						instance = null;
					}
				}
			}
			return new ObjectDataSourceResult(returnValue, affectedRows);
		}

		/// <summary>
		/// Prepara o valor do parametro.
		/// </summary>
		/// <param name="parameterType"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private object PrepareParameterValue(Type parameterType, object value)
		{
			if(parameterType == typeof(uint))
			{
				if(value is int)
					return (uint)(int)value;
				else if(value is short)
					return (uint)(short)value;
				else if(value is long)
					return (uint)(long)value;
			}
			else if(parameterType == typeof(int))
			{
				if(value is uint)
					return (int)(uint)value;
				else if(value is ushort)
					return (int)(ushort)value;
				else if(value is ulong)
					return (int)(ulong)value;
			}
			return value;
		}

		/// <summary>
		/// Chama o método informado.
		/// </summary>
		/// <param name="method"></param>
		/// <returns></returns>
		private ObjectDataSourceResult InvokeMethod(ObjectDataSourceMethod method)
		{
			object instance = null;
			return this.InvokeMethod(method, true, ref instance);
		}

		/// <summary>
		/// Consulta a quantiade total de linhas.
		/// </summary>
		/// <param name="mergedParameters"></param>
		/// <param name="arguments"></param>
		/// <param name="disposeInstance"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		private int QueryTotalRowCount(IOrderedDictionary mergedParameters, DataSourceSelectArguments arguments, bool disposeInstance, ref object instance)
		{
			if(this.SelectCountMethod.Length > 0)
			{
				var e = new VirtualObjectDataSourceSelectingEventArgs(mergedParameters, arguments, true);
				this.OnSelecting(e);
				if(e.Cancel)
					return -1;
				var type = this.GetType(this.TypeName);
				ObjectDataSourceMethod method = this.GetResolvedMethodData(type, this.SelectCountMethod, mergedParameters, DataSourceOperation.SelectCount);
				ObjectDataSourceResult result = this.InvokeMethod(method, disposeInstance, ref instance);
				if((result.ReturnValue != null) && (result.ReturnValue is int))
					return (int)result.ReturnValue;
			}
			return -1;
		}

		/// <summary>
		/// Cria um filtro para o DataView.
		/// </summary>
		/// <param name="dataTable"></param>
		/// <param name="sortExpression"></param>
		/// <param name="filterExpression"></param>
		/// <returns></returns>
		private IEnumerable CreateFilteredDataView(System.Data.DataTable dataTable, string sortExpression, string filterExpression)
		{
			IOrderedDictionary values = FilterParameters.GetValues(this._context, this._owner);
			if(filterExpression.Length > 0)
			{
				var e = new VirtualObjectDataSourceFilteringEventArgs(values);
				this.OnFiltering(e);
				if(e.Cancel)
					return null;
			}
			return FilteredDataSetHelper.CreateFilteredDataView(dataTable, sortExpression, filterExpression, values);
		}

		/// <summary>
		/// Cria o resultado da lista de dados.
		/// </summary>
		/// <param name="list"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private IEnumerable<object> CreateListData(IList list, DataSourceSelectArguments arguments)
		{
			for(int i = arguments.StartRowIndex; i < list.Count && (arguments.MaximumRows == 0 || (i < arguments.StartRowIndex + arguments.MaximumRows)); i++)
			{
				object item = null;
				try
				{
					item = list[i];
				}
				catch(System.ArgumentOutOfRangeException)
				{
					yield break;
				}
				yield return item;
			}
		}

		/// <summary>
		/// Recupera a Enumeração do dados da consulta.
		/// </summary>
		/// <param name="dataObject"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		private IEnumerable CreateEnumerableData(object dataObject, DataSourceSelectArguments arguments)
		{
			if(this.FilterExpression.Length > 0)
				throw new NotSupportedException(string.Format("Filter not supported in {0}.", new object[] {
					this._owner.ID
				}));
			if(!string.IsNullOrEmpty(arguments.SortExpression))
				throw new NotSupportedException(string.Format("Sort not supported on IEnumerable, in {0}.", new object[] {
					this._owner.ID
				}));
			var enumerable = dataObject as IEnumerable;
			if(enumerable != null)
			{
				if((!this.EnablePaging && arguments.RetrieveTotalRowCount) && (SelectCountMethod.Length == 0))
				{
					ICollection is2 = enumerable as ICollection;
					if(is2 != null)
						arguments.TotalRowCount = is2.Count;
				}
				if(this.EnablePaging && SelectCountMethod.Length == 0)
				{
					var list = enumerable as IList;
					if(list != null)
					{
						arguments.RetrieveTotalRowCount = true;
						arguments.TotalRowCount = list.Count;
						return CreateListData(list, arguments);
					}
				}
				return enumerable;
			}
			if(arguments.RetrieveTotalRowCount && (this.SelectCountMethod.Length == 0))
				arguments.TotalRowCount = 1;
			return new object[] {
				dataObject
			};
		}

		/// <summary>
		/// Executa o método de exclusão com os dados informado.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="oldValues"></param>
		/// <returns></returns>
		protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
		{
			ObjectDataSourceMethod method;
			if(!CanDelete)
				throw new NotSupportedException(string.Format("VirtualObjectDataSource {0} not support delete.", this._owner.ID));
			Type type = this.GetType(this.TypeName);
			Type dataObjectType = this.TryGetDataObjectType();
			if(dataObjectType != null)
			{
				IDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
				MergeDictionaries(this.DeleteParameters, keys, destination);
				if(this.ConflictDetection == ConflictOptions.CompareAllValues)
				{
					if(oldValues == null)
						throw new InvalidOperationException("Pessimistic delete oldValues");
					MergeDictionaries(this.DeleteParameters, oldValues, destination);
				}
				object oldDataObject = null;
				if(DeleteStrategy == VirtualObjectDataSourceDeleteStrategy.GetAndDelete)
				{
					IEnumerator enumerator = null;
					if(!string.IsNullOrEmpty(SelectByKeysMethod))
						enumerator = this.ExecuteSelectByKeys(keys).GetEnumerator();
					else
						enumerator = this.ExecuteSelect(new DataSourceSelectArguments(null, 0, 0)).GetEnumerator();
					if(enumerator.MoveNext())
					{
						oldDataObject = enumerator.Current;
						this.FillDataObject(oldDataObject, destination);
					}
					else
						throw new InvalidOperationException("Not found instance for update");
					if(enumerator is IDisposable)
						((IDisposable)enumerator).Dispose();
				}
				else
					oldDataObject = this.BuildDataObject(dataObjectType, destination);
				method = this.GetResolvedMethodData(type, this.DeleteMethod, dataObjectType, oldDataObject, null, DataSourceOperation.Delete);
				var e = new VirtualObjectDataSourceMethodEventArgs(method.Parameters);
				this.OnDeleting(e);
				if(e.Cancel)
					return 0;
			}
			else
			{
				var dictionary2 = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
				string oldValuesParameterFormatString = this.OldValuesParameterFormatString;
				MergeDictionaries(this.DeleteParameters, this.DeleteParameters.GetValues(this._context, this._owner), dictionary2);
				MergeDictionaries(this.DeleteParameters, keys, dictionary2, oldValuesParameterFormatString);
				if(this.ConflictDetection == ConflictOptions.CompareAllValues)
				{
					if(oldValues == null)
						throw new InvalidOperationException("Pessimistic delete oldValues");
					MergeDictionaries(this.DeleteParameters, oldValues, dictionary2, oldValuesParameterFormatString);
				}
				var args2 = new VirtualObjectDataSourceMethodEventArgs(dictionary2);
				this.OnDeleting(args2);
				if(args2.Cancel)
				{
					return 0;
				}
				method = this.GetResolvedMethodData(type, this.DeleteMethod, dictionary2, DataSourceOperation.Delete);
			}
			var result = this.InvokeMethod(method);
			this.OnDataSourceViewChanged(EventArgs.Empty);
			return result.AffectedRows;
		}

		/// <summary>
		/// Executa o método de inserção.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		protected override int ExecuteInsert(IDictionary values)
		{
			ObjectDataSourceMethod method;
			if(!this.CanInsert)
			{
				throw new NotSupportedException(string.Format("VirtualObjectDataSourceView {0} Insert not supported.", new object[] {
					_owner.ID
				}));
			}
			Type type = this.GetType(this.TypeName);
			Type dataObjectType = this.TryGetDataObjectType();
			if(dataObjectType != null)
			{
				if((values == null) || (values.Count == 0))
				{
					throw new InvalidOperationException(string.Format("Values not defined.", new object[] {
						this._owner.ID
					}));
				}
				IDictionary destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
				MergeDictionaries(this.InsertParameters, values, destination);
				object newDataObject = this.BuildDataObject(dataObjectType, destination);
				method = this.GetResolvedMethodData(type, this.InsertMethod, dataObjectType, null, newDataObject, DataSourceOperation.Insert);
				var e = new VirtualObjectDataSourceMethodEventArgs(method.Parameters);
				this.OnInserting(e);
				if(e.Cancel)
				{
					return 0;
				}
			}
			else
			{
				IOrderedDictionary dictionary2 = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
				MergeDictionaries(this.InsertParameters, this.InsertParameters.GetValues(this._context, this._owner), dictionary2);
				MergeDictionaries(this.InsertParameters, values, dictionary2);
				var args2 = new VirtualObjectDataSourceMethodEventArgs(dictionary2);
				this.OnInserting(args2);
				if(args2.Cancel)
				{
					return 0;
				}
				method = this.GetResolvedMethodData(type, this.InsertMethod, dictionary2, DataSourceOperation.Insert);
			}
			var result = this.InvokeMethod(method);
			this.OnDataSourceViewChanged(EventArgs.Empty);
			return result.AffectedRows;
		}

		/// <summary>
		/// Executa o método de atualização.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <param name="oldValues"></param>
		/// <returns></returns>
		protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
		{
			ObjectDataSourceMethod method;
			if(!this.CanUpdate)
				throw new NotSupportedException(string.Format("VirtualObjectDataSourceView {0} update not supported.", new object[] {
					this._owner.ID
				}));
			Type type = this.GetType(this.TypeName);
			Type dataObjectType = this.TryGetDataObjectType();
			if(dataObjectType != null)
			{
				if(this.ConflictDetection == ConflictOptions.CompareAllValues)
				{
					if(oldValues == null)
						throw new InvalidOperationException("Pessimistic update oldValues");
					var destination = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
					IDictionary dictionary2 = null;
					MergeDictionaries(this.UpdateParameters, oldValues, destination);
					MergeDictionaries(this.UpdateParameters, keys, destination);
					MergeDictionaries(this.UpdateParameters, values, destination);
					if(oldValues == null)
						throw new InvalidOperationException("Pessimistic update oldValues");
					dictionary2 = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
					MergeDictionaries(this.UpdateParameters, oldValues, dictionary2);
					MergeDictionaries(this.UpdateParameters, keys, dictionary2);
					object newDataObject = this.BuildDataObject(dataObjectType, destination);
					object oldDataObject = this.BuildDataObject(dataObjectType, dictionary2);
					method = this.GetResolvedMethodData(type, this.UpdateMethod, dataObjectType, oldDataObject, newDataObject, DataSourceOperation.Update);
				}
				else
				{
					IDictionary inputParameters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
					MergeDictionaries(this.UpdateParameters, oldValues, inputParameters);
					MergeDictionaries(this.UpdateParameters, keys, inputParameters);
					MergeDictionaries(this.UpdateParameters, values, inputParameters);
					object dataObject = null;
					if(UpdateStrategy == VirtualObjectDataSourceUpdateStrategy.GetAndUpdate)
					{
						IEnumerator enumerator = null;
						if(!string.IsNullOrEmpty(SelectByKeysMethod))
							enumerator = this.ExecuteSelectByKeys(keys).GetEnumerator();
						else
							enumerator = this.ExecuteSelect(new DataSourceSelectArguments(null, 0, 0)).GetEnumerator();
						if(enumerator.MoveNext())
						{
							dataObject = enumerator.Current;
							this.FillDataObject(dataObject, inputParameters);
						}
						else
							throw new InvalidOperationException("Not found instance for update");
						if(enumerator is IDisposable)
							((IDisposable)enumerator).Dispose();
					}
					else
					{
						dataObject = this.BuildDataObject(dataObjectType, inputParameters);
					}
					method = this.GetResolvedMethodData(type, this.UpdateMethod, dataObjectType, null, dataObject, DataSourceOperation.Update);
				}
				var e = new VirtualObjectDataSourceMethodEventArgs(method.Parameters);
				this.OnUpdating(e);
				if(e.Cancel)
					return 0;
			}
			else
			{
				IOrderedDictionary dictionary4 = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
				string oldValuesParameterFormatString = this.OldValuesParameterFormatString;
				IDictionary source = this.UpdateParameters.GetValues(this._context, this._owner);
				if(keys != null)
				{
					foreach (DictionaryEntry entry in keys)
					{
						if(source.Contains(entry.Key))
							source.Remove(entry.Key);
					}
				}
				MergeDictionaries(this.UpdateParameters, source, dictionary4);
				MergeDictionaries(this.UpdateParameters, values, dictionary4);
				if(this.ConflictDetection == ConflictOptions.CompareAllValues)
				{
					MergeDictionaries(this.UpdateParameters, oldValues, dictionary4, oldValuesParameterFormatString);
				}
				MergeDictionaries(this.UpdateParameters, keys, dictionary4, oldValuesParameterFormatString);
				var args2 = new VirtualObjectDataSourceMethodEventArgs(dictionary4);
				this.OnUpdating(args2);
				if(args2.Cancel)
				{
					return 0;
				}
				method = this.GetResolvedMethodData(type, this.UpdateMethod, dictionary4, DataSourceOperation.Update);
			}
			var result = this.InvokeMethod(method);
			if(_owner.Cache.Enabled)
				_owner.InvalidateCacheEntry();
			this.OnDataSourceViewChanged(EventArgs.Empty);
			return result.AffectedRows;
		}

		/// <summary>
		/// Executa o método de seleção.
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
		{
			if(this.SelectMethod.Length == 0)
				throw new InvalidOperationException(string.Format("VirtualObjectDataSourceView {0} select not supported.", new object[] {
					this._owner.ID
				}));
			if(this.CanSort)
				arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);
			if(this.CanPage)
				arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);
			if(this.CanRetrieveTotalRowCount)
				arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
			arguments.RaiseUnsupportedCapabilitiesError(this);
			IOrderedDictionary parameters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
			foreach (DictionaryEntry entry in this.SelectParameters.GetValues(this._context, this._owner))
				parameters[entry.Key] = entry.Value;
			var e = new VirtualObjectDataSourceSelectingEventArgs(parameters, arguments, false);
			this.OnSelecting(e);
			if(e.Cancel)
				return null;
			var mergedParameters = new OrderedDictionary(parameters.Count);
			foreach (DictionaryEntry entry2 in parameters)
				mergedParameters.Add(entry2.Key, entry2.Value);
			var sortParameterName = this.SortParameterName;
			var sortExpression = arguments.SortExpression;
			if(sortParameterName.Length > 0)
			{
				parameters[sortParameterName] = arguments.SortExpression;
				arguments.SortExpression = string.Empty;
			}
			if(this.EnablePaging)
			{
				string maximumRowsParameterName = this.MaximumRowsParameterName;
				string startRowIndexParameterName = this.StartRowIndexParameterName;
				var source = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
				if(!string.IsNullOrEmpty(maximumRowsParameterName))
					source[maximumRowsParameterName] = arguments.MaximumRows;
				if(!string.IsNullOrEmpty(startRowIndexParameterName))
					source[startRowIndexParameterName] = arguments.StartRowIndex;
				MergeDictionaries(this.SelectParameters, source, parameters);
			}
			var type = this.GetType(this.TypeName);
			object instance = null;
			ObjectDataSourceResult result = null;
			try
			{
				var method = this.GetResolvedMethodData(type, this.SelectMethod, parameters, DataSourceOperation.Select);
				result = this.InvokeMethod(method, false, ref instance);
				if(result.ReturnValue == null)
					return null;
				if(result.ReturnValue is Colosoft.Collections.IVirtualList && method.ConfigureMaximumRows)
				{
					var virtualList = (Colosoft.Collections.IVirtualList)result.ReturnValue;
					virtualList.Configure(arguments.MaximumRows);
				}
				if(result.ReturnValue is Colosoft.Collections.ISortableCollection && !string.IsNullOrEmpty(SortParameterName) && !string.IsNullOrEmpty(sortExpression))
				{
					var sortableList = (Colosoft.Collections.ISortableCollection)result.ReturnValue;
					sortableList.ApplySort(sortExpression);
				}
				if(arguments.RetrieveTotalRowCount && (this.SelectCountMethod.Length > 0))
				{
					int totalRowCount = -1;
					if(totalRowCount < 0)
					{
						totalRowCount = this.QueryTotalRowCount(mergedParameters, arguments, true, ref instance);
						arguments.TotalRowCount = totalRowCount;
					}
				}
				else if(arguments.RetrieveTotalRowCount && result.ReturnValue is ICollection)
				{
					arguments.TotalRowCount = ((ICollection)result.ReturnValue).Count;
				}
			}
			finally
			{
				if(instance != null)
				{
					this.ReleaseInstance(instance);
				}
			}
			var returnValue = result.ReturnValue as System.Data.DataView;
			if(returnValue != null)
			{
				if(arguments.RetrieveTotalRowCount && (this.SelectCountMethod.Length == 0))
				{
					arguments.TotalRowCount = returnValue.Count;
				}
				if(this.FilterExpression.Length > 0)
					throw new NotSupportedException(string.Format("Filter not supported in {0}.", new object[] {
						this._owner.ID
					}));
				if(!string.IsNullOrEmpty(arguments.SortExpression))
					returnValue.Sort = arguments.SortExpression;
				return returnValue;
			}
			var dataTable = FilteredDataSetHelper.GetDataTable(_owner, result.ReturnValue);
			if(dataTable != null)
			{
				if(arguments.RetrieveTotalRowCount && (this.SelectCountMethod.Length == 0))
					arguments.TotalRowCount = dataTable.Rows.Count;
				return this.CreateFilteredDataView(dataTable, arguments.SortExpression, this.FilterExpression);
			}
			return this.CreateEnumerableData(result.ReturnValue, arguments);
		}

		/// <summary>
		/// Executa o método para recupera o item com base nas chaves informadas.
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		protected virtual IEnumerable ExecuteSelectByKeys(IDictionary keys)
		{
			if(this.SelectByKeysMethod.Length == 0)
				throw new InvalidOperationException(string.Format("VirtualObjectDataSourceView {0} get by keys not supported.", new object[] {
					_owner.ID
				}));
			IOrderedDictionary parameters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
			foreach (DictionaryEntry entry in this.SelectByKeysParameters.GetValues(_context, _owner))
				parameters[entry.Key] = entry.Value;
			var e = new VirtualObjectDataSourceSeletingByKeysEventArgs(parameters, keys);
			this.OnSelectingByKeys(e);
			if(e.Cancel)
				return null;
			var mergedParameters = new OrderedDictionary(parameters.Count);
			foreach (DictionaryEntry entry2 in parameters)
				mergedParameters.Add(entry2.Key, entry2.Value);
			MergeDictionaries(this.SelectByKeysParameters, keys, parameters);
			var type = this.GetType(this.TypeName);
			object instance = null;
			ObjectDataSourceResult result = null;
			try
			{
				var method = this.GetResolvedMethodData(type, this.SelectByKeysMethod, parameters, DataSourceOperation.Select);
				result = this.InvokeMethod(method, false, ref instance);
				if(result.ReturnValue == null)
					return null;
			}
			finally
			{
				if(instance != null)
				{
					this.ReleaseInstance(instance);
				}
			}
			return this.CreateEnumerableData(result.ReturnValue, new DataSourceSelectArguments(null, 0, 0));
		}

		/// <summary>
		/// Executa o método de criação do objeto de dados.
		/// </summary>
		/// <returns></returns>
		protected virtual object ExecuteCreateDataObject()
		{
			IOrderedDictionary parameters = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
			foreach (DictionaryEntry entry in this.CreateDataObjectParameters.GetValues(this._context, this._owner))
				parameters[entry.Key] = entry.Value;
			var type = this.GetType(this.TypeName);
			object instance = null;
			ObjectDataSourceResult result = null;
			try
			{
				var method = this.GetResolvedMethodData(type, this.CreateDataObjectMethod, parameters, DataSourceOperation.Select);
				result = this.InvokeMethod(method, false, ref instance);
				if(result.ReturnValue == null)
					return null;
				return result.ReturnValue;
			}
			finally
			{
				if(instance != null)
				{
					this.ReleaseInstance(instance);
				}
			}
		}

		/// <summary>
		/// Método acionado quando um objeto está sendo inserido.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnInserting(VirtualObjectDataSourceMethodEventArgs e)
		{
			var handler = base.Events[EventInserting] as VirtualObjectDataSourceMethodEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando um objeto é apagado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDeleted(VirtualObjectDataSourceStatusEventArgs e)
		{
			var handler = base.Events[EventDeleted] as VirtualObjectDataSourceStatusEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando um objeto está sendo apagado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnDeleting(VirtualObjectDataSourceMethodEventArgs e)
		{
			var handler = base.Events[EventDeleting] as VirtualObjectDataSourceMethodEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando os dados estão sendo filtrados.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnFiltering(VirtualObjectDataSourceFilteringEventArgs e)
		{
			var handler = base.Events[EventFiltering] as VirtualObjectDataSourceFilteringEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando um objeto é inserido.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnInserted(VirtualObjectDataSourceStatusEventArgs e)
		{
			var handler = base.Events[EventInserted] as VirtualObjectDataSourceStatusEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando a instancia de um objeto da fonte de dados é criada.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnObjectCreated(VirtualObjectDataSourceEventArgs e)
		{
			var handler = base.Events[EventObjectCreated] as VirtualObjectDataSourceObjectEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando o objeto está senco criado.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnObjectCreating(VirtualObjectDataSourceEventArgs e)
		{
			var handler = base.Events[EventObjectCreating] as VirtualObjectDataSourceObjectEventHandler;
			if(handler != null)
				handler(this, e);
		}

		protected virtual void OnObjectDisposing(VirtualObjectDataSourceDisposingEventArgs e)
		{
			var handler = base.Events[EventObjectDisposing] as VirtualObjectDataSourceDisposingEventHandler;
			if(handler != null)
				handler(this, e);
		}

		protected virtual void OnSelected(VirtualObjectDataSourceStatusEventArgs e)
		{
			var handler = base.Events[EventSelected] as VirtualObjectDataSourceStatusEventHandler;
			if(handler != null)
				handler(this, e);
		}

		protected virtual void OnSelecting(VirtualObjectDataSourceSelectingEventArgs e)
		{
			var handler = base.Events[EventSelecting] as VirtualObjectDataSourceSelectingEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado qunado os itens estiverem sendo selecionados pelas chaves.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnSelectingByKeys(VirtualObjectDataSourceSeletingByKeysEventArgs e)
		{
			var handler = base.Events[EventSelectingByKeys] as VirtualObjectDataSourceSeletingByKeysEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Método acionado quando os itens forem selecionados pelas chaves.
		/// </summary>
		/// <param name="e"></param>
		protected virtual void OnSelectedByKeys(VirtualObjectDataSourceStatusEventArgs e)
		{
			var handler = base.Events[EventSelectedByKeys] as VirtualObjectDataSourceStatusEventHandler;
			if(handler != null)
				handler(this, e);
		}

		protected virtual void OnUpdated(VirtualObjectDataSourceStatusEventArgs e)
		{
			var handler = base.Events[EventUpdated] as VirtualObjectDataSourceStatusEventHandler;
			if(handler != null)
				handler(this, e);
		}

		protected virtual void OnUpdating(VirtualObjectDataSourceMethodEventArgs e)
		{
			var handler = base.Events[EventUpdating] as VirtualObjectDataSourceMethodEventHandler;
			if(handler != null)
				handler(this, e);
		}

		/// <summary>
		/// Configura os parametros padrão para a fabrica usada na instância.
		/// </summary>
		/// <param name="factoryTypeName"></param>
		/// <param name="factoryMethod"></param>
		public static void ConfigureDefaultFactoryType(string factoryTypeName, string factoryMethod)
		{
			DefaultValues.FactoryTypeName = factoryTypeName;
			DefaultValues.FactoryMethod = factoryMethod;
		}

		/// <summary>
		/// Configura os parametros padrão para a fabrica usada na instância.
		/// </summary>
		/// <param name="factoryTypeName"></param>
		/// <param name="factoryMethod"></param>
		/// <param name="factoryInstanceMethod"></param>
		public static void ConfigureDefaultFactoryType(string factoryTypeName, string factoryMethod, string factoryInstanceMethod)
		{
			DefaultValues.FactoryTypeName = factoryTypeName;
			DefaultValues.FactoryMethod = factoryMethod;
			DefaultValues.FactoryInstanceMethod = factoryInstanceMethod;
		}

		/// <summary>
		/// Executa o comando para inserir nos novos dados.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		public int Insert(IDictionary values)
		{
			return ExecuteInsert(values);
		}

		/// <summary>
		/// Executa o comando para apagar os dados informados.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="oldValues"></param>
		/// <returns></returns>
		public int Delete(IDictionary keys, IDictionary oldValues)
		{
			return ExecuteDelete(keys, oldValues);
		}

		/// <summary>
		/// Executa o comando para atualizar os dados do item que está sendo editado.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="values"></param>
		/// <param name="oldValues"></param>
		/// <returns></returns>
		public int Update(IDictionary keys, IDictionary values, IDictionary oldValues)
		{
			return ExecuteUpdate(keys, values, oldValues);
		}

		/// <summary>
		/// Executa comando para selecionar e recupera os itens.
		/// </summary>
		/// <param name="arguments">Argumentos da consulta.</param>
		/// <returns></returns>
		public IEnumerable Select(DataSourceSelectArguments arguments)
		{
			return ExecuteSelect(arguments);
		}

		/// <summary>
		/// Obtém um valor indicando se o objeto de instância está salvando alterações em seu estado de exibição.
		/// </summary>
		public bool IsTrackingViewState
		{
			get
			{
				return _tracking;
			}
		}

		public virtual void LoadViewState(object savedState)
		{
			if(savedState != null)
			{
				Pair pair = (Pair)savedState;
				if(pair.First != null)
				{
					((IStateManager)this.SelectParameters).LoadViewState(pair.First);
				}
				if(pair.Second != null)
				{
					((IStateManager)this.FilterParameters).LoadViewState(pair.Second);
				}
			}
		}

		public virtual object SaveViewState()
		{
			var pair = new Pair();
			pair.First = (this._selectParameters != null) ? ((IStateManager)this._selectParameters).SaveViewState() : null;
			pair.Second = (this._filterParameters != null) ? ((IStateManager)this._filterParameters).SaveViewState() : null;
			if((pair.First == null) && (pair.Second == null))
			{
				return null;
			}
			return pair;
		}

		public virtual void TrackViewState()
		{
			_tracking = true;
			if(_selectParameters != null)
			{
				((IStateManager)_selectParameters).TrackViewState();
			}
			if(_filterParameters != null)
			{
				((IStateManager)_filterParameters).TrackViewState();
			}
		}
	}
}
