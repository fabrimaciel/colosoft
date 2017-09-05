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
	/// Implementação da configuração do filho dinâmico.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TModel"></typeparam>
	public class EntityDynamicChild<TEntity, TModel> : EntityDynamicChild where TModel : class, Colosoft.Data.IModel where TEntity : IEntity<TModel>
	{
		/// <summary>
		/// Recupera a instancia de configuração.
		/// </summary>
		/// <returns></returns>
		protected FluentEntityDynamicChild Configure()
		{
			return new FluentEntityDynamicChild(this);
		}

		/// <summary>
		/// Registra uma entrada.
		/// </summary>
		/// <param name="entityType">Tipo da entidade associada com a entrada.</param>
		/// <param name="parentPropertyUidName">Nome da propriedade do pai que faz associação.</param>
		/// <param name="foreignPropertyName">Nome da propriedade que representa a chave estrangeira.</param>
		/// <param name="conditional">Condicional usado na carga.</param>
		/// <param name="executePredicate">Predicado para executar a consulta do filho.</param>
		/// <returns></returns>
		public void Register(Type entityType, string parentPropertyUidName, string foreignPropertyName, EntityLoaderConditional conditional, Colosoft.Query.QueryExecutePredicate executePredicate)
		{
			entityType.Require("entityType").NotNull();
			Type entityModelType = null;
			var baseType = entityType;
			while (baseType != typeof(object))
			{
				if(baseType.IsGenericType)
				{
					var argument = baseType.GetGenericArguments().FirstOrDefault();
					if(argument != null)
					{
						if(typeof(Colosoft.Data.IModel).IsAssignableFrom(argument))
						{
							entityModelType = argument;
							break;
						}
					}
				}
				baseType = baseType.BaseType;
			}
			if(entityModelType == null)
				throw new InvalidOperationException(string.Format("Entity type '{0}' not support IEntity<Model>", entityType.FullName));
			var entryType = typeof(EntityDynamicChildEntry<, , , >).MakeGenericType(typeof(TEntity), typeof(TModel), entityType, entityModelType);
			var constructor = entryType.GetConstructor(new Type[] {
				typeof(string),
				typeof(string),
				typeof(EntityLoaderConditional),
				typeof(Colosoft.Query.QueryExecutePredicate)
			});
			var entry = (EntityDynamicChildEntry)constructor.Invoke(new object[] {
				parentPropertyUidName,
				foreignPropertyName,
				conditional,
				executePredicate
			});
			Entries.Add(entry);
		}

		/// <summary>
		/// Classe usada para a configuração do filho dinâmico.
		/// </summary>
		public sealed class FluentEntityDynamicChild
		{
			private EntityDynamicChild<TEntity, TModel> _owner;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="owner"></param>
			internal FluentEntityDynamicChild(EntityDynamicChild<TEntity, TModel> owner)
			{
				_owner = owner;
			}

			/// <summary>
			/// Registra uma entrada.
			/// </summary>
			/// <typeparam name="TEntity1">Tipo da entidade associada com a entrada.</typeparam>
			/// <typeparam name="TModel1">Tipo do modelo de dados da entidade da entrada.</typeparam>
			/// <param name="parentPropertyUid">Propriedade que representa o Uid do pai.</param>
			/// <param name="foreignProperty">Propriedade que representa a chave estrangeira.</param>
			/// <param name="conditional">Condicional usado na carga.</param>
			/// <param name="executePredicate">Predicado para executar a consulta do filho.</param>
			/// <returns></returns>
			public FluentEntityDynamicChild Register<TEntity1, TModel1>(System.Linq.Expressions.Expression<Func<TModel, int>> parentPropertyUid, System.Linq.Expressions.Expression<Func<TModel1, int>> foreignProperty, EntityLoaderConditional conditional, Colosoft.Query.QueryExecutePredicate executePredicate) where TModel1 : class, Colosoft.Data.IModel where TEntity1 : IEntity<TModel1>
			{
				_owner.Entries.Add(new EntityDynamicChildEntry<TEntity, TModel, TEntity1, TModel1>(parentPropertyUid, foreignProperty, conditional, executePredicate));
				return this;
			}

			/// <summary>
			/// Registra uma entrada.
			/// </summary>
			/// <param name="entityType">Tipo da entidade associada com a entrada.</param>
			/// <param name="parentPropertyUidName">Nome da propriedade do pai que faz associação.</param>
			/// <param name="foreignPropertyName">Nome da propriedade que representa a chave estrangeira.</param>
			/// <param name="conditional">Condicional usado na carga.</param>
			/// <param name="executePredicate">Predicado para executar a consulta do filho.</param>
			/// <returns></returns>
			public FluentEntityDynamicChild Register(Type entityType, string parentPropertyUidName, string foreignPropertyName, EntityLoaderConditional conditional, Colosoft.Query.QueryExecutePredicate executePredicate)
			{
				_owner.Register(entityType, parentPropertyUidName, foreignPropertyName, conditional, executePredicate);
				return this;
			}
		}
	}
}
