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
using System.Xml;

namespace Colosoft.Serialization
{
	/// <summary>
	/// Classe que auxilia na manipulação de Xml.
	/// </summary>
	public static class XmlHelper
	{
		private static byte[] _zeroLengthArrayOfByte;

		[ThreadStatic]
		private static string[] ts_stringList;

		/// <summary>
		/// Representa um vetor vazio.
		/// </summary>
		public static byte[] ZeroLengthArrayOfByte
		{
			get
			{
				if(_zeroLengthArrayOfByte == null)
					_zeroLengthArrayOfByte = new byte[0];
				return _zeroLengthArrayOfByte;
			}
		}

		/// <summary>
		/// Adiciona um atributo para o nó informado.
		/// </summary>
		/// <param name="node">Instancia do nó onde o atributo será inserido.</param>
		/// <param name="attrName">Nome do atributo.</param>
		/// <param name="value">Valor do atributo.</param>
		public static void AddXmlAttribute(XmlNode node, string attrName, string value)
		{
			if(value != null)
			{
				XmlAttribute attribute = node.OwnerDocument.CreateAttribute(null, attrName, null);
				node.Attributes.Append(attribute);
				attribute.InnerText = value;
			}
		}

		/// <summary>
		/// Recupera um vetor de bytes do XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static byte[] ArrayOfByteFromXml(XmlReader reader)
		{
			string s = StringFromXmlElement(reader);
			if(s != null)
				return Convert.FromBase64String(s);
			return ZeroLengthArrayOfByte;
		}

		/// <summary>
		/// Recupera um vetor de bytes do atributo do XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static byte[] ArrayOfByteFromXmlAttribute(XmlReader reader)
		{
			if(reader.Value.Length != 0)
				return Convert.FromBase64String(reader.Value);
			return ZeroLengthArrayOfByte;
		}

		/// <summary>
		/// Recupera um vetor de objeto do XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static object[] ArrayOfObjectFromXml(XmlReader reader)
		{
			List<object> list = new List<object>();
			bool isEmptyElement = reader.IsEmptyElement;
			reader.Read();
			if(!isEmptyElement)
			{
				while (reader.NodeType == XmlNodeType.Element)
				{
					if(reader.HasAttributes && (reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true"))
					{
						list.Add(null);
						reader.Read();
					}
					else
						list.Add(ObjectFromXmlElement(reader));
				}
				reader.ReadEndElement();
			}
			return list.ToArray();
		}

		/// <summary>
		/// Recupera um enum do xml.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object EnumFromXmlAttribute(XmlReader reader, Type type)
		{
			string str = StringFromXmlAttribute(reader).Replace(' ', ',');
			return Enum.Parse(type, str, true);
		}

		/// <summary>
		/// Recupera o enum do elemento xml.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object EnumFromXmlElement(XmlReader reader, Type type)
		{
			string str = StringFromXmlElement(reader).Replace(' ', ',');
			return Enum.Parse(type, str, true);
		}

		/// <summary>
		/// Define o valor do enum para o atributo XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attr"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		public static void EnumToXmlAttribute(XmlWriter writer, string attr, object value, Type type)
		{
			string str = Enum.Format(type, value, "G").Replace(",", "");
			writer.WriteAttributeString(attr, str);
		}

		/// <summary>
		/// Define o valor do enum para o elemento XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="value"></param>
		/// <param name="type"></param>
		public static void EnumToXmlElement(XmlWriter writer, string element, object value, Type type)
		{
			string str = Enum.Format(type, value, "G").Replace(",", "");
			writer.WriteElementString(element, str);
		}

		/// <summary>
		/// Recupera a string armazenada no XML.
		/// </summary>
		/// <param name="fromXml"></param>
		/// <returns></returns>
		private static string GetCachedString(string fromXml)
		{
			if(fromXml == null)
			{
				return null;
			}
			int length = fromXml.Length;
			if(length <= 0x100)
			{
				if(length == 0)
				{
					return string.Empty;
				}
				string[] strArray = ts_stringList;
				if(strArray == null)
				{
					strArray = new string[0x10];
					ts_stringList = strArray;
				}
				for(int i = 0; i < 0x10; i++)
				{
					string b = strArray[i];
					if(b == null)
					{
						break;
					}
					if((((b.Length == length) && (fromXml[0] == b[0])) && ((length <= 5) || (fromXml[length - 5] == b[length - 5]))) && string.Equals(fromXml, b, StringComparison.Ordinal))
					{
						for(int k = i - 1; k >= 0; k--)
						{
							strArray[k + 1] = strArray[k];
						}
						strArray[0] = b;
						return b;
					}
				}
				for(int j = 14; j >= 0; j--)
				{
					strArray[j + 1] = strArray[j];
				}
				strArray[0] = fromXml;
			}
			return fromXml;
		}

		/// <summary>
		/// Recupera um objeto do elemento XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static object ObjectFromXmlElement(XmlReader reader)
		{
			string attribute = reader.GetAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
			if(!string.IsNullOrEmpty(attribute))
			{
				string[] strArray = attribute.Split(new char[] {
					':'
				}, StringSplitOptions.None);
				if(strArray.Length == 2)
				{
					attribute = strArray[1];
				}
				switch(attribute)
				{
				case "base64Binary":
				{
					string s = StringFromXmlElement(reader);
					if(s == null)
					{
						return ZeroLengthArrayOfByte;
					}
					return Convert.FromBase64String(s);
				}
				case "boolean":
					return XmlConvert.ToBoolean(StringFromXmlElement(reader));
				case "char":
					return XmlConvert.ToChar(StringFromXmlElement(reader));
				case "dateTime":
					return ToDateTime(StringFromXmlElement(reader));
				case "decimal":
					return XmlConvert.ToDecimal(StringFromXmlElement(reader));
				case "double":
					return XmlConvert.ToDouble(StringFromXmlElement(reader));
				case "float":
					return XmlConvert.ToSingle(StringFromXmlElement(reader));
				case "int":
					return XmlConvert.ToInt32(StringFromXmlElement(reader));
				case "guid":
					return XmlConvert.ToGuid(StringFromXmlElement(reader));
				case "long":
					return XmlConvert.ToInt64(StringFromXmlElement(reader));
				case "short":
					return XmlConvert.ToInt16(StringFromXmlElement(reader));
				case "string":
					return StringFromXmlElement(reader);
				case "unsignedByte":
					return XmlConvert.ToByte(StringFromXmlElement(reader));
				case "ArrayOfAnyType":
					return ArrayOfObjectFromXml(reader);
				}
				reader.ReadOuterXml();
			}
			else if(reader.GetAttribute("nil", "http://www.w3.org/2001/XMLSchema-instance") == "true")
			{
				reader.ReadInnerXml();
				return null;
			}
			return null;
		}

		/// <summary>
		/// Define um objeto para o elemento XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="o"></param>
		public static void ObjectToXmlElement(XmlWriter writer, string element, object o)
		{
			if(o == null)
			{
				writer.WriteStartElement(element);
				writer.WriteAttributeString("nil", "http://www.w3.org/2001/XMLSchema-instance", "true");
				writer.WriteEndElement();
			}
			else
			{
				string fullName = o.GetType().FullName;
				string localName = null;
				string str3 = null;
				string ns = null;
				switch(fullName)
				{
				case "System.Boolean":
					localName = "boolean";
					str3 = XmlConvert.ToString((bool)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Byte":
					localName = "unsignedByte";
					str3 = XmlConvert.ToString((byte)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Byte[]":
				{
					localName = "base64Binary";
					byte[] inArray = (byte[])o;
					str3 = Convert.ToBase64String(inArray, 0, inArray.Length, Base64FormattingOptions.None);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				}
				case "System.Char":
					localName = "char";
					str3 = XmlConvert.ToString((char)o);
					ns = "http://microsoft.com/wsdl/types/";
					break;
				case "System.DateTime":
					localName = "dateTime";
					str3 = ToString((DateTime)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Decimal":
					localName = "decimal";
					str3 = XmlConvert.ToString((decimal)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Double":
					localName = "double";
					str3 = XmlConvert.ToString((double)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Guid":
					localName = "guid";
					str3 = XmlConvert.ToString((Guid)o);
					ns = "http://microsoft.com/wsdl/types/";
					break;
				case "System.Int16":
					localName = "short";
					str3 = XmlConvert.ToString((short)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Int32":
					localName = "int";
					str3 = XmlConvert.ToString((int)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Int64":
					localName = "long";
					str3 = XmlConvert.ToString((long)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.Single":
					localName = "float";
					str3 = XmlConvert.ToString((float)o);
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				case "System.String":
					localName = "string";
					str3 = (string)o;
					ns = "http://www.w3.org/2001/XMLSchema";
					break;
				default:
					if(o.GetType().IsArray)
					{
						writer.WriteStartElement(element);
						writer.WriteAttributeString("type", "http://www.w3.org/2001/XMLSchema-instance", "ArrayOfAnyType");
						ToXml(writer, null, (object[])o);
						writer.WriteEndElement();
					}
					return;
				}
				writer.WriteStartElement(element);
				writer.WriteStartAttribute("type", "http://www.w3.org/2001/XMLSchema-instance");
				writer.WriteQualifiedName(localName, ns);
				writer.WriteEndAttribute();
				writer.WriteValue(str3);
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Recupera uma string do atributo XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static string StringFromXmlAttribute(XmlReader reader)
		{
			return GetCachedString(reader.Value);
		}

		/// <summary>
		/// Recupera uma string do elemento XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static string StringFromXmlElement(XmlReader reader)
		{
			string cachedString = string.Empty;
			bool isEmptyElement = reader.IsEmptyElement;
			reader.Read();
			if(!isEmptyElement)
			{
				if((reader.NodeType == XmlNodeType.Text) || (reader.NodeType == XmlNodeType.CDATA))
				{
					cachedString = GetCachedString(reader.Value.Replace("\n", "\r\n"));
					reader.Read();
				}
				reader.ReadEndElement();
			}
			return cachedString;
		}

		/// <summary>
		/// Define a string para o atributo xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attr"></param>
		/// <param name="str"></param>
		public static void StringToXmlAttribute(XmlWriter writer, string attr, string str)
		{
			if(str != null)
				writer.WriteAttributeString(attr, str);
		}

		/// <summary>
		/// Define a string para o elemento XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="str"></param>
		public static void StringToXmlElement(XmlWriter writer, string element, string str)
		{
			if(str != null)
			{
				try
				{
					writer.WriteElementString(element, str);
				}
				catch(ArgumentException exception)
				{
					throw new Exception("String contains illegal chars", exception);
				}
			}
		}

		/// <summary>
		/// Converter a string contendo a data para DateTime.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DateTime ToDateOnly(string s)
		{
			return XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
		}

		/// <summary>
		/// Converter a string cpara DateTime.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static DateTime ToDateTime(string s)
		{
			DateTime time = XmlConvert.ToDateTime(s, XmlDateTimeSerializationMode.RoundtripKind);
			if(time.Year == 1)
				return DateTime.MinValue;
			return time.ToLocalTime();
		}

		/// <summary>
		/// Recupera a string a partir do DateTime.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static string ToString(DateTime d)
		{
			return XmlConvert.ToString(d, XmlDateTimeSerializationMode.RoundtripKind);
		}

		/// <summary>
		/// Recupera a string da URI.
		/// </summary>
		/// <param name="uri"></param>
		/// <returns></returns>
		public static string ToString(Uri uri)
		{
			return uri.AbsoluteUri;
		}

		/// <summary>
		/// Converte um DateTime para uma string.
		/// </summary>
		/// <param name="d"></param>
		/// <returns></returns>
		public static string ToStringDateOnly(DateTime d)
		{
			return d.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Converte o string para um Uri.
		/// </summary>
		/// <param name="s"></param>
		/// <returns></returns>
		public static Uri ToUri(string s)
		{
			if(string.IsNullOrEmpty(s))
				return null;
			return new Uri(s);
		}

		/// <summary>
		/// Converte o vetor de bytes para um elemento xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="array"></param>
		public static void ToXml(XmlWriter writer, string element, byte[] array)
		{
			if((array != null) && (array.Length != 0))
				writer.WriteElementString(element, Convert.ToBase64String(array, 0, array.Length, Base64FormattingOptions.None));
		}

		/// <summary>
		/// Converte o vetor de objetos para um elemento xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="element"></param>
		/// <param name="array"></param>
		public static void ToXml(XmlWriter writer, string element, object[] array)
		{
			if((array != null) && (array.Length != 0))
			{
				if(!string.IsNullOrEmpty(element))
					writer.WriteStartElement(element);
				for(int i = 0; i < array.Length; i++)
				{
					if(array[i] == null)
						throw new ArgumentNullException("array[" + i + "]");
					ObjectToXmlElement(writer, "anyType", array[i]);
				}
				if(!string.IsNullOrEmpty(element))
					writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Define o vetor de bytes para o atributo XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="attr"></param>
		/// <param name="array"></param>
		public static void ToXmlAttribute(XmlWriter writer, string attr, byte[] array)
		{
			if((array != null) && (array.Length != 0))
			{
				writer.WriteAttributeString(attr, Convert.ToBase64String(array, 0, array.Length, Base64FormattingOptions.None));
			}
		}

		/// <summary>
		/// Converte o nó em um elemento XML.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="elementName"></param>
		/// <param name="node"></param>
		public static void ToXmlElement(XmlWriter writer, string elementName, XmlNode node)
		{
			if(node != null)
			{
				writer.WriteStartElement(elementName);
				node.WriteTo(writer);
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Recupera o nó xml do elemento XML.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static XmlNode XmlNodeFromXmlElement(XmlReader reader)
		{
			reader.Read();
			XmlNode node = new XmlDocument().ReadNode(reader);
			reader.ReadEndElement();
			return node;
		}
	}
}
