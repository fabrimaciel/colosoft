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
using System.Web;
using System.Collections;
using System.Collections.Specialized;
using System.Runtime.Serialization;

namespace Colosoft.WebControls
{
	/// <summary>
	/// Representa uma coleção de valores HTTP.
	/// </summary>
	public class HttpValueCollection : NameValueCollection
	{
		public HttpValueCollection() : base(StringComparer.OrdinalIgnoreCase)
		{
		}

		public HttpValueCollection(int capacity) : base(capacity, (IEqualityComparer)StringComparer.OrdinalIgnoreCase)
		{
		}

		protected HttpValueCollection(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public HttpValueCollection(string str, bool readOnly, bool urlencoded, Encoding encoding) : base(StringComparer.OrdinalIgnoreCase)
		{
			if(!string.IsNullOrEmpty(str))
			{
				this.FillFromString(str, urlencoded, encoding);
			}
			base.IsReadOnly = readOnly;
		}

		public void FillFromEncodedBytes(byte[] bytes, Encoding encoding)
		{
			int num = (bytes != null) ? bytes.Length : 0;
			for(int i = 0; i < num; i++)
			{
				string str;
				string str2;
				int offset = i;
				int num4 = -1;
				while (i < num)
				{
					byte num5 = bytes[i];
					if(num5 == 0x3d)
					{
						if(num4 < 0)
						{
							num4 = i;
						}
					}
					else if(num5 == 0x26)
					{
						break;
					}
					i++;
				}
				if(num4 >= 0)
				{
					str = HttpUtility.UrlDecode(bytes, offset, num4 - offset, encoding);
					str2 = HttpUtility.UrlDecode(bytes, num4 + 1, (i - num4) - 1, encoding);
				}
				else
				{
					str = null;
					str2 = HttpUtility.UrlDecode(bytes, offset, i - offset, encoding);
				}
				base.Add(str, str2);
				if((i == (num - 1)) && (bytes[i] == 0x26))
				{
					base.Add(null, string.Empty);
				}
			}
		}

		public void FillFromString(string s)
		{
			this.FillFromString(s, false, null);
		}

		public void FillFromString(string s, bool urlencoded, Encoding encoding)
		{
			int num = (s != null) ? s.Length : 0;
			for(int i = 0; i < num; i++)
			{
				int startIndex = i;
				int num4 = -1;
				while (i < num)
				{
					char ch = s[i];
					if(ch == '=')
					{
						if(num4 < 0)
						{
							num4 = i;
						}
					}
					else if(ch == '&')
					{
						break;
					}
					i++;
				}
				string str = null;
				string str2 = null;
				if(num4 >= 0)
				{
					str = s.Substring(startIndex, num4 - startIndex);
					str2 = s.Substring(num4 + 1, (i - num4) - 1);
				}
				else
				{
					str2 = s.Substring(startIndex, i - startIndex);
				}
				if(urlencoded)
				{
					base.Add(HttpUtility.UrlDecode(str, encoding), HttpUtility.UrlDecode(str2, encoding));
				}
				else
				{
					base.Add(str, str2);
				}
				if((i == (num - 1)) && (s[i] == '&'))
				{
					base.Add(null, string.Empty);
				}
			}
		}

		public override string ToString()
		{
			return this.ToString(true);
		}

		public virtual string ToString(bool urlencoded)
		{
			return this.ToString(urlencoded, null);
		}

		public virtual string ToString(bool urlencoded, IDictionary excludeKeys)
		{
			int count = this.Count;
			if(count == 0)
			{
				return string.Empty;
			}
			StringBuilder builder = new StringBuilder();
			bool flag = (excludeKeys != null) && (excludeKeys["__VIEWSTATE"] != null);
			for(int i = 0; i < count; i++)
			{
				string key = this.GetKey(i);
				if(((!flag || (key == null)) || !key.StartsWith("__VIEWSTATE", StringComparison.Ordinal)) && (((excludeKeys == null) || (key == null)) || (excludeKeys[key] == null)))
				{
					string str3;
					if(urlencoded)
					{
						key = HttpUtility.UrlEncodeUnicode(key);
					}
					string str2 = !string.IsNullOrEmpty(key) ? (key + "=") : string.Empty;
					ArrayList list = (ArrayList)base.BaseGet(i);
					int num3 = (list != null) ? list.Count : 0;
					if(builder.Length > 0)
					{
						builder.Append('&');
					}
					if(num3 == 1)
					{
						builder.Append(str2);
						str3 = (string)list[0];
						if(urlencoded)
						{
							str3 = HttpUtility.UrlEncodeUnicode(str3);
						}
						builder.Append(str3);
					}
					else if(num3 == 0)
					{
						builder.Append(str2);
					}
					else
					{
						for(int j = 0; j < num3; j++)
						{
							if(j > 0)
							{
								builder.Append('&');
							}
							builder.Append(str2);
							str3 = (string)list[j];
							if(urlencoded)
							{
								str3 = HttpUtility.UrlEncodeUnicode(str3);
							}
							builder.Append(str3);
						}
					}
				}
			}
			return builder.ToString();
		}

		internal void MakeReadOnly()
		{
			base.IsReadOnly = true;
		}

		internal void MakeReadWrite()
		{
			base.IsReadOnly = false;
		}

		internal void Reset()
		{
			base.Clear();
		}
	}
}
