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
	/// Implementação do enumerable dos filhos de uma entidade.
	/// </summary>
	class EntityChildEnumerable : QueryResultEnumerable<IEntity>
	{
		private IEntityLoaderChildInfo _childInfo;

		private IEntityLoader _childLoader;

		private IEntityTypeManager _entityTypeManager;

		private Query.ISourceContext _sourceContext;

		private string _uiContext;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="enumerable">Instancia do Enumerable que será adaptada.</param>
		/// <param name="childInfo"></param>
		/// <param name="childLoader">Loader da entidade do filho.</param>
		/// <param name="entityTypeManager">Instancia do gerenciador dos tipos de entidades.</param>
		/// <param name="sourceContext">Instancia do contexto de origem.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		public EntityChildEnumerable(IEnumerable<IEntity> enumerable, IEntityLoaderChildInfo childInfo, IEntityLoader childLoader, IEntityTypeManager entityTypeManager, Query.ISourceContext sourceContext, string uiContext) : base(enumerable, null, null)
		{
			_childInfo = childInfo;
			_childLoader = childLoader;
			_entityTypeManager = entityTypeManager;
			_sourceContext = sourceContext;
			_uiContext = uiContext;
		}

		/// <summary>
		/// Recupera o nome do tipo.
		/// </summary>
		/// <returns></returns>
		protected override Reflection.TypeName GetTypeName()
		{
			var dataModelType = _childInfo.DataModelType;
			if(dataModelType != null)
				return Colosoft.Reflection.TypeName.Get(dataModelType);
			return null;
		}

		/// <summary>
		/// Cria o observer para as alterações do resultado da consulta.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		protected override Collections.INotifyCollectionChangedObserver CreateQueryResultChangedObserver(Reflection.TypeName typeName, System.Collections.IList collection)
		{
			if(Colosoft.Query.RecordObserverManager.Instance.IsEnabled)
				return new EntityChildQueryResultChangedObserver(_childInfo, _childLoader, _entityTypeManager, _sourceContext, _uiContext, typeName, collection);
			return null;
		}

		/// <summary>
		/// Implementação do observer da alteração do resultado da consulta.
		/// </summary>
		class EntityChildQueryResultChangedObserver : BusinessQueryResultChangedObserver<IEntity>
		{
			private IEntityLoaderChildInfo _childInfo;

			/// <summary>
			/// Informações do filho associado.
			/// </summary>
			public IEntityLoaderChildInfo ChildInfo
			{
				get
				{
					return _childInfo;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="childInfo"></param>
			/// <param name="childLoader">Loader da entidade filha.</param>
			/// <param name="entityTypeManager">Instancia do gerenciador dos tipos e entidade.</param>
			/// <param name="sourceContext">Instancia do contexto de origem.</param>
			/// <param name="uiContext">Contexto da interface com o usuário.</param>
			/// <param name="typeName"></param>
			/// <param name="collection"></param>
			public EntityChildQueryResultChangedObserver(IEntityLoaderChildInfo childInfo, IEntityLoader childLoader, IEntityTypeManager entityTypeManager, Query.ISourceContext sourceContext, string uiContext, Colosoft.Reflection.TypeName typeName, System.Collections.IList collection) : base(childLoader, entityTypeManager, sourceContext, uiContext, typeName, collection)
			{
				_childInfo = childInfo;
			}

			/// <summary>
			/// Verifica se o registor é compatível com o observer.
			/// </summary>
			/// <param name="record"></param>
			/// <returns></returns>
			public override bool Evaluate(Query.IRecord record)
			{
				if(!IsAlive)
					return false;
				var entity = Collection as IEntity;
				if(entity != null && entity.Owner != null)
					return _childInfo.Evaluate(record, entity.Owner);
				return false;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("EntityChildObserver -> Child: {0}", _childInfo);
			}
		}
	}
}
