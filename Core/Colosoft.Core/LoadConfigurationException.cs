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
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Configuration.Exceptions
{
	/// <summary>
	/// Armazena as informações do erro ocorrido ao carregar as configurações do GDA.
	/// </summary>
	[Serializable]
	public class LoadConfigurationException : ConfigException
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message"></param>
		public LoadConfigurationException(string message) : base(message)
		{
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="innerException"></param>
		public LoadConfigurationException(Exception innerException) : base(innerException)
		{
		}

		/// <summary>
		/// Cria uma instancia com os dados iniciais.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public LoadConfigurationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado na serializacao.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected LoadConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
