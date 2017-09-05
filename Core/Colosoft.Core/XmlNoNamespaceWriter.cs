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
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Colosoft.Serialization
{
	/// <summary>
	/// Modified XML writer that writes (almost) no namespaces out with pretty formatting
	/// http://blogs.msdn.com/b/kaevans/archive/2004/08/02/206432.aspx
	/// </summary>
	public class XmlNoNamespaceWriter : XmlTextWriter
	{
		private bool _SkipAttribute = false;

		private int _EncounteredNamespaceCount = 0;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="writer"></param>
		public XmlNoNamespaceWriter(TextWriter writer) : base(writer)
		{
			this.Formatting = System.Xml.Formatting.Indented;
		}

		/// <summary>
		/// Escreve o inicio do elemento.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="localName"></param>
		/// <param name="ns"></param>
		public override void WriteStartElement(string prefix, string localName, string ns)
		{
			base.WriteStartElement(null, localName, null);
		}

		/// <summary>
		/// Escreve o início do atributo;.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="localName"></param>
		/// <param name="ns"></param>
		public override void WriteStartAttribute(string prefix, string localName, string ns)
		{
			if((prefix.CompareTo("xmlns") == 0 || localName.CompareTo("xmlns") == 0) && _EncounteredNamespaceCount++ > 0)
			{
				_SkipAttribute = true;
			}
			else
			{
				base.WriteStartAttribute(null, localName, null);
			}
		}

		/// <summary>
		/// Escreve o texto.
		/// </summary>
		/// <param name="text"></param>
		public override void WriteString(string text)
		{
			if(!_SkipAttribute)
			{
				base.WriteString(text);
			}
		}

		/// <summary>
		/// Escreve o fin do atributo.
		/// </summary>
		public override void WriteEndAttribute()
		{
			if(!_SkipAttribute)
			{
				base.WriteEndAttribute();
			}
			_SkipAttribute = false;
		}

		/// <summary>
		/// Escreve o nome qualificado.
		/// </summary>
		/// <param name="localName"></param>
		/// <param name="ns"></param>
		public override void WriteQualifiedName(string localName, string ns)
		{
			base.WriteQualifiedName(localName, null);
		}
	}
}
