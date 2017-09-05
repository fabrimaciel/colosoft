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

namespace Colosoft.Mef
{
	/// <summary>
	/// Armazena os dados da descrição do import.
	/// </summary>
	class ImportDescription
	{
		/// <summary>
		/// Identifica se permite um valor padrão.
		/// </summary>
		public bool AllowDefault
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do contrato.
		/// </summary>
		public string ContractName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do tipo de contrato associado.
		/// </summary>
		public Colosoft.Reflection.TypeName ContractTypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo do contrato.
		/// </summary>
		public Type ContractType
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do membro que será importado.
		/// </summary>
		public string MemberName
		{
			get;
			set;
		}

		/// <summary>
		/// Indica se a definição de import é requerida para satisfazer 
		/// uma parte antes de iniciar a produção dos objetos exportados.
		/// </summary>
		public bool Prerequisite
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se a definição de importação satisfaz multiplas vezes.
		/// </summary>
		public bool Recomposable
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera a lista dos nomes dos metadados que o import necessita para satisfazer.
		/// </summary>
		public ICollection<string> RequiredMetadata
		{
			get;
			set;
		}
	}
}
