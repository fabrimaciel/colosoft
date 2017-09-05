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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation
{
	/// <summary>
	/// Implementação do parse de filtro.
	/// </summary>
	public class FilterParser
	{
		private int _currentTokenIndex;

		private readonly IList<FilterToken> _tokens;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="input">Texto que será tratado.</param>
		public FilterParser(string input)
		{
			_tokens = new FilterLexer(input).Tokenize();
		}

		/// <summary>
		/// And expression.
		/// </summary>
		/// <returns></returns>
		private IFilterNode AndExpression()
		{
			var firstArgument = ComparisonExpression();
			if(Is(FilterTokenType.And))
				return this.ParseAndExpression(firstArgument);
			return firstArgument;
		}

		/// <summary>
		/// Recupera um nó da expressão de comparação.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ComparisonExpression()
		{
			IFilterNode firstArgument = PrimaryExpression();
			if(!this.Is(FilterTokenType.ComparisonOperator) && !this.Is(FilterTokenType.Function))
			{
				return firstArgument;
			}
			return this.ParseComparisonExpression(firstArgument);
		}

		/// <summary>
		/// Espera pelo tipo de token.
		/// </summary>
		/// <param name="tokenType"></param>
		/// <returns></returns>
		private FilterToken Expect(FilterTokenType tokenType)
		{
			if(!Is(tokenType))
				throw new FilterParserException("Expected " + tokenType);
			FilterToken token = this.Peek();
			_currentTokenIndex++;
			return token;
		}

		/// <summary>
		/// Esperase uma expressão,
		/// </summary>
		/// <returns></returns>
		private IFilterNode Expression()
		{
			return OrExpression();
		}

		/// <summary>
		/// Verifica se o atual toke é do tipo informado.
		/// </summary>
		/// <param name="tokenType"></param>
		/// <returns></returns>
		private bool Is(FilterTokenType tokenType)
		{
			FilterToken token = Peek();
			return ((token != null) && (token.TokenType == tokenType));
		}

		/// <summary>
		/// Recupera a expressão OR.
		/// </summary>
		/// <returns></returns>
		private IFilterNode OrExpression()
		{
			IFilterNode firstArgument = this.AndExpression();
			if(Is(FilterTokenType.Or))
			{
				return ParseOrExpression(firstArgument);
			}
			if(Is(FilterTokenType.And))
			{
				Expect(FilterTokenType.And);
				return new AndNode {
					First = firstArgument,
					Second = OrExpression()
				};
			}
			return firstArgument;
		}

		/// <summary>
		/// Executa o parse.
		/// </summary>
		/// <returns></returns>
		public IFilterNode Parse()
		{
			if(_tokens.Count > 0)
				return Expression();
			return null;
		}

		/// <summary>
		/// Executa o parse sobre a expressão AND.
		/// </summary>
		/// <param name="firstArgument"></param>
		/// <returns></returns>
		private IFilterNode ParseAndExpression(IFilterNode firstArgument)
		{
			Expect(FilterTokenType.And);
			var node = ComparisonExpression();
			return new AndNode {
				First = firstArgument,
				Second = node
			};
		}

		/// <summary>
		/// Executa o parse sobre um nó com o valor boolean.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParseBoolean()
		{
			FilterToken token = this.Expect(FilterTokenType.Boolean);
			return new BooleanNode {
				Value = Convert.ToBoolean(token.Value)
			};
		}

		/// <summary>
		/// Executa o parser a expressão de comparação.
		/// </summary>
		/// <param name="firstArgument"></param>
		/// <returns></returns>
		private IFilterNode ParseComparisonExpression(IFilterNode firstArgument)
		{
			if(this.Is(FilterTokenType.ComparisonOperator))
			{
				FilterToken token = this.Expect(FilterTokenType.ComparisonOperator);
				IFilterNode node = this.PrimaryExpression();
				return new ComparisonNode {
					First = firstArgument,
					FilterOperator = token.ToFilterOperator(),
					Second = node
				};
			}
			FilterToken token2 = this.Expect(FilterTokenType.Function);
			FunctionNode node3 = new FunctionNode {
				FilterOperator = token2.ToFilterOperator()
			};
			node3.Arguments.Add(firstArgument);
			node3.Arguments.Add(this.PrimaryExpression());
			return node3;
		}

		/// <summary>
		/// Executa o parse em uma expressão com um DateTime.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParseDateTimeExpression()
		{
			FilterToken token = this.Expect(FilterTokenType.DateTime);
			return new DateTimeNode {
				Value = DateTime.ParseExact(token.Value, "yyyy-MM-ddTHH-mm-ss", null)
			};
		}

		/// <summary>
		/// Executa o parse na expressão com uma função.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParseFunctionExpression()
		{
			FilterToken token = this.Expect(FilterTokenType.Function);
			FunctionNode node = new FunctionNode {
				FilterOperator = token.ToFilterOperator()
			};
			this.Expect(FilterTokenType.LeftParenthesis);
			node.Arguments.Add(this.Expression());
			while (this.Is(FilterTokenType.Comma))
			{
				this.Expect(FilterTokenType.Comma);
				node.Arguments.Add(this.Expression());
			}
			this.Expect(FilterTokenType.RightParenthesis);
			return node;
		}

		/// <summary>
		/// Executa o parse na expressão com uma estrutura aninhada.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParseNestedExpression()
		{
			this.Expect(FilterTokenType.LeftParenthesis);
			IFilterNode node = this.Expression();
			this.Expect(FilterTokenType.RightParenthesis);
			return node;
		}

		/// <summary>
		/// Executa o parse na expressão contendo um número.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParseNumberExpression()
		{
			FilterToken token = this.Expect(FilterTokenType.Number);
			return new NumberNode {
				Value = Convert.ToDouble(token.Value, System.Globalization.CultureInfo.InvariantCulture)
			};
		}

		/// <summary>
		/// Executa o parse na expressão com a estrutura OR.
		/// </summary>
		/// <param name="firstArgument"></param>
		/// <returns></returns>
		private IFilterNode ParseOrExpression(IFilterNode firstArgument)
		{
			this.Expect(FilterTokenType.Or);
			IFilterNode node = this.OrExpression();
			return new OrNode {
				First = firstArgument,
				Second = node
			};
		}

		/// <summary>
		/// Executa o parser na expressão com os dado da propriedade.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParsePropertyExpression()
		{
			FilterToken token = this.Expect(FilterTokenType.Property);
			return new PropertyNode {
				Name = token.Value
			};
		}

		/// <summary>
		/// Executa o parse na expressão com um texto.
		/// </summary>
		/// <returns></returns>
		private IFilterNode ParseStringExpression()
		{
			FilterToken token = this.Expect(FilterTokenType.String);
			return new StringNode {
				Value = token.Value
			};
		}

		/// <summary>
		/// Recupera o token que está no topo.
		/// </summary>
		/// <returns></returns>
		private FilterToken Peek()
		{
			if(_currentTokenIndex < this._tokens.Count)
			{
				return _tokens[this._currentTokenIndex];
			}
			return null;
		}

		/// <summary>
		/// Recupera a expressão primária.
		/// </summary>
		/// <returns></returns>
		private IFilterNode PrimaryExpression()
		{
			if(this.Is(FilterTokenType.LeftParenthesis))
				return this.ParseNestedExpression();
			if(this.Is(FilterTokenType.Function))
				return this.ParseFunctionExpression();
			if(this.Is(FilterTokenType.Boolean))
				return this.ParseBoolean();
			if(this.Is(FilterTokenType.DateTime))
				return this.ParseDateTimeExpression();
			if(this.Is(FilterTokenType.Property))
				return this.ParsePropertyExpression();
			if(this.Is(FilterTokenType.Number))
				return this.ParseNumberExpression();
			if(!this.Is(FilterTokenType.String))
				throw new FilterParserException("Expected primaryExpression");
			return this.ParseStringExpression();
		}
	}
}
