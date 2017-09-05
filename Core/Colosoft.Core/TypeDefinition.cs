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
	/// Implementação básica da interface <see cref="ITypeDefinition"/>.
	/// </summary>
	public class TypeDefinition : ITypeDefinition
	{
		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Espaço de novo do tipo.
		/// </summary>
		public string TypeNamespace
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do assembly do tipo.
		/// </summary>
		public string TypeAssembly
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TypeDefinition()
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo os valores iniciais para a instancia.
		/// </summary>
		/// <param name="typeNamespace">Espaço de nome do tipo.</param>
		/// <param name="typeName">Nome do tipo.</param>
		/// <param name="typeAssembly">Assembly do tipo.</param>
		public TypeDefinition(string typeNamespace, string typeName, string typeAssembly)
		{
			TypeNamespace = typeNamespace;
			TypeName = typeName;
			TypeAssembly = typeAssembly;
		}

		/// <summary>
		/// Recupera a definição do tipo informado por T.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static TypeDefinition Get<T>()
		{
			var type = typeof(T);
			return new TypeDefinition(type.Namespace, type.Name, type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(",")));
		}

		/// <summary>
		/// Recupera a definição do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypeDefinition Get(Type type)
		{
			type.Require("type").NotNull();
			return new TypeDefinition(type.Namespace, type.Name, type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(",")));
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0}.{1}{2}", TypeNamespace, TypeName, TypeAssembly != null ? ", " + TypeAssembly : null);
		}
	}
}
