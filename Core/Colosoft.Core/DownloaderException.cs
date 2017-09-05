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

namespace Colosoft.Net
{
	/// <summary>
	/// Representa um erro do Downloader.
	/// </summary>
	[Serializable]
	public class DownloaderException : Exception, ICommunicationException
	{
		private CommunicationExceptionDetails _details;

		/// <summary>
		/// Detalhes do erro.
		/// </summary>
		public CommunicationExceptionDetails Details
		{
			get
			{
				return _details;
			}
		}

		/// <summary>
		/// Construtor vazio.
		/// </summary>
		public DownloaderException()
		{
		}

		/// <summary>
		/// Cria a instancia com os dados do detalhe.
		/// </summary>
		/// <param name="details"></param>
		public DownloaderException(CommunicationExceptionDetails details) : base(details != null ? details.Message : null, details.InnerException != null ? new DownloaderException(details.InnerException) : null)
		{
			_details = details;
		}

		/// <summary>
		/// Cria uma instancia com a mensagem informada.
		/// </summary>
		/// <param name="message"></param>
		public DownloaderException(string message) : base(message)
		{
		}

		/// <summary>
		/// Cria uma instancia com a mensagem e o erro interno.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="innerException"></param>
		public DownloaderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		/// <summary>
		/// Construtor usado na serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecuritySafeCritical]
		protected DownloaderException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Método usado na serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		[System.Security.SecurityCritical]
		[System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand)]
		public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
