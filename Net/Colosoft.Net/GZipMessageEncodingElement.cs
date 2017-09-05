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
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Configuration;
using System.Text;

namespace Colosoft.Net
{
	/// <summary>
	/// Essa classe é necessária para habilitar o plug do GZip encoder binding element no arquivo de configuração.
	/// </summary>
	public class GZipMessageEncodingElement : BindingElementExtensionElement
	{
		private const string Category_ReaderQuotas = "ReaderQuotas";

		private const string Config_Property_ReaderQuotas = "readerQuotas";

		/// <summary>
		/// Propriedade necessário para configurar o nosso binding elemento do tipo do encoder interno.
		/// Aqui suporta textual e binário.
		/// </summary>
		[ConfigurationProperty("innerMessageEncoding", DefaultValue = "textMessageEncoding")]
		public string InnerMessageEncoding
		{
			get
			{
				return (string)base["innerMessageEncoding"];
			}
			set
			{
				base["innerMessageEncoding"] = value;
			}
		}

		/// <summary>
		/// Recupera e define o tamanho máximio do vetor de bytes.
		/// </summary>
		/// <value>Tamanho máximo do vetor.</value>
		[ConfigurationProperty("MaxArrayLength", DefaultValue = 999999999, IsRequired = false)]
		[Category(Category_ReaderQuotas)]
		[Description("MaxArrayLength")]
		public int MaxArrayLength
		{
			get
			{
				return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxArrayLength;
			}
			set
			{
				((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxArrayLength = value;
			}
		}

		/// <summary>
		/// Recupera e define a profundidade máximoa suportada.
		/// </summary>
		[ConfigurationProperty("maxDepth", DefaultValue = 999999999, IsRequired = false)]
		[Category(Category_ReaderQuotas)]
		[Description("maxDepth")]
		public int MaxDepth
		{
			get
			{
				return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxDepth;
			}
			set
			{
				((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxDepth = value;
			}
		}

		/// <summary>
		/// Recupera e define o tamanho máximo de um contexto textual.
		/// </summary>
		[ConfigurationProperty("maxStringContentLength", DefaultValue = 999999999, IsRequired = false)]
		[Category(Category_ReaderQuotas)]
		[Description("maxStringContentLength")]
		public int MaxStringContentLength
		{
			get
			{
				return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxStringContentLength;
			}
			set
			{
				((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxStringContentLength = value;
			}
		}

		/// <summary>
		/// Recupera e define o tamanho máximo em bytes por leitura.
		/// </summary>
		[ConfigurationProperty("maxBytesPerRead", DefaultValue = 999999999, IsRequired = false)]
		[Category(Category_ReaderQuotas)]
		[Description("maxBytesPerRead")]
		public int MaxBytesPerRead
		{
			get
			{
				return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxBytesPerRead;
			}
			set
			{
				((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxBytesPerRead = value;
			}
		}

		/// <summary>
		/// Recupera e define a quantidade de caracteres do nome de uma tabela.
		/// </summary>
		[ConfigurationProperty("maxNameTableCharCount", DefaultValue = 999999999, IsRequired = false)]
		[Category(Category_ReaderQuotas)]
		[Description("maxNameTableCharCount")]
		public int MaxNameTableCharCount
		{
			get
			{
				return ((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxNameTableCharCount;
			}
			set
			{
				((XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas]).MaxNameTableCharCount = value;
			}
		}

		/// <summary>
		/// Recupera as quotas do leitor.
		/// </summary>
		[ConfigurationProperty(Config_Property_ReaderQuotas)]
		public XmlDictionaryReaderQuotasElement ReaderQuotas
		{
			get
			{
				return (XmlDictionaryReaderQuotasElement)base[Config_Property_ReaderQuotas];
			}
		}

		/// <summary>
		/// Tipo do elemento do binding.
		/// </summary>
		public override Type BindingElementType
		{
			get
			{
				return typeof(GZipMessageEncodingBindingElement);
			}
		}

		/// <summary>
		/// Aplica a configuração do elemento.
		/// </summary>
		/// <param name="bindingElement"></param>
		public override void ApplyConfiguration(BindingElement bindingElement)
		{
			var binding = (GZipMessageEncodingBindingElement)bindingElement;
			PropertyInformationCollection propertyInfo = this.ElementInformation.Properties;
			if(propertyInfo["innerMessageEncoding"].ValueOrigin != PropertyValueOrigin.Default)
			{
				switch(this.InnerMessageEncoding)
				{
				case "textMessageEncoding":
					binding.InnerMessageEncodingBindingElement = new TextMessageEncodingBindingElement();
					if(ReaderQuotas.MaxArrayLength > 0)
					{
						((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxArrayLength = ReaderQuotas.MaxArrayLength;
					}
					if(ReaderQuotas.MaxBytesPerRead > 0)
					{
						((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
					}
					if(ReaderQuotas.MaxDepth > 0)
					{
						((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxDepth = ReaderQuotas.MaxDepth;
					}
					if(ReaderQuotas.MaxNameTableCharCount > 0)
					{
						((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
					}
					if(ReaderQuotas.MaxStringContentLength > 0)
					{
						((TextMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
					}
					break;
				case "binaryMessageEncoding":
					binding.InnerMessageEncodingBindingElement = new BinaryMessageEncodingBindingElement();
					if(ReaderQuotas.MaxArrayLength > 0)
					{
						((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxArrayLength = ReaderQuotas.MaxArrayLength;
					}
					if(ReaderQuotas.MaxBytesPerRead > 0)
					{
						((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
					}
					if(ReaderQuotas.MaxDepth > 0)
					{
						((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxDepth = ReaderQuotas.MaxDepth;
					}
					if(ReaderQuotas.MaxNameTableCharCount > 0)
					{
						((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
					}
					if(ReaderQuotas.MaxStringContentLength > 0)
					{
						((BinaryMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
					}
					break;
				case "mtomMessageEncoding":
					binding.InnerMessageEncodingBindingElement = new MtomMessageEncodingBindingElement();
					if(ReaderQuotas.MaxArrayLength > 0)
					{
						((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxArrayLength = ReaderQuotas.MaxArrayLength;
					}
					if(ReaderQuotas.MaxBytesPerRead > 0)
					{
						((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxBytesPerRead = ReaderQuotas.MaxBytesPerRead;
					}
					if(ReaderQuotas.MaxDepth > 0)
					{
						((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxDepth = ReaderQuotas.MaxDepth;
					}
					if(ReaderQuotas.MaxNameTableCharCount > 0)
					{
						((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxNameTableCharCount = ReaderQuotas.MaxNameTableCharCount;
					}
					if(ReaderQuotas.MaxStringContentLength > 0)
					{
						((MtomMessageEncodingBindingElement)binding.InnerMessageEncodingBindingElement).ReaderQuotas.MaxStringContentLength = ReaderQuotas.MaxStringContentLength;
					}
					break;
				}
			}
		}

		/// <summary>
		/// Cria uma instancia do elemento.
		/// </summary>
		/// <returns></returns>
		protected override BindingElement CreateBindingElement()
		{
			var bindingElement = new GZipMessageEncodingBindingElement();
			ApplyConfiguration(bindingElement);
			return bindingElement;
		}
	}
}
