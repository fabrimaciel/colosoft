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
	/// Assinatuda da lista links de uma entidade.
	/// </summary>
	public interface IEntityLinksList : IEntityList
	{
	}
	/// <summary>
	/// Assinatura da lista de links da entidade.
	/// </summary>
	/// <typeparam name="TEntity"></typeparam>
	public interface IEntityLinksList<TEntity> : IEntityList<TEntity>, IEntityLinksList where TEntity : IEntity
	{
		/// <summary>
		/// Recupera o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		new TEntity this[int index]
		{
			get;
			set;
		}

		/// <summary>
		/// Quantidade de registro na lista.
		/// </summary>
		new int Count
		{
			get;
		}

		/// <summary>
		/// Remove o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		new void RemoveAt(int index);
	}
}
