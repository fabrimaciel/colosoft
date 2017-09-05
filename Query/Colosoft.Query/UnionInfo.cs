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

using Colosoft.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Query
{
	/// <summary>
	/// Armazena as informações de uma união.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public class UnionInfo : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private QueryInfo _query;

		private bool _all;

		/// <summary>
		/// Consulta da união.
		/// </summary>
		public QueryInfo Query
		{
			get
			{
				return _query;
			}
			set
			{
				_query = value;
			}
		}

		/// <summary>
		/// Identifica se é uma união ALL.
		/// </summary>
		public bool All
		{
			get
			{
				return _all;
			}
			set
			{
				_all = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public UnionInfo()
		{
		}

		/// <summary>
		/// Cria a instancia informando os parametros iniciais.
		/// </summary>
		/// <param name="query"></param>
		/// <param name="all"></param>
		public UnionInfo(QueryInfo query, bool all)
		{
			_query = query;
			_all = all;
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private UnionInfo(SerializationInfo info, StreamingContext context)
		{
			_query = (QueryInfo)info.GetValue("Query", typeof(QueryInfo));
			_all = info.GetBoolean("All");
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[UNION{0}: {1}]", _all ? " ALL" : null, _query);
		}

		/// <summary>
		/// Clona uma projeção.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new UnionInfo(_query != null ? (QueryInfo)_query.Clone() : null, _all);
		}

		/// <summary>
		/// Recupera os dados da instância anteriormente serializada.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Query", _query, typeof(QueryInfo));
			info.AddValue("All", _all);
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("UnionInfo", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Recupera os dados serializados
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			var allAttribute = reader.GetAttribute("All");
			bool.TryParse(allAttribute, out _all);
			var query = new QueryInfo();
			((System.Xml.Serialization.IXmlSerializable)query).ReadXml(reader);
			_query = query;
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("All", _all.ToString());
			((System.Xml.Serialization.IXmlSerializable)_query).WriteXml(writer);
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_all = reader.ReadBoolean();
			if(reader.ReadBoolean())
			{
				_query = new QueryInfo();
				((ICompactSerializable)_query).Deserialize(reader);
			}
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_all);
			writer.Write(_query != null);
			if(_query != null)
				((ICompactSerializable)_query).Serialize(writer);
		}
	}
	/// <summary>
	/// Representa uma coleção de uniões.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetSchema")]
	public class UnionInfoCollection : IEnumerable<UnionInfo>, ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private List<UnionInfo> _unions = new List<UnionInfo>();

		/// <summary>
		/// Quantidade de consultas da união.
		/// </summary>
		public int Count
		{
			get
			{
				return _unions.Count;
			}
		}

		/// <summary>
		/// Recupera a consulta que está no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public UnionInfo this[int index]
		{
			get
			{
				return _unions[index];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public UnionInfoCollection()
		{
		}

		/// <summary>
		/// Cria uma instancia da projeção já com as uniões que serão usadas.
		/// </summary>
		/// <param name="unions"></param>
		public UnionInfoCollection(IEnumerable<UnionInfo> unions)
		{
			if(unions == null)
				throw new ArgumentNullException("unions");
			_unions.AddRange(unions);
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private UnionInfoCollection(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("c");
			for(var i = 0; i < count; i++)
				_unions.Add(info.GetValue(i.ToString(), typeof(UnionInfo)) as UnionInfo);
		}

		/// <summary>
		/// Adiciona a união.
		/// </summary>
		/// <param name="union"></param>
		public void Add(UnionInfo union)
		{
			_unions.Add(union);
		}

		/// <summary>
		/// Remove a união.
		/// </summary>
		/// <param name="union"></param>
		public bool Remove(UnionInfo union)
		{
			return _unions.Remove(union);
		}

		/// <summary>
		/// Remove a união no indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public void RemoveAt(int index)
		{
			_unions.RemoveAt(index);
		}

		/// <summary>
		/// Limpa as consultas da união.
		/// </summary>
		public void Clear()
		{
			_unions.Clear();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("Count: {0}", Count);
		}

		/// <summary>
		/// Clona a projeção.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new UnionInfoCollection(_unions.Select(f => (UnionInfo)f.Clone()));
		}

		IEnumerator<UnionInfo> IEnumerable<UnionInfo>.GetEnumerator()
		{
			return _unions.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _unions.GetEnumerator();
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("c", _unions.Count);
			for(var i = 0; i < _unions.Count; i++)
				info.AddValue(i.ToString(), _unions[i]);
		}

		/// <summary>
		/// Recupera o esquema Xml da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("UnionInfoCollection", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				var union = new UnionInfo();
				((System.Xml.Serialization.IXmlSerializable)union).ReadXml(reader);
				_unions.Add(union);
			}
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (System.Xml.Serialization.IXmlSerializable i in _unions)
			{
				writer.WriteStartElement("Union", Namespaces.Query);
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			while (reader.ReadBoolean())
			{
				var union = new UnionInfo();
				((ICompactSerializable)union).Deserialize(reader);
				_unions.Add(union);
			}
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			foreach (ICompactSerializable i in _unions)
			{
				writer.Write(true);
				i.Serialize(writer);
			}
			writer.Write(false);
		}
	}
}
