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
using Colosoft.Kendo.Mvc.UI.Fluent;

namespace Colosoft.Kendo.Mvc.UI
{
	/// <summary>
	/// Métodos de extensão.
	/// </summary>
	public static class ExtensionMethods
	{
		/// <summary>
		/// Configura a entidade para o DataSource.
		/// </summary>
		/// <typeparam name="TEntity"></typeparam>
		/// <typeparam name="TDataSourceBuilder"></typeparam>
		/// <param name="builder"></param>
		/// <param name="configurator"></param>
		/// <returns></returns>
		public static global::Kendo.Mvc.UI.Fluent.AjaxDataSourceBuilderBase<TEntity, TDataSourceBuilder> Entity<TEntity, TDataSourceBuilder>(this global::Kendo.Mvc.UI.Fluent.AjaxDataSourceBuilderBase<TEntity, TDataSourceBuilder> builder, Action<Kendo.Mvc.UI.Fluent.DataSourceEntityDescriptorFactory<TEntity>> configurator) where TEntity : class, Colosoft.Business.IEntity where TDataSourceBuilder : global::Kendo.Mvc.UI.Fluent.AjaxDataSourceBuilderBase<TEntity, TDataSourceBuilder>
		{
			var entityTypeManager = Colosoft.Business.EntityTypeManager.Instance;
			if(entityTypeManager != null)
			{
				var loader = entityTypeManager.GetLoader(typeof(TEntity));
				builder.Model(model =>  {
					model.Field("RowVersion", typeof(long));
					if(loader.HasUid)
						model.Field(loader.UidPropertyName, typeof(int));
					else
					{
						var properties = typeof(TEntity).GetProperties();
						foreach (var key in loader.KeysPropertyNames)
						{
							var property = properties.FirstOrDefault(f => f.Name == key);
							if(property != null)
								model.Field(key, property.PropertyType);
						}
					}
				});
			}
			return builder;
		}

		/// <summary>
		/// Configura a fonte de dados.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="widget"></param>
		/// <returns></returns>
		public static Colosoft.Kendo.Mvc.UI.Fluent.DataSourceBuilder<T> DataSource<T>(this global::Kendo.Mvc.UI.Fluent.WidgetFactory widget) where T : class
		{
			var dataSource = new global::Kendo.Mvc.UI.DataSource() {
				Type = global::Kendo.Mvc.UI.DataSourceType.Server,
				ServerAggregates = true,
				ServerFiltering = true,
				ServerGrouping = true,
				ServerPaging = true,
				ServerSorting = true
			};
			dataSource.Schema.Model = new CustomModelDescriptor(typeof(T)).ToModelDescriptor();
			var builder = new Colosoft.Kendo.Mvc.UI.Fluent.DataSourceBuilder<T>(dataSource, widget.HtmlHelper.ViewContext, widget.Initializer, widget.UrlGenerator);
			return builder;
		}

		/// <summary>
		/// Configura a fonte de dados.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="widget"></param>
		/// <returns></returns>
		public static Colosoft.Kendo.Mvc.UI.Fluent.ModelBuilder<T> Model<T>(this global::Kendo.Mvc.UI.Fluent.WidgetFactory widget) where T : class
		{
			var modelDescriptor = new CustomModelDescriptor(typeof(T));
			var builder = new Colosoft.Kendo.Mvc.UI.Fluent.ModelBuilder<T>(modelDescriptor, widget.HtmlHelper.ViewContext, widget.Initializer, widget.UrlGenerator);
			return builder;
		}

		/// <summary>
		/// Verifica se o tipo informado é nullable.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsNullableType(this Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// Recupera o tipo não nullable do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static Type GetNonNullableType(this Type type)
		{
			return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
		}

		/// <summary>
		/// Recupera o tipo número do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static int GetNumericTypeKind(this Type type)
		{
			if(type == null)
			{
				return 0;
			}
			type = GetNonNullableType(type);
			if(type.IsEnum)
			{
				return 0;
			}
			switch(Type.GetTypeCode(type))
			{
			case TypeCode.Char:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return 1;
			case TypeCode.SByte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
				return 2;
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return 3;
			default:
				return 0;
			}
		}

		/// <summary>
		/// Verifica se o tipo é um número.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static bool IsNumericType(this Type type)
		{
			return GetNumericTypeKind(type) != 0;
		}

		/// <summary>
		/// Converte o tipo para um tipo javascript.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		internal static string ToJavaScriptType(this Type type)
		{
			if(type == null)
				return "Object";
			if(type == typeof(char) || type == typeof(char?))
				return "String";
			if(IsNumericType(type))
				return "Number";
			if(type == typeof(DateTime) || type == typeof(DateTime?))
				return "Date";
			if(type == typeof(string))
				return "String";
			if(type == typeof(bool) || type == typeof(bool?))
				return "Boolean";
			if(type.GetNonNullableType().IsEnum)
				return "Number";
			if(type.GetNonNullableType() == typeof(Guid))
				return "String";
			return "Object";
		}
	}
}
