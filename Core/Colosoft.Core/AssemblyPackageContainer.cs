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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Implementação de um container de pacotes de assemblies.
	/// </summary>
	public class AssemblyPackageContainer : IEnumerable<IAssemblyPackage>
	{
		private List<IAssemblyPackage> _packages;

		/// <summary>
		/// Quantidade de pacotes no container.
		/// </summary>
		public int Count
		{
			get
			{
				return _packages.Count;
			}
		}

		/// <summary>
		/// Recupera o pacote pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IAssemblyPackage this[int index]
		{
			get
			{
				return _packages[index];
			}
		}

		/// <summary>
		/// Cria o container com apenas um pacote.
		/// </summary>
		public AssemblyPackageContainer(IAssemblyPackage package)
		{
			_packages = new List<IAssemblyPackage>();
			_packages.Add(package);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="packages"></param>
		public AssemblyPackageContainer(IEnumerable<IAssemblyPackage> packages)
		{
			_packages = new List<IAssemblyPackage>(packages);
		}

		/// <summary>
		/// Recupera o enumerador dos pacotes.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IAssemblyPackage> GetEnumerator()
		{
			return _packages.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos pacotes.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _packages.GetEnumerator();
		}
	}
}
