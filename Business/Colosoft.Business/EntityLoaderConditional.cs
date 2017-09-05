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

namespace Colosoft.Business
{
	/// <summary>
	/// Condicional usado no loader.
	/// </summary>
	public class EntityLoaderConditional
	{
		private string _expression;

		private List<Parameter> _parameters;

		/// <summary>
		/// Expressão condicional.
		/// </summary>
		internal string Expression
		{
			get
			{
				return _expression;
			}
		}

		/// <summary>
		/// Parametros do condicional.
		/// </summary>
		internal List<Parameter> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="expression">Expressão condicional.</param>
		public EntityLoaderConditional(string expression)
		{
			expression.Require("expression").NotNull().NotEmpty();
			_expression = expression;
			_parameters = new List<Parameter>();
		}

		/// <summary>
		/// Adiciona um parametro para o condicional.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public EntityLoaderConditional Add(string name, object value)
		{
			_parameters.Add(new Parameter(name, value));
			return this;
		}

		/// <summary>
		/// Recupera o texto da instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("{0} :: {1}", _expression, string.Join(",", _parameters.Select(f => f.ToString()).ToArray()));
		}

		/// <summary>
		/// Armaze o parametro
		/// </summary>
		internal class Parameter
		{
			private string _name;

			private object _value;

			/// <summary>
			/// Nome do parametro.
			/// </summary>
			public string Name
			{
				get
				{
					return _name;
				}
			}

			/// <summary>
			/// Valor do parametro.
			/// </summary>
			public object Value
			{
				get
				{
					return _value;
				}
			}

			/// <summary>
			/// COnstrutor padrão.
			/// </summary>
			/// <param name="name"></param>
			/// <param name="value"></param>
			public Parameter(string name, object value)
			{
				name.Require("name").NotNull();
				_name = name;
				_value = value;
			}

			/// <summary>
			/// Texto que representa o parametro.
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return string.Format("{0} : {1}", _name, _value);
			}
		}
	}
}
