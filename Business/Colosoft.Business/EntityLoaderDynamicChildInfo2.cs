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
	/// Armazena as informações de um filho dinamico.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	/// <typeparam name="TParentEntity"></typeparam>
	class EntityLoaderDynamicChildInfo<TEntity, TParentEntity> : EntityLoaderDynamicChildInfo<TEntity>, IEntityPropertyAccessor where TEntity : IEntity where TParentEntity : IEntity
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do filho.</param>
		/// <param name="dynamicChild">Dados do filho dinamico.</param>
		/// <param name="parentProperty">Lambda da propriedade da entidade pai.</param>
		/// <param name="parentUidSetter"></param>
		/// <param name="isSingle"></param>
		/// <param name="parentLoader">Instancia do loader do pai.</param>
		/// <param name="options">Opções de carga.</param>
		public EntityLoaderDynamicChildInfo(string name, EntityDynamicChild dynamicChild, System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty, Action<IEntity, IEntity> parentUidSetter, bool isSingle, IEntityLoader parentLoader, LoadOptions options) : base(name, parentProperty.GetMember().Name, dynamicChild, f => parentProperty.Compile()((TParentEntity)f), GetParentValueSetter(parentProperty), parentUidSetter, isSingle, parentLoader, options)
		{
		}

		/// <summary>
		/// Recupera o método usado para define o valor do filho para o pai.
		/// </summary>
		/// <param name="parentProperty"></param>
		/// <returns></returns>
		private static Action<IEntity, IEntity> GetParentValueSetter(System.Linq.Expressions.Expression<Func<TParentEntity, IEntity>> parentProperty)
		{
			var property = parentProperty.GetMember() as System.Reflection.PropertyInfo;
			if(property.CanWrite)
			{
				return (parent, child) =>  {
					if(parent == null)
						throw new ArgumentNullException("parent");
					property.SetValue(parent, child, null);
				};
			}
			else
				return null;
		}

		/// <summary>
		/// Avalia se o registro contém dados associados
		/// com a lista dos filhos..
		/// </summary>
		/// <param name="record"></param>
		/// <param name="parent">Instancia do pai.</param>
		/// <returns></returns>
		public override bool Evaluate(Query.IRecord record, IEntity parent)
		{
			return false;
		}
	}
}
