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

namespace Colosoft.Query
{
	/// <summary>
	/// Classe responsável por executar o parser das condicionais do sistema.
	/// </summary>
	class ConditionalParser
	{
		private static Text.InterpreterExpression.Lexer _conditionalLexer;

		/// <summary>
		/// Instancia do analizador lexo das condicionais.
		/// </summary>
		private static Text.InterpreterExpression.Lexer ConditionalLexer
		{
			get
			{
				if(_conditionalLexer == null)
					_conditionalLexer = new Text.InterpreterExpression.Lexer(new Parser.SqlTokenParser(), new Parser.SqlDefaultLexerConfiguration());
				return _conditionalLexer;
			}
		}

		/// <summary>
		/// Recupera container condicional.
		/// </summary>
		/// <param name="enumerator">Enumerador dos items que serão analizados.</param>
		/// <param name="configuration"></param>
		/// <param name="containerInfo"></param>
		/// <returns></returns>
		private static ConditionalTerm GetContainer(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration, Text.InterpreterExpression.ContainerChars containerInfo)
		{
			ConditionalContainer container = null;
			var logical = Text.InterpreterExpression.TokenID.And;
			while (containerInfo == null || (string.IsNullOrEmpty(enumerator.Current.Text) || enumerator.Current.Text[0] != containerInfo.Stop))
			{
				var conditional = GetConditional(ref enumerator, configuration, containerInfo != null ? (char?)containerInfo.Stop : null);
				if(((conditional is ValuesArray) || (conditional is FunctionCall)) && container == null)
					return conditional;
				else if(container == null)
					container = new ConditionalContainer(conditional);
				else if(logical == Text.InterpreterExpression.TokenID.And)
					container.And(conditional);
				else if(logical == Text.InterpreterExpression.TokenID.Plus || logical == Text.InterpreterExpression.TokenID.Minus || logical == Text.InterpreterExpression.TokenID.Star || logical == Text.InterpreterExpression.TokenID.Slash)
				{
					Operator oper = null;
					switch(logical)
					{
					case Text.InterpreterExpression.TokenID.Plus:
						oper = new Operator("+");
						break;
					case Text.InterpreterExpression.TokenID.Minus:
						oper = new Operator("-");
						break;
					case Text.InterpreterExpression.TokenID.Star:
						oper = new Operator("*");
						break;
					case Text.InterpreterExpression.TokenID.Slash:
						oper = new Operator("/");
						break;
					}
					ConditionalTerm left = container;
					if(container.ConditionalsCount == 1)
						left = container.Conditionals.First();
					container = new ConditionalContainer(new Conditional(left, oper, conditional));
				}
				else
					container.Or(conditional);
				if(enumerator.Current == null || (containerInfo != null && enumerator.Current.Text[0] == containerInfo.Stop))
					return container;
				else if(enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.And || enumerator.Current.Token == (int)Query.Parser.SqlTokenID.kAnd)
					logical = Text.InterpreterExpression.TokenID.And;
				else if(enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.Or || enumerator.Current.Token == (int)Query.Parser.SqlTokenID.kOr)
					logical = Text.InterpreterExpression.TokenID.Or;
				else if(enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.Plus || enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.Minus || enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.Star || enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.Slash)
					logical = (Text.InterpreterExpression.TokenID)enumerator.Current.Token;
				if(!enumerator.MoveNext())
					break;
			}
			return container;
		}

		/// <summary>
		/// Recupera o valor do termo condicional.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		private static ConditionalTerm GetTerm(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration)
		{
			var item = enumerator.Current;
			if(item.Token == (int)Text.InterpreterExpression.TokenID.Minus)
			{
				if(!enumerator.MoveNext())
					throw new ConditionalParserException("Expected expression after minus");
				return new MinusTerm(GetTerm(ref enumerator, configuration));
			}
			else if(item.Token == (int)Parser.SqlTokenID.kCase)
			{
				return GetCaseConditional(ref enumerator, configuration);
			}
			var container = item.Text.Length > 0 && item.Token != (int)Colosoft.Text.InterpreterExpression.TokenID.StringLiteral ? Array.Find(configuration.Containers, f => f.Start == item.Text[0]) : null;
			if(container != null)
			{
				enumerator.MoveNext();
				return GetContainer(ref enumerator, configuration, container);
			}
			var expression = new Parser.SqlExpression(item);
			if(expression.Type == Parser.SqlExpressionType.Column)
				return new Column(expression);
			else if(expression.Type == Parser.SqlExpressionType.Variable)
				return new Variable(expression.Value.Text);
			return new Constant(item.ToString());
		}

		/// <summary>
		/// Recupera a formula contida nas expressões.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <param name="firstPart"></param>
		/// <returns></returns>
		private static Formula GetFormula(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration, ConditionalTerm firstPart)
		{
			var formula = new Formula();
			formula.Parts.Add(firstPart);
			formula.Operators.Add(Formula.GetMathematicalOperator(enumerator.Current));
			while (true)
			{
				if(!enumerator.MoveNext())
					throw new ConditionalParserException(string.Format("Expected expression after mathematical operator {0}", formula.Operators.First().ToString()));
				var part = GetTerm(ref enumerator, configuration);
				if(!enumerator.MoveNext())
				{
					formula.Parts.Add(part);
					break;
				}
				if(part is Column && (enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.LParen))
				{
					part = GetFunctionCall(ref enumerator, configuration, ')', (Column)part);
				}
				formula.Parts.Add(part);
				if(Formula.IsArithmeticOperator(enumerator.Current))
				{
					formula.Operators.Add(Formula.GetMathematicalOperator(enumerator.Current));
				}
				else
					break;
			}
			return formula;
		}

		/// <summary>
		/// Recupera o termo da condicional CASE.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		private static CaseConditional GetCaseConditional(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration)
		{
			if(!enumerator.MoveNext())
				throw new ConditionalParserException("Expected input expression or WHEN keyword after CASE");
			ConditionalTerm inputExpression = null;
			var whenExpressions = new List<CaseWhenExpression>();
			if(enumerator.Current.Token != (int)Parser.SqlTokenID.kWhen)
			{
				inputExpression = GetTerm(ref enumerator, configuration);
				if(!enumerator.MoveNext())
					throw new ConditionalParserException("Expected expression WHEN");
			}
			if(enumerator.Current.Token == (int)Parser.SqlTokenID.kWhen)
			{
				while (enumerator.Current.Token == (int)Parser.SqlTokenID.kWhen)
				{
					if(!enumerator.MoveNext())
						throw new ConditionalParserException("Expected expression after WHEN");
					IEnumerator<Text.InterpreterExpression.Expression> enumerator2 = new StopControlEnumerator(enumerator, new StopControlEnumerator.ContainerStartStop[] {
						new StopControlEnumerator.ContainerStartStop("CASE", "END")
					}, new string[] {
						"THEN"
					}, StringComparer.InvariantCultureIgnoreCase);
					var expression = GetTerm(ref enumerator2, configuration);
					if(!enumerator.MoveNext() || enumerator.Current.Token != (int)Parser.SqlTokenID.kThen)
						throw new ConditionalParserException(string.Format("Expected THEN after {0}", expression.ToString()));
					if(!enumerator2.MoveNext())
						throw new ConditionalParserException("Expected expression after THEN");
					enumerator2 = new StopControlEnumerator(enumerator, new StopControlEnumerator.ContainerStartStop[] {
						new StopControlEnumerator.ContainerStartStop("CASE", "END")
					}, new string[] {
						"THEN",
						"ELSE",
						"END"
					}, StringComparer.InvariantCultureIgnoreCase);
					var resultExpression = GetTerm(ref enumerator2, configuration);
					whenExpressions.Add(new CaseWhenExpression(expression, resultExpression));
					if(!enumerator2.MoveNext())
						break;
				}
			}
			else
				throw new ConditionalParserException("Expected expression WHEN");
			ConditionalTerm elseExpression = null;
			if(enumerator.Current.Token == (int)Parser.SqlTokenID.kElse)
			{
				if(enumerator.MoveNext())
				{
					IEnumerator<Text.InterpreterExpression.Expression> enumerator2 = new StopControlEnumerator(enumerator, new StopControlEnumerator.ContainerStartStop[] {
						new StopControlEnumerator.ContainerStartStop("CASE", "END")
					}, new string[] {
						"END"
					}, StringComparer.InvariantCultureIgnoreCase);
					elseExpression = GetTerm(ref enumerator2, configuration);
				}
				if(elseExpression is FunctionCall && enumerator.Current.Token == (int)Parser.SqlTokenID.kEnd)
				{
				}
				else if(!enumerator.MoveNext())
					throw new ConditionalParserException("Expected expression END");
			}
			if(enumerator.Current.Token == (int)Parser.SqlTokenID.kEnd)
				return new CaseConditional(inputExpression, whenExpressions, elseExpression);
			throw new ConditionalParserException("Expected expression END");
		}

		/// <summary>
		/// Recupera uma condição.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <param name="containerStop">Caracter que identifica o fim da condicional.</param>
		/// <returns></returns>
		private static ConditionalTerm GetConditional(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration, char? containerStop)
		{
			var left = GetTerm(ref enumerator, configuration);
			var isLeftNotToken = enumerator.Current != null && (enumerator.Current.Token == (int)Parser.SqlTokenID.kNot);
			int leftToken = enumerator.Current != null ? (int)enumerator.Current.Token : -1;
			if(!enumerator.MoveNext() || enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.And || enumerator.Current.Token == (int)Parser.SqlTokenID.kAnd || enumerator.Current.Token == (int)Text.InterpreterExpression.TokenID.Or || enumerator.Current.Token == (int)Parser.SqlTokenID.kOr)
				return left;
			if(containerStop != null && enumerator.Current.Text[0] == containerStop)
				return left;
			var oper = enumerator.Current;
			if(Formula.IsArithmeticOperator(oper))
			{
				left = GetFormula(ref enumerator, configuration, left);
				oper = enumerator.Current;
				if(enumerator.Current == null || (containerStop != null && enumerator.Current.Text[0] == containerStop))
					return left;
			}
			var operToken = _conditionalLexer.TokenParser.Parse(oper.Text);
			if(left is Constant && operToken == (int)Text.InterpreterExpression.TokenID.LParen && (leftToken == (int)Parser.SqlTokenID.kDay || leftToken == (int)Parser.SqlTokenID.kMonth || leftToken == (int)Parser.SqlTokenID.kYear || leftToken == (int)Parser.SqlTokenID.kHour || leftToken == (int)Parser.SqlTokenID.kMinute || leftToken == (int)Parser.SqlTokenID.kSecond))
				left = new Column(((Constant)left).Text);
			if(isLeftNotToken && operToken == (int)Parser.SqlTokenID.kExists)
			{
				enumerator.MoveNext();
				left = GetFunctionCall(ref enumerator, configuration, containerStop, new Column("NOT EXISTS"));
				return new Conditional(left, new Operator("NOT EXISTS"), null);
			}
			else if((operToken == (int)Text.InterpreterExpression.TokenID.LParen) && (left is Column))
			{
				left = GetFunctionCall(ref enumerator, configuration, containerStop, (Column)left);
				var function = (FunctionCall)left;
				if(StringComparer.InvariantCultureIgnoreCase.Equals(((Column)function.Call).Name, "exists"))
				{
					function.Call = new Constant("EXISTS");
					return new Conditional(left, new Operator("EXISTS"), null);
				}
				oper = enumerator.Current;
				if(oper == null)
					return function;
				operToken = _conditionalLexer.TokenParser.Parse(oper.Text);
			}
			else if(operToken == (int)Colosoft.Text.InterpreterExpression.TokenID.Comma)
				return GetValuesArray(ref enumerator, configuration, containerStop, left);
			if(!enumerator.MoveNext())
			{
				if(left is FunctionCall)
					return left;
				throw new ConditionalParserException("Right conditional not found");
			}
			if(left is FunctionCall && enumerator.Current.Token == (int)Colosoft.Query.Parser.SqlTokenID.kEnd)
				return left;
			var optOper = enumerator.Current;
			var optOperToken = _conditionalLexer.TokenParser.Parse(optOper.Text);
			bool isOptOper = (optOperToken == (int)Colosoft.Query.Parser.SqlTokenID.kNot) || (operToken == (int)Colosoft.Query.Parser.SqlTokenID.kNot);
			if(isOptOper)
			{
				if(!enumerator.MoveNext())
					throw new ConditionalParserException("Right conditional not found");
			}
			var right = GetTerm(ref enumerator, configuration);
			enumerator.MoveNext();
			if((enumerator.Current != null) && (enumerator.Current.Text[0] == '(') && (right is Column))
			{
				right = GetFunctionCall(ref enumerator, configuration, containerStop, (Column)right);
			}
			else if(operToken == (int)Colosoft.Query.Parser.SqlTokenID.kIn || optOperToken == (int)Colosoft.Query.Parser.SqlTokenID.kIn)
			{
				if(!(right is ValuesArray))
				{
					var rcc = right as ConditionalContainer;
					if(rcc != null)
					{
						right = new ValuesArray() {
							Values = rcc.Conditionals.ToArray()
						};
					}
					else
						right = new ValuesArray {
							Values = new ConditionalTerm[] {
								right
							}
						};
				}
			}
			return new Conditional(left, new Operator((isOptOper) ? oper.Text + ' ' + optOper.Text : oper.Text), right);
		}

		/// <summary>
		/// Recupera o vetor de valores.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <param name="containerStop"></param>
		/// <param name="firstConstants"></param>
		/// <returns></returns>
		private static ConditionalTerm GetValuesArray(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration, char? containerStop, params ConditionalTerm[] firstConstants)
		{
			var values = new List<ConditionalTerm>(firstConstants);
			var commaExpression = _conditionalLexer.TokenParser.GetTerm((int)Colosoft.Text.InterpreterExpression.TokenID.Comma);
			while (containerStop == null || enumerator.Current.Text[0] != containerStop)
			{
				if(enumerator.Current.Text != commaExpression)
					throw new ConditionalParserException(string.Format("Expected comma after '{0}'", values.Last().ToString()));
				if(!enumerator.MoveNext())
					break;
				var term = GetTerm(ref enumerator, configuration);
				values.Add(term);
				if(!enumerator.MoveNext())
					break;
			}
			return new ValuesArray() {
				Values = values.ToArray()
			};
		}

		/// <summary>
		/// Recupera a chamada de função.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <param name="containerStop"></param>
		/// <param name="callName"></param>
		/// <returns></returns>
		private static ConditionalTerm GetFunctionCall(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration, char? containerStop, Column callName)
		{
			var call = callName;
			var pars = new List<ConditionalTerm>();
			var commaExpression = _conditionalLexer.TokenParser.GetTerm((int)Colosoft.Text.InterpreterExpression.TokenID.Comma);
			var token = enumerator.Current;
			var distinctFlag = false;
			while (!token.ToString().Equals(")"))
			{
				if(!enumerator.MoveNext())
					break;
				token = enumerator.Current;
				if(token.ToString().Equals(")"))
				{
					enumerator.MoveNext();
					break;
				}
				var termText = enumerator.Current.Text;
				ConditionalTerm term = null;
				while (true)
				{
					term = GetTerm(ref enumerator, configuration);
					if(term is Column && StringComparer.InvariantCultureIgnoreCase.Equals(((Column)term).Name, "DISTINCT"))
					{
						enumerator.MoveNext();
						distinctFlag = true;
					}
					else
						break;
				}
				if(StringComparer.InvariantCultureIgnoreCase.Equals(call.Name, "CAST") && pars.Count == 1 && term is Column)
					term = new Constant(((Column)term).Name);
				if(!enumerator.MoveNext())
				{
					pars.Add(term);
					break;
				}
				if(Conditional.IsConditionalOperator(enumerator.Current))
				{
					var left = term;
					var oper = new Operator(enumerator.Current.Text);
					var right = GetTerm(ref enumerator);
					term = new Conditional(left, oper, right);
					if(!enumerator.MoveNext())
					{
						pars.Add(term);
						break;
					}
				}
				if(Formula.IsArithmeticOperator(enumerator.Current))
					term = GetFormula(ref enumerator, configuration, term);
				if((token.Token == (int)Colosoft.Text.InterpreterExpression.TokenID.Identifier || Colosoft.Query.Parser.SqlTokenParser.IsSqlAnsiFunction(token.Token)) && enumerator.Current.Token == (int)Colosoft.Text.InterpreterExpression.TokenID.LParen)
				{
					var columnExpression = ParserColumnExpression(termText);
					term = GetFunctionCall(ref enumerator, configuration, ')', (columnExpression as Column) ?? new Column(termText));
					if(Formula.IsArithmeticOperator(enumerator.Current))
						term = GetFormula(ref enumerator, configuration, term);
					pars.Add(term);
					token = enumerator.Current;
				}
				else
				{
					pars.Add(term);
					token = enumerator.Current;
				}
				if(token.ToString().Equals(")"))
				{
					enumerator.MoveNext();
					break;
				}
				if(!token.ToString().Equals(commaExpression))
				{
					throw new ConditionalParserException(string.Format("Expected comma after '{0}'", pars.Last().ToString()));
				}
			}
			return new FunctionCall() {
				Call = call,
				Parameters = pars.ToArray(),
				Options = distinctFlag ? FunctionCallOptions.Distinct : FunctionCallOptions.None
			};
		}

		/// <summary>
		/// Recupera o container condicional.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="configuration"></param>
		/// <returns></returns>
		private static ConditionalTerm GetContainer(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ILexerConfiguration configuration)
		{
			if(enumerator.MoveNext())
				return GetTerm(ref enumerator, configuration);
			return null;
		}

		/// <summary>
		/// Recupera o enumerator das expressões.
		/// </summary>
		/// <param name="expression"></param>
		/// <param name="lexer"></param>
		/// <returns></returns>
		private static IEnumerator<Text.InterpreterExpression.Expression> GetExpressions(string expression, Text.InterpreterExpression.Lexer lexer)
		{
			var lexResult = lexer.Execute(expression);
			var source = new List<Text.InterpreterExpression.Expression>(lexResult.Expressions);
			var size = source.Count;
			var offset = 0;
			for(var i = 0; i < size; i++)
			{
				var current = source[i];
				if(current.Token == (int)Text.InterpreterExpression.TokenID.Identifier && ((i + 2) < size) && source[i + 1].Token == (int)Text.InterpreterExpression.TokenID.Dot && source[i + 2].Token == (int)Text.InterpreterExpression.TokenID.Identifier)
				{
					var e2 = source[i + 2];
					var ee = new Text.InterpreterExpression.Expression(current.Container, current.BeginPoint, current.Line, string.Format("{0}{1}{2}", current.Text, source[i + 1].Text, e2.Text));
					lexResult.Expressions.RemoveAt(i - offset);
					lexResult.Expressions.RemoveAt(i - offset);
					lexResult.Expressions.RemoveAt(i - offset);
					lexResult.Expressions.Insert(i - offset, ee);
					offset += 2;
				}
			}
			IEnumerator<Text.InterpreterExpression.Expression> enumerator = ((IEnumerable<Text.InterpreterExpression.Expression>)lexResult.Expressions).GetEnumerator();
			return enumerator;
		}

		/// <summary>
		/// Recupera o termo com base na expressões informadas.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		public static ConditionalTerm GetTerm(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator)
		{
			enumerator.Require("enumerator").NotNull();
			if(enumerator.MoveNext())
				return GetTerm(ref enumerator, ConditionalLexer.Configuration);
			else
				return null;
		}

		/// <summary>
		/// Recupera o container condicional.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <param name="containerInfo"></param>
		/// <returns></returns>
		public static ConditionalTerm GetContainer(ref IEnumerator<Text.InterpreterExpression.Expression> enumerator, Text.InterpreterExpression.ContainerChars containerInfo)
		{
			return GetContainer(ref enumerator, ConditionalLexer.Configuration, containerInfo);
		}

		/// <summary>
		/// Executa o parser para termo da expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ConditionalTerm ParseTerm(string expression)
		{
			var lexer = ConditionalLexer;
			var lexerConfiguration = lexer.Configuration;
			var enumerator = GetExpressions(string.Format("({0})", expression), lexer);
			if(enumerator.MoveNext())
				return GetTerm(ref enumerator, lexerConfiguration);
			else
				return null;
		}

		/// <summary>
		/// Realiza o parser na expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public static ConditionalContainer Parse(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				throw new ArgumentNullException("expression");
			var lexer = ConditionalLexer;
			var lexerConfiguration = lexer.Configuration;
			var enumerator = GetExpressions(expression, lexer);
			if(enumerator.MoveNext())
			{
				var result = GetContainer(ref enumerator, lexerConfiguration, new Text.InterpreterExpression.ContainerChars(' ', ' '));
				if(!(result is ConditionalContainer))
					return new ConditionalContainer(result);
				else
					return (ConditionalContainer)result;
			}
			else
				return null;
		}

		/// <summary>
		/// Executa o parse sobre a expressão da coluna.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		private static ConditionalTerm ParserColumnExpression(string expression)
		{
			if(string.IsNullOrEmpty(expression))
				throw new ArgumentNullException("expression");
			var lexer = ConditionalLexer;
			var lexerConfiguration = lexer.Configuration;
			var enumerator = GetExpressions(expression, lexer);
			try
			{
				if(enumerator.MoveNext())
					return GetTerm(ref enumerator, lexerConfiguration);
			}
			finally
			{
				enumerator.Dispose();
			}
			return null;
		}

		/// <summary>
		/// Implementação do enumerador que realiza o controle de parada.
		/// </summary>
		class StopControlEnumerator : IEnumerator<Text.InterpreterExpression.Expression>
		{
			private IEnumerator<Text.InterpreterExpression.Expression> _reference;

			private IEnumerable<ContainerStartStop> _containersStartStop;

			private IEqualityComparer<string> _comparer;

			private IEnumerable<string> _stopTexts;

			/// <summary>
			/// Armazena a relação das tags abertas.
			/// </summary>
			private Stack<string> _tagsOpened = new Stack<string>();

			private bool _isValid = true;

			/// <summary>
			/// Recupera a atual instancia do enumerador.
			/// </summary>
			public Text.InterpreterExpression.Expression Current
			{
				get
				{
					return _isValid ? _reference.Current : null;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="reference">Referencia do enumerador principal.</param>
			/// <param name="containersStartStop">Tag de inicio e para da containers que deve ser ignoradas.</param>
			/// <param name="stopTexts">Textos de parada que será considerado.</param>
			/// <param name="comparer">Comparador que será usado.</param>
			public StopControlEnumerator(IEnumerator<Text.InterpreterExpression.Expression> reference, IEnumerable<ContainerStartStop> containersStartStop, IEnumerable<string> stopTexts, IEqualityComparer<string> comparer)
			{
				_reference = reference;
				_containersStartStop = containersStartStop;
				_stopTexts = stopTexts;
				_comparer = comparer;
			}

			/// <summary>
			/// Libera a instancia.
			/// </summary>
			public void Dispose()
			{
			}

			/// <summary>
			/// Move para o próximo item.
			/// </summary>
			/// <returns></returns>
			public bool MoveNext()
			{
				if(!_isValid)
					return false;
				if(_reference.MoveNext())
				{
					foreach (var i in _containersStartStop)
					{
						if(_comparer.Equals(_reference.Current.Text, i.Start))
						{
							_tagsOpened.Push(i.Stop);
							break;
						}
					}
					_isValid = _reference.Current != null && (_tagsOpened.Count > 0 || !_stopTexts.Any(f => _comparer.Equals(_reference.Current.Text, f)));
					if(_tagsOpened.Count > 0 && _comparer.Equals(_reference.Current.Text, _tagsOpened.Peek()))
						_tagsOpened.Pop();
					return _isValid;
				}
				_isValid = false;
				return false;
			}

			/// <summary>
			/// Reseta a instancia.
			/// </summary>
			public void Reset()
			{
				throw new NotSupportedException();
			}

			/// <summary>
			/// Recupera o atual item.
			/// </summary>
			object System.Collections.IEnumerator.Current
			{
				get
				{
					return Current;
				}
			}

			/// <summary>
			/// Representa as tags de inicio e fim de um container.
			/// </summary>
			public class ContainerStartStop
			{
				public readonly string Start;

				public readonly string Stop;

				/// <summary>
				/// Construtor padrão.
				/// </summary>
				/// <param name="start"></param>
				/// <param name="stop"></param>
				public ContainerStartStop(string start, string stop)
				{
					Start = start;
					Stop = stop;
				}
			}
		}
	}
}
