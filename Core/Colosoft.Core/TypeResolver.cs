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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Classe responsável por resolver os tipos do sistema.
	/// </summary>
	public static class TypeResolver
	{
		/// <summary>
		/// Resolve o tipo para o nome informado.
		/// </summary>
		/// <param name="typeNameOrAlias">Nome ou apelido do tipo para ser resolvido.</param>
		/// <returns></returns>
		public static Type ResolveType(string typeNameOrAlias)
		{
			return ResolveType(typeNameOrAlias, true);
		}

		/// <summary>
		/// Resolve o tipo para o nome informado.
		/// </summary>
		/// <param name="typeNameOrAlias">Nome ou apelido do tipo para ser resolvido.</param>
		/// <param name="throwIfResolveFails">Se true e o tipo não for resolvido dispara um <see cref="InvalidOperationException"/>.</param>
		/// <returns></returns>
		public static Type ResolveType(string typeNameOrAlias, bool throwIfResolveFails)
		{
			typeNameOrAlias.Require("typeNameOrAlias").NotNull().NotEmpty();
			var typeName = new TypeName(typeNameOrAlias);
			return ResolveType(typeName, throwIfResolveFails);
		}

		/// <summary>
		/// Resolve o tipo para o nome informado.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será resolvido.</param>
		/// <param name="throwIfResolveFails">Se true e o tipo não for resolvido dispara um <see cref="InvalidOperationException"/>.</param>
		/// <returns></returns>
		public static Type ResolveType(TypeName typeName, bool throwIfResolveFails)
		{
			typeName.Require("typeName").NotNull();
			System.Reflection.Assembly assembly = null;
			var assemblyName = typeName.AssemblyName != null ? typeName.AssemblyName.FullName : null;
			var assemblyFile = string.Format("{0}.dll", assemblyName);
			Exception error = null;
			if(string.IsNullOrEmpty(assemblyName) || !AssemblyLoader.Instance.TryGet(assemblyFile, out assembly, out error))
			{
				if(throwIfResolveFails)
				{
					var errorMessage = string.Format("Assembly '{0}' of type '{1}' not found", assemblyName, typeName.FullName);
					if(error != null)
						throw new InvalidOperationException(errorMessage, error);
					else
						throw new InvalidOperationException(errorMessage);
				}
				return null;
			}
			return assembly.GetType(typeName.FullName, throwIfResolveFails, false);
		}
	}
}
