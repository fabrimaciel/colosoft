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

namespace Colosoft
{
	/// <summary>
	/// Assinatura de classe que contém definição de tipo.
	/// </summary>
	public interface ITypeDefinition
	{
		/// <summary>
		/// Nome do tipo.
		/// </summary>
		string TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Espaço de novo do tipo.
		/// </summary>
		string TypeNamespace
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do assembly do tipo.
		/// </summary>
		string TypeAssembly
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Implementação do comparador da definição de tipo.
	/// </summary>
	public class TypeDefinitionComparer : IComparer<ITypeDefinition>, IEqualityComparer<ITypeDefinition>
	{
		/// <summary>
		/// Recupera a string que representa a definição do tipo.
		/// </summary>
		/// <param name="typeDefinition"></param>
		/// <returns></returns>
		private static string GetTypeDefinitionString(ITypeDefinition typeDefinition)
		{
			if(typeDefinition == null)
				return null;
			return string.Format("{0}.{1}{2}", typeDefinition.TypeNamespace, typeDefinition.TypeName, typeDefinition.TypeAssembly != null ? ", " + typeDefinition.TypeAssembly : null);
		}

		/// <summary>
		/// Compara duas instancia da definição do tipo.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(ITypeDefinition x, ITypeDefinition y)
		{
			return string.Compare(GetTypeDefinitionString(x), GetTypeDefinitionString(y));
		}

		/// <summary>
		/// Verifica se os valores informados são iguais.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(ITypeDefinition x, ITypeDefinition y)
		{
			return string.Equals(GetTypeDefinitionString(x), GetTypeDefinitionString(y));
		}

		/// <summary>
		/// HashCode.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(ITypeDefinition obj)
		{
			if(obj == null)
				return 0;
			return GetTypeDefinitionString(obj).GetHashCode();
		}
	}
}
