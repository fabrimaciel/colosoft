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
	/// Tipo de junção.
	/// </summary>
	public enum JoinType
	{
		/// <summary>
		/// INNER JOIN.
		/// </summary>
		Inner,
		/// <summary>
		/// Junção a partir de esquerda.
		/// </summary>
		Left,
		/// <summary>
		/// Junção a partir de direita.
		/// </summary>
		Right,
		/// <summary>
		/// Jução cruzada.
		/// </summary>
		Cross
	}
	/// <summary>
	/// Armazena as informações de uma junção de uma consulta.
	/// </summary>
	[Serializable]
	public sealed class JoinInfo : ISerializable, System.Xml.Serialization.IXmlSerializable, ICloneable, ICompactSerializable
	{
		private string _left;

		private string _right;

		private ConditionalContainer _conditional;

		private JoinType _type = JoinType.Inner;

		/// <summary>
		/// Apelido da entidade da esquerda.
		/// </summary>
		public string Left
		{
			get
			{
				return _left;
			}
			set
			{
				_left = value;
			}
		}

		/// <summary>
		/// Apelido da entidade de direita.
		/// </summary>
		public string Right
		{
			get
			{
				return _right;
			}
			set
			{
				_right = value;
			}
		}

		/// <summary>
		/// Condicional da junção.
		/// </summary>
		public ConditionalContainer Conditional
		{
			get
			{
				return _conditional;
			}
			set
			{
				_conditional = value;
			}
		}

		/// <summary>
		/// Tipo da junção.
		/// </summary>
		public JoinType Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public JoinInfo()
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private JoinInfo(SerializationInfo info, StreamingContext context)
		{
			_left = info.GetString("Left");
			_right = info.GetString("Right");
			_conditional = info.GetValue("Conditional", typeof(ConditionalContainer)) as ConditionalContainer;
			_type = (JoinType)Enum.Parse(typeof(JoinType), info.GetString("Type"));
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(Type == JoinType.Cross)
				return string.Format("Cross Join {0}", _left);
			return string.Format("[Left:'{0}', Right:'{1}']", _left, _right);
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Left", _left);
			info.AddValue("Right", _right);
			info.AddValue("Conditional", _conditional);
			info.AddValue("Type", _type.ToString());
		}

		private static System.Xml.XmlQualifiedName GetSchema(System.Xml.Schema.XmlSchemaSet xs)
		{
			xs.ResolveQuerySchema();
			return new System.Xml.XmlQualifiedName("JoinInfo", Namespaces.Query);
		}

		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("Left"))
				_left = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Right"))
				_right = reader.ReadContentAsString();
			if(reader.MoveToAttribute("Type"))
				_type = (JoinType)Enum.Parse(typeof(JoinType), reader.ReadContentAsString());
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				if(!reader.IsEmptyElement && reader.LocalName == "Conditional")
				{
					_conditional = new ConditionalContainer();
					((System.Xml.Serialization.IXmlSerializable)_conditional).ReadXml(reader);
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("Left", _left);
			writer.WriteAttributeString("Right", _right);
			writer.WriteAttributeString("Type", _type.ToString());
			writer.WriteStartElement("Conditional", Namespaces.Query);
			if(_conditional != null)
				((System.Xml.Serialization.IXmlSerializable)_conditional).WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Clona as informações de join.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new JoinInfo {
				Type = this.Type,
				Left = this.Left,
				Right = this.Right,
				Conditional = (ConditionalContainer)this.Conditional
			};
		}

		/// <summary>
		/// Desserializa o objeto.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			_left = reader.ReadString();
			_right = reader.ReadString();
			_type = (JoinType)Enum.Parse(typeof(JoinType), reader.ReadString());
			if(reader.ReadBoolean())
			{
				_conditional = new ConditionalContainer();
				((ICompactSerializable)_conditional).Deserialize(reader);
			}
		}

		/// <summary>
		/// Serializa o objeto.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(_left);
			writer.Write(_right);
			writer.Write(_type.ToString());
			if(_conditional != null)
			{
				writer.Write(true);
				((ICompactSerializable)_conditional).Serialize(writer);
			}
			else
			{
				writer.Write(false);
			}
		}
	}
}
