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

using Colosoft.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Validation.Configuration
{
	/// <summary>
	/// Propriedade de uma versão da entidade.
	/// </summary>
	public class EntityTypeVersionProperty : NotificationObject, System.Xml.Serialization.IXmlSerializable, IEntityPropertyInfo
	{
		private IObservableCollection<PropertyDefaultLabel> _defaultLabels = new BaseObservableCollection<PropertyDefaultLabel>();

		private string _name;

		private string _typeName;

		private string _namespace;

		private string _assembly;

		private Guid? _inputRulesGroupUid;

		private bool _isInstance;

		/// <summary>
		/// Rotulos padrão.
		/// </summary>
		public IObservableCollection<PropertyDefaultLabel> DefaultLabels
		{
			get
			{
				return _defaultLabels;
			}
		}

		/// <summary>
		/// Nome da propriedade.
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
		/// Nome do tipo.
		/// </summary>
		public string TypeName
		{
			get
			{
				return _typeName;
			}
			set
			{
				if(_typeName != value)
				{
					_typeName = value;
					RaisePropertyChanged("TypeName", "Type");
				}
			}
		}

		/// <summary>
		/// Namespace.
		/// </summary>
		public string Namespace
		{
			get
			{
				return _namespace;
			}
			set
			{
				if(_namespace != value)
				{
					_namespace = value;
					RaisePropertyChanged("Namespace", "Type");
				}
			}
		}

		/// <summary>
		/// Assembly.
		/// </summary>
		public string Assembly
		{
			get
			{
				return _assembly;
			}
			set
			{
				if(_assembly != value)
				{
					_assembly = value;
					RaisePropertyChanged("Assembly", "Type");
				}
			}
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public Reflection.TypeName Type
		{
			get
			{
				var name = string.Join(".", new[] {
					Namespace,
					TypeName
				}.Where(f => f != null));
				if(string.IsNullOrEmpty(name))
					return null;
				if(!string.IsNullOrEmpty(Assembly))
					return new Reflection.TypeName(string.Format("{0}, {1}", name, Assembly));
				return new Reflection.TypeName(name);
			}
			set
			{
				if(Type != value)
				{
					if(value != null)
					{
						_typeName = value.Name;
						_namespace = string.Join(".", value.Namespace);
						_assembly = value.AssemblyName.Name;
					}
					else
					{
						_typeName = null;
						_namespace = null;
						_assembly = null;
					}
					RaisePropertyChanged("TypeName", "Namespace", "Assembly");
				}
			}
		}

		/// <summary>
		/// Identificador do grupo de regras de entrada.
		/// </summary>
		public Guid? InputRulesGroupUid
		{
			get
			{
				return _inputRulesGroupUid;
			}
			set
			{
				if(_inputRulesGroupUid != value)
				{
					_inputRulesGroupUid = value;
					RaisePropertyChanged("InputRulesGroupUid");
				}
			}
		}

		/// <summary>
		/// Indica que deve ser buscado configurações de validação na própria instância.
		/// </summary>
		public bool IsInstance
		{
			get
			{
				return _isInstance;
			}
			set
			{
				if(_isInstance != value)
				{
					_isInstance = value;
					RaisePropertyChanged("IsInstance");
				}
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

		/// <summary>
		/// Lê os dados serializados.
		/// </summary>
		/// <param name="reader"></param>
		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("name"))
				Name = reader.ReadContentAsString();
			if(reader.MoveToAttribute("isInstance"))
				IsInstance = reader.ReadContentAsBoolean();
			if(reader.MoveToAttribute("type"))
			{
				var type = reader.ReadContentAsString();
				Type = !string.IsNullOrEmpty(type) ? new Reflection.TypeName(reader.ReadContentAsString()) : null;
			}
			if(reader.MoveToAttribute("inputRulesGroup"))
			{
				var inputRulesGroupUid = reader.ReadContentAsString();
				InputRulesGroupUid = !string.IsNullOrEmpty(inputRulesGroupUid) ? (Guid?)Guid.Parse(inputRulesGroupUid) : null;
			}
			reader.MoveToElement();
			if(!reader.IsEmptyElement)
			{
				reader.ReadStartElement();
				while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
				{
					if(reader.LocalName == "DefaultLabels" && !reader.IsEmptyElement)
					{
						reader.ReadStartElement();
						while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
						{
							if(reader.LocalName == "PropertyDefaultLabel")
							{
								var label = new PropertyDefaultLabel();
								((System.Xml.Serialization.IXmlSerializable)label).ReadXml(reader);
								DefaultLabels.Add(label);
							}
							else
								reader.Skip();
						}
						reader.ReadEndElement();
					}
					else
						reader.Skip();
				}
				reader.ReadEndElement();
			}
			else
				reader.Skip();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("isInstance", IsInstance.ToString().ToLower());
			writer.WriteAttributeString("type", Type != null ? Type.AssemblyQualifiedName : null);
			writer.WriteAttributeString("inputRulesGroup", InputRulesGroupUid.HasValue ? InputRulesGroupUid.ToString() : null);
			writer.WriteStartElement("DefaultLabels");
			foreach (System.Xml.Serialization.IXmlSerializable label in DefaultLabels)
			{
				writer.WriteStartElement("PropertyDefaultLabel");
				label.WriteXml(writer);
				writer.WriteEndElement();
			}
			writer.WriteEndElement();
		}

		string IEntityPropertyInfo.FullName
		{
			get
			{
				return Name;
			}
		}

		bool IEntityPropertyInfo.IsInstance
		{
			get
			{
				return IsInstance;
			}
		}

		Reflection.TypeName IEntityPropertyInfo.PropertyType
		{
			get
			{
				return Type;
			}
		}
	}
}
