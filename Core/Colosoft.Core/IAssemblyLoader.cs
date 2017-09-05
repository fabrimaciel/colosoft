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
	/// Representa o resultado
	/// </summary>
	public class AssemblyLoaderGetResult : IEnumerable<AssemblyLoaderGetResult.Entry>
	{
		/// <summary>
		/// Representa uma entrada do resultado.
		/// </summary>
		public class Entry
		{
			/// <summary>
			/// Nome do assembly.
			/// </summary>
			public string AssemblyName
			{
				get;
				private set;
			}

			/// <summary>
			/// Instancia do assembly carregado.
			/// </summary>
			public System.Reflection.Assembly Assembly
			{
				get;
				private set;
			}

			/// <summary>
			/// Identifica se o assembly foi carregado com sucesso.
			/// </summary>
			public bool Success
			{
				get;
				private set;
			}

			/// <summary>
			/// Error ocorrid na carga do assembly.
			/// </summary>
			public Exception Error
			{
				get;
				private set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="assemblyName">Nome do assembly da entrada.</param>
			/// <param name="assembly">Instancia do assembly carregado.</param>
			/// <param name="success">Identifica se a carga do assembly foi realizada com sucesso.</param>
			/// <param name="error"></param>
			public Entry(string assemblyName, System.Reflection.Assembly assembly, bool success, Exception error)
			{
				this.AssemblyName = assemblyName;
				this.Assembly = assembly;
				this.Success = success;
				this.Error = error;
			}
		}

		private List<Entry> _entries;

		/// <summary>
		/// Quantidade de entradas no resultado.
		/// </summary>
		public int Count
		{
			get
			{
				return _entries.Count;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entries">Entradas do resultado.</param>
		public AssemblyLoaderGetResult(IEnumerable<Entry> entries)
		{
			_entries = new List<Entry>(entries);
		}

		/// <summary>
		/// Recupera o enumerador das entradas do resultado.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyLoaderGetResult.Entry> GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das entradas do resultado.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _entries.GetEnumerator();
		}
	}
	/// <summary>
	/// Assinatura da classe responsável por carregar um assembly.
	/// </summary>
	public interface IAssemblyLoader
	{
		/// <summary>
		/// Tenta carregar o assembly associado com o nome informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly.</param>
		/// <param name="assembly">Instancia do assembly carregado.</param>
		/// <returns>True caso o assembly tenha sido carregado com sucesso.</returns>
		bool TryGet(string assemblyName, out System.Reflection.Assembly assembly);

		/// <summary>
		/// Tenta carregar o assembly associado com o nome informado.
		/// </summary>
		/// <param name="assemblyName">Nome do assembly.</param>
		/// <param name="assembly">Instancia do assembly carregado.</param>
		/// <param name="exception">Error ocorrido</param>
		/// <returns>True caso o assembly tenha sido carregado com sucesso.</returns>
		bool TryGet(string assemblyName, out System.Reflection.Assembly assembly, out Exception exception);

		/// <summary>
		/// Tenta carrega os assemblies informados.
		/// </summary>
		/// <param name="assemblyNames">Nomes dos assemblies que serão carregados.</param>
		/// <returns></returns>
		AssemblyLoaderGetResult Get(string[] assemblyNames);
	}
}
