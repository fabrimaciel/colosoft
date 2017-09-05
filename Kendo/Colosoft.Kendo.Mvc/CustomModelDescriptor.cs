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
using System.Threading.Tasks;
using Colosoft.Kendo.Mvc.UI.DataSource;
using Kendo.Mvc.Infrastructure;

namespace Colosoft.Kendo.Mvc.UI
{
	/// <summary>
	/// Representa o descritor da model.
	/// </summary>
	public class CustomModelDescriptor : global::Kendo.Mvc.JsonObject
	{
		private IList<global::Kendo.Mvc.UI.ModelFieldDescriptor> _fields = new List<global::Kendo.Mvc.UI.ModelFieldDescriptor>();

		/// <summary>
		/// Campos associados;
		/// </summary>
		public IList<global::Kendo.Mvc.UI.ModelFieldDescriptor> Fields
		{
			get
			{
				return _fields;
			}
		}

		/// <summary>
		/// Identificador da model.
		/// </summary>
		public global::Kendo.Mvc.UI.IDataKey Id
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="modelType"></param>
		public CustomModelDescriptor(Type modelType)
		{
			var metadata = System.Web.Mvc.ModelMetadataProviders.Current.GetMetadataForType(null, modelType);
			foreach (var i in Translate(metadata))
				Fields.Add(i);
		}

		/// <summary>
		/// Converte para um ModelDescriptor.
		/// </summary>
		/// <returns></returns>
		public global::Kendo.Mvc.UI.ModelDescriptor ToModelDescriptor()
		{
			var result = new global::Kendo.Mvc.UI.ModelDescriptor(typeof(object));
			result.Fields.Clear();
			foreach (var i in Fields)
				result.Fields.Add(i);
			return result;
		}

		/// <summary>
		/// Adiciona um novo descritor.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		public global::Kendo.Mvc.UI.ModelFieldDescriptor AddDescriptor(string member)
		{
			var descriptor = Fields.FirstOrDefault(f => f.Member == member);
			if(descriptor != null)
				return descriptor;
			descriptor = new global::Kendo.Mvc.UI.ModelFieldDescriptor {
				Member = member
			};
			Fields.Add(descriptor);
			return descriptor;
		}

		/// <summary>
		/// Serializa os dados do descritor.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
			if(Id != null)
			{
				json["id"] = Id.Name;
			}
			if(Fields.Count > 0)
			{
				var fields = new Dictionary<string, object>();
				json["fields"] = fields;
				foreach (var prop in Fields)
				{
					var field = new Dictionary<string, object>();
					fields[prop.Member] = field;
					if(!prop.IsEditable)
					{
						field["editable"] = false;
					}
					field["type"] = prop.MemberType.ToJavaScriptType().ToLowerInvariant();
					if(prop.MemberType.IsNullableType() || prop.DefaultValue != null)
					{
						var defaultValue = prop.DefaultValue;
						if(prop.MemberType.GetNonNullableType().IsEnum && defaultValue is Enum)
						{
							var underlyingType = Enum.GetUnderlyingType(prop.MemberType.GetNonNullableType());
							defaultValue = Convert.ChangeType(defaultValue, underlyingType);
						}
						field["defaultValue"] = defaultValue;
					}
					if(!string.IsNullOrEmpty(prop.From))
						field["from"] = prop.From;
					if(prop.IsNullable)
						field["nullable"] = prop.IsNullable;
					if(prop.Parse.HasValue())
						field["parse"] = prop.Parse;
				}
			}
		}

		/// <summary>
		/// Traduz os metadados.
		/// </summary>
		/// <param name="metadata"></param>
		/// <returns></returns>
		private IList<global::Kendo.Mvc.UI.ModelFieldDescriptor> Translate(System.Web.Mvc.ModelMetadata metadata)
		{
			var result = new List<global::Kendo.Mvc.UI.ModelFieldDescriptor>();
			foreach (var i in metadata.Properties)
			{
				global::Kendo.Mvc.UI.ModelFieldDescriptor field = null;
				object item = null;
				if(i.AdditionalValues.TryGetValue("StateItem", out item))
					field = new CustomModelFieldDescriptor((Colosoft.Validation.IStatebleItem)item);
				else
					field = new global::Kendo.Mvc.UI.ModelFieldDescriptor();
				field.Member = i.PropertyName;
				field.MemberType = i.ModelType;
				field.IsEditable = !i.IsReadOnly;
				result.Add(field);
			}
			return result;
		}
	}
}
