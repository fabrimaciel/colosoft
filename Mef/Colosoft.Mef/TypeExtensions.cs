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

namespace Colosoft.Mef
{
	/// <summary>
	/// 
	/// </summary>
	public static class TypeExtensions
	{
		private static Type[] _knownLazyTypes;

		/// <summary>
		/// Verifica se o tipo informado é um enumerable.
		/// </summary>
		/// <param name="source">Tipo que será inspecionado.</param>
		/// <returns></returns>
		public static bool IsEnumerable(this Type source)
		{
			if(source == typeof(string))
				return false;
			return source == typeof(System.Collections.IEnumerable) || source.GetInterfaces().Contains(typeof(System.Collections.IEnumerable));
		}

		/// <summary>
		/// Verifica se o tipo é um tipo lazy conhecido.
		/// </summary>
		/// <param name="source">Tipo que será inspecionado.</param>
		/// <returns></returns>
		public static bool IsLazyType(this Type source)
		{
			if(_knownLazyTypes == null)
				_knownLazyTypes = new[] {
					typeof(Lazy<>),
					typeof(Lazy<, >)
				};
			while (!source.IsGenericType && source.BaseType != null)
				source = source.BaseType;
			if(!source.IsGenericType)
				return false;
			var genericTypeDefinition = source.GetGenericTypeDefinition();
			source = genericTypeDefinition.IsAssignableFrom(typeof(IEnumerable<>)) ? source.GetGenericArguments().FirstOrDefault().GetGenericTypeDefinition() : genericTypeDefinition;
			return _knownLazyTypes.Any(known => known.IsAssignableFrom(source));
		}

		/// <summary>
		/// Cria um tipo de objeto export com base no tipo informado.
		/// </summary>
		/// <param name="source">Tipo que será exportado.</param>
		/// <returns><see cref="Lazy{TContract,TMetadata}"/></returns>
		public static Type ToLazyType(this Type source)
		{
			if(!source.IsLazyType())
				return null;
			Type[] arguments = source.GetGenericArguments();
			Type contractType = (arguments.Length > 0) ? arguments[0] : typeof(object);
			Type metadataType = (arguments.Length > 1) ? arguments[1] : typeof(IDictionary<string, object>);
			return typeof(Lazy<, >).MakeGenericType(contractType, metadataType);
		}

		/// <summary>
		/// Cria um tipo de objeto para exporta um coleção de um tipo especifico.
		/// </summary>
		/// <param name="source">Tipo da coleção que será exportada.</param>
		/// <returns></returns>
		public static Type ToLazyCollectionType(this Type source)
		{
			if(!source.IsLazyType())
				return null;
			Type[] arguments = source.GetGenericArguments();
			Type contractType = (arguments.Length > 0) ? arguments[0] : null;
			Type metadataType = (arguments.Length > 1) ? arguments[1] : null;
			if(contractType == null)
				return typeof(List<Lazy<object>>);
			return (metadataType == null) ? typeof(List<>).MakeGenericType(typeof(Lazy<>).MakeGenericType(contractType)) : typeof(List<>).MakeGenericType(typeof(Lazy<, >).MakeGenericType(contractType, metadataType));
		}
	}
}
