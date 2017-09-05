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
using System.Collections;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Storage for parsed HTML data returned by ParsedHtmlData();
	/// </summary>
	/// <remarks>
	/// We use this object to pass around information about HTML documents that
	/// are processed above. It encapsulates *some* functionality (Content-Type parsing
	/// and Robots directives) and will probably have more functions added to
	/// it over time.
	/// Potentially other classes might be added (PdfDocument) in which case it
	/// might make sense to have an IDocument interface or base class (and maybe
	/// a Factory to create them) to help the parser deal with different document
	/// types.
	/// </remarks>
	public class HtmlDocument
	{
		public HtmlDocument(Uri location)
		{
			m_Uri = location;
			Url = location.AbsoluteUri.ToString();
			LocalLinks = null;
			ExternalLinks = null;
		}

		private Uri m_Uri;

		private String m_contentType;

		/// <summary>http://www.ietf.org/rfc/rfc2396.txt</summary>
		public Uri Uri
		{
			get
			{
				return m_Uri;
			}
			set
			{
				m_Uri = value;
				Url = value.AbsoluteUri.ToString();
			}
		}

		/// <summary>Raw content of page, as downloaded from the server</summary>
		public string All = String.Empty;

		/// <summary>Encoding eg. "utf-8", "Shift_JIS", "iso-8859-1", "gb2312", etc</summary>
		public string Encoding = String.Empty;

		/// <summary>MimeType so we know whether to try and parse the contents, eg. "text/html", "text/plain", etc</summary>
		public string MimeType = String.Empty;

		public bool RobotIndexOK = true;

		public bool RobotFollowOK = true;

		public String ContentType
		{
			get
			{
				return m_contentType;
			}
			set
			{
				m_contentType = value.ToString();
				string[] contentTypeArray = m_contentType.Split(';');
				if(MimeType == String.Empty && contentTypeArray.Length >= 1)
					MimeType = contentTypeArray[0];
				if(Encoding == String.Empty && contentTypeArray.Length >= 2)
				{
					int charsetpos = contentTypeArray[1].IndexOf("charset");
					if(charsetpos > 0)
					{
						Encoding = contentTypeArray[1].Substring(charsetpos + 8, contentTypeArray[1].Length - charsetpos - 8);
					}
				}
			}
		}

		/// <summary>Sort of obsolete with the Uri field being the main data to use</summary>
		public String Url;

		/// <summary>Html &lt;title&gt; tag</summary>
		public String Title = String.Empty;

		/// <summary>Html &lt;meta http-equiv='description'&gt; tag</summary>
		public String Description = String.Empty;

		/// <summary>Html &lt;meta http-equiv='keywords'&gt; tag</summary>
		public String Keywords = String.Empty;

		/// <summary>Length as reported by the server in the Http headers</summary>
		public Int64 Length;

		public String Html;

		public ArrayList LocalLinks;

		public ArrayList ExternalLinks;

		/// <summary>
		/// Robots Exclusion Protocol
		/// http://www.robotstxt.org/wc/meta-user.html
		/// </summary>
		public void SetRobotDirective(string robotMetaContent)
		{
			robotMetaContent = robotMetaContent.ToLower();
			if(robotMetaContent.IndexOf("none") >= 0)
			{
				RobotIndexOK = false;
				RobotFollowOK = false;
			}
			else
			{
				if(robotMetaContent.IndexOf("noindex") >= 0)
				{
					RobotIndexOK = false;
				}
				if(robotMetaContent.IndexOf("nofollow") >= 0)
				{
					RobotFollowOK = false;
				}
			}
		}

		/// <summary>for debugging</summary>
		public override string ToString()
		{
			string linkstring = "";
			foreach (object link in LocalLinks)
			{
				linkstring += Convert.ToString(link) + "<br>";
			}
			return Title + " " + Description + " " + linkstring + "<hr>" + Html;
		}
	}
}
