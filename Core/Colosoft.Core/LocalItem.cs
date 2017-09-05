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

namespace Colosoft.IO.FileRepository.Local
{
	/// <summary>
	/// Representa um arquivo local.
	/// </summary>
	public class LocalItem : IItem
	{
		private System.IO.FileSystemInfo _info;

		/// <summary>
		/// Nome do item.
		/// </summary>
		public string Name
		{
			get
			{
				return _info.Name;
			}
		}

		/// <summary>
		/// Nome completo.
		/// </summary>
		public string FullName
		{
			get
			{
				return _info.FullName;
			}
		}

		/// <summary>
		/// Tamanho do item.
		/// </summary>
		public long ContentLength
		{
			get
			{
				if(_info is System.IO.FileInfo)
					return ((System.IO.FileInfo)_info).Length;
				return 0;
			}
		}

		/// <summary>
		/// Tipo do item.
		/// </summary>
		public ItemType Type
		{
			get
			{
				return _info is System.IO.FileInfo ? ItemType.File : ItemType.Folder;
			}
		}

		/// <summary>
		/// Quanto o item foi criado.
		/// </summary>
		public DateTimeOffset CreationTime
		{
			get
			{
				return _info == null ? DateTimeOffset.Now : _info.CreationTime;
			}
		}

		/// <summary>
		/// Última vez que o item foi alterado.
		/// </summary>
		public DateTimeOffset LastWriteTime
		{
			get
			{
				return _info == null ? DateTimeOffset.Now : _info.LastWriteTime;
			}
		}

		/// <summary>
		/// Identifica se o item tem suporte para leitura.
		/// </summary>
		public bool CanRead
		{
			get
			{
				return _info is System.IO.FileInfo;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="info">Informações do arquivo associado.</param>
		public LocalItem(System.IO.FileSystemInfo info)
		{
			info.Require("info").NotNull();
			_info = info;
		}

		/// <summary>
		/// Abre o item para leitura.
		/// </summary>
		/// <returns></returns>
		public System.IO.Stream OpenRead()
		{
			if(!CanRead)
				throw new NotSupportedException();
			return ((System.IO.FileInfo)_info).OpenRead();
		}

		/// <summary>
		/// Recupera o texto que presenta a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return FullName;
		}
	}
}
