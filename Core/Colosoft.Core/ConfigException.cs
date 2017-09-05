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
	/// Representa um erro de configuracao.
	/// </summary>
	[Serializable]
	public class ConfigException : Exception
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="message"></param>
		public ConfigException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria uma instancia com base na exception interna.
		/// </summary>
		/// <param name="innerException"></param>
		public ConfigException(Exception innerException) : base(innerException.Message, innerException)
		{
		}

		/// <summary>
		/// Cria uma instancia do erro.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public ConfigException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Cria uma instancia do erro com mensagem de formatacao.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public ConfigException(string message, params object[] args) : this(String.Format(message, args))
		{
		}

		/// <summary>
		/// Construtor usado na serializacao.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ConfigException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
