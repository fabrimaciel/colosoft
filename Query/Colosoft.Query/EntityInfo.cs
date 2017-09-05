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
using System.Runtime.Serialization;
using Colosoft.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Armazena as informações da entidade onde será realizada a consulta.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public sealed class EntityInfo : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private string _fullName;

		private string _alias;

		private QueryInfo _subQuery;

		/// <summary>
		/// Nome completeo da entidade.
		/// </summary>
		public string FullName
		{
			get
			{
				return _fullName;
			}
			set
			{
				_fullName = value;
			}
		}

		/// <summary>
		/// Apelido da entrada.
		/// </summary>
		public string Alias
		{
			get
			{
				return _alias;
			}
			set
			{
				_alias = value;
			}
		}

		/// <summary>
		/// Query aninhada.
		/// </summary>
		public QueryInfo SubQuery
		{
			get
			{
				return _subQuery;
			}
			set
			{
				_subQuery = value;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public EntityInfo()
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="fullName">Nome da entidade.</param>
		/// <param name="alias">Apelido da entidade.</param>
		public EntityInfo(string fullName, string alias = null)
		{
			_fullName = fullName;
			_alias = alias;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="subQuery">Query aninhada.</param>
		/// <param name="alias">Apelido da entidade.</param>
		public EntityInfo(QueryInfo subQuery, string alias = null)
		{
			_alias = alias;
			_subQuery = subQuery;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private EntityInfo(SerializationInfo info, StreamingContext context)
		{
			_fullName = info.GetString("FullName");
			_alias = info.GetString("Alias");
			_subQuery = (QueryInfo)info.GetValue("SubQuery", typeof(QueryInfo));
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(!string.IsNullOrEmpty(Alias))
				return string.Format("{0} AS {1}", FullName, Alias);
			return FullName;
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("FullName", _fullName);
			info.AddValue("Alias", _alias);
			info.AddValue("SubQuery", _subQuery);
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("EntityInfo", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("FullName"))
				_fullName = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Alias"))
				_alias = reader.ReadContentAsString();
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				_subQuery = new QueryInfo();
				((System.Xml.Serialization.IXmlSerializable)_subQuery).ReadXml(reader);
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			if(!string.IsNullOrEmpty(_fullName))
				writer.WriteAttributeString("FullName", _fullName);
			if(!string.IsNullOrEmpty(_alias))
				writer.WriteAttributeString("Alias", _alias);
			if(_subQuery != null)
				((System.Xml.Serialization.IXmlSerializable)_subQuery).WriteXml(writer);
		}

		/// <summary>
		/// Clona as informações de entidade.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new EntityInfo(FullName, Alias) {
				SubQuery = this.SubQuery != null ? (QueryInfo)this.SubQuery.Clone() : null
			};
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			if(reader.ReadBoolean())
				_fullName = reader.ReadString();
			if(reader.ReadBoolean())
			{
				_subQuery = new QueryInfo();
				((ICompactSerializable)_subQuery).Deserialize(reader);
			}
			_alias = reader.ReadString();
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			if(!string.IsNullOrEmpty(_fullName))
			{
				writer.Write(true);
				writer.Write(_fullName);
			}
			else
				writer.Write(false);
			if(_subQuery != null)
			{
				writer.Write(true);
				((ICompactSerializable)_subQuery).Serialize(writer);
			}
			else
				writer.Write(false);
			writer.Write(_alias);
		}
	}
	/// <summary>
	/// Implementação do comparador para o alias do EntityInfo.
	/// </summary>
	public class EntityInfoAliasComparer : IEqualityComparer<EntityInfo>, IComparer<EntityInfo>
	{
		/// <summary>
		/// Instancia geral do comparador.
		/// </summary>
		public static readonly EntityInfoAliasComparer Instance = new EntityInfoAliasComparer();

		/// <summary>
		/// Recupera o alias do EntityInfo.
		/// </summary>
		/// <param name="info"></param>
		/// <returns></returns>
		private static string GetAlias(EntityInfo info)
		{
			if(info == null)
				return null;
			return info.Alias ?? info.FullName;
		}

		/// <summary>
		/// Compara as duas entidades informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(EntityInfo x, EntityInfo y)
		{
			return StringComparer.InvariantCultureIgnoreCase.Equals(GetAlias(x), GetAlias(y));
		}

		/// <summary>
		/// Recupera o hash code da entidade informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(EntityInfo obj)
		{
			return (GetAlias(obj) ?? string.Empty).GetHashCode();
		}

		/// <summary>
		/// Compara as instancias informadas.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public int Compare(EntityInfo x, EntityInfo y)
		{
			return StringComparer.InvariantCultureIgnoreCase.Compare(GetAlias(x), GetAlias(y));
		}
	}
}
