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

namespace Colosoft.Business
{
	/// <summary>
	/// Implementção do observer do resultados de consulta associados com um <see cref="IEntityDescriptor"/>.
	/// </summary>
	class EntityDescriptorQueryResultChangedObserver : Colosoft.Query.IQueryResultChangedObserver, Collections.INotifyCollectionChangedObserver, IDisposableState
	{
		private IEntityLoader _entityLoader;

		private WeakReference _collectionReference;

		private Colosoft.Query.ISourceContext _sourceContext;

		private ulong _uid;

		private bool _isDisposed;

		/// <summary>
		/// Identificador unicod do observer.
		/// </summary>
		public ulong Uid
		{
			get
			{
				return _uid;
			}
		}

		/// <summary>
		/// Instancia da coleção associada.
		/// </summary>
		protected System.Collections.IList Collection
		{
			get
			{
				return _collectionReference.Target as System.Collections.IList;
			}
		}

		/// <summary>
		/// Identifica se o observer está vivo.
		/// </summary>
		public virtual bool IsAlive
		{
			get
			{
				if(_isDisposed || !_collectionReference.IsAlive)
					return false;
				var collection = _collectionReference.Target;
				if(collection is IDisposableState)
					return !((IDisposableState)collection).IsDisposed;
				return collection != null;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityLoader"></param>
		/// <param name="sourceContext"></param>
		/// <param name="collection"></param>
		public EntityDescriptorQueryResultChangedObserver(IEntityLoader entityLoader, Query.ISourceContext sourceContext, System.Collections.IList collection)
		{
			entityLoader.Require("entityLoader").NotNull();
			collection.Require("collection").NotNull();
			_uid = Query.QueryResultChangedObserverUidGenerator.CreateUid();
			_collectionReference = new WeakReference(collection);
			_entityLoader = entityLoader;
			_sourceContext = sourceContext;
			if(Colosoft.Query.RecordObserverManager.Instance.IsEnabled)
				Colosoft.Query.RecordObserverManager.Instance.Register(_entityLoader.DataModelTypeName, this);
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~EntityDescriptorQueryResultChangedObserver()
		{
			Dispose(false);
		}

		/// <summary>
		/// Cria um registro para a coleção com base no registro informado.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		protected IEntityDescriptor CreateCollectionItem(Colosoft.Query.IRecord record)
		{
			var bindStrategy = _entityLoader.GetEntityDescriptorBindStrategy();
			var objectCreator = _entityLoader.GetEntityDescriptorCreator();
			var instance = objectCreator.Create();
			bindStrategy.Bind(record, Query.BindStrategyMode.All, ref instance);
			if(instance is IConnectedEntity)
				((IConnectedEntity)instance).Connect(_sourceContext);
			if(instance is IEntityRecordObserver && instance is BusinessEntityDescriptor)
				((IEntityRecordObserver)instance).RegisterObserver(((BusinessEntityDescriptor)instance).RecordKey);
			if(instance is ILoadableEntity)
				((ILoadableEntity)instance).NotifyLoaded();
			return (IEntityDescriptor)instance;
		}

		/// <summary>
		/// Avalia se o registro informado é compatível.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public bool Evaluate(Colosoft.Query.IRecord record)
		{
			return IsAlive;
		}

		/// <summary>
		/// Avalia se a chave do registro é compatível.
		/// </summary>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		public bool Evaluate(Colosoft.Query.RecordKey recordKey)
		{
			return IsAlive;
		}

		/// <summary>
		/// Método acionado quando um registro é criado.
		/// </summary>
		/// <param name="record"></param>
		public virtual void OnRecordInserted(Colosoft.Query.IRecord record)
		{
			if(!IsAlive)
				return;
			var item = CreateCollectionItem(record);
			var collection = Collection;
			if(collection != null && !collection.Contains(item))
				collection.Add(item);
		}

		/// <summary>
		/// Método acionado quando um registro for removido.
		/// </summary>
		/// <param name="recordKey"></param>
		public void OnRecordDeleted(Colosoft.Query.RecordKey recordKey)
		{
			if(!IsAlive)
				return;
			var collection = Collection;
			if(collection != null)
			{
				for(var i = 0; i < collection.Count; i++)
				{
					var recordKeyEquatable = collection[i] as Colosoft.Query.IRecordKeyEquatable;
					if(recordKeyEquatable != null && recordKeyEquatable.Equals(recordKey, Colosoft.Query.RecordKeyComparisonType.Key))
					{
						collection.RemoveAt(i--);
					}
				}
			}
		}

		/// <summary>
		/// Método acionado quando a coleção sore alguma alteração.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public virtual void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_entityLoader != null)
				return string.Format("ObserverType: {0}; Collection: {1}", _entityLoader.DataModelTypeName, Collection);
			return base.ToString();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_entityLoader != null && Colosoft.Query.RecordObserverManager.Instance.IsEnabled)
				Colosoft.Query.RecordObserverManager.Instance.Unregister(_entityLoader.DataModelTypeName, this);
			_entityLoader = null;
			_sourceContext = null;
			_isDisposed = false;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Identifica se a instancia está liberada.
		/// </summary>
		public bool IsDisposed
		{
			get
			{
				return _isDisposed;
			}
		}
	}
}
