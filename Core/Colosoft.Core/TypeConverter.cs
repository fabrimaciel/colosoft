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
using System.Text;
using System.Xml;

namespace Colosoft.Reflection
{
	/// <summary>
	/// Classe com método que auxiliam na conversão de tipos.
	/// </summary>
	public class TypeConverter
	{
		/// <summary>
		/// Converto o XmlNode submetido em um tipo especifico. Usa-se o InnerXml do XmlNode para re
		/// cuperar o valor usado na conversão.
		/// </summary>
		/// <param name="targetType">Tipo de destino da conversão</param>
		/// <param name="node">XmlNode fonte dos dados para a conversão.</param>
		/// <returns>Valor convertido.</returns>
		public static object Get(Type targetType, XmlNode node)
		{
			if(targetType == typeof(XmlNode))
				return node;
			else
				return Get(targetType, node.InnerXml);
		}

		/// <summary>
		/// Convert a string submetida em um tipo especifico.
		/// </summary>
		/// <param name="targetType">Tipo de destino da conversão.</param>
		/// <param name="value">Fonte usada na conversão</param>
		/// <returns>Valor convertido.</returns>
		public static object Get(Type targetType, string value)
		{
			return Get(targetType, value, System.Globalization.CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Convert a string submetida em um tipo especifico.
		/// </summary>
		/// <param name="targetType">Tipo de destino da conversão.</param>
		/// <param name="value">Fonte usada na conversão</param>
		/// <param name="culture">Cultura que será usada na conversão.</param>
		/// <returns>Valor convertido.</returns>
		public static object Get(Type targetType, string value, System.Globalization.CultureInfo culture)
		{
			if(targetType.IsEnum)
			{
				try
				{
					return Enum.Parse(targetType, value, true);
				}
				catch
				{
					return Enum.ToObject(targetType, Convert.ToInt32(value));
				}
			}
			if(null != value)
			{
				return Convert.ChangeType(value, targetType, culture);
			}
			return null;
		}

		/// <summary>
		/// Convert o objeto para o tipo especificado.
		/// </summary>
		/// <param name="targetType">Tipo de destino da conversão.</param>
		/// <param name="obj">Objeto fonte para conversão.</param>
		/// <returns>Valor convertido.</returns>
		public static object Get(Type targetType, object obj)
		{
			return Get(targetType, obj, System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Convert o objeto para o tipo especificado.
		/// </summary>
		/// <param name="targetType">Tipo de destino da conversão.</param>
		/// <param name="obj">Objeto fonte para conversão.</param>
		/// <param name="culture">Cultura que será usada na conversão.</param>
		/// <returns>Valor convertido.</returns>
		public static object Get(Type targetType, object obj, System.Globalization.CultureInfo culture)
		{
			if(targetType.IsEnum)
			{
				try
				{
					return Enum.Parse(targetType, obj.ToString(), true);
				}
				catch
				{
					return Enum.ToObject(targetType, Convert.ToInt32(obj));
				}
			}
			if(null != obj)
			{
				return Convert.ChangeType(obj, targetType, culture);
			}
			return null;
		}

		/// <summary>
		/// Verifica se o tipo aceita valores nulo
		/// </summary>
		/// <param name="type">Tipo a ser verificado.</param>
		/// <returns>True if null can be assigned, false otherwise.</returns>
		public static bool IsNullAssignable(Type type)
		{
			return !type.IsValueType;
		}
	}
}
