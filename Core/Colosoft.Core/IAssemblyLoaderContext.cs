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
	/// Armazena o resultado da inicialização do contexto.
	/// </summary>
	public class AssemblyLoaderContextInitializeResult
	{
		/// <summary>
		/// Relação dos assemblies carregados.
		/// </summary>
		public string[] AssembliesLoaded
		{
			get;
			set;
		}

		/// <summary>
		/// Errors ocorridos na carga dos assemblies
		/// </summary>
		public AssemblyLoadError[] AssemblyLoadErrors
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a inicializa foi executa com sucesso.
		/// </summary>
		public bool Success
		{
			get;
			set;
		}

		/// <summary>
		/// Erro ocorrido na inicialização.
		/// </summary>
		public Exception Error
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(Error != null)
				return string.Format("Success={0}, Error: {1}", Success, Error.Message);
			return string.Format("Success={0}", Success);
		}

		/// <summary>
		/// Armazena o erro ao carregar o assembly.
		/// </summary>
		public class AssemblyLoadError
		{
			/// <summary>
			/// Nome do assembly.
			/// </summary>
			public string AssemblyName
			{
				get;
				set;
			}

			/// <summary>
			/// Erro ocorrido.
			/// </summary>
			public Exception Error
			{
				get;
				set;
			}

			/// <summary>
			/// Recupera o texto que representa a instancia.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				if(Error != null)
					return string.Format("{0}, Error: {1}", AssemblyName, Error.Message);
				return AssemblyName;
			}
		}
	}
	/// <summary>
	/// Assinatura da classe que representa um contexto do loader de assemblies.
	/// </summary>
	public interface IAssemblyLoaderContext
	{
		/// <summary>
		/// Inicializa o carga dos assemblies do contexto informado.
		/// </summary>
		/// <param name="contextNames">Nomes dos contextos.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		AssemblyLoaderContextInitializeResult InitializeContext(string[] contextNames, string uiContext);

		/// <summary>
		/// Recupera os assemblies do contexto informado.
		/// </summary>
		/// <param name="contextNames">Nomes dos contextos.</param>
		/// <param name="uiContext">Contexto de interface com o usuário.</param>
		/// <returns></returns>
		IEnumerable<string> GetContextAssemblies(string[] contextNames, string uiContext);
	}
}
