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
	/// Armazena a relação do tipo com a instancia.
	/// </summary>
	class TypeInstance
	{
		private string _contractName;

		private string _typeFullName;

		private Lazy<object> _value;

		/// <summary>
		/// Nome do contrato do tipo.
		/// </summary>
		public string ContractName
		{
			get
			{
				return string.IsNullOrEmpty(_contractName) ? _typeFullName : _contractName;
			}
		}

		/// <summary>
		/// Tipo associado.
		/// </summary>
		public string TypeFullName
		{
			get
			{
				return _typeFullName;
			}
		}

		/// <summary>
		/// Valor da instancia.
		/// </summary>
		public object Value
		{
			get
			{
				return _value.Value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type">Tipo da instancia registrada.</param>
		/// <param name="value">Valor da instancia.</param>
		/// <param name="contractName">Nome do contrato associado com a instancia.</param>
		public TypeInstance(Type type, Lazy<object> value, string contractName)
		{
			type.Require("type").NotNull();
			_typeFullName = type.FullName;
			_contractName = contractName;
			_value = value;
		}
	}
	/// <summary>
	/// Armazena o relação da instancia com o tipo.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	class TypeInstance<T> : TypeInstance
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="contractName">Nome do contrato associado com a instancia.</param>
		public TypeInstance(Lazy<T> instance, string contractName) : base(typeof(T), new Lazy<object>(() => instance.Value), contractName)
		{
		}
	}
}
