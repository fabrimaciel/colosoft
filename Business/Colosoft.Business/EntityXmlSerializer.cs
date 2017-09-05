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

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura das classe que serializa os dados da entidade como Xml.
	/// </summary>
	public interface IEntityXmlSerializable : System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Identifica se pode serializar os dados.
		/// </summary>
		bool CanSerialize
		{
			get;
		}
	}
	/// <summary>
	/// Implementação do serializador xml para as entidades do sistema.
	/// </summary>
	static class EntityXmlSerializer
	{
		private static readonly Type BooleanType = typeof(bool);

		private static readonly Type ByteArrayType = typeof(byte[]);

		private static readonly Type ByteType = typeof(byte);

		private static readonly Type DateTimeOffsetType = typeof(DateTimeOffset);

		private static readonly Type DateTimeType = typeof(DateTime);

		private static readonly Type DecimalType = typeof(decimal);

		private static readonly Type DoubleType = typeof(double);

		private static readonly Type ICollectionType = typeof(System.Collections.ICollection);

		private static readonly Type IEnumerableType = typeof(System.Collections.IEnumerable);

		private static readonly Type IListType = typeof(System.Collections.IList);

		private static readonly Type Int16Type = typeof(short);

		private static readonly Type Int32Type = typeof(int);

		private static readonly Type Int64Type = typeof(long);

		private static readonly Type ObjectArrayType = typeof(object[]);

		private static readonly Type SByteType = typeof(sbyte);

		private static readonly Type SingleType = typeof(float);

		private static readonly Type StringArrayType = typeof(string[]);

		private static readonly Type StringType = typeof(string);

		private static readonly Type TimeSpanType = typeof(TimeSpan);

		private static readonly Type UInt16Type = typeof(ushort);

		private static readonly Type UInt32Type = typeof(uint);

		private static readonly Type UInt64Type = typeof(ulong);

		/// <summary>
		/// Verifica se o tipo deriva do tipo base.
		/// </summary>
		/// <param name="derivedType"></param>
		/// <param name="baseTypes"></param>
		/// <returns></returns>
		private static bool IsDerivedFrom(Type derivedType, params Type[] baseTypes)
		{
			while (derivedType != null)
			{
				if(baseTypes.Any(f => f == derivedType))
					return true;
				derivedType = derivedType.BaseType;
			}
			return false;
		}

		/// <summary>
		/// Verifica se o tipo possui suporte para serialização.
		/// </summary>
		/// <param name="derivedType"></param>
		/// <returns></returns>
		private static bool Support(Type derivedType)
		{
			return IsDerivedFrom(derivedType, new Type[] {
				BooleanType,
				ByteArrayType,
				ByteType,
				DateTimeOffsetType,
				DateTimeType,
				DecimalType,
				DoubleType,
				ICollectionType,
				IEnumerableType,
				IListType,
				Int16Type,
				Int32Type,
				Int64Type,
				ObjectArrayType,
				SByteType,
				SingleType,
				StringArrayType,
				StringType,
				TimeSpanType,
				UInt16Type,
				UInt32Type,
				UInt64Type
			});
		}

		/// <summary>
		/// Recupera os dados que precisam ser serializados.
		/// </summary>
		/// <param name="entityTypeManager"></param>
		/// <param name="entity"></param>
		/// <returns></returns>
		private static IEnumerable<KeyValuePair<string, object>> GetSerializeData(IEntityTypeManager entityTypeManager, IEntity entity)
		{
			entity.Require("entity").NotNull();
			var type = entity.GetType();
			var loader = entityTypeManager.GetLoader(type);
			var children = loader.GetChildrenAccessors().Where(f => f is IEntityPropertyAccessor).Select(f => (IEntityPropertyAccessor)f).ToArray();
			var links = loader.GetLinksAccessors().Where(f => f is IEntityPropertyAccessor).Select(f => (IEntityPropertyAccessor)f).ToArray();
			var references = loader.GetReferences().Where(f => f is IEntityPropertyAccessor).Select(f => (IEntityPropertyAccessor)f).ToArray();
			foreach (var prop in type.GetProperties())
			{
				if(prop.CanRead && prop.GetIndexParameters().Length == 0 && !prop.GetCustomAttributes(true).Any(f => f is System.Xml.Serialization.XmlIgnoreAttribute) && !children.Any(f => f.PropertyName == prop.Name) && !links.Any(f => f.PropertyName == prop.Name) && !references.Any(f => f.PropertyName == prop.Name))
				{
					object value = null;
					try
					{
						value = prop.GetValue(entity, null);
					}
					catch(System.Reflection.TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
					yield return new KeyValuePair<string, object>(prop.Name, value);
				}
			}
			foreach (var i in children.Concat(links).Concat(references))
				yield return new KeyValuePair<string, object>(i.PropertyName, i.Get(entity));
		}

		/// <summary>
		/// Serializa os dados da entidade.
		/// </summary>
		/// <param name="writer">Writer onde serão registrados os dados serializados.</param>
		/// <param name="entityTypeManager"></param>
		/// <param name="entity">Entidade que será serializada.</param>
		public static void Serialize(System.Xml.XmlWriter writer, IEntityTypeManager entityTypeManager, IEntity entity)
		{
			entity.Require("entity").NotNull();
			if(entity is IEntityXmlSerializable && !((IEntityXmlSerializable)entity).CanSerialize)
				return;
			foreach (var data in GetSerializeData(entityTypeManager, entity))
			{
				writer.WriteStartElement(data.Key);
				if(data.Value is System.Xml.Serialization.IXmlSerializable)
					((System.Xml.Serialization.IXmlSerializable)data.Value).WriteXml(writer);
				else if(data.Value != null && Support(data.Value.GetType()))
					writer.WriteValue(data.Value);
				writer.WriteEndElement();
			}
		}
	}
}
