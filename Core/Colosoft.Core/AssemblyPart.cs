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
	/// Representa uma parte de assembly.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class AssemblyPart : System.Runtime.Serialization.ISerializable, Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable
	{
		private string _source;

		[NonSerialized]
		private System.Reflection.Assembly _assembly;

		/// <summary>
		/// Origem.
		/// </summary>
		public string Source
		{
			get
			{
				return _source;
			}
			set
			{
				_source = value;
			}
		}

		/// <summary>
		/// Instancia do assembly associado que foi carregado.
		/// </summary>
		[System.Xml.Serialization.XmlIgnore]
		internal System.Reflection.Assembly Assembly
		{
			get
			{
				return _assembly;
			}
			set
			{
				_assembly = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AssemblyPart()
		{
		}

		/// <summary>
		/// Cria a instancia já definindo o source.
		/// </summary>
		/// <param name="source"></param>
		public AssemblyPart(string source)
		{
			_source = source;
		}

		/// <summary>
		/// Cria a parte já associando o assembly.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="assembly"></param>
		internal AssemblyPart(string source, System.Reflection.Assembly assembly) : this(source)
		{
			_assembly = assembly;
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected AssemblyPart(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_source = info.GetString("Source");
		}

		/// <summary>
		/// Carrega o assembly pela stream informada.
		/// </summary>
		/// <param name="appDomain">Domínio onde o assembly será carregado.</param>
		/// <param name="assemblyStream"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		[System.Security.SecuritySafeCritical]
		public System.Reflection.Assembly Load(AppDomain appDomain, System.IO.Stream assemblyStream, int length)
		{
			byte[] buffer = new byte[length];
			int offset = 0;
			while (length > 0)
			{
				int num3 = assemblyStream.Read(buffer, offset, length);
				if(num3 == 0)
					break;
				offset += num3;
				length -= num3;
			}
			if(appDomain != null)
				return appDomain.Load(buffer);
			else
				return System.Reflection.Assembly.Load(buffer);
		}

		/// <summary>
		/// Carrega o assembly pelo buffer informadao.
		/// </summary>
		/// <param name="appDomain">Domínio onde o assembly será carregado.</param>
		/// <param name="raw"></param>
		/// <returns></returns>
		[System.Security.SecuritySafeCritical]
		public System.Reflection.Assembly Load(AppDomain appDomain, byte[] raw)
		{
			raw.Require("raw").NotNull();
			if(appDomain != null)
				return appDomain.Load(raw);
			else
				return System.Reflection.Assembly.Load(raw);
		}

		/// <summary>
		/// Carrega o assembly pelo caminho informado
		/// </summary>
		/// <param name="appDomain">Domínio onde o assembly será carregado.</param>
		/// <param name="assemblyCodeBase"></param>
		/// <returns></returns>
		[System.Security.SecuritySafeCritical]
		public System.Reflection.Assembly Load(AppDomain appDomain, string assemblyCodeBase)
		{
			assemblyCodeBase.Require("assemblyCodeBase").NotNull();
			var name = System.Reflection.AssemblyName.GetAssemblyName(assemblyCodeBase);
			if(appDomain != null)
				return appDomain.Load(name);
			else
				return System.Reflection.Assembly.Load(name);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _source;
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Source", _source);
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			_source = reader.ReadString();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(_source);
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			ReflectionNamespace.ResolveReflectionSchema(xs);
			return new System.Xml.XmlQualifiedName("AssemblyPart", ReflectionNamespace.Data);
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
			if(reader.MoveToAttribute("Source"))
				_source = reader.ReadContentAsString();
			reader.MoveToElement();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Source", _source);
		}
	}
	/// <summary>
	/// Comparador do nome do assembly.
	/// </summary>
	public class AssemblyPartEqualityComparer : IEqualityComparer<AssemblyPart>
	{
		/// <summary>
		/// Instancia unico do comparador.
		/// </summary>
		public static readonly AssemblyPartEqualityComparer Instance = new AssemblyPartEqualityComparer();

		/// <summary>
		/// Compara os valores.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public bool Equals(AssemblyPart x, AssemblyPart y)
		{
			return string.Equals(x.Source, y.Source);
		}

		/// <summary>
		/// Recupera o hash code da instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int GetHashCode(AssemblyPart obj)
		{
			if(!object.ReferenceEquals(obj, null))
				return obj.GetHashCode();
			return 0;
		}
	}
}
