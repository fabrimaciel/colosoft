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

namespace Colosoft.IO.Xap
{
	/// <summary>
	/// Reprensenta o erro ocorrido quando for carregados dos assemblies do pacote XAP.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")]
	[Serializable]
	public class LoadXapPackageAssembliesException : XapPackageException
	{
		private AssemblyLoadError[] _errors;

		/// <summary>
		/// Errors ocorridos.
		/// </summary>
		public AssemblyLoadError[] Errors
		{
			get
			{
				return _errors;
			}
		}

		/// <summary>
		/// Cria a instancia com a mensagem.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="errors"></param>
		public LoadXapPackageAssembliesException(string message, AssemblyLoadError[] errors) : base(message, new AggregateException(message, errors.Select(f => f.Error).ToArray()))
		{
			_errors = errors;
		}

		/// <summary>
		/// Armazena os dados do erro ocorrido no load do assembly.
		/// </summary>
		public class AssemblyLoadError
		{
			/// <summary>
			/// AssemblyPart.
			/// </summary>
			public Reflection.AssemblyPart AssemblyPart
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
			/// Construtor padrão.
			/// </summary>
			/// <param name="assemblyPart"></param>
			/// <param name="error"></param>
			public AssemblyLoadError(Reflection.AssemblyPart assemblyPart, Exception error)
			{
				this.AssemblyPart = assemblyPart;
				this.Error = error;
			}
		}
	}
}
