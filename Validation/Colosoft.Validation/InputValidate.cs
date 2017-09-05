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

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Representa o modelo de dados de validação de entrada de dados.
	/// </summary>
	public class InputValidate : NotificationObject, System.Xml.Serialization.IXmlSerializable
	{
		private static int _nextInputValidateId = 1;

		private int _inputValidateId = _nextInputValidateId++;

		private Guid _uid = Guid.NewGuid();

		private List<InputValidateIndexedValue> _indexedValues = new List<InputValidateIndexedValue>();

		private List<InputValidateValue> _values = new List<InputValidateValue>();

		private string _name;

		private InputValidateType _type = InputValidateType.DefaultValue;

		private InputValidateCheckDigits _checkDigits;

		private InputValidateDefault _default = new InputValidateDefault();

		private InputValidateCustomization _customization;

		private InputValidateLength _length;

		private InputValidateMask _mask;

		private InputValidateRange _range;

		private InputValidateValidChars _validChars;

		private InputValidateGroup _group;

		/// <summary>
		/// Identificador da validação de entrada.
		/// </summary>
		public int InputValidateId
		{
			get
			{
				return _inputValidateId;
			}
		}

		/// <summary>
		/// Identificador único.
		/// </summary>
		public Guid Uid
		{
			get
			{
				return _uid;
			}
			set
			{
				if(_uid != value)
				{
					_uid = value;
					RaisePropertyChanged("Uid");
				}
			}
		}

		/// <summary>
		/// Tipo.
		/// </summary>
		public InputValidateType Type
		{
			get
			{
				return _type;
			}
			set
			{
				if(_type != value)
				{
					_type = value;
					switch(value)
					{
					case InputValidateType.CheckDigits:
						CheckDigits = CheckDigits ?? new InputValidateCheckDigits();
						break;
					case InputValidateType.Customization:
						Customization = Customization ?? new InputValidateCustomization();
						break;
					case InputValidateType.DefaultValue:
						Default = Default ?? new InputValidateDefault();
						break;
					case InputValidateType.Length:
						Length = Length ?? new InputValidateLength();
						break;
					case InputValidateType.Mask:
						Mask = Mask ?? new InputValidateMask();
						break;
					case InputValidateType.Range:
						Range = Range ?? new InputValidateRange();
						break;
					case InputValidateType.ValidChars:
						ValidChars = ValidChars ?? new InputValidateValidChars();
						break;
					case InputValidateType.Group:
						Group = Group ?? new InputValidateGroup();
						break;
					}
					RaisePropertyChanged("Type");
				}
			}
		}

		/// <summary>
		/// Nome.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				if(_name != value)
				{
					_name = value;
					RaisePropertyChanged("Name");
				}
			}
		}

		/// <summary>
		/// Validação por digitos verificaveis.
		/// </summary>
		public InputValidateCheckDigits CheckDigits
		{
			get
			{
				return _checkDigits;
			}
			set
			{
				if(_checkDigits != value)
				{
					_checkDigits = value;
					RaisePropertyChanged("CheckDigits");
				}
			}
		}

		/// <summary>
		/// Valor padrão.
		/// </summary>
		public InputValidateDefault Default
		{
			get
			{
				return _default;
			}
			set
			{
				if(_default != value)
				{
					_default = value;
					RaisePropertyChanged("Default");
				}
			}
		}

		/// <summary>
		/// Customização.
		/// </summary>
		public InputValidateCustomization Customization
		{
			get
			{
				return _customization;
			}
			set
			{
				if(_customization != value)
				{
					_customization = value;
					RaisePropertyChanged("Customization");
				}
			}
		}

		/// <summary>
		/// Valor indexados.
		/// </summary>
		public List<InputValidateIndexedValue> IndexedValues
		{
			get
			{
				return _indexedValues;
			}
		}

		/// <summary>
		/// Validação por comprimento.
		/// </summary>
		public InputValidateLength Length
		{
			get
			{
				return _length;
			}
			set
			{
				if(_length != value)
				{
					_length = value;
					RaisePropertyChanged("Length");
				}
			}
		}

		/// <summary>
		/// Validação por máscara.
		/// </summary>
		public InputValidateMask Mask
		{
			get
			{
				return _mask;
			}
			set
			{
				if(_mask != value)
				{
					_mask = value;
					RaisePropertyChanged("Mask");
				}
			}
		}

		/// <summary>
		/// Validação por faixa.
		/// </summary>
		public InputValidateRange Range
		{
			get
			{
				return _range;
			}
			set
			{
				if(_range != value)
				{
					_range = value;
					RaisePropertyChanged("Range");
				}
			}
		}

		/// <summary>
		/// Validação por caracteres.
		/// </summary>
		public InputValidateValidChars ValidChars
		{
			get
			{
				return _validChars;
			}
			set
			{
				if(_validChars != value)
				{
					_validChars = value;
					RaisePropertyChanged("ValidChars");
				}
			}
		}

		/// <summary>
		/// Grupo de validações.
		/// </summary>
		public InputValidateGroup Group
		{
			get
			{
				return _group;
			}
			set
			{
				if(_group != value)
				{
					_group = value;
					RaisePropertyChanged("Group");
				}
			}
		}

		/// <summary>
		/// Validação por valores.
		/// </summary>
		public List<InputValidateValue> Values
		{
			get
			{
				return _values;
			}
		}

		/// <summary>
		/// Recupera o esquema do tipo.
		/// </summary>
		/// <returns></returns>
		System.Xml.Schema.XmlSchema System.Xml.Serialization.IXmlSerializable.GetSchema()
		{
			throw new NotImplementedException();
		}

		private static T ReadElement<T>(System.Xml.XmlReader reader) where T : System.Xml.Serialization.IXmlSerializable, new()
		{
			var item = new T();
			item.ReadXml(reader);
			return item;
		}

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("uid"))
				Uid = Guid.Parse(reader.ReadContentAsString());
			if(reader.MoveToAttribute("name"))
				Name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("type"))
				Type = (InputValidateType)Enum.Parse(typeof(InputValidateType), reader.ReadContentAsString());
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.IsEmptyElement && !reader.HasAttributes)
					{
						reader.Skip();
						continue;
					}
					switch(reader.LocalName)
					{
					case "CheckDigits":
						CheckDigits = ReadElement<InputValidateCheckDigits>(reader);
						break;
					case "Default":
						Default = ReadElement<InputValidateDefault>(reader);
						break;
					case "Customization":
						Customization = ReadElement<InputValidateCustomization>(reader);
						break;
					case "IndexedValues":
						{
							reader.ReadStartElement();
							while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
							{
								if(reader.LocalName == "InputValidateIndexedValue")
								{
									var value = new InputValidateIndexedValue();
									((System.Xml.Serialization.IXmlSerializable)value).ReadXml(reader);
									IndexedValues.Add(value);
								}
								else
									reader.Skip();
							}
							reader.ReadEndElement();
						}
						break;
					case "Length":
						Length = ReadElement<InputValidateLength>(reader);
						break;
					case "Mask":
						Mask = ReadElement<InputValidateMask>(reader);
						break;
					case "Range":
						Range = ReadElement<InputValidateRange>(reader);
						break;
					case "ValidChars":
						ValidChars = ReadElement<InputValidateValidChars>(reader);
						break;
					case "Group":
						Group = ReadElement<InputValidateGroup>(reader);
						break;
					case "Values":
						{
							reader.ReadStartElement();
							while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
							{
								if(reader.LocalName == "InputValidateValue")
								{
									var value = new InputValidateValue();
									((System.Xml.Serialization.IXmlSerializable)value).ReadXml(reader);
									Values.Add(value);
								}
								else
									reader.Skip();
							}
							reader.ReadEndElement();
						}
						break;
					default:
						reader.Skip();
						break;
					}
				}
				reader.ReadEndElement();
			}
		}

		private static void WriteElement<T>(System.Xml.XmlWriter writer, string name, T instance) where T : class, System.Xml.Serialization.IXmlSerializable
		{
			writer.WriteStartElement(name);
			if(instance != null)
				instance.WriteXml(writer);
			writer.WriteEndElement();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("uid", Uid.ToString());
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("type", Type.ToString());
			WriteElement(writer, "CheckDigits", CheckDigits);
			WriteElement(writer, "Default", Default);
			WriteElement(writer, "Customization", Customization);
			writer.WriteStartElement("IndexedValues");
			foreach (var i in IndexedValues)
				WriteElement(writer, "InputValidateIndexedValue", i);
			writer.WriteEndElement();
			WriteElement(writer, "Length", Length);
			WriteElement(writer, "Mask", Mask);
			WriteElement(writer, "Range", Range);
			WriteElement(writer, "ValidChars", ValidChars);
			WriteElement(writer, "Group", Group);
			writer.WriteStartElement("Values");
			foreach (var i in IndexedValues)
				WriteElement(writer, "InputValidateValue", i);
			writer.WriteEndElement();
		}
	}
}
