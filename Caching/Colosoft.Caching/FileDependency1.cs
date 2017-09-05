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

namespace Colosoft.Caching.Dependencies
{
	/// <summary>
	/// Representa uma dependencia para um arquivo físico.
	/// </summary>
	[Serializable]
	public class FileDependency : CacheDependency
	{
		private string[] _fileNames;

		private long _startAfterTicks;

		/// <summary>
		/// Nomes dos arquivos associados.
		/// </summary>
		public string[] fileNames
		{
			get
			{
				return _fileNames;
			}
		}

		/// <summary>
		/// Ticks que identificam o inicio da validação da dependencia.
		/// </summary>
		public long StartAfterTicks
		{
			get
			{
				return _startAfterTicks;
			}
		}

		/// <summary>
		/// Cria uma instancia de dependencia para apenas um arquivo.
		/// </summary>
		/// <param name="fileName">Nome do arquivo associado com a dependencia.</param>
		public FileDependency(string fileName) : this(fileName, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos.
		/// </summary>
		/// <param name="fileName">Nomes dos arquivos associados.</param>
		public FileDependency(string[] fileName) : this(fileName, DateTime.Now)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para um arquivo.
		/// </summary>
		/// <param name="fileName">Nome do arquivos associado.</param>
		/// <param name="startAfter">Data que será iniciada a verificação.</param>
		public FileDependency(string fileName, DateTime startAfter) : this(new string[] {
			fileName
		}, startAfter)
		{
		}

		/// <summary>
		/// Cria uma instancia de dependencia para vários arquivos.
		/// </summary>
		/// <param name="fileName">Nomes dos arquivos associados.</param>
		/// <param name="startAfter">Data que será iniciada a verificação.</param>
		public FileDependency(string[] fileName, DateTime startAfter)
		{
			_fileNames = fileName;
			_startAfterTicks = startAfter.Ticks;
		}
	}
}
