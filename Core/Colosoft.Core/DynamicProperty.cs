﻿/* 
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

namespace Colosoft.Reflection.Dynamic
{
	/// <summary>
	/// Armazena os dados de uma propriedade dinâmica.
	/// </summary>
	class DynamicProperty
	{
		private string _name;

		private Type _type;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome da propriedade.</param>
		/// <param name="type">Tipo da propriedade.</param>
		public DynamicProperty(string name, Type type)
		{
			name.Require("name").NotNull().NotEmpty();
			type.Require("name").NotNull();
			_name = name;
			_type = type;
		}

		/// <summary>
		/// Nome da propriedade.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Tipo da propriedade.
		/// </summary>
		public Type Type
		{
			get
			{
				return _type;
			}
		}
	}
}