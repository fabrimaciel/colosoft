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
	/// Class ConfigServiceElement.
	/// </summary>
	public abstract class ConfigServiceElement : ConfigurationElement
	{
		/// <summary>
		/// Gets the key.
		/// </summary>
		/// <value>The key.</value>
		public abstract object Key
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether an unknown attribute is encountered during deserialization.
		/// </summary>
		/// <param name="name">The name of the unrecognized attribute.</param>
		/// <param name="value">The value of the unrecognized attribute.</param>
		/// <returns>true when an unknown attribute is encountered while deserializing; otherwise, false.</returns>
		protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
		{
			if(name.Equals("xmlns"))
				return true;
			return base.OnDeserializeUnrecognizedAttribute(name, value);
		}
	}
}
