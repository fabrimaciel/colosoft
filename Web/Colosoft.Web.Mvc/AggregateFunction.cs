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

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Representa um função agregada.
	/// </summary>
	public abstract class AggregateFunction : JsonObject
	{
		private string _functionName;

		/// <summary>
		/// Nome do método agregado.
		/// </summary>
		public abstract string AggregateMethodName
		{
			get;
		}

		/// <summary>
		/// Rótulo.
		/// </summary>
		public string Caption
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da função.
		/// </summary>
		public virtual string FunctionName
		{
			get
			{
				if(string.IsNullOrEmpty(_functionName))
					_functionName = GenerateFunctionName();
				return _functionName;
			}
			set
			{
				_functionName = value;
			}
		}

		/// <summary>
		/// Tipo do membro.
		/// </summary>
		public Type MemberType
		{
			get;
			set;
		}

		/// <summary>
		/// Texto do formato do resultado.
		/// </summary>
		public virtual string ResultFormatString
		{
			get;
			set;
		}

		/// <summary>
		/// Campo de origem.
		/// </summary>
		public virtual string SourceField
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected AggregateFunction()
		{
		}

		/// <summary>
		/// Cria uma expressão agregada.
		/// </summary>
		/// <param name="enumerableExpression">Expressões que serão agregadas.</param>
		/// <param name="liftMemberAccessToNull"></param>
		/// <returns></returns>
		public abstract System.Linq.Expressions.Expression CreateAggregateExpression(System.Linq.Expressions.Expression enumerableExpression, bool liftMemberAccessToNull);

		/// <summary>
		/// Gera o nome da função.
		/// </summary>
		/// <returns></returns>
		protected virtual string GenerateFunctionName()
		{
			return string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}_{1}", base.GetType().Name, this.GetHashCode());
		}

		/// <summary>
		/// Serializa os dados da função.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
			json["field"] = SourceField;
			json["aggregate"] = FunctionName.Split('_')[0].ToLowerInvariant();
		}
	}
}
