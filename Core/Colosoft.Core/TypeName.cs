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
using Colosoft.Text;

namespace Colosoft.Reflection
{
	/// <summary>
	/// Parses names of .NET types.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlSchemaProvider("GetMySchema")]
	public class TypeName : System.Runtime.Serialization.ISerializable, Serialization.ICompactSerializable, System.Xml.Serialization.IXmlSerializable
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public TypeName()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TypeName"/> class.
		/// </summary>
		/// <param name="assemblyQualifiedName">Assembly qualified name of the type.</param>
		public TypeName(string assemblyQualifiedName)
		{
			new Parser().Parse(assemblyQualifiedName, this);
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected TypeName(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			var name = info.GetString("AssemblyQualifiedName");
			new Parser().Parse(name, this);
		}

		/// <summary>
		/// Gets or sets the name of the type, e.g. "IEnumerable"
		/// </summary>
		public string Name
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the namespace as a list, e.g. ["System", "Collections", "Generic"].
		/// </summary>
		public IList<string> Namespace
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the container types.
		/// </summary>
		public IList<string> Nesting
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the list of type arguments, e.g. [ "System.String" ].
		/// </summary>
		public IList<TypeName> TypeArguments
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets or sets the name of the assembly.
		/// </summary>
		public System.Reflection.AssemblyName AssemblyName
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether the System.Type is a pointer.
		/// </summary>
		public bool IsPointer
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets a value indicating whether the System.Type is passed by reference.
		/// </summary>
		public bool IsByRef
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the full name of the type, including the assembly name.
		/// </summary>
		public string AssemblyQualifiedName
		{
			get
			{
				if(AssemblyName == null)
					return FullName;
				else
					return string.Format("{0}, {1}", FullName, AssemblyName.FullName);
			}
		}

		/// <summary>
		/// Gets the full name of the type.
		/// </summary>
		public string FullName
		{
			get
			{
				var args = TypeArguments.Select(t => t.AssemblyQualifiedName).DelimitWith(",", format: "[{0}]", prefix: string.Format("`{0}[", TypeArguments.Count), suffix: "]");
				return string.Concat(Namespace.DelimitWith("", "{0}."), Nesting.DelimitWith("", "{0}+"), Name, args, Suffix);
			}
		}

		/// <summary>
		/// Sufixo.
		/// </summary>
		private string Suffix
		{
			get
			{
				var result = new StringBuilder();
				if(IsPointer)
				{
					result.Append('*');
				}
				if(IsByRef)
				{
					result.Append('&');
				}
				return result.ToString();
			}
		}

		/// <summary>
		/// Recupera o nome para o tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static TypeName Get<T>()
		{
			return new TypeName(typeof(T).AssemblyQualifiedName);
		}

		/// <summary>
		/// Recupera o nome para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static TypeName Get(Type type)
		{
			type.Require("type").NotNull();
			return new TypeName(type.AssemblyQualifiedName);
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			var args = TypeArguments.Select(r => r.ToString()).DelimitWith(", ", null, "<", ">");
			return Name + args + Suffix;
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("AssemblyQualifiedName", AssemblyQualifiedName);
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(Serialization.IO.CompactReader reader)
		{
			var name = reader.ReadString();
			new Parser().Parse(name, this);
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(AssemblyQualifiedName);
		}

		/// <summary>
		/// Implementação usada para compara o nome dos tipos.
		/// </summary>
		public class TypeNameEqualityComparer : IEqualityComparer<TypeName>
		{
			/// <summary>
			/// Instancia única do comparador.
			/// </summary>
			public readonly static TypeNameEqualityComparer Instance = new TypeNameEqualityComparer();

			/// <summary>
			/// Compara as duas instancias.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(TypeName x, TypeName y)
			{
				var xIsNull = object.ReferenceEquals(x, null);
				var yIsNull = object.ReferenceEquals(y, null);
				if(xIsNull && yIsNull)
					return true;
				else if((!xIsNull && yIsNull) || (xIsNull && !yIsNull))
					return false;
				var xName = string.Concat(x.FullName, ", ", x.AssemblyName.Name);
				var yName = string.Concat(y.FullName, ", ", y.AssemblyName.Name);
				return xName.Equals(yName);
			}

			/// <summary>
			/// Recupera o hashcode da instancia informada.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(TypeName obj)
			{
				if(object.ReferenceEquals(obj, null))
					return 0;
				return string.Concat(obj.FullName, ", ", obj.AssemblyName.Name).GetHashCode();
			}
		}

		/// <summary>
		/// Implementação do comparador do fullname do tipo.
		/// </summary>
		public class TypeNameFullNameComparer : IComparer<TypeName>, IEqualityComparer<TypeName>
		{
			/// <summary>
			/// Instancia única do comparador.
			/// </summary>
			public readonly static TypeNameFullNameComparer Instance = new TypeNameFullNameComparer();

			/// <summary>
			/// Compara os dois nomes informados.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(TypeName x, TypeName y)
			{
				var xIsNull = object.ReferenceEquals(x, null);
				var yIsNull = object.ReferenceEquals(y, null);
				if(xIsNull && yIsNull)
					return 0;
				else if((!xIsNull && yIsNull) || (xIsNull && !yIsNull))
					return -1;
				return StringComparer.Ordinal.Compare(x.FullName, y.FullName);
			}

			/// <summary>
			/// Compara as duas instancias.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public bool Equals(TypeName x, TypeName y)
			{
				var xIsNull = object.ReferenceEquals(x, null);
				var yIsNull = object.ReferenceEquals(y, null);
				if(xIsNull && yIsNull)
					return true;
				else if((!xIsNull && yIsNull) || (xIsNull && !yIsNull))
					return false;
				return StringComparer.Ordinal.Equals(x.FullName, y.FullName);
			}

			/// <summary>
			/// Recupera o hashcode da instancia informada.
			/// </summary>
			/// <param name="obj"></param>
			/// <returns></returns>
			public int GetHashCode(TypeName obj)
			{
				if(object.ReferenceEquals(obj, null) || obj.FullName == null)
					return 0;
				return obj.FullName.GetHashCode();
			}
		}

		/// <summary>
		/// Classe que executa o parser do nome do tipo.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
		private sealed class Parser
		{
			private System.IO.TextReader _reader;

			private char? _nextChar;

			private char Read()
			{
				var result = _nextChar ?? '\0';
				ReadNext();
				if(_nextChar == '\\')
				{
					ReadNext();
				}
				return result;
			}

			private void ReadNext()
			{
				var next = _reader.Read();
				_nextChar = next >= 0 ? (char)next : (char?)null;
			}

			private void Read(char expected)
			{
				if(Read() != expected)
				{
					throw new FormatException(string.Format("Expected '{0}'.", expected));
				}
			}

			private void IgnoreSpaces()
			{
				do
				{
					Read();
				}
				while (_nextChar == ' ');
			}

			private void ReadUntil(StringBuilder buffer, string delimiters)
			{
				while (_nextChar != null && delimiters.IndexOf(_nextChar.Value) < 0)
				{
					buffer.Append(Read());
				}
			}

			private string ReadUntil(string delimiters)
			{
				var buffer = new StringBuilder();
				ReadUntil(buffer, delimiters);
				return buffer.ToString();
			}

			public void Parse(string assemblyQualifiedName, TypeName typeName)
			{
				_reader = new System.IO.StringReader(assemblyQualifiedName);
				Read();
				TypeSpec(typeName);
				if(_nextChar != null)
				{
					throw new FormatException("There are remaining unparsed characters.");
				}
			}

			private void TypeSpec(TypeName typeName)
			{
				var @namespace = new List<string>();
				while (true)
				{
					@namespace.Add(ReadUntil(".,+`[&*"));
					if(_nextChar != '.')
					{
						break;
					}
					Read('.');
				}
				var typeNameList = @namespace;
				var nesting = new List<string>();
				if(_nextChar == '+')
				{
					while (true)
					{
						nesting.Add(ReadUntil(",+`&*"));
						if(_nextChar != '+')
						{
							break;
						}
						Read('+');
					}
					typeNameList = nesting;
				}
				typeName.Name = typeNameList[typeNameList.Count - 1];
				typeNameList.RemoveAt(typeNameList.Count - 1);
				typeName.Namespace = new System.Collections.ObjectModel.ReadOnlyCollection<string>(@namespace);
				typeName.Nesting = new System.Collections.ObjectModel.ReadOnlyCollection<string>(nesting);
				while (_nextChar == '[')
				{
					typeName.Name += ReadUntil("]") + ']';
					Read(']');
				}
				if(_nextChar == '`')
				{
					Read('`');
					var argCount = int.Parse(ReadUntil("["), System.Globalization.CultureInfo.InvariantCulture);
					var typeArgs = new TypeName[argCount];
					Read('[');
					for(var i = 0; i < argCount; ++i)
					{
						Read('[');
						typeArgs[i] = new TypeName();
						TypeSpec(typeArgs[i]);
						if(i < argCount - 1)
						{
							ReadUntil("[");
						}
						else
						{
							Read(']');
						}
					}
					typeName.TypeArguments = new System.Collections.ObjectModel.ReadOnlyCollection<TypeName>(typeArgs);
					Read(']');
				}
				else
				{
					typeName.TypeArguments = _emptyTypes;
				}
				if(_nextChar == '*')
				{
					Read('*');
					typeName.IsPointer = true;
				}
				if(_nextChar == '&')
				{
					Read('&');
					typeName.IsByRef = true;
				}
				if(_nextChar == ',')
				{
					IgnoreSpaces();
					var assemblyName = ReadUntil("]");
					typeName.AssemblyName = new System.Reflection.AssemblyName(assemblyName);
				}
			}

			private static readonly IList<TypeName> _emptyTypes = new TypeName[0];
		}

		/// <summary>
		/// Recupera o esquema XML do tipo.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static System.Xml.XmlQualifiedName GetMySchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			ReflectionNamespace.ResolveReflectionSchema(xs);
			return new System.Xml.XmlQualifiedName("TypeName", ReflectionNamespace.Data);
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
			if(reader.MoveToAttribute("AssemblyQualifiedName"))
				new Parser().Parse(reader.ReadContentAsString(), this);
			reader.MoveToElement();
			reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("AssemblyQualifiedName", AssemblyQualifiedName);
		}
	}
}
