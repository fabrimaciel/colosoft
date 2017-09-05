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
using Colosoft.Query;

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação do observer para o resultado de consultas de entities.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class BusinessQueryResultChangedObserver<T> : QueryResultChangedObserver<T> where T : IEntity
	{
		private Colosoft.Reflection.TypeName _typeName;

		private IEntityLoader _entityLoader;

		private IEntityTypeManager _entityTypeManager;

		private Query.ISourceContext _sourceContext;

		private string _uiContext;

		private Colosoft.Reflection.TypeName _dataModelTypeName;

		/// <summary>
		/// Instancia do loader da entidade.
		/// </summary>
		public IEntityLoader EntityLoader
		{
			get
			{
				return _entityLoader;
			}
		}

		/// <summary>
		/// Instancia do gerenciador dos tipos de entidades.
		/// </summary>
		public IEntityTypeManager EntityTypeManager
		{
			get
			{
				return _entityTypeManager;
			}
		}

		/// <summary>
		/// Contexto de origem.
		/// </summary>
		public ISourceContext SourceContext
		{
			get
			{
				return _sourceContext;
			}
		}

		/// <summary>
		/// Contexto de interface com o usuário.
		/// </summary>
		public string UIContext
		{
			get
			{
				return _uiContext;
			}
		}

		/// <summary>
		/// Nome do tipo do modelo de dados do filho.
		/// </summary>
		public Colosoft.Reflection.TypeName DataModelTypeName
		{
			get
			{
				if(_dataModelTypeName == null)
					_dataModelTypeName = Colosoft.Reflection.TypeName.Get(EntityLoader.DataModelType);
				return _dataModelTypeName;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityLoader">Instancia do loader da entidade associada.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador dos tipos de entidades.</param>
		/// <param name="sourceContext">Contexto de origem.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <param name="typeName">Nome do associado com o observer.</param>
		/// <param name="collection">Coleção que sera observada.</param>
		public BusinessQueryResultChangedObserver(IEntityLoader entityLoader, IEntityTypeManager entityTypeManager, Query.ISourceContext sourceContext, string uiContext, Colosoft.Reflection.TypeName typeName, System.Collections.IList collection) : base(collection)
		{
			_entityLoader = entityLoader;
			_entityTypeManager = entityTypeManager;
			_sourceContext = sourceContext;
			_uiContext = uiContext;
			_typeName = typeName;
			if(Colosoft.Query.RecordObserverManager.Instance.IsEnabled)
				Colosoft.Query.RecordObserverManager.Instance.Register(_typeName, this);
		}

		/// <summary>
		/// Método usado para criar um item a partir dos dados do registro.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		protected override T CreateCollectionItem(IRecord record)
		{
			var bindStrategy = EntityLoader.GetBindStrategy().CreateSession(record.Descriptor);
			var objectCretaor = EntityLoader.GetObjectCreator();
			var recordKeyFactory = EntityLoader.GetRecordKeyFactory();
			var recordKey = recordKeyFactory.Create(DataModelTypeName, record);
			return (T)EntityLoader.FullLoad(record, recordKey, bindStrategy, objectCretaor, SourceContext, UIContext, EntityTypeManager);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected override void Dispose(bool disposing)
		{
			if(Colosoft.Query.RecordObserverManager.Instance.IsEnabled)
				Colosoft.Query.RecordObserverManager.Instance.Unregister(_typeName, this);
			base.Dispose(disposing);
		}

		/// <summary>
		/// Avalia se a chave do registro é compatível com o observer.
		/// </summary>
		/// <param name="recordKey"></param>
		/// <returns></returns>
		public override bool Evaluate(RecordKey recordKey)
		{
			return true;
		}

		/// <summary>
		/// Avalia se os dados do registro são compatíveis com o observer.
		/// </summary>
		/// <param name="record"></param>
		/// <returns></returns>
		public override bool Evaluate(IRecord record)
		{
			return false;
		}

		/// <summary>
		/// Processa o registro apagado.
		/// </summary>
		/// <param name="recordKey"></param>
		public override void OnRecordDeleted(RecordKey recordKey)
		{
			if(!IsAlive)
				return;
			var collection = Collection;
			if(collection != null)
			{
				for(var i = 0; i < collection.Count; i++)
				{
					var recordKeyEquatable = collection[i] as IRecordKeyEquatable;
					if(recordKeyEquatable != null && recordKeyEquatable.Equals(recordKey, RecordKeyComparisonType.Key))
					{
						collection.RemoveAt(i--);
					}
				}
			}
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(_entityLoader != null)
				return string.Format("Business ObserverType: {0}; Collection: {1}", _entityLoader.DataModelTypeName, Collection);
			return base.ToString();
		}
	}
}
