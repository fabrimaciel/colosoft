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
using System.Linq;
using System.Text;
using Colosoft.Caching;
using Colosoft.Query;
using Colosoft.Data.Schema;
using Microsoft.Practices.ServiceLocation;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Assinatura da classe de inicialização do cache.
	/// </summary>
	public class DataCacheLoader : ICacheLoader
	{
		private ISourceContext _sourceContext;

		private ITypeSchema _typeSchema;

		/// <summary>
		/// Relação dos metadados que será processados.
		/// </summary>
		private IList<ITypeMetadata> _typesMetadata;

		private IDataCacheManager _manager;

		private Colosoft.Logging.ILogger _logger;

		private AggregateCacheLoaderObserver _observer = new AggregateCacheLoaderObserver();

		private Colosoft.Query.IRecordKeyFactory _cacheItemKeyFactory;

		/// <summary>
		/// Armazena a relação dos nomes dos tipos que tiveram um erro na carga.
		/// </summary>
		private List<Colosoft.Reflection.TypeName> _errorTypeNames = new List<Reflection.TypeName>();

		/// <summary>
		/// Identifica se a carga foi iniciada.
		/// </summary>
		private bool _started = false;

		private ItemsLoader _itemsLoader;

		/// <summary>
		/// Evento acionado quando ocorre um erro no processamento da carga.
		/// </summary>
		public event CacheErrorEventHandler LoadProcessingError;

		/// <summary>
		/// Instancia do gerador de chave
		/// </summary>
		public Colosoft.Query.IRecordKeyFactory RecordKeyFactory
		{
			get
			{
				if(Colosoft.Query.RecordKeyFactory.Instance == null)
				{
					if(_cacheItemKeyFactory == null)
						_cacheItemKeyFactory = new Data.Schema.RecordKeyFactory(_typeSchema);
					return _cacheItemKeyFactory;
				}
				return Colosoft.Query.RecordKeyFactory.Instance;
			}
		}

		/// <summary>
		/// Agregador dos observers da instancia.
		/// </summary>
		public AggregateCacheLoaderObserver Observer
		{
			get
			{
				return _observer;
			}
			set
			{
				if(_observer != value)
					throw new InvalidOperationException();
			}
		}

		/// <summary>
		/// Instancia do contexto de origem associado.
		/// </summary>
		protected ISourceContext SourceContext
		{
			get
			{
				return _sourceContext;
			}
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~DataCacheLoader()
		{
			Dispose(false);
		}

		/// <summary>
		///  Inicializa a instancia com os parametros informados.
		/// </summary>
		/// <param name="parameters">Dicionário com os parametros de inicialização.</param>
		public void Init(System.Collections.IDictionary parameters)
		{
			var sourceContext = parameters["sourceContext"] as ISourceContext;
			var manager = parameters["manager"] as IDataCacheManager;
			var logger = parameters["logger"] as Colosoft.Logging.ILogger;
			var typeSchema = parameters["typeSchema"] as Colosoft.Data.Schema.ITypeSchema;
			var cacheLoaderObserver = parameters["cacheLoaderObserver"] as ICacheLoaderObserver;
			var typesMetadata = parameters["typesMetadata"] as IList<ITypeMetadata>;
			sourceContext.Require("sourceContext").NotNull();
			manager.Require("manager").NotNull();
			if(typesMetadata == null)
				typeSchema.Require("typeSchema").NotNull();
			logger.Require("logger").NotNull();
			_sourceContext = sourceContext;
			_manager = manager;
			_logger = logger;
			_typeSchema = typeSchema;
			_typesMetadata = typesMetadata;
			_started = false;
			if(cacheLoaderObserver != null)
				_observer += cacheLoaderObserver;
		}

		/// <summary>
		/// Carrega o próximo item do cache.
		/// </summary>
		/// <param name="data">Lista ordenada com os dados a serem inseridos</param>
		/// <param name="index">Objeto de indexeção para saber em que estágio da geração do cache estamos</param>
		/// <returns>Retorna true enquanto falta objetos para se carregar para o cache</returns>
		public bool LoadNext(ref OrderedDictionary data, ref object index)
		{
			try
			{
				if(!_started)
					Start(null);
				if(_itemsLoader == null)
					return false;
				Record record = null;
				IRecordKeyGenerator generator = null;
				while (true)
				{
					if(!_itemsLoader.MoveNext())
					{
						_itemsLoader.Dispose();
						_itemsLoader = null;
						OnLoadFinish();
						return false;
					}
					record = _itemsLoader.Current;
					generator = _itemsLoader.CurrentGenerator;
					if(generator == null)
						continue;
					break;
				}
				var key = generator.GetKey(record);
				var values = new object[record.FieldCount];
				for(var i = 0; i < record.FieldCount; i++)
					values[i] = record.GetValue(i);
				var cacheItem = new ProviderCacheItem(new CacheItemRecord(_itemsLoader.CurrentTypeName, values, record.Descriptor));
				cacheItem.AbsoluteExpiration = DateTime.MaxValue;
				data.Add(key, cacheItem);
				return true;
			}
			catch
			{
				throw;
			}
		}

		/// <summary>
		/// Inicializa a carga do cache.
		/// </summary>
		private void Start(object userState)
		{
			Observer.OnDoWork(userState);
			var typeNames = _manager.Select(f => f.FullName).ToList();
			typeNames.Sort();
			IList<ITypeMetadata> metadata = null;
			if(_typesMetadata != null)
				metadata = _typesMetadata;
			else
				metadata = _typeSchema.GetTypeMetadatas().Where(f => typeNames.BinarySearch(f.FullName) >= 0).ToList();
			_itemsLoader = new ItemsLoader(this, metadata);
			_started = true;
		}

		/// <summary>
		/// Método acionado quando a carga finalizar.
		/// </summary>
		private void OnLoadFinish()
		{
			if(_errorTypeNames.Count > 0)
			{
				foreach (var typeName in _errorTypeNames)
					_manager.Unregister(typeName);
				_errorTypeNames.Clear();
			}
		}

		/// <summary>
		/// Método acionado quando ocorrer 
		/// </summary>
		/// <param name="typeName">Nome do tipo onde ocorreu o erro de carga</param>
		/// <param name="e"></param>
		protected void OnLoadError(Colosoft.Reflection.TypeName typeName, CacheLoaderErrorEventArgs e)
		{
			var index = _errorTypeNames.BinarySearch(typeName, Colosoft.Reflection.TypeName.TypeNameFullNameComparer.Instance);
			if(index < 0)
				_errorTypeNames.Insert(~index, typeName);
			if(LoadProcessingError != null)
				LoadProcessingError(this, e);
			Observer.OnLoadError(e);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_itemsLoader != null)
			{
				_itemsLoader.Dispose();
				_itemsLoader = null;
			}
		}

		/// <summary>
		/// Classe usada para realizar a carga dos itens para o cache.
		/// </summary>
		sealed class ItemsLoader : IEnumerator<Colosoft.Query.Record>
		{
			private DataCacheLoader _loader;

			private IEnumerator<Record> _recordEnumerator;

			private IEnumerator<ITypeMetadata> _typeMetadatasEnumerator;

			private int _typeMetadatasCount;

			private Colosoft.Query.IRecordKeyGenerator _currentGenerator;

			private Colosoft.Reflection.TypeName _currentTypeName;

			private int _position = 0;

			/// <summary>
			/// Instancia da atual registro o enumerador.
			/// </summary>
			public Record Current
			{
				get
				{
					return _recordEnumerator != null ? _recordEnumerator.Current : null;
				}
			}

			/// <summary>
			/// Instancia do atual registro do enumerador.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return _recordEnumerator != null ? _recordEnumerator.Current : null;
				}
			}

			/// <summary>
			/// Instancia do atual gerador de chave.
			/// </summary>
			public Colosoft.Query.IRecordKeyGenerator CurrentGenerator
			{
				get
				{
					return _currentGenerator;
				}
			}

			/// <summary>
			/// Nome do atual tipo que está sendo processado.
			/// </summary>
			public Colosoft.Reflection.TypeName CurrentTypeName
			{
				get
				{
					return _currentTypeName;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="loader"></param>
			/// <param name="typeMetadatas"></param>
			public ItemsLoader(DataCacheLoader loader, IList<ITypeMetadata> typeMetadatas)
			{
				_loader = loader;
				_typeMetadatasCount = typeMetadatas.Count;
				_typeMetadatasEnumerator = typeMetadatas.GetEnumerator();
			}

			/// <summary>
			/// Destrutor.
			/// </summary>
			~ItemsLoader()
			{
				Dispose();
			}

			/// <summary>
			/// Move para o próximo nome de tipo que será processado.
			/// </summary>
			/// <returns></returns>
			private bool MoveNextTypeMetadata()
			{
				while (_typeMetadatasEnumerator != null && _typeMetadatasEnumerator.MoveNext())
				{
					var typeMetadata = _typeMetadatasEnumerator.Current;
					_loader.Observer.OnStageChanged(new CacheLoaderStage("LoadingType", ResourceMessageFormatter.Create(() => Properties.Resources.CacheLoader_LoadingTypeName, typeMetadata.FullName)));
					_loader.Observer.OnCurrentProgressChanged(new System.ComponentModel.ProgressChangedEventArgs((_position++ * 100 / _typeMetadatasCount), null));
					if(_loader.Observer is IDataCacheLoaderObserver)
						((IDataCacheLoaderObserver)_loader.Observer).OnBeginLoadTypeMetadata(typeMetadata);
					var typeName = new Colosoft.Reflection.TypeName(!string.IsNullOrEmpty(typeMetadata.Assembly) ? string.Format("{0}, {1}", typeMetadata.FullName, typeMetadata.Assembly) : typeMetadata.FullName);
					Query.IQueryResult result = null;
					try
					{
						var columns = new List<Query.ProjectionEntry>(typeMetadata.Select(f => new Query.ProjectionEntry(f.Name, null)));
						if(typeMetadata.IsVersioned && !columns.Exists(f =>  {
							var column = f.GetColumnInfo();
							return column != null && StringComparer.InvariantCultureIgnoreCase.Equals(column.Name, "RowVersion");
						}))
							columns.Add(new ProjectionEntry("RowVersion", null));
						result = _loader.SourceContext.CreateQuery().From(new Query.EntityInfo(typeMetadata.FullName)).Select(new Query.Projection(columns)).NoUseCache().Execute();
						_recordEnumerator = result.GetEnumerator();
					}
					catch(Exception ex)
					{
						var args = new CacheLoaderErrorEventArgs(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheLoader_CreateQueryError, typeMetadata.FullName), ex);
						_loader.OnLoadError(typeName, args);
						if(_loader.Observer is IDataCacheLoaderObserver)
							((IDataCacheLoaderObserver)_loader.Observer).OnEndLoadTypeMetadata(typeMetadata, ex);
						continue;
					}
					_currentTypeName = typeName;
					_currentGenerator = _loader.RecordKeyFactory.CreateGenerator(_currentTypeName);
					return true;
				}
				_currentTypeName = null;
				_currentGenerator = null;
				if(_recordEnumerator != null)
					try
					{
						_recordEnumerator.Dispose();
					}
					catch
					{
					}
				_recordEnumerator = null;
				return false;
			}

			/// <summary>
			/// Move o próximo registro.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				var moved = false;
				while (true)
				{
					moved = false;
					try
					{
						if(_recordEnumerator != null)
							moved = _recordEnumerator.MoveNext();
					}
					catch(Exception ex)
					{
						if(_recordEnumerator != null)
							_recordEnumerator.Dispose();
						_recordEnumerator = null;
						var args = new CacheLoaderErrorEventArgs(ResourceMessageFormatter.Create(() => Properties.Resources.DataCacheLoader_GetRecordError, _typeMetadatasEnumerator.Current.FullName), ex);
						_loader.OnLoadError(_currentTypeName, args);
						if(_loader.Observer is IDataCacheLoaderObserver)
							((IDataCacheLoaderObserver)_loader.Observer).OnEndLoadTypeMetadata(_typeMetadatasEnumerator.Current, ex);
					}
					if(_recordEnumerator != null && !moved)
					{
						_recordEnumerator.Dispose();
						_recordEnumerator = null;
						if(_loader.Observer is IDataCacheLoaderObserver)
							((IDataCacheLoaderObserver)_loader.Observer).OnEndLoadTypeMetadata(_typeMetadatasEnumerator.Current, null);
					}
					if(_recordEnumerator == null && MoveNextTypeMetadata())
					{
						continue;
					}
					break;
				}
				if(_recordEnumerator == null)
				{
					_loader.Observer.OnCurrentProgressChanged(new System.ComponentModel.ProgressChangedEventArgs(100, null));
					return false;
				}
				return moved;
			}

			/// <summary>
			/// Reseta ao enumerador.
			/// </summary>
			public void Reset()
			{
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
				if(_typeMetadatasEnumerator != null)
				{
					_typeMetadatasEnumerator.Dispose();
					_typeMetadatasEnumerator = null;
				}
				if(_recordEnumerator != null)
				{
					_recordEnumerator.Dispose();
					_recordEnumerator = null;
				}
			}
		}
	}
}
