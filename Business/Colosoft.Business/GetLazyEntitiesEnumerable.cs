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
	partial class EntityLoader<TEntity1, Model>
	{
		/// <summary>
		/// Implementação do Enumerable que recuperea as entidades
		/// para serem carregadas de forma tardia.
		/// </summary>
		class GetLazyEntitiesEnumerable : GetEntitiesEnumerable
		{
			private EntityLoaderLazyArgs _entityLazyArgs;

			private IEnumerable<Colosoft.Query.IRecord> _result;

			/// <summary>
			/// Construtor usado para carregar as entidade com base no resultado preparado.
			/// </summary>
			/// <param name="entityLoader">Loader associado.</param>
			/// <param name="queryable"></param>
			/// <param name="sourceContext"></param>
			/// <param name="result"></param>
			/// <param name="uiContext"></param>
			/// <param name="entityTypeManager"></param>
			/// <param name="entityLazyArgs"></param>
			public GetLazyEntitiesEnumerable(EntityLoader<TEntity1, Model> entityLoader, Colosoft.Query.Queryable queryable, Colosoft.Query.ISourceContext sourceContext, IEnumerable<Colosoft.Query.IRecord> result, string uiContext, IEntityTypeManager entityTypeManager, EntityLoaderLazyArgs entityLazyArgs) : base(entityLoader, queryable, sourceContext, uiContext, entityTypeManager)
			{
				_entityLazyArgs = entityLazyArgs;
				_result = result;
			}

			/// <summary>
			/// Recupera o nome do tipo principal associado com o resultado da consulta.
			/// </summary>
			/// <returns></returns>
			protected override Colosoft.Reflection.TypeName GetTypeName()
			{
				if(Queryable != null && Queryable.Entity != null)
					return new Colosoft.Reflection.TypeName(Queryable.Entity.FullName);
				return typeof(Model).GetName();
			}

			/// <summary>
			/// Recupera o enumerador das entidade com base no resultado preparado.
			/// </summary>
			/// <param name="result"></param>
			/// <returns></returns>
			private IEnumerator<IEntity> GetEnumerator(IEnumerable<Colosoft.Query.IRecord> result)
			{
				if(result == null)
					yield break;
				else
				{
					var recordKeyFactory = EntityLoader.GetRecordKeyFactory();
					var typeName = Colosoft.Reflection.TypeName.Get(EntityLoader.DataModelType);
					foreach (var i in result)
					{
						var recordKey = recordKeyFactory.Create(typeName, i);
						yield return EntityLoader.LazyLoad(i, recordKey, SourceContext, UiContext, EntityTypeManager, _entityLazyArgs);
					}
				}
			}

			/// <summary>
			/// Recupera o enumerador das entidade.
			/// </summary>
			/// <returns></returns>
			public override IEnumerator<IEntity> GetEnumerator()
			{
				if(_result == null && Queryable != null)
					using (var result = Queryable.Execute(Queryable.DataSource))
						return GetEnumerator(result);
				return GetEnumerator(_result);
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			/// <param name="disposing"></param>
			protected override void Dispose(bool disposing)
			{
				if(_result is IDisposable)
				{
					((IDisposable)_result).Dispose();
					_result = null;
				}
				base.Dispose(disposing);
			}
		}
	}
}
