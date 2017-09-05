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

using System.Xml;
using Colosoft.Excel.ExcelXml.Extensions;

namespace Colosoft.Excel.ExcelXml
{
	/// <summary>
	/// Gets or sets document properties
	/// </summary>
	public class DocumentProperties
	{
		/// <summary>
		/// Gets or sets the author of the workbook
		/// </summary>
		public string Author
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the last author of the workbook
		/// </summary>
		public string LastAuthor
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the manager of the workbook
		/// </summary>
		public string Manager
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the company of the workbook
		/// </summary>
		public string Company
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the subject of the workbook
		/// </summary>
		public string Subject
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the title of the workbook
		/// </summary>
		public string Title
		{
			get;
			set;
		}

		/// <summary>
		/// Creates an instance with empty document properties
		/// </summary>
		public DocumentProperties()
		{
			Author = "";
			LastAuthor = "";
			Manager = "";
			Company = "";
			Subject = "";
			Title = "";
		}

		internal void Export(XmlWriter writer)
		{
			writer.WriteStartElement("", "DocumentProperties", "urn:schemas-microsoft-com:office:office");
			if(!Author.IsNullOrEmpty())
				writer.WriteElementString("Author", Author);
			if(!LastAuthor.IsNullOrEmpty())
				writer.WriteElementString("LastAuthor", LastAuthor);
			if(!Manager.IsNullOrEmpty())
				writer.WriteElementString("Manager", Manager);
			if(!Company.IsNullOrEmpty())
				writer.WriteElementString("Company", Company);
			if(!Subject.IsNullOrEmpty())
				writer.WriteElementString("Subject", Subject);
			if(!Title.IsNullOrEmpty())
				writer.WriteElementString("Title", Title);
			writer.WriteEndElement();
		}

		internal void Import(XmlReader reader)
		{
			while (reader.Read() && !(reader.Name == "DocumentProperties" && reader.NodeType == XmlNodeType.EndElement))
			{
				if(reader.NodeType == XmlNodeType.Element)
				{
					switch(reader.Name)
					{
					case "Author":
					{
						if(reader.IsEmptyElement)
							continue;
						reader.Read();
						if(reader.NodeType == XmlNodeType.Text)
							Author = reader.Value;
						break;
					}
					case "LastAuthor":
					{
						if(reader.IsEmptyElement)
							continue;
						reader.Read();
						if(reader.NodeType == XmlNodeType.Text)
							LastAuthor = reader.Value;
						break;
					}
					case "Manager":
					{
						if(reader.IsEmptyElement)
							continue;
						reader.Read();
						if(reader.NodeType == XmlNodeType.Text)
							Manager = reader.Value;
						break;
					}
					case "Company":
					{
						if(reader.IsEmptyElement)
							continue;
						reader.Read();
						if(reader.NodeType == XmlNodeType.Text)
							Company = reader.Value;
						break;
					}
					case "Subject":
					{
						if(reader.IsEmptyElement)
							continue;
						reader.Read();
						if(reader.NodeType == XmlNodeType.Text)
							Subject = reader.Value;
						break;
					}
					case "Title":
					{
						if(reader.IsEmptyElement)
							continue;
						reader.Read();
						if(reader.NodeType == XmlNodeType.Text)
							Title = reader.Value;
						break;
					}
					}
				}
			}
		}
	}
}
