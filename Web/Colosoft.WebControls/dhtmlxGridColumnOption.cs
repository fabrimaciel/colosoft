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

namespace Colosoft.WebControls.dhtmlx
{
	/// <summary>
	/// Representa as opções que podem ser atribuidas para uma coluna da grid.
	/// </summary>
	public class dhtmlxGridColumnOption
	{
		private string _value;

		private string _text;

		/// <summary>
		/// Valor da opção.
		/// </summary>
		public string Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
			}
		}

		/// <summary>
		/// Texto da opção.
		/// </summary>
		public string Text
		{
			get
			{
				return _text;
			}
			set
			{
				_text = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="value">Valor da opção.</param>
		/// <param name="text">Texto da opção.</param>
		public dhtmlxGridColumnOption(string value, string text)
		{
			_value = value;
			_text = text;
		}

		/// <summary>
		/// Carrega os dados do elemento.
		/// </summary>
		/// <param name="doc">Documento que está sendo trabalhado.</param>
		/// <param name="parent">Elemento pai o elemento deve ser inserido.</param>
		internal void LoadElement(XmlDocument doc, XmlElement parent)
		{
			XmlElement option = doc.CreateElement("option");
			option.SetAttribute("value", _value);
			option.InnerText = _text;
			parent.AppendChild(option);
		}
	}
}
