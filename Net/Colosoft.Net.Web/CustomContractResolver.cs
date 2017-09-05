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
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Colosoft.Net.Json
{
	/// <summary>
	/// Class CustomContractResolver.
	/// </summary>
	public class CustomContractResolver : DefaultContractResolver
	{
		private readonly bool includeFields;

		private readonly Func<Type, Type> normalizer;

		/// <summary>
		/// Initializes a new instance of the <see cref="CustomContractResolver"/> class.
		/// </summary>
		/// <param name="shareCache">if set to <c>true</c> [share cache].</param>
		/// <param name="includeFields">if set to <c>true</c> [include fields].</param>
		/// <param name="normalizer">The normalizer.</param>
		public CustomContractResolver(bool shareCache, bool includeFields, Func<Type, Type> normalizer) : base(shareCache)
		{
			this.includeFields = includeFields;
			this.normalizer = normalizer;
		}

		/// <summary>
		/// Gets the property members.
		/// </summary>
		/// <param name="type">The type.</param>
		/// <returns>IEnumerable&lt;PropertyInfo&gt;.</returns>
		private static IEnumerable<PropertyInfo> GetPropertyMembers(Type type)
		{
			return type.GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy);
		}

		/// <summary>
		/// Creates properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.
		/// </summary>
		/// <param name="type">The type to create properties for.</param>
		/// <param name="memberSerialization">The member serialization mode for the type.</param>
		/// <returns>Properties for the given <see cref="T:Newtonsoft.Json.Serialization.JsonContract" />.</returns>
		protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
		{
			List<JsonProperty> properties = new List<JsonProperty>(base.CreateProperties(type, memberSerialization));
			if(!includeFields)
			{
				IEnumerable<string> propertyMembers = CustomContractResolver.GetPropertyMembers(type).Select(n => n.Name);
				properties.RemoveAll(n => !propertyMembers.Contains(n.PropertyName));
				foreach (var property in properties)
				{
					Type normalized = normalizer.Invoke(property.PropertyType);
					if(normalized != null && normalized != property.PropertyType)
						property.MemberConverter = new JsonReaderConverter(normalized);
				}
			}
			return properties;
		}
	}
}
