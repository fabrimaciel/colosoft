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
	/// Assinatura da classe responsável para resolver os assemblies.
	/// </summary>
	public interface IAssemblyResolver
	{
		/// <summary>
		/// Identifica se a instancia está em um estado válido.
		/// </summary>
		bool IsValid
		{
			get;
		}

		/// <summary>
		/// Resolve as informações do assembly informado.
		/// </summary>
		/// <param name="args"></param>
		/// <param name="assembly">Instancia do assembly que foi resolvido.</param>
		/// <param name="error">Erro caso tenha ocorrido.</param>
		/// <returns>True para identificar que o assembly foi resolvido.</returns>
		bool Resolve(ResolveEventArgs args, out System.Reflection.Assembly assembly, out Exception error);
	}
	/// <summary>
	/// Armazena os argumentos do evento acionado quando os resolver for carregado.
	/// </summary>
	public class AssemblyResolverLoadEventArgs : EventArgs
	{
		/// <summary>
		/// Resultado da carga dos assemblies..
		/// </summary>
		public AssemblyLoaderGetResult Result
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Representa o evento acionado quando o resolver for carregado.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void AssemblyResolverLoadHandler (object sender, AssemblyResolverLoadEventArgs e);
	/// <summary>
	/// Extensão do resolvedor de assemblies.
	/// </summary>
	public interface IAssemblyResolverExt : IAssemblyResolver
	{
		/// <summary>
		/// Evento acionado quando o resolver for carregado.
		/// </summary>
		event AssemblyResolverLoadHandler Loaded;
	}
}
