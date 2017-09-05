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
using System.Xml.Linq;

namespace Colosoft.Net
{
	/// <summary>
	/// Implementação do Writer para o corpo da mensagem de error no formato JSON.
	/// </summary>
	class JsonErrorBodyWriter : System.ServiceModel.Channels.BodyWriter
	{
		private Encoding _utf8Encoding = new UTF8Encoding(false);

		private XDocument _exMsg;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="exMsg"></param>
		public JsonErrorBodyWriter(XDocument exMsg) : base(true)
		{
			_exMsg = exMsg;
		}

		/// <summary>
		/// Método acionad quando for escreve o contúdo do corpo.
		/// </summary>
		/// <param name="writer"></param>
		protected override void OnWriteBodyContents(System.Xml.XmlDictionaryWriter writer)
		{
			writer.WriteStartElement("root");
			writer.WriteAttributeString("type", "object");
			XElement root = _exMsg.Root;
			foreach (XElement el in root.Descendants())
			{
				writer.WriteStartElement(el.Name.ToString());
				writer.WriteAttributeString("type", "string");
				writer.WriteString(el.Value.ToString());
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}
	}
}
