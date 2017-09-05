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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Implementação de uma coleção de pacotes de assembly.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class AssemblyPackageCollection : ICollection<AssemblyPackage>, System.Runtime.Serialization.ISerializable, Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable
	{
		private List<AssemblyPackage> _innerList;

		/// <summary>
		/// Quantidade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _innerList.Count;
			}
		}

		/// <summary>
		/// Identifica se é uma coleção somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Recupera de define o item na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public AssemblyPackage this[int index]
		{
			get
			{
				return _innerList[index];
			}
			set
			{
				_innerList[index] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="packages"></param>
		public AssemblyPackageCollection(IEnumerable<AssemblyPackage> packages)
		{
			packages.Require("packages").NotNull();
			_innerList = new List<AssemblyPackage>(packages);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AssemblyPackageCollection()
		{
			_innerList = new List<AssemblyPackage>();
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected AssemblyPackageCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			var count = info.GetInt32("Count");
			for(var i = 0; i < count; i++)
				_innerList.Add((AssemblyPackage)info.GetValue("i" + i, typeof(AssemblyPackage)));
		}

		/// <summary>
		/// Adiciona um novo item na coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(AssemblyPackage item)
		{
			_innerList.Add(item);
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			_innerList.Clear();
		}

		/// <summary>
		/// Verifica se coleção contém o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(AssemblyPackage item)
		{
			return _innerList.Contains(item);
		}

		/// <summary>
		/// Copia os itens para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(AssemblyPackage[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(AssemblyPackage item)
		{
			return _innerList.Remove(item);
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyPackage> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Count", _innerList.Count);
			for(var i = 0; i < _innerList.Count; i++)
				info.AddValue("i" + i, _innerList[i]);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			ReflectionNamespace.ResolveReflectionSchema(xs);
			return new System.Xml.XmlQualifiedName("ArrayOfAssemblyPackage", ReflectionNamespace.Data);
		}

		/// <summary>
		/// Recupera o esquema.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.LocalName == "AssemblyPackage")
				{
					var part = new AssemblyPackage();
					((System.Xml.Serialization.IXmlSerializable)part).ReadXml(reader);
					_innerList.Add(part);
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (System.Xml.Serialization.IXmlSerializable i in _innerList)
			{
				writer.WriteStartElement("AssemblyPackage");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		void Serialization.ICompactSerializable.Deserialize(Serialization.IO.CompactReader reader)
		{
			var count = reader.ReadInt32();
			_innerList = new List<AssemblyPackage>(count);
			while (count-- > 0)
			{
				var package = new AssemblyPackage();
				package.Deserialize(reader);
				_innerList.Add(package);
			}
		}

		void Serialization.ICompactSerializable.Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(Count);
			foreach (Serialization.ICompactSerializable i in _innerList)
				i.Serialize(writer);
		}
	}
}
