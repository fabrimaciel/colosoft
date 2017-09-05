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
	/// Implementação do comparador de entidades pelo Uid.
	/// </summary>
	public class EntityUidComparer : IComparer<IEntity>, System.Collections.IComparer
	{
		private static readonly EntityUidComparer _instance = new EntityUidComparer();

		/// <summary>
		/// Instancia unica do comparador.
		/// </summary>
		public static EntityUidComparer Instance
		{
			get
			{
				return _instance;
			}
		}

		/// <summary>
		/// Compara as instancias.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(IEntity x, IEntity y)
		{
			if(!object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null))
				return x.Uid < y.Uid ? -1 : x.Uid == y.Uid ? 0 : 1;
			return object.ReferenceEquals(x, null) && object.ReferenceEquals(y, null) ? 0 : object.ReferenceEquals(x, null) && !object.ReferenceEquals(y, null) ? -1 : 1;
		}

		/// <summary>
		/// Compara as instancia informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		int System.Collections.IComparer.Compare(object x, object y)
		{
			return Compare(x as IEntity, y as IEntity);
		}
	}
}
