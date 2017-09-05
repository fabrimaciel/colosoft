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
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Drawing.Design;
using System.Drawing;
using System.Web;
using System.Security.Permissions;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Reprensenta um fonte de dados de objetos virtuais, ou seja,
	/// pode ser que existe uma lista de objeto com uma quantidade X
	/// mas na realidade não quer dizer que essa lista possua esse X 
	/// já carregados em memória, esse itens serão recuperados a medida
	/// que forem chamados dessa lista.
	/// </summary>
	[ToolboxBitmap(typeof(VirtualObjectDataSource))]
	[ControlBuilder(typeof(DataSourceControlBuilder))]
	[DefaultProperty("TypeName")]
	[PersistChildren(false)]
	[DefaultEvent("Selecting")]
	[ParseChildren(true)]
	[Designer("Colosoft.WebControls.Design.VirtualObjectDataSourceDesigner, Colosoft.WebControls")]
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public class VirtualObjectDataSource : DataSourceControl
	{
		private VirtualObjectDataSourceView _view;

		private ICollection _viewNames;

		private SqlDataSourceCache _cache;

		[Category("Data")]
		public event VirtualObjectDataSourceStatusEventHandler Deleted {
			add
			{
				this.GetView().Deleted += value;
			}
			remove {
				this.GetView().Deleted -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceMethodEventHandler Deleting {
			add
			{
				this.GetView().Deleting += value;
			}
			remove {
				this.GetView().Deleting -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceFilteringEventHandler Filtering {
			add
			{
				this.GetView().Filtering += value;
			}
			remove {
				this.GetView().Filtering -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceStatusEventHandler Inserted {
			add
			{
				this.GetView().Inserted += value;
			}
			remove {
				this.GetView().Inserted -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceMethodEventHandler Inserting {
			add
			{
				this.GetView().Inserting += value;
			}
			remove {
				this.GetView().Inserting -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceObjectEventHandler ObjectCreated {
			add
			{
				this.GetView().ObjectCreated += value;
			}
			remove {
				this.GetView().ObjectCreated -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceObjectEventHandler ObjectCreating {
			add
			{
				this.GetView().ObjectCreating += value;
			}
			remove {
				this.GetView().ObjectCreating -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceDisposingEventHandler ObjectDisposing {
			add
			{
				this.GetView().ObjectDisposing += value;
			}
			remove {
				this.GetView().ObjectDisposing -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceStatusEventHandler Selected {
			add
			{
				this.GetView().Selected += value;
			}
			remove {
				this.GetView().Selected -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceSelectingEventHandler Selecting {
			add
			{
				this.GetView().Selecting += value;
			}
			remove {
				this.GetView().Selecting -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceStatusEventHandler Updated {
			add
			{
				this.GetView().Updated += value;
			}
			remove {
				this.GetView().Updated -= value;
			}
		}

		[Category("Data")]
		public event VirtualObjectDataSourceMethodEventHandler Updating {
			add
			{
				this.GetView().Updating += value;
			}
			remove {
				this.GetView().Updating -= value;
			}
		}

		/// <summary>
		/// Identifica se o tema está habilitado.
		/// </summary>
		public override bool EnableTheming
		{
			get
			{
				return true;
			}
			set
			{
			}
		}

		/// <summary>
		/// Identificador do Skin associado.
		/// </summary>
		[Browsable(false), Description("SkinId"), DefaultValue(""), Filterable(false), Category("Behavior")]
		public override string SkinID
		{
			get
			{
				var field = typeof(Control).GetField("_occasionalFields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(field != null)
				{
					var occasionalFields = field.GetValue(this);
					if(occasionalFields != null)
					{
						field = occasionalFields.GetType().GetField("SkinId");
						return field.GetValue(occasionalFields) as string;
					}
				}
				return string.Empty;
			}
			set
			{
				var method = typeof(Control).GetMethod("EnsureOccasionalFields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(method != null)
					method.Invoke(this, null);
				var field = typeof(Control).GetField("_occasionalFields", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				if(field != null)
				{
					var occasionalFields = field.GetValue(this);
					if(occasionalFields != null)
					{
						field = occasionalFields.GetType().GetField("SkinId");
						field.SetValue(occasionalFields, value);
					}
				}
			}
		}

		/// <summary>
		/// Instancia do cache associado.
		/// </summary>
		internal SqlDataSourceCache Cache
		{
			get
			{
				if(_cache == null)
					_cache = new SqlDataSourceCache();
				return _cache;
			}
		}

		/// <summary>
		/// Duração do cache.
		/// </summary>
		[DefaultValue(0), TypeConverter(typeof(DataSourceCacheDurationConverter)), Category("Cache"), Description("DataSourceCache_Duration")]
		public virtual int CacheDuration
		{
			get
			{
				return this.Cache.Duration;
			}
			set
			{
				this.Cache.Duration = value;
			}
		}

		/// <summary>
		/// Política de expiração do cache.
		/// </summary>
		[DefaultValue(0), Category("Cache"), Description("DataSourceCache_ExpirationPolicy")]
		public virtual DataSourceCacheExpiry CacheExpirationPolicy
		{
			get
			{
				return this.Cache.ExpirationPolicy;
			}
			set
			{
				this.Cache.ExpirationPolicy = value;
			}
		}

		/// <summary>
		/// Chave de dependencia do cache.
		/// </summary>
		[Description("DataSourceCache_KeyDependency"), Category("Cache"), DefaultValue("")]
		public virtual string CacheKeyDependency
		{
			get
			{
				return this.Cache.KeyDependency;
			}
			set
			{
				this.Cache.KeyDependency = value;
			}
		}

		/// <summary>
		/// Chavede dependencia do cache.
		/// </summary>
		[Description("SqlDataSourceCache_SqlCacheDependency"), Category("Cache"), DefaultValue("")]
		public virtual string SqlCacheDependency
		{
			get
			{
				return this.Cache.SqlCacheDependency;
			}
			set
			{
				this.Cache.SqlCacheDependency = value;
			}
		}

		/// <summary>
		/// Identifica se é para habilitar o cache.
		/// </summary>
		[Description("DataSourceCache_Enabled"), DefaultValue(false), Category("Cache")]
		public virtual bool EnableCaching
		{
			get
			{
				return this.Cache.Enabled;
			}
			set
			{
				this.Cache.Enabled = value;
			}
		}

		/// <summary>
		/// Opções de detectar conflitos.
		/// </summary>
		[Category("Data"), Description("ObjectDataSource_ConflictDetection"), DefaultValue(0)]
		public ConflictOptions ConflictDetection
		{
			get
			{
				return this.GetView().ConflictDetection;
			}
			set
			{
				this.GetView().ConflictDetection = value;
			}
		}

		/// <summary>
		/// Identifica se é para converter null para DBNull.
		/// </summary>
		[Description("ObjectDataSource_ConvertNullToDBNull"), DefaultValue(false), Category("Data")]
		public bool ConvertNullToDBNull
		{
			get
			{
				return this.GetView().ConvertNullToDBNull;
			}
			set
			{
				this.GetView().ConvertNullToDBNull = value;
			}
		}

		/// <summary>
		/// Nome do tipo do objeto de dados.
		/// </summary>
		[DefaultValue(""), Description("ObjectDataSource_DataObjectTypeName"), Category("Data")]
		public string DataObjectTypeName
		{
			get
			{
				return this.GetView().DataObjectTypeName;
			}
			set
			{
				this.GetView().DataObjectTypeName = value;
			}
		}

		/// <summary>
		/// Método de exclusão.
		/// </summary>
		[Description("ObjectDataSource_DeleteMethod"), DefaultValue(""), Category("Data")]
		public string DeleteMethod
		{
			get
			{
				return this.GetView().DeleteMethod;
			}
			set
			{
				this.GetView().DeleteMethod = value;
			}
		}

		/// <summary>
		/// Parametros do método de exclusão.
		/// </summary>
		[PersistenceMode(PersistenceMode.InnerProperty), MergableProperty(false), DefaultValue((string)null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Description("ObjectDataSource_DeleteParameters"), Category("Data")]
		public ParameterCollection DeleteParameters
		{
			get
			{
				return this.GetView().DeleteParameters;
			}
		}

		/// <summary>
		/// Identifica se permite paginação.
		/// </summary>
		[DefaultValue(false), Description("ObjectDataSource_EnablePaging"), Category("Paging")]
		public bool EnablePaging
		{
			get
			{
				return this.GetView().EnablePaging;
			}
			set
			{
				this.GetView().EnablePaging = value;
			}
		}

		/// <summary>
		/// Cultura que será utilizada.
		/// </summary>
		public string Culture
		{
			get
			{
				return this.GetView().Culture;
			}
			set
			{
				this.GetView().Culture = value;
			}
		}

		/// <summary>
		/// Expressão de filtro.
		/// </summary>
		[Description("ObjectDataSource_FilterExpression"), Category("Data"), DefaultValue("")]
		public string FilterExpression
		{
			get
			{
				return this.GetView().FilterExpression;
			}
			set
			{
				this.GetView().FilterExpression = value;
			}
		}

		/// <summary>
		/// Parametros de filtro.
		/// </summary>
		[DefaultValue((string)null), MergableProperty(false), Description("ObjectDataSource_FilterParameters"), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), Category("Data")]
		public ParameterCollection FilterParameters
		{
			get
			{
				return this.GetView().FilterParameters;
			}
		}

		/// <summary>
		/// Nome do método de inserção.
		/// </summary>
		[DefaultValue(""), Description("ObjectDataSource_InsertMethod"), Category("Data")]
		public string InsertMethod
		{
			get
			{
				return this.GetView().InsertMethod;
			}
			set
			{
				this.GetView().InsertMethod = value;
			}
		}

		/// <summary>
		/// Parametros do método de inserção.
		/// </summary>
		[DefaultValue((string)null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), PersistenceMode(PersistenceMode.InnerProperty), Category("Data"), Description("ObjectDataSource_InsertParameters"), MergableProperty(false)]
		public ParameterCollection InsertParameters
		{
			get
			{
				return this.GetView().InsertParameters;
			}
		}

		/// <summary>
		/// Nome do parametro do número máximo de linhas.
		/// </summary>
		[DefaultValue("maximumRows"), Description("ObjectDataSource_MaximumRowsParameterName"), Category("Paging")]
		public string MaximumRowsParameterName
		{
			get
			{
				return this.GetView().MaximumRowsParameterName;
			}
			set
			{
				this.GetView().MaximumRowsParameterName = value;
			}
		}

		/// <summary>
		/// String de formatação do parametro dos valores antigos.
		/// </summary>
		[DefaultValue("{0}"), Category("Data"), Description("DataSource_OldValuesParameterFormatString")]
		public string OldValuesParameterFormatString
		{
			get
			{
				return this.GetView().OldValuesParameterFormatString;
			}
			set
			{
				this.GetView().OldValuesParameterFormatString = value;
			}
		}

		/// <summary>
		/// Nome do método que recupera a quantidade da seleção.
		/// </summary>
		[Category("Paging"), Description("ObjectDataSource_SelectCountMethod"), DefaultValue("")]
		public string SelectCountMethod
		{
			get
			{
				return this.GetView().SelectCountMethod;
			}
			set
			{
				this.GetView().SelectCountMethod = value;
			}
		}

		/// <summary>
		/// Nome do método de seleção.
		/// </summary>
		[DefaultValue(""), Category("Data"), Description("ObjectDataSource_SelectMethod")]
		public string SelectMethod
		{
			get
			{
				return this.GetView().SelectMethod;
			}
			set
			{
				this.GetView().SelectMethod = value;
			}
		}

		/// <summary>
		/// Parametros do método de seleção.
		/// </summary>
		[Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Data"), MergableProperty(false), Description("ObjectDataSource_SelectParameters"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null)]
		public ParameterCollection SelectParameters
		{
			get
			{
				return this.GetView().SelectParameters;
			}
		}

		/// <summary>
		/// Nome do método para recuperar os dados pelas chaves associadas.
		/// </summary>
		[DefaultValue(""), Category("Data"), Description("ObjectDataSource_SelectByKeysMethod")]
		public string SelectByKeysMethod
		{
			get
			{
				return this.GetView().SelectByKeysMethod;
			}
			set
			{
				this.GetView().SelectByKeysMethod = value;
			}
		}

		/// <summary>
		/// Parametros do método GetByKeys.
		/// </summary>
		[Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Data"), MergableProperty(false), Description("ObjectDataSource_SelectByKeysParameters"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null)]
		public ParameterCollection SelectByKeysParameters
		{
			get
			{
				return this.GetView().SelectByKeysParameters;
			}
		}

		/// <summary>
		/// Nome do método usado para criar objeto de dados.
		/// </summary>
		[DefaultValue(""), Category("Data"), Description("ObjectDataSource_CreateDataObjectMethod")]
		public string CreateDataObjectMethod
		{
			get
			{
				return this.GetView().CreateDataObjectMethod;
			}
			set
			{
				this.GetView().CreateDataObjectMethod = value;
			}
		}

		/// <summary>
		/// Parametros do método GetByKeys.
		/// </summary>
		[Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), Category("Data"), MergableProperty(false), Description("ObjectDataSource_CreateDataObjectParameters"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null)]
		public ParameterCollection CreateDataObjectParameters
		{
			get
			{
				return this.GetView().CreateDataObjectParameters;
			}
		}

		/// <summary>
		/// Nome do parametro de ordenação.
		/// </summary>
		[DefaultValue(""), Category("Data"), Description("ObjectDataSource_SortParameterName")]
		public string SortParameterName
		{
			get
			{
				return this.GetView().SortParameterName;
			}
			set
			{
				this.GetView().SortParameterName = value;
			}
		}

		/// <summary>
		/// Nome do parametro que define o indice da linha de inicio.
		/// </summary>
		[Description("ObjectDataSource_StartRowIndexParameterName"), DefaultValue("startRowIndex"), Category("Paging")]
		public string StartRowIndexParameterName
		{
			get
			{
				return this.GetView().StartRowIndexParameterName;
			}
			set
			{
				this.GetView().StartRowIndexParameterName = value;
			}
		}

		/// <summary>
		/// Nome do tipo da origem de dados.
		/// </summary>
		[DefaultValue(""), Category("Data"), Description("ObjectDataSource_TypeName")]
		public string TypeName
		{
			get
			{
				return this.GetView().TypeName;
			}
			set
			{
				this.GetView().TypeName = value;
			}
		}

		/// <summary>
		/// Nome do método de atualização.
		/// </summary>
		[Description("ObjectDataSource_UpdateMethod"), DefaultValue(""), Category("Data")]
		public string UpdateMethod
		{
			get
			{
				return this.GetView().UpdateMethod;
			}
			set
			{
				this.GetView().UpdateMethod = value;
			}
		}

		/// <summary>
		/// Parametros da atualização.
		/// </summary>
		[Description("ObjectDataSource_UpdateParameters"), MergableProperty(false), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(System.Drawing.Design.UITypeEditor)), Category("Data"), DefaultValue((string)null), PersistenceMode(PersistenceMode.InnerProperty)]
		public ParameterCollection UpdateParameters
		{
			get
			{
				return this.GetView().UpdateParameters;
			}
		}

		/// <summary>
		/// Nome do tipo da classe de fabrica abstrata que será usada.
		/// </summary>
		[DefaultValue(""), Category("Factory"), Description("ObjectDataSource_FactoryTypeName")]
		public string FactoryTypeName
		{
			get
			{
				return this.GetView().FactoryTypeName;
			}
			set
			{
				this.GetView().FactoryTypeName = value;
			}
		}

		/// <summary>
		/// Método da fabrica abstrata usado para recupera a instancia da classe abstrata.
		/// </summary>        
		[DefaultValue("get_Instance"), Category("Factory"), Description("ObjectDataSource_FactoryInstanceMethod")]
		public string FactoryInstanceMethod
		{
			get
			{
				return this.GetView().FactoryInstanceMethod;
			}
			set
			{
				this.GetView().FactoryInstanceMethod = value;
			}
		}

		/// <summary>
		/// Método usado para criar a instancia do tipo que será trabalhada.
		/// </summary>
		[DefaultValue(""), Category("Factory"), Description("ObjectDataSource_FactoryMethod")]
		public string FactoryMethod
		{
			get
			{
				return this.GetView().FactoryMethod;
			}
			set
			{
				this.GetView().FactoryMethod = value;
			}
		}

		/// <summary>
		/// Nome do contrato que serão utilizado para recupera 
		/// a instancia do tipo que fornece acesso aos métodos
		/// das operações.
		/// </summary>
		[DefaultValue(""), Category("Factory"), Description("ContractName")]
		public string ContractName
		{
			get
			{
				return this.GetView().ContractName;
			}
			set
			{
				this.GetView().ContractName = value;
			}
		}

		/// <summary>
		/// Estratégia de atualização.
		/// </summary>
		[DefaultValue("Normal"), Category("Data"), Description("UpdateStrategy")]
		public VirtualObjectDataSourceUpdateStrategy UpdateStrategy
		{
			get
			{
				return this.GetView().UpdateStrategy;
			}
			set
			{
				this.GetView().UpdateStrategy = value;
			}
		}

		/// <summary>
		/// Estratégia de exclusão.
		/// </summary>
		[DefaultValue("Normal"), Category("Data"), Description("DeleteStrategy")]
		public VirtualObjectDataSourceDeleteStrategy DeleteStrategy
		{
			get
			{
				return this.GetView().DeleteStrategy;
			}
			set
			{
				this.GetView().DeleteStrategy = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public VirtualObjectDataSource()
		{
			base.Init += OnInit;
		}

		/// <summary>
		/// Cria a instancia com o nome do tipo e o método de seleção.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="selectMethod"></param>
		public VirtualObjectDataSource(string typeName, string selectMethod)
		{
			this.TypeName = typeName;
			this.SelectMethod = selectMethod;
			base.Init += OnInit;
		}

		/// <summary>
		/// Configura os parametros padrão para a fabrica usada na instância.
		/// </summary>
		/// <param name="factoryTypeName"></param>
		/// <param name="factoryMethod"></param>
		public static void ConfigureDefaultFactoryType(string factoryTypeName, string factoryMethod)
		{
			VirtualObjectDataSourceView.ConfigureDefaultFactoryType(factoryTypeName, factoryMethod);
		}

		/// <summary>
		/// Configura os parametros padrão para a fabrica usada na instância.
		/// </summary>
		/// <param name="factoryTypeName"></param>
		/// <param name="factoryMethod"></param>
		/// <param name="factoryInstanceMethod"></param>
		public static void ConfigureDefaultFactoryType(string factoryTypeName, string factoryMethod, string factoryInstanceMethod)
		{
			VirtualObjectDataSourceView.ConfigureDefaultFactoryType(factoryTypeName, factoryMethod, factoryInstanceMethod);
		}

		/// <summary>
		/// Cria a chave do cache.
		/// </summary>
		/// <returns></returns>
		private StringBuilder CreateRawCacheKey()
		{
			StringBuilder builder = new StringBuilder("u", 0x400);
			builder.Append(base.GetType().GetHashCode().ToString(CultureInfo.InvariantCulture));
			builder.Append(":");
			builder.Append(this.CacheDuration.ToString(CultureInfo.InvariantCulture));
			builder.Append(':');
			builder.Append(((int)this.CacheExpirationPolicy).ToString(CultureInfo.InvariantCulture));
			builder.Append(":");
			builder.Append(this.SqlCacheDependency);
			builder.Append(":");
			builder.Append(this.TypeName);
			builder.Append(":");
			builder.Append(this.SelectMethod);
			if(this.SelectParameters.Count > 0)
			{
				builder.Append("?");
				foreach (DictionaryEntry entry in this.SelectParameters.GetValues(this.Context, this))
				{
					builder.Append(entry.Key.ToString());
					if((entry.Value != null) && (entry.Value != DBNull.Value))
					{
						builder.Append("=");
						builder.Append(entry.Value.ToString());
					}
					else if(entry.Value == DBNull.Value)
					{
						builder.Append("(dbnull)");
					}
					else
					{
						builder.Append("(null)");
					}
					builder.Append("&");
				}
			}
			return builder;
		}

		/// <summary>
		/// Cria a chave mestre do cache.
		/// </summary>
		/// <returns></returns>
		internal string CreateMasterCacheKey()
		{
			return this.CreateRawCacheKey().ToString();
		}

		/// <summary>
		/// Cria a chave do cache.
		/// </summary>
		/// <param name="startRowIndex"></param>
		/// <param name="maximumRows"></param>
		/// <returns></returns>
		internal string CreateCacheKey(int startRowIndex, int maximumRows)
		{
			StringBuilder builder = this.CreateRawCacheKey();
			builder.Append(':');
			builder.Append(startRowIndex.ToString(CultureInfo.InvariantCulture));
			builder.Append(':');
			builder.Append(maximumRows.ToString(CultureInfo.InvariantCulture));
			return builder.ToString();
		}

		/// <summary>
		/// Invalida a entrada do cache.
		/// </summary>
		internal void InvalidateCacheEntry()
		{
			string key = this.CreateMasterCacheKey();
			this.Cache.Invalidate(key);
		}

		/// <summary>
		/// Carrega os dados do cache.
		/// </summary>
		/// <param name="startRowIndex"></param>
		/// <param name="maximumRows"></param>
		/// <returns></returns>
		internal object LoadDataFromCache(int startRowIndex, int maximumRows)
		{
			string key = this.CreateCacheKey(startRowIndex, maximumRows);
			return this.Cache.LoadDataFromCache(key);
		}

		/// <summary>
		/// Recupera a quantidade total de linhas do cache.
		/// </summary>
		/// <returns></returns>
		internal int LoadTotalRowCountFromCache()
		{
			string key = this.CreateMasterCacheKey();
			object obj2 = this.Cache.LoadDataFromCache(key);
			if(obj2 is int)
				return (int)obj2;
			return -1;
		}

		/// <summary>
		/// Salva os dados para o cache.
		/// </summary>
		/// <param name="startRowIndex"></param>
		/// <param name="maximumRows"></param>
		/// <param name="data"></param>
		internal void SaveDataToCache(int startRowIndex, int maximumRows, object data)
		{
			string key = this.CreateCacheKey(startRowIndex, maximumRows);
			string str2 = this.CreateMasterCacheKey();
			if(this.Cache.LoadDataFromCache(str2) == null)
				this.Cache.SaveDataToCache(str2, -1);
			var constructor = typeof(System.Web.Caching.CacheDependency).GetConstructor(new Type[] {
				typeof(int),
				typeof(string[]),
				typeof(string[])
			});
			System.Web.Caching.CacheDependency dependency = null;
			if(constructor == null)
				dependency = new System.Web.Caching.CacheDependency(new string[0], new string[] {
					str2
				});
			else
			{
				try
				{
					dependency = (System.Web.Caching.CacheDependency)constructor.Invoke(new object[] {
						0,
						new string[0],
						new string[] {
							str2
						}
					});
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}
			this.Cache.SaveDataToCache(key, data, dependency);
		}

		/// <summary>
		/// Salva o total de linhas para o cache.
		/// </summary>
		/// <param name="totalRowCount"></param>
		internal void SaveTotalRowCountToCache(int totalRowCount)
		{
			string key = this.CreateMasterCacheKey();
			this.Cache.SaveDataToCache(key, totalRowCount);
		}

		/// <summary>
		/// Aciona o método delete.
		/// </summary>
		/// <returns></returns>
		public int Delete()
		{
			return this.GetView().Delete(null, null);
		}

		/// <summary>
		/// Recupera a visualização da instancia.
		/// </summary>
		/// <returns></returns>
		private VirtualObjectDataSourceView GetView()
		{
			if(_view == null)
			{
				_view = new VirtualObjectDataSourceView(this, "DefaultView", this.Context);
				if(base.IsTrackingViewState)
					((IStateManager)_view).TrackViewState();
			}
			return _view;
		}

		protected override DataSourceView GetView(string viewName)
		{
			if((viewName == null) || ((viewName.Length != 0) && !string.Equals(viewName, "DefaultView", StringComparison.OrdinalIgnoreCase)))
				throw new ArgumentException("InvalidViewName");
			return this.GetView();
		}

		protected override ICollection GetViewNames()
		{
			if(this._viewNames == null)
				this._viewNames = new string[] {
					"DefaultView"
				};
			return this._viewNames;
		}

		public int Insert()
		{
			return this.GetView().Insert(null);
		}

		private void LoadCompleteEventHandler(object sender, EventArgs e)
		{
			this.SelectParameters.UpdateValues(this.Context, this);
			this.FilterParameters.UpdateValues(this.Context, this);
		}

		protected override void LoadViewState(object savedState)
		{
			Pair pair = (Pair)savedState;
			if(savedState == null)
			{
				base.LoadViewState(null);
			}
			else
			{
				base.LoadViewState(pair.First);
				if(pair.Second != null)
				{
					((IStateManager)this.GetView()).LoadViewState(pair.Second);
				}
			}
		}

		protected void OnInit(object sender, EventArgs e)
		{
			if(this.Page != null)
				this.Page.LoadComplete += new EventHandler(this.LoadCompleteEventHandler);
		}

		protected override object SaveViewState()
		{
			Pair pair = new Pair();
			pair.First = base.SaveViewState();
			if(this._view != null)
			{
				pair.Second = ((IStateManager)this._view).SaveViewState();
			}
			if((pair.First == null) && (pair.Second == null))
			{
				return null;
			}
			return pair;
		}

		/// <summary>
		/// Seleciona os valores da fonte de dados.
		/// </summary>
		/// <returns></returns>
		public IEnumerable Select()
		{
			return this.GetView().Select(DataSourceSelectArguments.Empty);
		}

		protected override void TrackViewState()
		{
			base.TrackViewState();
			if(this._view != null)
			{
				((IStateManager)this._view).TrackViewState();
			}
		}

		/// <summary>
		/// Chama o método para atualiza os dados.
		/// </summary>
		/// <returns></returns>
		public int Update()
		{
			return this.GetView().Update(null, null, null);
		}
	}
}
