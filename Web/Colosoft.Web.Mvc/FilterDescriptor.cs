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
using Colosoft.Web.Mvc.Extensions;
using Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions;
using System.Linq.Expressions;

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Possíveis operadores de filtro.
	/// </summary>
	public enum FilterOperator
	{
		/// <summary>
		/// É menor que.
		/// </summary>
		IsLessThan,
		/// <summary>
		/// É menor que ou igual
		/// </summary>
		IsLessThanOrEqualTo,
		/// <summary>
		/// É igual.
		/// </summary>
		IsEqualTo,
		/// <summary>
		/// É diferente.
		/// </summary>
		IsNotEqualTo,
		/// <summary>
		/// É maior que ou igual.
		/// </summary>
		IsGreaterThanOrEqualTo,
		/// <summary>
		/// É maior que.
		/// </summary>
		IsGreaterThan,
		/// <summary>
		/// Começa com.
		/// </summary>
		StartsWith,
		/// <summary>
		/// Termina com.
		/// </summary>
		EndsWith,
		/// <summary>
		/// Contém.
		/// </summary>
		Contains,
		/// <summary>
		/// Está contido.
		/// </summary>
		IsContainedIn,
		/// <summary>
		/// Não contém.
		/// </summary>
		DoesNotContain
	}
	/// <summary>
	/// Representa o descritor de filtro.
	/// </summary>
	public class FilterDescriptor : FilterDescriptorBase
	{
		/// <summary>
		/// Valor convertivo.
		/// </summary>
		public object ConvertedValue
		{
			get
			{
				return this.Value;
			}
		}

		/// <summary>
		/// Nome do membro.
		/// </summary>
		public string Member
		{
			get;
			set;
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
		/// Operador.
		/// </summary>
		public FilterOperator Operator
		{
			get;
			set;
		}

		/// <summary>
		/// Valor.
		/// </summary>
		public object Value
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public FilterDescriptor() : this(string.Empty, FilterOperator.IsEqualTo, null)
		{
		}

		/// <summary>
		/// Cria a instancia com os parametros iniciais.
		/// </summary>
		/// <param name="member">Nome do membro.</param>
		/// <param name="filterOperator">Operador do filtro.</param>
		/// <param name="filterValue">Valor do filtro.</param>
		public FilterDescriptor(string member, FilterOperator filterOperator, object filterValue)
		{
			this.Member = member;
			this.Operator = filterOperator;
			this.Value = filterValue;
		}

		/// <summary>
		/// Cria a expressão do filtro.
		/// </summary>
		/// <param name="parameterExpression"></param>
		/// <returns></returns>
		protected override Expression CreateFilterExpression(ParameterExpression parameterExpression)
		{
			var builder = new Infrastructure.Implementation.Expressions.FilterDescriptorExpressionBuilder(parameterExpression, this);
			builder.Options.CopyFrom(base.ExpressionBuilderOptions);
			return builder.CreateBodyExpression();
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="json"></param>
		protected override void Serialize(IDictionary<string, object> json)
		{
			base.Serialize(json);
			json["field"] = this.Member;
			json["operator"] = this.Operator.ToToken();
			if((this.Value != null) && this.Value.GetType().GetNonNullableType().IsEnum)
			{
				Type underlyingType = Enum.GetUnderlyingType(this.Value.GetType().GetNonNullableType());
				json["value"] = Convert.ChangeType(this.Value, underlyingType);
			}
			else
			{
				json["value"] = this.Value;
			}
		}

		/// <summary>
		/// Compara a instancia com o descritor informado.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public virtual bool Equals(FilterDescriptor other)
		{
			if(object.ReferenceEquals(null, other))
			{
				return false;
			}
			return (object.ReferenceEquals(this, other) || ((object.Equals(other.Operator, this.Operator) && object.Equals(other.Member, this.Member)) && object.Equals(other.Value, this.Value)));
		}

		/// <summary>
		/// Compara a instancia com o objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			FilterDescriptor other = obj as FilterDescriptor;
			if(other == null)
			{
				return false;
			}
			return this.Equals(other);
		}

		/// <summary>
		/// Recupera o hashcode que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			int num = (Operator.GetHashCode() * 0x18d) ^ ((Member != null) ? Member.GetHashCode() : 0);
			return ((num * 0x18d) ^ ((Value != null) ? Value.GetHashCode() : 0));
		}
	}
}
