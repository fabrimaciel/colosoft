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

namespace Colosoft.Net.Share
{
	/// <summary>
	/// Armazena os dados de compartilhamento de diretório.
	/// </summary>
	public class DirectoryShare
	{
		private string _directory;

		private NetworkShareIdentity _identity;

		/// <summary>
		/// Instancia do comparador para o nome do diretório.
		/// </summary>
		public static readonly IComparer<DirectoryShare> DirectoryNameComparer = new DirectoryComparer();

		/// <summary>
		/// Caminho do diretório do compartilhamento.
		/// </summary>
		public string Directory
		{
			get
			{
				return _directory;
			}
			set
			{
				_directory = value;
			}
		}

		/// <summary>
		/// Identidade usada na autenticação.
		/// </summary>
		public NetworkShareIdentity Identity
		{
			get
			{
				return _identity;
			}
			set
			{
				_identity = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="directory">Diretório do compartilhamento.</param>
		/// <param name="identity">Identidade usada para a autenticação.</param>
		public DirectoryShare(string directory, NetworkShareIdentity identity)
		{
			directory.Require("directory").NotNull().NotEmpty();
			identity.Require("identity").NotNull();
			_directory = directory;
			_identity = identity;
		}

		/// <summary>
		/// Implementação do comparador para o nome do diretório.
		/// </summary>
		class DirectoryComparer : IComparer<DirectoryShare>
		{
			/// <summary>
			/// Compara as instancias informadas.
			/// </summary>
			/// <param name="x"></param>
			/// <param name="y"></param>
			/// <returns></returns>
			public int Compare(DirectoryShare x, DirectoryShare y)
			{
				return x == null && y != null ? -1 : (y == null && x != null ? 1 : (x == null && y == null ? 0 : StringComparer.InvariantCultureIgnoreCase.Compare(x.Directory, y.Directory)));
			}
		}
	}
}
