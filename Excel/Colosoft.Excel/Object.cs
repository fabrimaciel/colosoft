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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace Colosoft.Excel.ExcelXml.Extensions
{
	/// <summary>
	/// Extended xml attribute
	/// </summary>
	public class XmlReaderAttributeItem
	{
		/// <summary>
		/// Name of attribute
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Local name of attribute
		/// </summary>
		public string LocalName
		{
			get;
			set;
		}

		/// <summary>
		/// Value of attribute
		/// </summary>
		public string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Prefix if any
		/// </summary>
		public string Prefix
		{
			get;
			set;
		}

		/// <summary>
		/// Has a value?
		/// </summary>
		public bool HasValue
		{
			get;
			set;
		}
	}
	/// <summary>
	/// DateSpan class used to computer difference of two dates
	/// </summary>
	public class DateSpan
	{
		/// <summary>
		/// Difference in days
		/// </summary>
		public int Days
		{
			get;
			set;
		}

		/// <summary>
		/// Difference in months
		/// </summary>
		public int Months
		{
			get;
			set;
		}

		/// <summary>
		/// Difference in years
		/// </summary>
		public int Years
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Object extensions methods which are available if you are using VS2008
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// Returns specified number of characters from the left of string
		/// </summary>
		/// <param name="text">String on which extension method is used</param>
		/// <param name="chars">Number of chars to return</param>
		/// <returns>Returns specified number of characters from the left of string</returns>
		public static string Left(this string text, int chars)
		{
			return text.Substring(0, Math.Min(text.Length, chars));
		}

		/// <summary>
		/// Returns specified number of characters from the right of string
		/// </summary>
		/// <param name="text">String on which extension method is used</param>
		/// <param name="chars">Number of chars to return</param>
		/// <returns>Returns specified number of characters from the right of string</returns>
		public static string Right(this string text, int chars)
		{
			if(text.Length <= chars)
				return text;
			return text.Substring(text.Length - chars, chars);
		}

		/// <summary>
		/// Checks if the string is empty or only contains spaces
		/// </summary>
		/// <param name="text">String on which extension method is used</param>
		/// <returns>true if empty, false otherwise</returns>
		public static bool IsNullOrEmpty(this string text)
		{
			if(text == null)
				return true;
			return string.IsNullOrEmpty(text.Trim());
		}

		/// <summary>
		/// Checks if the string is a valid email address
		/// </summary>
		/// <param name="inputEmail">String on which extension method is used</param>
		/// <returns>true if valid email address, false otherwise</returns>
		public static bool IsValidEmail(this string inputEmail)
		{
			return IsValidEmail(inputEmail, false);
		}

		/// <summary>
		/// Checks if the string is empty or only contains spaces
		/// </summary>
		/// <param name="inputEmail">String on which extension method is used</param>
		/// <param name="isEmptyValid">Is a empty string is considered a valid email address?</param>
		/// <returns>true if empty, false otherwise</returns>
		public static bool IsValidEmail(this string inputEmail, bool isEmptyValid)
		{
			if(inputEmail.IsNullOrEmpty())
			{
				if(isEmptyValid)
					return true;
				else
					return false;
			}
			string strRegex = @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" + @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" + @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$";
			Regex re = new Regex(strRegex);
			if(re.IsMatch(inputEmail))
				return true;
			return false;
		}

		private static XmlDocument _staticDoc;

		/// <summary>
		/// XML encodes a string
		/// </summary>
		/// <param name="text">String on which extension method is used</param>
		/// <returns>returns the encoded string</returns>
		public static string XmlEncode(this string text)
		{
			if(text == null)
				return "";
			if(_staticDoc == null)
			{
				_staticDoc = new XmlDocument();
				_staticDoc.LoadXml("<text></text>");
			}
			lock (_staticDoc)
			{
				_staticDoc.LastChild.InnerText = text;
				return _staticDoc.LastChild.InnerXml;
			}
		}

		/// <summary>
		/// Parses a string to a integar form
		/// </summary>
		/// <typeparam name="T">Integar type</typeparam>
		/// <param name="text">String to parse</param>
		/// <param name="variable">Variable to store value to</param>
		/// <returns>true if parse was successful, false otherwiseParsed string</returns>
		[SuppressMessage("Microsoft.Design", "CA1021:AvoidOutParameters", MessageId = "1#")]
		public static bool ParseToInt<T>(this string text, out T variable)
		{
			if(!typeof(T).IsPrimitive && typeof(T).FullName != "System.Decimal")
				throw new ArgumentException("'variable' must only be a primitive type");
			decimal d;
			if(!decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out d))
			{
				variable = default(T);
				return false;
			}
			variable = (T)Convert.ChangeType(d, typeof(T), CultureInfo.InvariantCulture);
			return true;
		}

		/// <summary>
		/// Checks whether a enum value is correct or not
		/// </summary>
		/// <param name="enumerator">Enumerator on which extension method is used</param>
		/// <returns>returns true if valid, false otherwise</returns>
		/// <remarks>
		/// This is a interesting function. If you set a enum to a int value, you can use
		/// this function to test the value to be correct or not. What is special about this
		/// function is that if you use the Flags attribute for the enum, it will check if the
		/// value is a right bit wise match. This can better be understood with the example
		/// provided.
		/// </remarks>
		/// <example>
		/// <code>
		/// public enum NonFlagEnum
		/// {
		/// 	One = 1,
		/// 	Two = 2,
		/// 	Three = 4,
		/// 	Four = 8,
		/// 	Five = 16
		/// }
		/// 
		/// [Flags]
		/// public enum FlagEnum
		/// {
		/// 	One = 1,
		/// 	Two = 2,
		/// 	Three = 4,
		/// 	Four = 8,
		/// 	Five = 16,
		/// }
		/// 
		/// // Non flaged
		/// NonFlagEnum nfe = NonFlagEnum.One;
		/// nfe.IsValid(); // true
		/// 
		/// nfe = (NonFlagEnum) 5;
		/// nfe.IsValid(); // false
		/// 
		/// nfe = (NonFlagEnum) 21;
		/// nfe.IsValid(); // false
		/// 
		/// nfe = (NonFlagEnum) 32;
		/// nfe.IsValid(); // false
		/// 
		/// nfe = NonFlagEnum.One | NonFlagEnum.Four;
		/// nfe.IsValid(); // false because 9 is not present in the enum
		/// 
		/// // Flaged
		/// FlagEnum fe = FlagEnum.One;
		/// fe.IsValid(); // true
		/// 
		/// fe = (FlagEnum) 5;
		/// // true because value will be FlagEnum.One | FlagEnum.Three
		/// fe.IsValid();
		/// 
		/// fe = (FlagEnum) 21;
		/// // true because value will be: 
		/// // FlagEnum.Five | FlagEnum.Three | FlagEnum.One
		/// fe.IsValid();
		/// 
		/// fe = (FlagEnum) 32;
		/// fe.IsValid(); // false
		/// 
		/// fe = FlagEnum.One | FlagEnum.Four;
		/// fe.IsValid(); // true
		/// </code>
		/// </example>
		public static bool IsValid(this Enum enumerator)
		{
			bool defined = Enum.IsDefined(enumerator.GetType(), enumerator);
			if(!defined)
			{
				FlagsAttribute[] attributes = (FlagsAttribute[])enumerator.GetType().GetCustomAttributes(typeof(FlagsAttribute), false);
				if(attributes != null && attributes.Length > 0)
					return enumerator.ToString().Contains(",");
			}
			return defined;
		}

		/// <summary>
		/// Returns enum's description
		/// </summary>
		/// <param name="enumerator">Enumerator on which extension method is used</param>
		/// <returns>If the enum value has a <code>DescriptionAttribute</code>, returns the
		/// description, otherwise returns <code>ToString()</code></returns>
		public static string GetDescription(this Enum enumerator)
		{
			FieldInfo fi = enumerator.GetType().GetField(enumerator.ToString());
			DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
			if(attributes != null && attributes.Length > 0)
				return attributes[0].Description;
			return enumerator.ToString();
		}

		/// <summary>
		/// Check whether a flag is set in a particular enum value
		/// </summary>
		/// <param name="enumerator">Enumerator on which extension method is used</param>
		/// <param name="enumFlag">Enumerator flag to check</param>
		/// <returns>returns true if flag set, false otherwise</returns>
		[SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
		public static bool IsFlagSet(this Enum enumerator, Enum enumFlag)
		{
			if(!enumerator.IsValid())
				return false;
			if(!enumFlag.IsValid())
				return false;
			if(enumerator.GetType() != enumFlag.GetType())
				return false;
			int enumValue = (int)Enum.Parse(enumerator.GetType(), enumerator.ToString());
			int flagValue = (int)Enum.Parse(enumFlag.GetType(), enumFlag.ToString());
			return ((enumValue & flagValue) == flagValue);
		}

		/// <summary>
		/// Returns difference of two dates
		/// </summary>
		/// <param name="date">DateTime on which extension method is used</param>
		/// <param name="dateToCompare">The second date to compare difference with</param>
		/// <returns>return DateSpan instance of the calculated date difference</returns>
		/// <remarks>The computation always is date - dateToCompare, so dateToCompare should be
		/// lesser than date. If it is not, the answer is always a little bit... ahem... weird.</remarks>
		public static DateSpan DateDifference(this DateTime date, DateTime dateToCompare)
		{
			int totalMonths = ((date.Year - dateToCompare.Year) * 12) + date.Month - dateToCompare.Month;
			int days = 0;
			if(date.Day < dateToCompare.Day)
			{
				int day, month, year;
				day = dateToCompare.Day;
				if(date.Month == 1)
				{
					month = 12;
					year = date.Year - 1;
				}
				else
				{
					month = date.Month - 1;
					year = date.Year;
				}
				DateTime dateCalculator = new DateTime(year, month, day);
				days = (date - dateCalculator).Days;
				totalMonths--;
			}
			else
			{
				days = date.Day - dateToCompare.Day;
			}
			DateSpan ds = new DateSpan();
			ds.Years = totalMonths / 12;
			ds.Months = totalMonths % 12;
			ds.Days = days;
			return ds;
		}

		/// <summary>
		/// Extension method to get a single attribute and its value
		/// </summary>
		/// <param name="reader">XmlReader on which extension method is used</param>
		/// <param name="attribute">Attribute to read</param>
		/// <returns>XmlReaderAttributeItem instance if attribute found, null otherwise</returns>
		public static XmlReaderAttributeItem GetSingleAttribute(this XmlReader reader, string attribute)
		{
			return GetSingleAttribute(reader, attribute, false);
		}

		/// <summary>
		/// Extension method to get a single attribute and its value
		/// </summary>
		/// <param name="reader">XmlReader on which extension method is used</param>
		/// <param name="attribute">Attribute to read</param>
		/// <param name="moveToEnd">Move to the end of the element?</param>
		/// <returns>XmlReaderAttributeItem instance if attribute found, null otherwise</returns>
		public static XmlReaderAttributeItem GetSingleAttribute(this XmlReader reader, string attribute, bool moveToEnd)
		{
			string element = reader.Name;
			if(moveToEnd && reader.IsEmptyElement)
				moveToEnd = false;
			foreach (XmlReaderAttributeItem xa in reader.GetAttributes())
			{
				if(xa.LocalName == attribute)
				{
					if(moveToEnd)
						while (reader.Read() && !(reader.Name == element && reader.NodeType == XmlNodeType.EndElement))
							;
					return xa;
				}
			}
			if(moveToEnd)
				while (reader.Read() && !(reader.Name == element && reader.NodeType == XmlNodeType.EndElement))
					;
			return null;
		}

		/// <summary>
		/// Extension method to get a all attribute of a element
		/// </summary>
		/// <param name="reader">XmlReader on which extension method is used</param>
		/// <returns>List of all attributes as XmlReaderAttributeItem</returns>
		public static IEnumerable<XmlReaderAttributeItem> GetAttributes(this XmlReader reader)
		{
			List<XmlReaderAttributeItem> list = new List<XmlReaderAttributeItem>();
			if(!reader.HasAttributes)
				return list;
			reader.MoveToFirstAttribute();
			list.Add(ReadAttribute(reader));
			while (reader.MoveToNextAttribute())
				list.Add(ReadAttribute(reader));
			return list;
		}

		private static XmlReaderAttributeItem ReadAttribute(XmlReader reader)
		{
			XmlReaderAttributeItem attr = new XmlReaderAttributeItem();
			attr.Name = reader.Name;
			attr.LocalName = reader.LocalName;
			attr.Prefix = reader.Prefix;
			attr.HasValue = reader.HasValue;
			if(attr.HasValue)
				attr.Value = reader.Value;
			else
				attr.Value = "";
			return attr;
		}

		/// <summary>
		/// Parses a enum from a string 
		/// </summary>
		/// <typeparam name="T">Enum type</typeparam>
		/// <param name="value">String to parse</param>
		/// <returns>Parsed enum value</returns>
		[SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
		public static T ParseEnum<T>(string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}

		/// <summary>
		/// Checks where a type is a numeric type
		/// </summary>
		/// <param name="type">Type to check</param>
		/// <returns>true if type is numeric type, false otherwise</returns>
		public static bool IsNumericType(Type type)
		{
			string typeName = type.FullName;
			switch(typeName)
			{
			case "System.SByte":
			case "System.Int16":
			case "System.Int32":
			case "System.Int64":
			case "System.Byte":
			case "System.UInt16":
			case "System.UInt32":
			case "System.UInt64":
			case "System.Single":
			case "System.Double":
			case "System.Decimal":
				return true;
			}
			return false;
		}
	}
}
