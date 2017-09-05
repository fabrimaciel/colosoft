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
	/// Representa um parametro que carrega o conteúdo do arquivo informado.
	/// </summary>
	public class FileContentParameter : MultipartFormDataParameter, IFileMultipartFormDataParameter
	{
		private string _path;

		/// <summary>
		/// Nome do arquivo.
		/// </summary>
		public string FileName
		{
			get
			{
				return System.IO.Path.GetFileName(_path);
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="path">Caminho do arquivo.</param>
		public FileContentParameter(string name, string path) : base(name)
		{
			path.Require("path").NotNull().NotEmpty();
			_path = path;
			ContentType = "application/x-object";
		}

		/// <summary>
		/// Tenta recupera o tamanho do arquivo.
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public override bool TryComputeLength(out long length)
		{
			try
			{
				var fileInfo = new System.IO.FileInfo(_path);
				if(fileInfo.Exists)
				{
					length = fileInfo.Length;
					return true;
				}
			}
			catch
			{
			}
			length = 0L;
			return false;
		}

		/// <summary>
		/// Escreve o conteúdo do parametro.
		/// </summary>
		/// <param name="stream"></param>
		public override void WriteContent(System.IO.Stream stream)
		{
			var fileInfo = new System.IO.FileInfo(_path);
			if(!fileInfo.Exists)
				throw new System.IO.FileNotFoundException("File not found", _path, null);
			var buffer = new byte[1024];
			var read = 0;
			using (var inputStream = fileInfo.OpenRead())
				while ((read = inputStream.Read(buffer, 0, buffer.Length)) > 0)
					stream.Write(buffer, 0, read);
		}
	}
}
