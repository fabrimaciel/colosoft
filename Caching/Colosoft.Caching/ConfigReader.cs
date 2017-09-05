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
using System.Collections;

namespace Colosoft.Caching.Configuration
{
	/// <summary>
	/// Representa um leitor de configuração.
	/// </summary>
	public abstract class ConfigReader
	{
		/// <summary>
		/// Tabela com as propriedades.
		/// </summary>
		public abstract Hashtable Properties
		{
			get;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected ConfigReader()
		{
		}

		/// <summary>
		/// Converte o dicionario das propriedades para string.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <returns></returns>
		public static string ToPropertiesString(IDictionary properties)
		{
			return ToPropertiesString(properties, false);
		}

		/// <summary>
		/// Converte o dicionario das propriedades para string.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <param name="formatted">True para usar os dados formatados.</param>
		/// <returns></returns>
		public static string ToPropertiesString(IDictionary properties, bool formatted)
		{
			return ConfigHelper.CreatePropertyString(properties, 0, formatted);
		}

		/// <summary>
		/// Converte o dicionario das propriedades para um XML.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <returns></returns>
		public static string ToPropertiesXml(IDictionary properties)
		{
			return ToPropertiesXml(properties, false);
		}

		/// <summary>
		/// Converte o dicionario das propriedades para um XML.
		/// </summary>
		/// <param name="properties">Lista das propriedades.</param>
		/// <param name="formatted">True para usar os dados formatados.</param>
		/// <returns></returns>
		public static string ToPropertiesXml(IDictionary properties, bool formatted)
		{
			return ConfigHelper.CreatePropertiesXml(properties, 0, formatted);
		}

		/// <summary>
		/// Converte o dicionario das propriedades para um XML.
		/// </summary>
		/// <param name="properties"></param>
		/// <param name="formatted"></param>
		/// <returns></returns>
		public static string ToPropertiesXml2(IDictionary properties, bool formatted)
		{
			return ConfigHelper.CreatePropertiesXml2(properties, 0, formatted);
		}
	}
}
