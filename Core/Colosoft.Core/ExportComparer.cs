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

namespace Colosoft.Reflection.Composition
{
	/// <summary>
	/// Implementação usada para comparar se dois IExport são equivalentes
	/// </summary>
	public class ExportComparer : IEqualityComparer<IExport>, IComparer<IExport>
	{
		/// <summary>
		/// Instancia única do comparador.
		/// </summary>
		public readonly static ExportComparer Instance = new ExportComparer();

		/// <summary>
		/// Recupera o texto que representa o IExport.
		/// </summary>
		/// <param name="export"></param>
		/// <returns></returns>
		public string ToString(IExport export)
		{
			return string.Format("[{0}, {1}]", export.ContractType != null ? export.ContractType.FullName : null, export.ContractName);
		}

		/// <summary>
		/// Verifica se as instancia informadas são compatíveis.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(Reflection.Composition.IExport x, Reflection.Composition.IExport y)
		{
			return ((x == null && y == null) || (x != null && y != null && x.ContractName == y.ContractName && TypeName.TypeNameEqualityComparer.Instance.Equals(x.ContractType, y.ContractType)));
		}

		/// <summary>
		/// HashCode.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(Reflection.Composition.IExport obj)
		{
			if(obj == null)
				return 0;
			return string.Format("[{0} : {1}]", obj.ContractType, obj.ContractName).GetHashCode();
		}

		/// <summary>
		/// Compara as instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(IExport x, IExport y)
		{
			if(Equals(x, y))
				return 0;
			return StringComparer.Ordinal.Compare(ToString(x), ToString(y));
		}
	}
}
