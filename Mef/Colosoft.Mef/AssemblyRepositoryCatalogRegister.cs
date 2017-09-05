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
	/// Classe usada para auxiliar o registro no catálogo.
	/// </summary>
	public class AssemblyRepositoryCatalogRegister
	{
		private List<AssemblyRepositoryCatalog.ContractInfo> _contracts = new List<AssemblyRepositoryCatalog.ContractInfo>();

		/// <summary>
		/// Lista dos contratos carregados.
		/// </summary>
		internal List<AssemblyRepositoryCatalog.ContractInfo> Contracts
		{
			get
			{
				return _contracts;
			}
		}

		/// <summary>
		/// Adiciona um novo tipo para o catálogo.
		/// </summary>
		/// <typeparam name="T">Tipo que será adicionado.</typeparam>
		/// <returns></returns>
		public AssemblyRepositoryCatalogRegister Add<T>()
		{
			return Add<T>(null);
		}

		/// <summary>
		/// Adiciona um novo tipo para o catálogo.
		/// </summary>
		/// <typeparam name="T">Tipo que será adicionado.</typeparam>
		/// <param name="contractName">Nome do contrato.</param>
		/// <returns></returns>
		public AssemblyRepositoryCatalogRegister Add<T>(string contractName)
		{
			var typeName = new Colosoft.Reflection.TypeName(typeof(T).AssemblyQualifiedName);
			return Add(typeName, contractName, null);
		}

		/// <summary>
		/// Adiciona um novo tipo para o catálogo.
		/// </summary>
		/// <param name="typeName">Nome do tipo que será adicionado.</param>
		/// <returns></returns>
		public AssemblyRepositoryCatalogRegister Add(Colosoft.Reflection.TypeName typeName)
		{
			return Add(typeName, null, null);
		}

		/// <summary>
		/// Adiciona um novo tipo para o catálogo.
		/// </summary>
		/// <param name="contractTypeName">Nome do tipo que será adicionado.</param>
		/// <param name="contractName">Nome do contrato.</param>
		/// <param name="type">Tipo associado.</param>
		/// <returns></returns>
		public AssemblyRepositoryCatalogRegister Add(Colosoft.Reflection.TypeName contractTypeName, string contractName, Colosoft.Reflection.TypeName type)
		{
			contractName = string.IsNullOrEmpty(contractName) ? null : contractName;
			var index = _contracts.FindIndex(f => f.ContractName == contractName && Colosoft.Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(f.ContractType, contractTypeName));
			if(index < 0)
				_contracts.Add(new AssemblyRepositoryCatalog.ContractInfo(contractTypeName, contractName, type));
			else
			{
				var index2 = _contracts[index].Types.FindIndex(f => Colosoft.Reflection.TypeName.TypeNameEqualityComparer.Instance.Equals(f, type));
				if(index < 0)
					_contracts[index].Types.Add(type);
			}
			return this;
		}
	}
}
