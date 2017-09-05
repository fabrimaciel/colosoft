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

namespace Colosoft.Reflection.Composition
{
	/// <summary>
	/// Assinatura de uma exportação.
	/// </summary>
	public interface IExport
	{
		/// <summary>
		/// Tipo exportado.
		/// </summary>
		TypeName Type
		{
			get;
		}

		/// <summary>
		/// Nome do contrato.
		/// </summary>
		string ContractName
		{
			get;
		}

		/// <summary>
		/// Nome do tipo do contrato.
		/// </summary>
		TypeName ContractType
		{
			get;
		}

		/// <summary>
		/// Identifica se é para importa o construtor.
		/// </summary>
		bool ImportingConstructor
		{
			get;
		}

		/// <summary>
		/// Política de criação.
		/// </summary>
		CreationPolicy CreationPolicy
		{
			get;
		}

		/// <summary>
		/// Identifica se é para usar o dispatcher
		/// padrão do sistema para criar a instancia.
		/// </summary>
		bool UseDispatcher
		{
			get;
		}

		/// <summary>
		/// Metadados da instancia do export.
		/// </summary>
		IDictionary<string, object> Metadata
		{
			get;
		}
	}
	/// <summary>
	/// Assinatura com o completo da interface <see cref="IExport"/>
	/// </summary>
	public interface IExport2 : IExport
	{
		/// <summary>
		/// Contexto de interface com o usuário.
		/// </summary>
		string UIContext
		{
			get;
		}
	}
}
