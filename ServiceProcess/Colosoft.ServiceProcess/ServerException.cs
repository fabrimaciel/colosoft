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

namespace Colosoft.ServiceProcess
{
	/// <summary>
	/// Representa um erro do servidor.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly"), Serializable]
	public class ServerException : ApplicationException
	{
		private Dictionary<object, object> m_properties;

		/// <summary>
		/// Dados associados.
		/// </summary>
		public override System.Collections.IDictionary Data
		{
			get
			{
				if(m_properties == null)
					m_properties = new Dictionary<object, object>();
				return m_properties;
			}
		}

		/// <summary>
		/// Identifica se é uma exception remota.
		/// </summary>
		public bool IsRemoteException
		{
			get
			{
				return (base.InnerException is System.Web.Services.Protocols.SoapException);
			}
		}

		/// <summary>
		/// Código do erro.
		/// </summary>
		public string RemoteExceptionCode
		{
			get
			{
				var innerException = base.InnerException as System.Web.Services.Protocols.SoapException;
				if(((innerException.SubCode != null) && (innerException.SubCode.Code != null)) && (innerException.SubCode.Code.Name != null))
					return null;
				return innerException.SubCode.Code.Name;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ServerException()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		public ServerException(string message) : base(message)
		{
		}

		/// <summary>
		/// Construtor da serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ServerException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ServerException(string message, Exception innerException) : base(message, innerException)
		{
			var exception = innerException as System.Web.Services.Protocols.SoapException;
			if((exception != null) && (exception.Detail != null))
			{
				try
				{
					using (var reader = new System.IO.StringReader(exception.Detail.OuterXml))
					{
						var reader2 = System.Xml.XmlReader.Create(reader);
						reader2.Read();
						while (reader2.NodeType == System.Xml.XmlNodeType.Element)
						{
							if(reader2.Name == "ExceptionProperties")
							{
								reader2.Read();
								m_properties = new Dictionary<object, object>();
								foreach (KeyValuePair<string, object> pair in ExceptionPropertyCollection.FromXml(reader2))
								{
									this.m_properties[pair.Key.ToUpperInvariant()] = pair.Value;
								}
							}
							reader2.Read();
						}
					}
				}
				catch(Exception)
				{
				}
			}
		}

		/// <summary>
		/// Recupera o valor da propriedade pelo nome informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		/// <returns></returns>
		public T GetProperty<T>(string name)
		{
			object obj2;
			if((this.m_properties != null) && this.m_properties.TryGetValue(name.ToUpperInvariant(), out obj2))
			{
				return (T)obj2;
			}
			return default(T);
		}
	}
}
