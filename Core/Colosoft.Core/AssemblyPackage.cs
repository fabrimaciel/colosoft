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
	/// Representa um pacote de assemblies.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class AssemblyPackage : IEnumerable<AssemblyPart>, System.Runtime.Serialization.ISerializable, Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable, IDisposable, IAssemblyPackage
	{
		private Guid _uid = Guid.NewGuid();

		private DateTime _createTime = DateTime.Now;

		private List<AssemblyPart> _items = new List<AssemblyPart>();

		[NonSerialized]
		private IAssemblyPackageResult _result;

		/// <summary>
		/// Identificador único do pacote.
		/// </summary>
		public Guid Uid
		{
			get
			{
				return _uid;
			}
			set
			{
				_uid = value;
			}
		}

		/// <summary>
		/// Horário de criação do pacote.
		/// </summary>
		public DateTime CreateTime
		{
			get
			{
				return _createTime;
			}
			set
			{
				_createTime = value;
			}
		}

		/// <summary>
		/// Recupera a quantidade de nomes no pacote.
		/// </summary>
		public int Count
		{
			get
			{
				return _items.Count;
			}
		}

		/// <summary>
		/// Recupera o nome que está na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public AssemblyPart this[int index]
		{
			get
			{
				return _items[index];
			}
		}

		/// <summary>
		/// Resultado da carga do pacote.
		/// </summary>
		public IAssemblyPackageResult Result
		{
			get
			{
				return _result;
			}
			set
			{
				_result = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AssemblyPackage()
		{
		}

		/// <summary>
		/// Cria um pacote já definindo os assemblies associados.
		/// </summary>
		/// <param name="assemblies"></param>
		public AssemblyPackage(IEnumerable<AssemblyPart> assemblies)
		{
			foreach (var i in assemblies)
				if(!_items.Contains(i, AssemblyPartEqualityComparer.Instance))
					_items.Add(i);
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected AssemblyPackage(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_uid = (Guid)info.GetValue("Uid", typeof(Guid));
			_createTime = info.GetDateTime("CreateTime");
			var count = info.GetInt32("Count");
			for(var i = 0; i < count; i++)
				_items.Add((AssemblyPart)info.GetValue("i" + i, typeof(AssemblyPart)));
		}

		/// <summary>
		/// Adiciona um novo assembly para o pacote.
		/// </summary>
		/// <param name="name"></param>
		public void Add(AssemblyPart name)
		{
			name.Require("name").NotNull();
			if(!_items.Contains(name, AssemblyPartEqualityComparer.Instance))
				_items.Add(name);
		}

		/// <summary>
		/// Remove o nome do item informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Remove(AssemblyPart name)
		{
			return _items.Remove(name);
		}

		/// <summary>
		/// Recupera a instancia do assembly carregado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public System.Reflection.Assembly GetAssembly(AssemblyPart name)
		{
			if(name != null && name.Assembly != null)
				return name.Assembly;
			if(Result == null)
				throw new InvalidOperationException("Result not loaded");
			var assembly = Result.GetAssembly(name);
			return assembly;
		}

		/// <summary>
		/// Recupera o stream do assembly.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public System.IO.Stream GetAssemblyStream(AssemblyPart name)
		{
			if(name != null && name.Assembly != null)
			{
				var fileName = name.Assembly.Location;
				if(System.IO.File.Exists(fileName))
					return System.IO.File.OpenRead(fileName);
			}
			if(Result == null)
				throw new InvalidOperationException("Result not loaded");
			return Result.GetAssemblyStream(name);
		}

		/// <summary>
		/// Carrega o assembly guardado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="exception">Exception caso ocorra.</param>
		/// <returns></returns>
		public System.Reflection.Assembly LoadAssemblyGuarded(AssemblyPart name, out Exception exception)
		{
			if(name != null && name.Assembly != null)
			{
				exception = null;
				return name.Assembly;
			}
			if(Result == null)
				throw new InvalidOperationException("Result not loaded");
			Exception ex2 = null;
			var result = Result.LoadAssemblyGuarded(name, out ex2);
			exception = ex2;
			return result;
		}

		/// <summary>
		/// Extraí os arquivos do pacote.
		/// </summary>
		/// <param name="outputDirectory">Diretório de saída.</param>
		/// <param name="canOverride">True para sobreescrever os arquivos que existirem.</param>
		/// <rereturns>True caso a operação tenha sido realizada com sucesso.</rereturns>
		public bool ExtractPackageFiles(string outputDirectory, bool canOverride)
		{
			if(Result == null)
				return false;
			Result.ExtractPackageFiles(outputDirectory, canOverride);
			return true;
		}

		/// <summary>
		/// Verifica se existe no pacote uma parte compatível com a informada.
		/// </summary>
		/// <param name="assemblyPart">Parte que será comparada.</param>
		/// <returns></returns>
		public bool Contains(AssemblyPart assemblyPart)
		{
			return _items.Contains(assemblyPart, AssemblyPartEqualityComparer.Instance);
		}

		/// <summary>
		/// Recupera o enumerador dos items.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyPart> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			if(_result != null)
				_result.Dispose();
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Uid", _uid);
			info.AddValue("CreateTime", _createTime);
			info.AddValue("Count", _items.Count);
			for(var i = 0; i < _items.Count; i++)
				info.AddValue("i" + i, _items[i]);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			ReflectionNamespace.ResolveReflectionSchema(xs);
			return new System.Xml.XmlQualifiedName("AssemblyPackage", ReflectionNamespace.Data);
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
			if(reader.MoveToAttribute("Uid"))
				this._uid = Guid.Parse(reader.ReadContentAsString());
			if(reader.MoveToAttribute("CreateTime"))
				this._createTime = reader.ReadContentAsDateTime();
			reader.MoveToElement();
			reader.ReadStartElement();
			reader.ReadStartElement("AssemblyParts");
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.LocalName == "AssemblyPart")
				{
					AssemblyPart part = new AssemblyPart();
					((System.Xml.Serialization.IXmlSerializable)part).ReadXml(reader);
					this._items.Add(part);
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Uid", this._uid.ToString());
			writer.WriteStartAttribute("CreateTime");
			writer.WriteValue(_createTime);
			writer.WriteEndAttribute();
			writer.WriteStartElement("AssemblyParts");
			foreach (System.Xml.Serialization.IXmlSerializable i in this._items)
			{
				writer.WriteStartElement("AssemblyPart");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			_uid = reader.ReadGuid();
			_createTime = reader.ReadDateTime();
			var count = reader.ReadInt32();
			while (count-- > 0)
			{
				var part = new AssemblyPart();
				part.Deserialize(reader);
				_items.Add(part);
			}
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(Uid);
			writer.Write(CreateTime);
			writer.Write(_items.Count);
			foreach (var i in _items)
				i.Serialize(writer);
		}
	}
}
