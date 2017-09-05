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
	/// Representa um item de arquivo para o Uploader.
	/// </summary>
	public class UploaderFileItem : IUploaderItem
	{
		private System.IO.FileInfo _fileInfo;

		private UploaderItemAttributeSet _attributes = new UploaderItemAttributeSet();

		private long _numberBytesSent;

		/// <summary>
		/// Informações do arquivo associado.
		/// </summary>
		public System.IO.FileInfo FileInfo
		{
			get
			{
				return _fileInfo;
			}
		}

		/// <summary>
		/// Atributos do item.
		/// </summary>
		public UploaderItemAttributeSet Attributes
		{
			get
			{
				return _attributes;
			}
		}

		/// <summary>
		/// Tamanho dos dados do item.
		/// </summary>
		public long Length
		{
			get
			{
				if(_fileInfo.Exists)
					return _fileInfo.Length;
				else
					return 0;
			}
		}

		/// <summary>
		/// Número de bytes enviados.
		/// </summary>
		public long NumberBytesSent
		{
			get
			{
				return _numberBytesSent;
			}
			set
			{
				_numberBytesSent = value;
			}
		}

		/// <summary>
		/// Cria a instancia para o arquivo informado.
		/// </summary>
		/// <param name="fileName"></param>
		public UploaderFileItem(string fileName)
		{
			fileName.Require("fileName").NotNull().NotEmpty();
			_fileInfo = new System.IO.FileInfo(fileName);
			_attributes.Add("FileName", _fileInfo.Name);
		}

		/// <summary>
		/// Cria a instancia com as informações do arquivo.
		/// </summary>
		/// <param name="fileInfo">Informações do arquivo.</param>
		public UploaderFileItem(System.IO.FileInfo fileInfo)
		{
			fileInfo.Require("fileInfo").NotNull();
			_fileInfo = fileInfo;
			_attributes.Add("FileName", fileInfo.Name);
		}

		/// <summary>
		/// Recupera o conteúdo do item.
		/// </summary>
		/// <returns></returns>
		public System.IO.Stream GetContent()
		{
			return _fileInfo.OpenRead();
		}
	}
}
