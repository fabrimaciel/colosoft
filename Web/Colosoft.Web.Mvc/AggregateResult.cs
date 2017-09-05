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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Infrastructure
{
	/// <summary>
	/// Agregado de resultado.
	/// </summary>
	public class AggregateResult
	{
		private object _aggregateValue;

		private readonly AggregateFunction _function;

		private int _itemCount;

		/// <summary>
		/// Nome do método de agregação.
		/// </summary>
		public string AggregateMethodName
		{
			get
			{
				return _function.AggregateMethodName;
			}
		}

		/// <summary>
		/// Rótulo.
		/// </summary>
		public string Caption
		{
			get
			{
				return _function.Caption;
			}
		}

		/// <summary>
		/// Valor formatado.
		/// </summary>
		public object FormattedValue
		{
			get
			{
				if(string.IsNullOrEmpty(_function.ResultFormatString))
				{
					return _aggregateValue;
				}
				return string.Format(System.Globalization.CultureInfo.CurrentCulture, this._function.ResultFormatString, _aggregateValue);
			}
		}

		/// <summary>
		/// Nome da função.
		/// </summary>
		public string FunctionName
		{
			get
			{
				return _function.FunctionName;
			}
		}

		/// <summary>
		/// Quantidade de itens.
		/// </summary>
		public int ItemCount
		{
			get
			{
				return _itemCount;
			}
			set
			{
				_itemCount = value;
			}
		}

		/// <summary>
		/// Nome do membro.
		/// </summary>
		public string Member
		{
			get
			{
				return this._function.SourceField;
			}
		}

		/// <summary>
		/// Valor;
		/// </summary>
		public object Value
		{
			get
			{
				return _aggregateValue;
			}
			internal set
			{
				_aggregateValue = value;
			}
		}

		/// <summary>
		/// Cria a instancia com a função de agregação.
		/// </summary>
		/// <param name="function"></param>
		public AggregateResult(AggregateFunction function) : this(null, function)
		{
		}

		/// <summary>
		/// Cria a instancia com o valor do resultado e a função de agregação.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="function"></param>
		public AggregateResult(object value, AggregateFunction function) : this(value, 0, function)
		{
		}

		/// <summary>
		/// Cria a instancia com o valor do resultado, a quantidade de itens e a função de agregação.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="count"></param>
		/// <param name="function"></param>
		public AggregateResult(object value, int count, AggregateFunction function)
		{
			if(function == null)
			{
				throw new ArgumentNullException("function");
			}
			_aggregateValue = value;
			_itemCount = count;
			_function = function;
		}

		/// <summary>
		/// Formata o valor do resultado.
		/// </summary>
		/// <param name="format">Formato que será aplicado.</param>
		/// <returns></returns>
		public string Format(string format)
		{
			if(Value != null)
				return string.Format(format, Value);
			return this.ToString();
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(Value != null)
			{
				return Value.ToString();
			}
			return base.ToString();
		}
	}
}
