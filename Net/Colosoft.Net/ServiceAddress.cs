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
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

namespace Colosoft.Net
{
	/// <summary>
	/// Representa o endereço de um serviço.
	/// </summary>
	[Serializable]
	[XmlSchemaProvider("MySchema")]
	public sealed class ServiceAddress : ISerializable, System.Xml.Serialization.IXmlSerializable
	{
		private static Dictionary<string, Func<System.ServiceModel.Channels.Binding>> _bindingCreators;

		/// <summary>
		/// Nome do serviço.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Endereço do serviço.
		/// </summary>
		public string Address
		{
			get;
			set;
		}

		/// <summary>
		/// Nó contendo a configuração do endereço.
		/// </summary>
		public ServiceAddressNode Configuration
		{
			get;
			set;
		}

		/// <summary>
		/// Nós customizados.
		/// </summary>
		public IEnumerable<ServiceAddressNode> CustomNodes
		{
			get
			{
				var customNode = Configuration.FirstOrDefault(f => f.Name == "custom");
				if(customNode != null)
					return customNode;
				return new ServiceAddressNode[0];
			}
		}

		/// <summary>
		/// Geral.
		/// </summary>
		static ServiceAddress()
		{
			_bindingCreators = new Dictionary<string, Func<System.ServiceModel.Channels.Binding>> {
				{
					"basicHttpBinding",
					() => new System.ServiceModel.BasicHttpBinding()
				},
				{
					"wsHttpBinding",
					() => new System.ServiceModel.WSHttpBinding()
				},
				{
					"netTcpBinding",
					() => new System.ServiceModel.NetTcpBinding()
				}
			};
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServiceAddress()
		{
		}

		/// <summary>
		/// Cria um novo endereço com as dados de configuração informados.
		/// </summary>
		/// <param name="name">Nome do serviço.</param>
		/// <param name="address">Endereço do serviço.</param>
		/// <param name="bindingType"></param>
		/// <param name="contract"></param>
		/// <param name="bindingConfiguration"></param>
		/// <param name="identity"></param>
		public ServiceAddress(string name, string address, string bindingType, string contract, ServiceAddressNode bindingConfiguration, ServiceAddressNode identity) : this(name, address, bindingType, contract, bindingConfiguration, identity, null)
		{
		}

		/// <summary>
		/// Cria um novo endereço com as dados de configuração informados.
		/// </summary>
		/// <param name="name">Nome do serviço.</param>
		/// <param name="address">Endereço do serviço.</param>
		/// <param name="bindingType"></param>
		/// <param name="contract"></param>
		/// <param name="bindingConfiguration"></param>
		/// <param name="identity"></param>
		/// <param name="customNodes">Nós de customização.</param>
		public ServiceAddress(string name, string address, string bindingType, string contract, ServiceAddressNode bindingConfiguration, ServiceAddressNode identity, ServiceAddressNode[] customNodes)
		{
			if(string.IsNullOrEmpty(name))
				throw new ArgumentNullException("name");
			else if(string.IsNullOrEmpty(address))
				throw new ArgumentNullException("address");
			this.Name = name;
			this.Address = address;
			var endPointNode = CreateEndpointNode(name, bindingType, contract, identity);
			var children = new List<ServiceAddressNode>();
			children.Add(endPointNode);
			children.Add(bindingConfiguration);
			if(customNodes != null)
			{
				var customNode = new ServiceAddressNode("custom", null, customNodes);
				children.Add(customNode);
			}
			if(identity != null)
				children.Add(identity);
			Configuration = new ServiceAddressNode("configuration", null, children.ToArray());
		}

		/// <summary>
		/// Construtor usado na deserialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		private ServiceAddress(SerializationInfo info, StreamingContext context)
		{
			Name = info.GetString("Name");
			Address = info.GetString("Address");
			Configuration = (ServiceAddressNode)info.GetValue("Configuration", typeof(ServiceAddressNode));
		}

		/// <summary>
		/// Cria um nó com os dados basicos de um endpoint.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="bindingType"></param>
		/// <param name="contract"></param>
		/// <param name="children"></param>
		/// <returns></returns>
		private ServiceAddressNode CreateEndpointNode(string name, string bindingType, string contract, params ServiceAddressNode[] children)
		{
			var parameters = new ServiceAddressParameterCollection {
				new ServiceAddressParameter("binding", bindingType),
				new ServiceAddressParameter("contract", contract),
				new ServiceAddressParameter("name", name)
			};
			return new ServiceAddressNode("endpoint", parameters, children != null ? children.Where(f => f != null).ToArray() : null);
		}

		/// <summary>
		/// Recupera o nó com os dados básicos do endpoint.
		/// </summary>
		/// <returns></returns>
		private ServiceAddressNode GetEndpointNode()
		{
			return Configuration.GetNode("endpoint");
		}

		/// <summary>
		/// Recupera o nó com os dados básicos do binding.
		/// </summary>
		/// <returns></returns>
		private ServiceAddressNode GeBindingNode()
		{
			return Configuration.GetNode("binding");
		}

		/// <summary>
		/// Preenche as propriedades do objeto.
		/// </summary>
		/// <param name="node"></param>
		/// <param name="refObject"></param>
		private static void FillObjectProperties(ServiceAddressNode node, object refObject)
		{
			if(refObject == null)
				return;
			var properties = refObject.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
			foreach (ServiceAddressParameter i in node.Parameters)
			{
				var pp = properties.Where(f => string.Compare(f.Name, i.Name, true) == 0).FirstOrDefault();
				if(pp == null)
					continue;
				object value = null;
				try
				{
					if(pp.PropertyType.IsEnum)
						value = Enum.Parse(pp.PropertyType, i.Value);
					else if(pp.PropertyType.FullName == "System.Text.Encoding")
						value = System.Text.Encoding.GetEncoding(i.Value);
					else
					{
						var convertMethod = typeof(Convert).GetMethod("To" + pp.PropertyType.Name, new Type[] {
							typeof(string)
						});
						if(convertMethod == null)
							convertMethod = pp.PropertyType.GetMethod("Parse", new Type[] {
								typeof(string)
							});
						if(convertMethod != null)
							value = convertMethod.Invoke(null, new object[] {
								i.Value
							});
						else
							continue;
					}
				}
				catch(Exception)
				{
					continue;
				}
				pp.SetValue(refObject, value, null);
			}
			foreach (ServiceAddressNode child in node)
			{
				var pp = properties.Where(f => string.Compare(f.Name, child.Name, true) == 0).FirstOrDefault();
				if(pp == null)
					continue;
				var value = pp.GetValue(refObject, null);
				if(value == null)
				{
					var constructor = pp.PropertyType.GetConstructor(new Type[0]);
					if(constructor == null)
						continue;
					value = constructor.Invoke(new object[0]);
				}
				FillObjectProperties(child, value);
			}
		}

		/// <summary>
		/// Configura o criado do pai para o nome informado.
		/// </summary>
		/// <param name="name">Nome do binding.</param>
		/// <param name="creator">Instancia do criador.</param>
		public static void ConfigureBinding(string name, Func<System.ServiceModel.Channels.Binding> creator)
		{
			name.Require("name").NotNull();
			creator.Require("creator").NotNull();
			if(_bindingCreators.ContainsKey(name))
				_bindingCreators[name] = creator;
			else
				_bindingCreators.Add(name, creator);
		}

		/// <summary>
		/// Cria um nód de com a configuração padrão de Binding.
		/// </summary>
		/// <returns></returns>
		public static ServiceAddressNode CreateDefaultBindingConfiguration()
		{
			using (var sr = new System.IO.StringReader(@"<binding name='WSHttpBinding_IIntegrationService' closeTimeout='00:01:00'
                    openTimeout='00:01:00' receiveTimeout='00:10:00' sendTimeout='00:01:00'
                    bypassProxyOnLocal='false' transactionFlow='false' hostNameComparisonMode='StrongWildcard'
                    maxBufferPoolSize='524288' maxReceivedMessageSize='65536'
                    messageEncoding='Text' textEncoding='utf-8' useDefaultWebProxy='true'
                    allowCookies='false'>
                    <readerQuotas maxDepth='32' maxStringContentLength='8192' maxArrayLength='16384'
                                  maxBytesPerRead='4096' maxNameTableCharCount='16384' />
                    <reliableSession ordered='true' inactivityTimeout='00:10:00' enabled='false' />
                    <security mode='None'>
                        <transport clientCredentialType='None' proxyCredentialType='None' realm='' />
                        <message clientCredentialType='UserName' algorithmSuite='Default' />
                    </security>
                </binding>"))
			{
				var xmlReader = System.Xml.XmlReader.Create(sr);
				var xmlDocument = new System.Xml.XmlDocument();
				xmlDocument.Load(xmlReader);
				return ServiceAddressNode.CreateFromXmlElement(xmlDocument.DocumentElement);
			}
		}

		/// <summary>
		/// Recupera o <see cref="System.ServiceModel.EndpointAddress"/> da instancia.
		/// </summary>
		/// <returns></returns>
		public System.ServiceModel.EndpointAddress GetEndpointAddress()
		{
			var endpointNode = GetEndpointNode();
			System.ServiceModel.EndpointIdentity identity = null;
			var identityNode = endpointNode["identity"];
			if(identityNode != null)
			{
				if(identityNode.Contains("dns"))
					identity = System.ServiceModel.EndpointIdentity.CreateDnsIdentity(identityNode["dns"].Parameters["value"]);
				else if(identityNode.Contains("rsa"))
					identity = System.ServiceModel.EndpointIdentity.CreateRsaIdentity(identityNode["certificate"].Parameters["value"]);
				else if(identityNode.Contains("userPrincipalName"))
					identity = System.ServiceModel.EndpointIdentity.CreateUpnIdentity(identityNode["userPrincipalName"].Parameters["value"]);
				else if(identityNode.Contains("servicePrincipalName"))
					identity = System.ServiceModel.EndpointIdentity.CreateSpnIdentity(identityNode["servicePrincipalName"].Parameters["value"]);
				else if(identityNode.Contains("certificate"))
					identity = System.ServiceModel.EndpointIdentity.CreateX509CertificateIdentity(new System.Security.Cryptography.X509Certificates.X509Certificate2(Convert.FromBase64String(identityNode["certificate"].Parameters["encodedValue"])));
			}
			var endpoint = new System.ServiceModel.EndpointAddress(new Uri(this.Address), identity, new System.ServiceModel.Channels.AddressHeader[0]);
			FillObjectProperties(endpointNode, endpoint);
			return endpoint;
		}

		/// <summary>
		/// Recupera o binding dos dados do endereço.
		/// </summary>
		/// <returns></returns>
		public System.ServiceModel.Channels.Binding GetBinding()
		{
			var bindingNode = GeBindingNode();
			if(bindingNode == null)
				throw new InvalidOperationException(string.Format(Properties.Resources.InvalidOperation_BindingNodeNotFound, Configuration.Name));
			System.ServiceModel.Channels.Binding binding = null;
			var endpointNode = GetEndpointNode();
			Func<System.ServiceModel.Channels.Binding> creator = null;
			if(_bindingCreators.TryGetValue(endpointNode.Parameters["binding"], out creator))
				binding = creator();
			FillObjectProperties(bindingNode, binding);
			return binding;
		}

		/// <summary>
		/// Recupera a string que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Address;
		}

		void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("Name", Name);
			info.AddValue("Address", Address);
			info.AddValue("Configuration", Configuration);
		}

		/// <summary>
		/// Método usado para recupera o esquema da classe.
		/// </summary>
		/// <param name="xs"></param>
		/// <returns></returns>
		public static XmlSchemaType MySchema(XmlSchemaSet xs)
		{
			var complexType = new XmlSchemaComplexType();
			complexType.Attributes.Add(new XmlSchemaAttribute {
				Name = "name",
				SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"),
				Use = XmlSchemaUse.Required
			});
			complexType.Attributes.Add(new XmlSchemaAttribute {
				Name = "address",
				SchemaTypeName = new XmlQualifiedName("string", "http://www.w3.org/2001/XMLSchema"),
				Use = XmlSchemaUse.Required
			});
			var sequence = new XmlSchemaSequence();
			var configurationElement = new XmlSchemaElement() {
				Name = "Configuration",
				SchemaType = ServiceAddressNode.MySchema(null)
			};
			sequence.Items.Add(configurationElement);
			complexType.Particle = sequence;
			return complexType;
		}

		/// <summary>
		/// Não usar.
		/// </summary>
		/// <returns></returns>
		public System.Xml.Schema.XmlSchema GetSchema()
		{
			return null;
		}

		/// <summary>
		/// Recupera os dados do xml.
		/// </summary>
		/// <param name="reader"></param>
		public void ReadXml(System.Xml.XmlReader reader)
		{
			if(reader.MoveToAttribute("name"))
			{
				Name = reader.ReadContentAsString();
				reader.MoveToElement();
			}
			if(reader.MoveToAttribute("address"))
			{
				Address = reader.ReadContentAsString();
				reader.MoveToElement();
			}
			reader.ReadStartElement();
			while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
			{
				if(reader.Name == "Configuration")
				{
					if(!reader.IsEmptyElement)
					{
						var config = new ServiceAddressNode();
						config.ReadXml(reader);
						Configuration = config;
					}
					else
						reader.Skip();
				}
				else
					reader.Skip();
			}
			reader.ReadEndElement();
		}

		/// <summary>
		/// Escreve os dados do parametro no xml.
		/// </summary>
		/// <param name="writer"></param>
		public void WriteXml(System.Xml.XmlWriter writer)
		{
			writer.WriteAttributeString("name", Name);
			writer.WriteAttributeString("address", Address);
			writer.WriteStartElement("Configuration");
			if(Configuration != null)
				Configuration.WriteXml(writer);
			writer.WriteEndElement();
		}
	}
}
