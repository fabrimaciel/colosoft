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

namespace Colosoft.IO.FileRepository
{
	/// <summary>
	/// Assinatura de um item do repositório.
	/// </summary>
	public interface IItem
	{
		/// <summary>
		/// Nome do item.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Nome completo.
		/// </summary>
		string FullName
		{
			get;
		}

		/// <summary>
		/// Tamanho do item.
		/// </summary>
		long ContentLength
		{
			get;
		}

		/// <summary>
		/// Tipo do item.
		/// </summary>
		ItemType Type
		{
			get;
		}

		/// <summary>
		/// Quanto o item foi criado.
		/// </summary>
		DateTimeOffset CreationTime
		{
			get;
		}

		/// <summary>
		/// Última vez que o item foi alterado.
		/// </summary>
		DateTimeOffset LastWriteTime
		{
			get;
		}

		/// <summary>
		/// Identifica se o item tem suporte para leitura.
		/// </summary>
		bool CanRead
		{
			get;
		}

		/// <summary>
		/// Abre o item para leitura.
		/// </summary>
		/// <returns></returns>
		System.IO.Stream OpenRead();
	}
}
