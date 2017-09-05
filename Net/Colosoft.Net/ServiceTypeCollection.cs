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
using System.Configuration;
using System.Linq;
using System.Text;

namespace Colosoft.Net.Json.Configuration
{
	/// <summary>
	/// Class ServiceTypeCollection.
	/// </summary>
	/// <typeparam name="TElement">The type of the t element.</typeparam>
	public class ServiceTypeCollection<TElement> : ConfigurationElementCollection where TElement : ConfigServiceElement, new()
	{
		private readonly string propertyName;

		/// <summary>
		/// Initializes a new instance of the <see cref="ServiceTypeCollection{TElement}"/> class.
		/// </summary>
		public ServiceTypeCollection()
		{
			string naming = (typeof(TElement).Name);
			this.propertyName = string.Format("{0}{1}", naming.Substring(0, 1).ToLower(), naming.Substring(1));
		}

		/// <summary>
		/// Gets the name used to identify this collection of elements in the configuration file when overridden in a derived class.
		/// </summary>
		/// <value>The name of the element.</value>
		protected override string ElementName
		{
			get
			{
				return this.propertyName;
			}
		}

		/// <summary>
		/// Indicates whether the specified <see cref="T:System.Configuration.ConfigurationElement" /> exists in the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
		/// </summary>
		/// <param name="elementName">The name of the element to verify.</param>
		/// <returns>true if the element exists in the collection; otherwise, false. The default is false.</returns>
		protected override bool IsElementName(string elementName)
		{
			return elementName.Equals(propertyName, StringComparison.InvariantCultureIgnoreCase);
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object is read only.
		/// </summary>
		/// <returns>true if the <see cref="T:System.Configuration.ConfigurationElementCollection" /> object is read only; otherwise, false.</returns>
		public override bool IsReadOnly()
		{
			return false;
		}

		/// <summary>
		/// Adds the specified custom.
		/// </summary>
		/// <param name="custom">The custom.</param>
		public void Add(TElement custom)
		{
			BaseAdd(custom);
		}

		/// <summary>
		/// Adds a configuration element to the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to add.</param>
		protected override void BaseAdd(ConfigurationElement element)
		{
			BaseAdd(element, false);
		}

		/// <summary>
		/// Gets the type of the <see cref="T:System.Configuration.ConfigurationElementCollection" />.
		/// </summary>
		/// <value>The type of the collection.</value>
		public override ConfigurationElementCollectionType CollectionType
		{
			get
			{
				return ConfigurationElementCollectionType.BasicMapAlternate;
			}
		}

		/// <summary>
		/// When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement" />.
		/// </summary>
		/// <returns>A new <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected override ConfigurationElement CreateNewElement()
		{
			return new TElement();
		}

		/// <summary>
		/// Gets the element key for a specified configuration element when overridden in a derived class.
		/// </summary>
		/// <param name="element">The <see cref="T:System.Configuration.ConfigurationElement" /> to return the key for.</param>
		/// <returns>An <see cref="T:System.Object" /> that acts as the key for the specified <see cref="T:System.Configuration.ConfigurationElement" />.</returns>
		protected override object GetElementKey(ConfigurationElement element)
		{
			return ((TElement)element).Key;
		}

		/// <summary> 
		/// Gets or sets the <see cref="TElement"/> at the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <returns>TElement.</returns>
		public TElement this[int index]
		{
			get
			{
				return (TElement)BaseGet(index);
			}
			set
			{
				if(BaseGet(index) != null)
				{
					BaseRemoveAt(index);
				}
				BaseAdd(index, value);
			}
		}

		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="element">The element.</param>
		/// <returns>System.Int32.</returns>
		public int IndexOf(TElement element)
		{
			return BaseIndexOf(element);
		}

		/// <summary>
		/// Removes the specified URL.
		/// </summary>
		/// <param name="url">The URL.</param>
		public void Remove(TElement url)
		{
			if(BaseIndexOf(url) >= 0)
				BaseRemove(url.Key);
		}

		/// <summary>
		/// Removes at.
		/// </summary>
		/// <param name="index">The index.</param>
		public void RemoveAt(int index)
		{
			BaseRemoveAt(index);
		}

		///// <summary>
		///// 
		///// </summary>
		///// <param name="name"></param>
		/// <summary>
		/// Clears this instance.
		/// </summary>
		public void Clear()
		{
			BaseClear();
		}
	}
}
