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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace Colosoft.Excel.ExcelXml.Extensions
{
	/// <summary>
	/// Mark a property of this class to exclude this property from getting saved
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class XmlSettingIgnoreAttribute : Attribute
	{
	}
	/// <summary>
	/// Define the name of a property, or force encryption or both
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
	public sealed class XmlSettingAttribute : Attribute
	{
		private string nameValue;

		/// <summary>
		/// Description of the property which will be shown in the settings file
		/// </summary>
		public string Name
		{
			get
			{
				return nameValue;
			}
		}

		/// <summary>
		/// If true, the value of the property will be encrypted
		/// </summary>
		public bool Encrypt
		{
			get;
			set;
		}

		/// <summary>
		/// Default constructor
		/// </summary>
		public XmlSettingAttribute()
		{
			nameValue = "";
			Encrypt = false;
		}

		/// <summary>
		/// Define the name of a property
		/// </summary>
		/// <param name="name">Description of the property</param>
		public XmlSettingAttribute(string name)
		{
			nameValue = name;
			Encrypt = false;
		}
	}
	/// <summary>
	/// A implementation of a base application settings class
	/// </summary>
	/// <remarks>
	/// XmlSettings is a application settings class which saves all properties of all classes
	/// derived from it to the a .settings.xml file of the same name as of the application. All
	/// base types and any classes which support serialization (like generic lists and arrays or 
	/// even custom classes which support xml serialization) can be members of the derived class.
	/// If any other member is found, a exception is thrown.
	/// <para><code>XmlSettingAttribute</code> and <code>XmlSettingIgnore</code> attributes
	/// can be applied to the properties</para>
	/// </remarks>
	/// <example>
	/// <code>
	/// public class TestSettings : XmlSettings
	/// {
	/// 	[XmlSettingIgnore]
	/// 	public string TestString { get; set; }
	/// 	
	/// 	[XmlSetting("Integar")]
	/// 	public int TestIntegar { get; set; }
	/// 	
	/// 	public List&lt;string&gt; GenericListOfString { get; set; }
	/// 
	///		public TestSettings()
	///		{
	/// 		TestString = "Hello";
	/// 		TestIntegar = 0;
	/// 
	///			GenericListOfString = new List&lt;string&gt;();
	/// 		GenericListOfString.Add("item0");
	///			GenericListOfString.Add("item1");
	/// 		GenericListOfString.Add("item2");
	/// 		GenericListOfString.Add("item3");
	/// 		GenericListOfString.Add("item4");
	/// 		GenericListOfString.Add("item5");
	///		}
	/// }
	/// </code>
	/// Somewhere in the code...
	/// <code>
	/// TestSettings ts = new TestSettings();
	/// ts.Save();
	/// </code>
	/// This outputs this xml file...
	/// <code>
	/// &lt;Configuration&gt;
	///   &lt;Settings&gt;
	///     &lt;TestSettings&gt;
	///       &lt;Integar&gt;0&lt;/Integar&gt;
	///       &lt;GenericListOfString&gt;
	///         &lt;ArrayOfString xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance" xmlns:xsd="http://www.w3.org/2001/XMLSchema"&gt;
	///           &lt;string&gt;item0&lt;/string&gt;
	///           &lt;string&gt;item1&lt;/string&gt;
	///           &lt;string&gt;item2&lt;/string&gt;
	///           &lt;string&gt;item3&lt;/string&gt;
	///           &lt;string&gt;item4&lt;/string&gt;
	///           &lt;string&gt;item5&lt;/string&gt;
	///         &lt;/ArrayOfString&gt;
	///       &lt;/GenericListOfString&gt;
	///     &lt;/TestSettings&gt;
	///   &lt;/Settings&gt;
	/// &lt;/Configuration&gt;
	/// </code>
	/// Note that TestString is not outputed, because of the use of XmlIgnore attribute and name
	/// of Integar is outputed instead of TestIntegar because of the use of XmlAttribute(attributeName)
	/// attribute.
	/// </example>
	public abstract class XmlSettings
	{
		private const string defaultRootNotePath = "Configuration/Settings";

		private static string XmlSettingsFileName;

		private static XmlDocument XmlSettingsFile;

		/// <summary>
		/// Captura o caminho onde o assembly está sendo executado.
		/// </summary>
		/// <returns></returns>
		private static string GetExecutingAssemblyLocation()
		{
			string uriString = Assembly.GetExecutingAssembly().EscapedCodeBase;
			Uri uri = new Uri(uriString);
			string path = uri.IsFile ? Path.GetDirectoryName(uri.LocalPath) : null;
			return path;
		}

		[SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes"), SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static XmlSettings()
		{
			XmlSettingsFileName = Path.ChangeExtension(GetExecutingAssemblyLocation(), ".settings.xml");
			XmlSettingsFile = new XmlDocument();
			try
			{
				XmlSettingsFile.Load(XmlSettingsFileName);
			}
			catch
			{
				XmlSettingsFile.LoadXml("<Configuration><Settings></Settings></Configuration>");
			}
		}

		private static XmlNode CreateMissingNode(string xPath)
		{
			string[] xPathSections = xPath.Split('/');
			string currentXPath = "";
			XmlNode testNode = null;
			XmlNode currentNode = XmlSettingsFile.SelectSingleNode(defaultRootNotePath);
			foreach (string xPathSection in xPathSections)
			{
				currentXPath += xPathSection;
				testNode = XmlSettingsFile.SelectSingleNode(currentXPath);
				if(testNode == null)
				{
					currentNode.InnerXml += "<" + xPathSection + "></" + xPathSection + ">";
				}
				currentNode = XmlSettingsFile.SelectSingleNode(currentXPath);
				currentXPath += "/";
			}
			return currentNode;
		}

		private static bool IsSystemDataType(PropertyInfo property)
		{
			switch(property.PropertyType.FullName)
			{
			case "System.DateTime":
			case "System.Byte":
			case "System.SByte":
			case "System.Double":
			case "System.Single":
			case "System.Decimal":
			case "System.Int64":
			case "System.Int32":
			case "System.Int16":
			case "System.UInt64":
			case "System.UInt32":
			case "System.UInt16":
			case "System.Boolean":
			case "System.Char":
			case "System.String":
				return true;
			default:
				return false;
			}
		}

		private static bool IsIgnored(PropertyInfo property)
		{
			XmlSettingIgnoreAttribute[] attributes = (XmlSettingIgnoreAttribute[])property.GetCustomAttributes(typeof(XmlSettingIgnoreAttribute), false);
			if(attributes != null && attributes.Length > 0)
				return true;
			return false;
		}

		private string GetPropertyName(PropertyInfo property, out bool encrypt)
		{
			XmlSettingAttribute[] attributes = (XmlSettingAttribute[])property.GetCustomAttributes(typeof(XmlSettingAttribute), false);
			encrypt = false;
			if(attributes != null && attributes.Length > 0)
			{
				encrypt = attributes[0].Encrypt;
				if(!string.IsNullOrEmpty(attributes[0].Name))
					return this.GetType().Name + "/" + attributes[0].Name;
			}
			return this.GetType().Name + "/" + property.Name;
		}

		private void SaveProperty(PropertyInfo property)
		{
			if(IsIgnored(property))
				return;
			bool encrypt;
			string propertyName = GetPropertyName(property, out encrypt);
			string rootNodePath = defaultRootNotePath + "/" + propertyName;
			XmlNode xmlNode = XmlSettingsFile.SelectSingleNode(rootNodePath);
			if(xmlNode == null)
				xmlNode = CreateMissingNode(rootNodePath);
			if(IsSystemDataType(property))
			{
				string data;
				if(property.PropertyType.FullName == "System.DateTime")
				{
					data = ((DateTime)property.GetValue(this, null)).ToString("yyyy-MM-dd\\Thh:mm:ss.fff", CultureInfo.InvariantCulture);
				}
				else
				{
					data = property.GetValue(this, null).ToString();
				}
				if(encrypt)
					data = EncryptDecrypt.Encrypt(data, "XmlSettingsPK_L0020P");
				xmlNode.InnerText = data;
			}
			else
			{
				if(property.PropertyType.IsSerializable)
				{
					XmlSerializer xmlSerialize = new XmlSerializer(property.PropertyType);
					XmlNode serializedNode;
					using (MemoryStream ms = new MemoryStream())
					{
						xmlSerialize.Serialize(ms, property.GetValue(this, null));
						ms.Position = 0;
						XmlDocument doc = new XmlDocument();
						doc.Load(ms);
						serializedNode = doc.DocumentElement;
					}
					xmlNode.RemoveAll();
					xmlNode.AppendChild(XmlSettingsFile.ImportNode(serializedNode, true));
				}
				else
				{
					throw new NotSupportedException("Unsupported data found in " + this.GetType().Name + " class");
				}
			}
		}

		/// <summary>
		/// Saves the derived class to settings file
		/// </summary>
		public void Save()
		{
			PropertyInfo[] properties = this.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
				SaveProperty(property);
			XmlSettingsFile.Save(XmlSettingsFileName);
		}

		private void LoadProperty(PropertyInfo property)
		{
			if(IsIgnored(property))
				return;
			bool encrypt;
			string propertyName = GetPropertyName(property, out encrypt);
			string rootNodePath = defaultRootNotePath + "/" + propertyName;
			XmlNode xmlNode = XmlSettingsFile.SelectSingleNode(rootNodePath);
			if(xmlNode != null)
			{
				string data = xmlNode.InnerText;
				if(encrypt)
					data = EncryptDecrypt.Decrypt(data, "XmlSettingsPK_L0020P");
				if(IsSystemDataType(property))
				{
					if(property.PropertyType.FullName == "System.DateTime")
					{
						try
						{
							DateTime date = DateTime.ParseExact(data, "yyyy-MM-dd\\Thh:mm:ss.fff", CultureInfo.InvariantCulture);
							property.SetValue(this, date, null);
						}
						catch(FormatException)
						{
						}
					}
					else
					{
						try
						{
							property.SetValue(this, Convert.ChangeType(data, property.PropertyType, CultureInfo.InvariantCulture), null);
						}
						catch(FormatException)
						{
						}
					}
				}
				else
				{
					if(property.PropertyType.IsSerializable)
					{
						XmlSerializer xmlSerialize = new XmlSerializer(property.PropertyType);
						if(xmlNode.FirstChild != null)
						{
							XmlNodeReader xmlNodeReader = new XmlNodeReader(xmlNode.FirstChild);
							property.SetValue(this, xmlSerialize.Deserialize(xmlNodeReader), null);
						}
					}
					else
					{
						throw new NotSupportedException("Unsupported data found in " + this.GetType().Name + " class");
					}
				}
			}
		}

		/// <summary>
		/// Loads the derived class from settings file
		/// </summary>
		public void Load()
		{
			PropertyInfo[] properties = this.GetType().GetProperties();
			foreach (PropertyInfo property in properties)
				LoadProperty(property);
		}
	}
}
