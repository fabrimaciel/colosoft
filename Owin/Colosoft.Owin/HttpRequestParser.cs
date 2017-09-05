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
using System.Threading.Tasks;

namespace Colosoft.Owin.Server
{
	/// <summary>
	/// Implementação base do parser de requisição.
	/// </summary>
	abstract class HttpRequestParser : IDisposable
	{
		/// <summary>
		/// Cliente associado.
		/// </summary>
		protected HttpClient Client
		{
			get;
			private set;
		}

		/// <summary>
		/// Tamanho do conteúdo.
		/// </summary>
		protected int ContentLength
		{
			get;
			private set;
		}

		/// <summary>
		/// Cria a instancia com os parametros iniciais.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="contentLength"></param>
		protected HttpRequestParser(HttpClient client, int contentLength)
		{
			if(client == null)
				throw new ArgumentNullException("client");
			Client = client;
			ContentLength = contentLength;
		}

		/// <summary>
		/// Executa o parse no dados.
		/// </summary>
		public abstract void Parse();

		/// <summary>
		/// Finaliza o parse.
		/// </summary>
		protected void EndParsing()
		{
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
