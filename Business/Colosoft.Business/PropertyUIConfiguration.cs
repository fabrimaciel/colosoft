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
	/// Armazena as configuração da propriedade no contexto da UI.
	/// </summary>
	public class PropertyUIConfiguration
	{
		private string _uiContext;

		private string _controlType;

		private string _style;

		private PropertyUIConfigurationParameter[] _values;

		/// <summary>
		/// Nome do contexto.
		/// </summary>
		public string UIContext
		{
			get
			{
				return _uiContext;
			}
		}

		/// <summary>
		/// Tipo do controle.
		/// </summary>
		public string ControlType
		{
			get
			{
				return _controlType;
			}
		}

		/// <summary>
		/// Nome do estilo associado.
		/// </summary>
		public string Style
		{
			get
			{
				return _style;
			}
		}

		/// <summary>
		/// Valores da configuração.
		/// </summary>
		public PropertyUIConfigurationParameter[] Values
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="uiContext"></param>
		/// <param name="controlType"></param>
		/// <param name="style"></param>
		/// <param name="values"></param>
		public PropertyUIConfiguration(string uiContext, string controlType, string style, PropertyUIConfigurationParameter[] values)
		{
			_uiContext = uiContext;
			_controlType = controlType;
			_style = style;
			_values = values ?? new PropertyUIConfigurationParameter[0];
		}
	}
	/// <summary>
	/// Armazena os dados de um parametro.
	/// </summary>
	public class PropertyUIConfigurationParameter
	{
		private string _text;

		private string _value;

		/// <summary>
		/// Texto do parametro.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
		}

		/// <summary>
		/// Valor do parametro.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="value"></param>
		public PropertyUIConfigurationParameter(string text, string value)
		{
			_text = text;
			_value = value;
		}
	}
}
