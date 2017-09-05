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
using System.Configuration;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;

namespace Colosoft.Net
{
	/// <summary>
	/// Classe que contém as contantes para a politica de codificação da mensagem GZip.
	/// </summary>
	static class GZipMessageEncodingPolicyConstants
	{
		public const string GZipEncodingName = "GZipEncoding";

		public const string GZipEncodingNamespace = "http://schemas.microsoft.com/ws/06/2004/mspolicy/netgzip1";

		public const string GZipEncodingPrefix = "gzip";
	}
	/// <summary>
	/// Representa o binding element que, quando plugado em um custom binding, habilita o GZip encoder.
	/// </summary>
	public sealed class GZipMessageEncodingBindingElement : MessageEncodingBindingElement, IPolicyExportExtension
	{
		/// <summary>
		/// Elemento interno.
		/// </summary>
		private MessageEncodingBindingElement _innerBindingElement;

		private bool _isClient;

		/// <summary>
		/// Construtor padrão.
		/// Por padrão se define o encoder de texto como o encoder interno.
		/// </summary>
		public GZipMessageEncodingBindingElement() : this(new TextMessageEncodingBindingElement())
		{
		}

		/// <summary>
		/// Cria a instancia com o elemento interno.
		/// </summary>
		/// <param name="messageEncoderBindingElement">Instancia do elemento que será adaptado.</param>
		public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement) : this(messageEncoderBindingElement, false)
		{
		}

		/// <summary>
		/// Cria a instancia com o elemento interno.
		/// </summary>
		/// <param name="messageEncoderBindingElement">Instancia do elemento que será adaptado.</param>
		/// <param name="isClient">Identifica se é um cliente.</param>
		public GZipMessageEncodingBindingElement(MessageEncodingBindingElement messageEncoderBindingElement, bool isClient)
		{
			_innerBindingElement = messageEncoderBindingElement;
			_isClient = isClient;
		}

		/// <summary>
		/// Elemento de vinculação da codificação de menasgem interno.
		/// </summary>
		public MessageEncodingBindingElement InnerMessageEncodingBindingElement
		{
			get
			{
				return _innerBindingElement;
			}
			set
			{
				_innerBindingElement = value;
			}
		}

		/// <summary>
		/// Versão da mensagem.
		/// </summary>
		public override MessageVersion MessageVersion
		{
			get
			{
				return _innerBindingElement.MessageVersion;
			}
			set
			{
				_innerBindingElement.MessageVersion = value;
			}
		}

		/// <summary>
		/// Ponto de entrada principal para o encoder binding element.
		/// Chamado pelo WCF para recupera o factory que irá criar o encoder da mensagem.
		/// </summary>
		/// <returns></returns>
		public override MessageEncoderFactory CreateMessageEncoderFactory()
		{
			return new GZipMessageEncoderFactory(_innerBindingElement.CreateMessageEncoderFactory(), _isClient);
		}

		/// <summary>
		/// Clone os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public override BindingElement Clone()
		{
			return new GZipMessageEncodingBindingElement(_innerBindingElement, _isClient);
		}

		/// <summary>
		/// Recupera a propriedade.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="context"></param>
		/// <returns></returns>
		public override T GetProperty<T>(BindingContext context)
		{
			if(typeof(T) == typeof(XmlDictionaryReaderQuotas))
			{
				return _innerBindingElement.GetProperty<T>(context);
			}
			else
			{
				return base.GetProperty<T>(context);
			}
		}

		/// <summary>
		/// Constrói o ChannelFactory do tipo informado.
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="context"></param>
		/// <returns></returns>
		public override IChannelFactory<TChannel> BuildChannelFactory<TChannel>(BindingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			context.BindingParameters.Add(this);
			return context.BuildInnerChannelFactory<TChannel>();
		}

		/// <summary>
		/// Constrói o ChannelListener.
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="context"></param>
		/// <returns></returns>
		public override IChannelListener<TChannel> BuildChannelListener<TChannel>(BindingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			context.BindingParameters.Add(this);
			return context.BuildInnerChannelListener<TChannel>();
		}

		/// <summary>
		/// Verifica se pode construir o listener.
		/// </summary>
		/// <typeparam name="TChannel"></typeparam>
		/// <param name="context"></param>
		/// <returns></returns>
		public override bool CanBuildChannelListener<TChannel>(BindingContext context)
		{
			if(context == null)
				throw new ArgumentNullException("context");
			context.BindingParameters.Add(this);
			return context.CanBuildInnerChannelListener<TChannel>();
		}

		/// <summary>
		/// Configura a política de export.
		/// </summary>
		/// <param name="exporter"></param>
		/// <param name="policyContext"></param>
		void IPolicyExportExtension.ExportPolicy(MetadataExporter exporter, PolicyConversionContext policyContext)
		{
			if(policyContext == null)
			{
				throw new ArgumentNullException("policyContext");
			}
			XmlDocument document = new XmlDocument();
			policyContext.GetBindingAssertions().Add(document.CreateElement(GZipMessageEncodingPolicyConstants.GZipEncodingPrefix, GZipMessageEncodingPolicyConstants.GZipEncodingName, GZipMessageEncodingPolicyConstants.GZipEncodingNamespace));
		}
	}
}
