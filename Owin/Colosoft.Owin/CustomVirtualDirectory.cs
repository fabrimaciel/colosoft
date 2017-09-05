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

namespace Colosoft.Web.Hosting
{
	/// <summary>
	/// Implementação de um diretório virtual.
	/// </summary>
	class CustomVirtualDirectory : System.Web.Hosting.VirtualDirectory
	{
		/// <summary>
		/// Arquivos.
		/// </summary>
		public override System.Collections.IEnumerable Files
		{
			get
			{
				return new object[0];
			}
		}

		/// <summary>
		/// Diretórios.
		/// </summary>
		public override System.Collections.IEnumerable Directories
		{
			get
			{
				return new object[0];
			}
		}

		/// <summary>
		/// Filhos.
		/// </summary>
		public override System.Collections.IEnumerable Children
		{
			get
			{
				return new object[0];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="virtualPath"></param>
		public CustomVirtualDirectory(string virtualPath) : base(virtualPath)
		{
		}
	}
}
