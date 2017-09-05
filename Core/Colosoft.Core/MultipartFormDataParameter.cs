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

namespace Colosoft.Web
{
	/// <summary>
	/// Assinatura de uma parametro do form data associado a um arquivo.
	/// </summary>
	public interface IFileMultipartFormDataParameter
	{
		/// <summary>
		/// Nome do arquivo.
		/// </summary>
		string FileName
		{
			get;
		}
	}
	/// <summary>
	/// Representa um parametro do formdata.
	/// </summary>
	public abstract class MultipartFormDataParameter
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Charset do conteúdo.
		/// </summary>
		public string Charset
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo de conteúdo.
		/// </summary>
		public string ContentType
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		public MultipartFormDataParameter(string name)
		{
			this.Name = name;
			this.ContentType = "text/plain";
			this.Charset = "utf-8";
		}

		/// <summary>
		/// Tenta calcular o tamanho do parametro.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public abstract bool TryComputeLength(out long length);

		/// <summary>
		/// Registra o conteúdo na stream informada.
		/// </summary>
		/// <param name="stream"></param>
		public abstract void WriteContent(System.IO.Stream stream);
	}
}
