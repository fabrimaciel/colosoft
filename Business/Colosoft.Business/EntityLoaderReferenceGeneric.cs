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
	/// Implementação genérica para as informações da referencia da entidade.
	/// </summary>
	/// <typeparam name="TParentEntity"></typeparam>
	/// <typeparam name="TParentModel"></typeparam>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TModel"></typeparam>
	class EntityLoaderReference<TParentEntity, TParentModel, TEntity, TModel> : EntityLoaderReference, IEntityPropertyAccessor where TParentModel : class, Colosoft.Data.IModel where TParentEntity : IEntity<TParentModel> where TEntity : IEntity<TModel> where TModel : class, Colosoft.Data.IModel
	{
		private Func<IEntity, IEntity> _parentValueGetter;

		/// <summary>
		/// Tipo do modelo de dados.
		/// </summary>
		public override Type DataModelType
		{
			get
			{
				return typeof(TModel);
			}
		}

		/// <summary>
		/// Tipo da entidade.
		/// </summary>
		public override Type EntityType
		{
			get
			{
				return typeof(TEntity);
			}
		}

		/// <summary>
		/// Instancia responsável por recupera o valor pai.
		/// </summary>
		public override Func<IEntity, IEntity> ParentValueGetter
		{
			get
			{
				return _parentValueGetter;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome da referencia.</param>
		/// <param name="parentProperty">Propriedade do pai onde a referencia está.</param>
		/// <param name="conditional">Condicional para carregar a referencia.</param>
		/// <param name="isLoadLazy">Identifica se a referência terá carga tardia.</param>
		/// <param name="options">Opções de carga da referência.</param>
		/// <param name="savePriority"></param>
		/// <param name="watchedProperties">Propriedade que serão monitoradas.</param>
		public EntityLoaderReference(string name, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, ConditionalLoader conditional, bool isLoadLazy, LoadOptions options, EntityChildSavePriority savePriority, System.Linq.Expressions.Expression<Func<TParentModel, object>>[] watchedProperties) : base(name, GetParentPropertyName(parentProperty), conditional, isLoadLazy, options, savePriority, GetwatchedPropertyNames(watchedProperties))
		{
			_parentValueGetter = f => parentProperty.Compile()((TParentEntity)f);
		}

		/// <summary>
		/// Recupera o nome da propriedade da referencia no pai.
		/// </summary>
		/// <param name="parentProperty"></param>
		/// <returns></returns>
		private static string GetParentPropertyName(System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty)
		{
			parentProperty.Require("parentProperty").NotNull();
			return parentProperty.GetMember().Name;
		}

		/// <summary>
		/// Recupera os nomes das propriedades informadas.
		/// </summary>
		/// <param name="watchedProperties"></param>
		/// <returns></returns>
		private static string[] GetwatchedPropertyNames(System.Linq.Expressions.Expression<Func<TParentModel, object>>[] watchedProperties)
		{
			watchedProperties.Require("watchedProperties").NotNull();
			var result = new string[watchedProperties.Length];
			for(var i = 0; i < watchedProperties.Length; i++)
				result[i] = watchedProperties[i].GetMember().Name;
			return result;
		}

		/// <summary>
		/// Recupera o valor do item do pai.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		public IEntity Get(IEntity parent)
		{
			return ParentValueGetter(parent);
		}

		/// <summary>
		/// Propriedade onde o valor da referencia é armazenado.
		/// </summary>
		string IEntityPropertyAccessor.PropertyName
		{
			get
			{
				return ParentPropertyName;
			}
		}
	}
}
