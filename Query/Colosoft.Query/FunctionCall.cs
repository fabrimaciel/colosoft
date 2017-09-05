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
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Query
{
	/// <summary>
	/// Possíveis opções da chamada de função.
	/// </summary>
	[Flags]
	public enum FunctionCallOptions
	{
		/// <summary>
		/// Nenhum opção.
		/// </summary>
		None = 0,
		/// <summary>
		/// Identifica se a função possui uma opção de distinct.
		/// </summary>
		Distinct = 1
	}
	/// <summary>
	/// Representa uma chamada de função com seus parâmetros.
	/// </summary>
	[Serializable, System.Xml.Serialization.XmlSchemaProvider("GetFunctionCallSchema")]
	public class FunctionCall : ConditionalTerm
	{
		private ConditionalTerm _call;

		private ConditionalTerm[] _parameters;

		private FunctionCallOptions _options;

		private static readonly XmlQualifiedName _qualifiedName = new XmlQualifiedName("FunctionCall", Namespaces.Query);

		private static readonly XmlQualifiedName _listQualifiedName = new XmlQualifiedName("Parameters", Namespaces.Query);

		private static readonly XmlQualifiedName _emptyQualifiedName = new XmlQualifiedName("Empty", Namespaces.Query);

		/// <summary>
		/// O nome da função.
		/// </summary>
		public ConditionalTerm Call
		{
			get
			{
				return _call;
			}
			set
			{
				_call = value;
			}
		}

		/// <summary>
		/// A lista de parâmetros da chamada.
		/// </summary>
		public ConditionalTerm[] Parameters
		{
			get
			{
				return _parameters;
			}
			set
			{
				_parameters = value ?? new ConditionalTerm[0];
			}
		}

		/// <summary>
		/// Opções da chamada de função.
		/// </summary>
		public FunctionCallOptions Options
		{
			get
			{
				return _options;
			}
			set
			{
				_options = value;
			}
		}

		/// <summary>
		/// O nome do elemento na serialização.
		/// </summary>
		public override XmlQualifiedName QualifiedName
		{
			get
			{
				return _qualifiedName;
			}
		}

		/// <summary>
		/// O nome do elemento da lista de parâmetros.
		/// </summary>
		public XmlQualifiedName ListQualifiedName
		{
			get
			{
				return _listQualifiedName;
			}
		}

		/// <summary>
		/// Nome para identificar um elemento nulo na lista de parâmetros.
		/// </summary>
		public XmlQualifiedName EmptyQualifiedName
		{
			get
			{
				return _emptyQualifiedName;
			}
		}

		/// <summary>
		/// Consrutor padrão.
		/// </summary>
		public FunctionCall()
		{
		}

		/// <summary>
		/// Construtor para serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected FunctionCall(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			_call = (ConditionalTerm)info.GetValue("M", Type.GetType(info.GetString("T"), true));
			var size = info.GetInt32("S");
			_parameters = new ConditionalTerm[size];
			for(var i = 0; i < size; ++i)
				_parameters[i] = GetTerm(info, i);
			_options = (FunctionCallOptions)info.GetInt32("Options");
		}

		/// <summary>
		/// Recupera a informação de esquema para o elemento.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static XmlQualifiedName GetFunctionCallSchema(XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new XmlQualifiedName("FunctionCall", Namespaces.Query);
		}

		/// <summary>
		/// Retorna a represetação em formato texto do elemento.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("{0}({1}{2})", _call.ToString(), _options != FunctionCallOptions.None ? string.Format("{0} ", _options.ToString()) : "", _parameters.IsNullOrEmpty() ? String.Empty : String.Join(", ", _parameters.Select(p => (p == null) ? "NULL" : p.ToString())));
		}

		/// <summary>
		/// Cria uma cópia do objeto de todos os seus membros.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new FunctionCall {
				Call = (ConditionalTerm)_call.Clone(),
				Parameters = _parameters.Select(p => (ConditionalTerm)p.Clone()).ToArray(),
				Options = _options
			};
		}

		/// <summary>
		/// Lê um termo serializado com as informações da posição <paramref name="index"/>.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		private static ConditionalTerm GetTerm(SerializationInfo info, int index)
		{
			var tName = String.Format("t{0}", index);
			var pName = String.Format("p{0}", index);
			var tText = info.GetString(tName);
			if("NULL".Equals(tText))
			{
				return null;
			}
			return (ConditionalTerm)info.GetValue(pName, Type.GetType(tText, true));
		}

		/// <summary>
		/// Serializa um termo na posição <paramref name="index"/>.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="index"></param>
		/// <param name="term"></param>
		private static void SetTerm(SerializationInfo info, int index, ConditionalTerm term)
		{
			var tName = String.Format("t{0}", index);
			var pName = String.Format("p{0}", index);
			if(term == null)
			{
				info.AddValue(tName, "NULL");
			}
			else
			{
				info.AddValue(tName, term.GetType().FullName);
				info.AddValue(pName, term);
			}
		}

		/// <summary>
		/// Serializa o termo em formato xml.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="term"></param>
		private static void WriteTerm(XmlWriter writer, ConditionalTerm term)
		{
			var qName = term.QualifiedName;
			writer.WriteStartElement(qName.Name, qName.Namespace);
			if(!"ConditionalTerm".Equals(qName.Name))
			{
				var prefix = writer.LookupPrefix(qName.Namespace);
				if(string.IsNullOrEmpty(prefix))
				{
					writer.WriteAttributeString("xmlns", qName.Namespace);
				}
			}
			((IXmlSerializable)term).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Recupera os dados da instância anteriormente serializada.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("T", _call.GetType().FullName);
			info.AddValue("M", _call);
			info.AddValue("Options", (int)_options);
			var size = _parameters.IsNullOrEmpty() ? 0 : _parameters.Length;
			info.AddValue("S", size);
			for(var i = 0; i < size; ++i)
			{
				SetTerm(info, i, _parameters[i]);
			}
		}

		/// <summary>
		/// Serializa a instância em formato XML.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(XmlWriter writer)
		{
			writer.WriteAttributeString("options", ((int)_options).ToString());
			WriteTerm(writer, _call);
			var pName = ListQualifiedName;
			writer.WriteStartElement(pName.Name, pName.Namespace);
			foreach (var term in _parameters)
			{
				if(term == null)
				{
					writer.WriteStartElement(EmptyQualifiedName.Name, EmptyQualifiedName.Namespace);
					writer.WriteEndElement();
				}
				else
				{
					WriteTerm(writer, term);
				}
			}
			writer.WriteEndElement();
		}

		/// <summary>
		/// Desserializa a instância previamente serializada em formato XML.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(XmlReader reader)
		{
			if(reader.IsEmptyElement)
			{
				reader.Skip();
			}
			else
			{
				reader.MoveToAttribute("options");
				_options = (FunctionCallOptions)reader.ReadContentAsInt();
				reader.MoveToElement();
				reader.ReadStartElement();
				_call = GetConditionalTerm(reader);
				var par = new List<ConditionalTerm>();
				reader.ReadStartElement(ListQualifiedName.Name, ListQualifiedName.Namespace);
				while (reader.NodeType != XmlNodeType.EndElement)
				{
					par.Add(GetConditionalTerm(reader));
				}
				_parameters = par.ToArray();
				reader.ReadEndElement();
				reader.ReadEndElement();
			}
		}

		/// <summary>
		/// Serializa a instância em formato binário.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(CompactWriter writer)
		{
			writer.Write(_call.GetType().Name);
			((ICompactSerializable)_call).Serialize(writer);
			writer.Write((int)_options);
			var size = _parameters.IsNullOrEmpty() ? 0 : _parameters.Length;
			writer.Write(size);
			if(size == 0)
			{
				return;
			}
			foreach (var term in _parameters)
			{
				if(term == null)
				{
					writer.Write(false);
				}
				else
				{
					writer.Write(true);
					writer.Write(term.GetType().Name);
					((ICompactSerializable)term).Serialize(writer);
				}
			}
		}

		/// <summary>
		/// Desserializa a instância previamente serializada em formato binário.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(CompactReader reader)
		{
			_call = GetConditionalTerm(reader);
			_options = (FunctionCallOptions)(int)reader.ReadInt32();
			var size = reader.ReadInt32();
			_parameters = new ConditionalTerm[size];
			for(var i = 0; i < size; ++i)
			{
				if(reader.ReadBoolean())
				{
					_parameters[i] = GetConditionalTerm(reader);
				}
			}
		}
	}
}
