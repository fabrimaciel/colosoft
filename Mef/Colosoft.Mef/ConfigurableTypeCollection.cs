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
using System.Configuration;

namespace Colosoft.Mef
{
	/// <summary>
	/// Essa classe representa a coleção de tipos que serão exportados
	/// </summary>
	public class ConfigurableTypeCollection : System.Configuration.ConfigurationElementCollection
	{
		/// <summary>
		/// Nome do elemento no xml
		/// </summary>
		protected override string ElementName
		{
			get
			{
				return "part";
			}
		}

		/// <summary>
		/// Tipo da coleção
		/// </summary>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMap;
			}
		}

		/// <summary>
		/// Retorna a instância de um novo elemento
		/// </summary>
		/// <returns></returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new ConfigurableTypeElement();
		}

		/// <summary>
		/// Retorna a chave de identificação do elemento
		/// </summary>
		/// <param name="element"></param>
		/// <returns></returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			var part = element as ConfigurableTypeElement;
			if(part != null)
			{
				return part.TypeExport;
			}
			throw new InvalidOperationException();
		}
	}
}
