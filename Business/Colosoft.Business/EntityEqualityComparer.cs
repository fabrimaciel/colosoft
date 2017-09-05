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
	/// Implementaçao de um comparador de igualdade.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EntityEqualityComparer<T> : IEqualityComparer<T>, System.Collections.IEqualityComparer where T : IEntity
	{
		/// <summary>
		/// Instancia geral do sistema.
		/// </summary>
		public static readonly EntityEqualityComparer<T> Instance = new EntityEqualityComparer<T>();

		/// <summary>
		/// Compara duas entidades.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(T x, T y)
		{
			if(!object.ReferenceEquals(x, null))
				return x.Equals(y);
			return false;
		}

		/// <summary>
		/// Recupera o hash code da entidade.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(T obj)
		{
			if(obj != null)
			{
				if(obj.HasUid)
					return obj.Uid;
				return obj.GetHashCode();
			}
			return 0;
		}

		/// <summary>
		/// Compara as instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		bool System.Collections.IEqualityComparer.Equals(object x, object y)
		{
			return Equals(x is T ? (T)x : default(T), y is T ? (T)y : default(T));
		}

		/// <summary>
		/// Recupera o código Hash.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		int System.Collections.IEqualityComparer.GetHashCode(object obj)
		{
			return GetHashCode(obj is T ? (T)obj : default(T));
		}
	}
}
