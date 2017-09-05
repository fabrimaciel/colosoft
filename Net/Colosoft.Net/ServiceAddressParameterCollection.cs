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
using System.Collections;
using System.Security.Permissions;

namespace Colosoft.Net
{
	/// <summary>
	/// Representa a coleção de parametros.
	/// </summary>
	[Serializable]
	public sealed class ServiceAddressParameterCollection : IEnumerable<ServiceAddressParameter>, ISerializable, System.Xml.Serialization.IXmlSerializable
	{
		private List<ServiceAddressParameter> _parameters;

		/// <summary>
		/// Recupera o valor do parametro.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public string this[string name]
		{
			get
			{
				var parameter = GetParameter(name);
				return parameter != null ? parameter.Value : null;
			}
			set
			{
				var parameter = GetParameter(name);
				if(parameter != null)
					parameter.Value = value;
				else
				{
					Add(name, value);
				}
			}
		}

		/// <summary>
		/// Recupera o parametro na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public ServiceAddressParameter this[int index]
		{
			get
			{
				return _parameters[index];
			}
			set
			{
				_parameters[index] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServiceAddressParameterCollection()
		{
			_parameters = new List<ServiceAddressParameter>();
		}

		/// <summary>
		/// Construtor usado na deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ServiceAddressParameterCollection(SerializationInfo info, StreamingContext context)
		{
			_parameters = new List<ServiceAddressParameter>();
			var count = info.GetInt32("Count");
			for(var i = 0; i < count; i++)
				_parameters.Add((ServiceAddressParameter)info.GetValue(i.ToString(), typeof(ServiceAddressParameter)));
		}

		/// <summary>
		/// Verifica se a coleção contém o parametro informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public bool Contains(string name)
		{
			return _parameters.Exists(f => f.Name == name);
		}

		/// <summary>
		/// Adiciona um novo parametro para a coleção.
		/// </summary>
		/// <param name="parameter"></param>
		public void Add(object parameter)
		{
			if(!(parameter is ServiceAddressParameter))
				throw new ArgumentException("parameter");
			Add((ServiceAddressParameter)parameter);
		}

		/// <summary>
		/// Adiciona um novo parametro para a coleção.
		/// </summary>
		/// <param name="parameter"></param>
		public void Add(ServiceAddressParameter parameter)
		{
			if(parameter == null)
				throw new ArgumentNullException("parameter");
			if(Contains(parameter.Name))
				throw new ArgumentException(Properties.Resources.Argument_AddingDuplicate);
			_parameters.Add(parameter);
		}

		/// <summary>
		/// Adiciona um novo parametro para a coleção.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">Valor do parametro.</param>
		public void Add(string name, string value)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			Add(new ServiceAddressParameter(name, value));
		}

		/// <summary>
		/// Remove um
		/// </summary>
		/// <param name="name"></param>
		/// <returns>True caso tenha sido removido com sucesso.</returns>
		public bool Remove(string name)
		{
			var index = _parameters.FindIndex(f => f.Name == name);
			if(index < 0)
				return false;
			_parameters.RemoveAt(index);
			return true;
		}

		/// <summary>
		/// Recupera o parametro pelo nome.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ServiceAddressParameter GetParameter(string name)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			return _parameters.Find(f => f.Name == name);
		}

		/// <summary>
		/// Limpa a coleção.
		/// </summary>
		public void Clear()
		{
			_parameters.Clear();
		}

		/// <summary>
		/// Cria uma coleção com base nos atributos XML informados.
		/// </summary>
		/// <param name="attributes"></param>
		/// <returns></returns>
		public static ServiceAddressParameterCollection CreateFromXmlAttributes(System.Xml.XmlAttributeCollection attributes)
		{
			if(attributes == null)
				throw new ArgumentNullException("attributes");
			var result = new ServiceAddressParameterCollection();
			foreach (System.Xml.XmlAttribute i in attributes)
				result.Add(i.LocalName, i.Value);
			return result;
		}

		/// <summary>
		/// Quanitade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _parameters.Count;
			}
		}

		/// <summary>
		/// Identifica se a coleção é sincronizada.
		/// </summary>
		public bool IsSynchronized
		{
			get
			{
				return ((ICollection)_parameters).IsSynchronized;
			}
		}

		/// <summary>
		/// Instancia do objecto usado para a inscronização.
		/// </summary>
		public object SyncRoot
		{
			get
			{
				return ((ICollection)_parameters).SyncRoot;
			}
		}

		/// <summary>
		/// Recupera o enumerador para percorrer os parametros da coleção.
		/// </summary>
		/// <returns></returns>
		public IEnumerator GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Count", _parameters.Count);
			for(var i = 0; i < _parameters.Count; i++)
				info.AddValue(i.ToString(), _parameters[i]);
		}

		/// <summary>
		/// Verifica se na coleção contém o item informado.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(ServiceAddressParameter item)
		{
			return _parameters.Contains(item);
		}

		/// <summary>
		/// Copia os dados da instancia para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(ServiceAddressParameter[] array, int arrayIndex)
		{
			_parameters.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(ServiceAddressParameter item)
		{
			return _parameters.Remove(item);
		}

		IEnumerator<ServiceAddressParameter> IEnumerable<ServiceAddressParameter>.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		/// <summary>
		/// Não usa.
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			throw new NotImplementedException();
		}

		void System.Xml.Serialization.IXmlSerializable.ReadXml(System.Xml.XmlReader reader)
		{
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.Name == "Param")
				{
					var parameter = new ServiceAddressParameter();
					((System.Xml.Serialization.IXmlSerializable)parameter).ReadXml(reader);
					this.Add(parameter);
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		void System.Xml.Serialization.IXmlSerializable.WriteXml(System.Xml.XmlWriter writer)
		{
			foreach (var i in _parameters)
			{
				writer.WriteStartElement("Param");
				i.WriteXml(writer);
				writer.WriteEndElement();
			}
		}
	}
}
