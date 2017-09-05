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
	/// Assinatura do provedor de tipos de filhos dinamicos.
	/// </summary>
	public interface IEntityDynamicChildTypesProvider
	{
		/// <summary>
		/// Recupera a relação de todos os filhos dinâmicos.
		/// </summary>
		/// <returns></returns>
		IEnumerable<Colosoft.Reflection.TypeName> GetAllTypes();

		/// <summary>
		/// Recupera o tipo da entidade do filho dinamico.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		Type GetType(Colosoft.Reflection.TypeName typeName);
	}
	/// <summary>
	/// Assinatura de um modelo de dados que armazena os tipos de um filho dinâmico.
	/// </summary>
	public interface IEntityDynamicChildTypesModel : Colosoft.Data.IModel
	{
		/// <summary>
		/// Identificador do tipo da entidade de negócio.
		/// </summary>
		int BusinessEntityTypeId
		{
			get;
		}
	}
	/// <summary>
	/// Assinatura da classe provedora dos 
	/// </summary>
	public interface IEntityDynamicChildTypesLoader
	{
		/// <summary>
		/// Carrega os tipos dos filhos dinâmicos com base no tipo do modelo de dados informado.
		/// </summary>
		/// <param name="dataModelType">Tipo do modelo de dados que contém as informações dos tipos de filhos dinâmicos.</param>
		/// <returns></returns>
		IEnumerable<Colosoft.Reflection.TypeName> Load(Type dataModelType);
	}
	/// <summary>
	/// Implementação base para o provedor dos tipos dos filhos dinâmicos.
	/// </summary>
	/// <typeparam name="TModel"></typeparam>
	public class EntityDynamicChildTypesProvider<TModel> : IEntityDynamicChildTypesProvider where TModel : IEntityDynamicChildTypesModel, new()
	{
		private List<Colosoft.Reflection.TypeName> _types;

		/// <summary>
		/// Recupera a relação de todos os filhos dinâmicos.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<Colosoft.Reflection.TypeName> GetAllTypes()
		{
			if(_types == null)
			{
				ServiceLocatorValidator.Validate();
				var loader = Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance<IEntityDynamicChildTypesLoader>();
				_types = loader.Load(typeof(TModel)).ToList();
			}
			return _types;
		}

		/// <summary>
		/// Recupera o tipo da entidade do filho dinamico.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public virtual Type GetType(Colosoft.Reflection.TypeName typeName)
		{
			return Colosoft.Reflection.TypeResolver.ResolveType(typeName, true);
		}
	}
}
