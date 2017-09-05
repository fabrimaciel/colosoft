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
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// 
	/// </summary>
	public class BinaryRawBodyWriter : BodyWriter
	{
		private readonly byte[] content;

		private readonly string headerElement;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		public BinaryRawBodyWriter(byte[] content) : this(content, "Binary")
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="content"></param>
		/// <param name="headerElement"></param>
		public BinaryRawBodyWriter(byte[] content, string headerElement) : base(true)
		{
			if(headerElement == null || headerElement.Trim().Equals(string.Empty))
				throw new ArgumentException("The header element of RawBodyWriter cannot be empty or null.", "headerElement");
			if(content == null)
				throw new ArgumentNullException("content", "The body content cannot be null.");
			this.content = content;
			this.headerElement = headerElement;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="writer"></param>
		protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
		{
			writer.WriteStartElement(headerElement);
			writer.WriteBase64(content, 0, content.Length);
			writer.WriteEndElement();
		}

		/// <summary>
		/// 
		/// </summary>
		public static string DefaultRootName
		{
			get
			{
				return "Binary";
			}
		}
	}
}
