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
using System.Linq.Expressions;
using System.Reflection;

namespace Colosoft.Query.Dynamic
{
	/// <summary>
	/// Implementação do parser de expressões.
	/// </summary>
	class ExpressionParser
	{
		/// <summary>
		/// Token
		/// </summary>
		struct Token
		{
			public TokenId id;

			public string text;

			public int pos;
		}

		/// <summary>
		/// Identificador do token.
		/// </summary>
		enum TokenId
		{
			Unknown,
			End,
			Identifier,
			StringLiteral,
			IntegerLiteral,
			RealLiteral,
			Exclamation,
			Percent,
			Amphersand,
			OpenParen,
			CloseParen,
			Asterisk,
			Plus,
			Comma,
			Minus,
			Dot,
			Slash,
			Colon,
			LessThan,
			Equal,
			GreaterThan,
			Question,
			OpenBracket,
			CloseBracket,
			Bar,
			ExclamationEqual,
			DoubleAmphersand,
			LessThanEqual,
			LessGreater,
			DoubleEqual,
			GreaterThanEqual,
			DoubleBar
		}

		/// <summary>
		/// Interface as assinaturas lógicas.
		/// </summary>
		interface ILogicalSignatures
		{
			void F(bool x, bool y);

			void F(bool? x, bool? y);
		}

		/// <summary>
		/// Interface das assinaturas aritméticas.
		/// </summary>
		interface IArithmeticSignatures
		{
			void F(int x, int y);

			void F(uint x, uint y);

			void F(long x, long y);

			void F(ulong x, ulong y);

			void F(float x, float y);

			void F(double x, double y);

			void F(decimal x, decimal y);

			void F(int? x, int? y);

			void F(uint? x, uint? y);

			void F(long? x, long? y);

			void F(ulong? x, ulong? y);

			void F(float? x, float? y);

			void F(double? x, double? y);

			void F(decimal? x, decimal? y);
		}

		/// <summary>
		/// Interface das assinaturas relacionais.
		/// </summary>
		interface IRelationalSignatures : IArithmeticSignatures
		{
			void F(string x, string y);

			void F(char x, char y);

			void F(DateTime x, DateTime y);

			void F(TimeSpan x, TimeSpan y);

			void F(char? x, char? y);

			void F(DateTime? x, DateTime? y);

			void F(TimeSpan? x, TimeSpan? y);
		}

		/// <summary>
		/// Interface das assinaturas de comparação de igualdade.
		/// </summary>
		interface IEqualitySignatures : IRelationalSignatures
		{
			void F(bool x, bool y);

			void F(bool? x, bool? y);
		}

		/// <summary>
		/// Interface das assinaturas de operações de adição.
		/// </summary>
		interface IAddSignatures : IArithmeticSignatures
		{
			void F(DateTime x, TimeSpan y);

			void F(TimeSpan x, TimeSpan y);

			void F(DateTime? x, TimeSpan? y);

			void F(TimeSpan? x, TimeSpan? y);
		}

		/// <summary>
		/// Interface das assinaturas de subtração.
		/// </summary>
		interface ISubtractSignatures : IAddSignatures
		{
			void F(DateTime x, DateTime y);

			void F(DateTime? x, DateTime? y);
		}

		/// <summary>
		/// Interface das assinaturas de negação.
		/// </summary>
		interface INegationSignatures
		{
			void F(int x);

			void F(long x);

			void F(float x);

			void F(double x);

			void F(decimal x);

			void F(int? x);

			void F(long? x);

			void F(float? x);

			void F(double? x);

			void F(decimal? x);
		}

		/// <summary>
		/// Interface das assinatura da operação de Not.
		/// </summary>
		interface INotSignatures
		{
			void F(bool x);

			void F(bool? x);
		}

		/// <summary>
		/// Interface das assinaturas de enumeração.
		/// </summary>
		interface IEnumerableSignatures
		{
			void Where(bool predicate);

			void Any();

			void Any(bool predicate);

			void All(bool predicate);

			void Count();

			void Count(bool predicate);

			void Min(object selector);

			void Max(object selector);

			void Sum(int selector);

			void Sum(int? selector);

			void Sum(long selector);

			void Sum(long? selector);

			void Sum(float selector);

			void Sum(float? selector);

			void Sum(double selector);

			void Sum(double? selector);

			void Sum(decimal selector);

			void Sum(decimal? selector);

			void Average(int selector);

			void Average(int? selector);

			void Average(long selector);

			void Average(long? selector);

			void Average(float selector);

			void Average(float? selector);

			void Average(double selector);

			void Average(double? selector);

			void Average(decimal selector);

			void Average(decimal? selector);
		}

		/// <summary>
		/// Armanze os dados de um método.
		/// </summary>
		class MethodData
		{
			/// <summary>
			/// Referencia do método base.
			/// </summary>
			public MethodBase MethodBase;

			/// <summary>
			/// Informações dos parametros do método.
			/// </summary>
			public ParameterInfo[] Parameters;

			/// <summary>
			/// Argumentos que serão repassados para o método.
			/// </summary>
			public Expression[] Args;
		}

		/// <summary>
		/// Relação dos tipos pré definidos.
		/// </summary>
		private static readonly Type[] predefinedTypes =  {
			typeof(Object),
			typeof(Boolean),
			typeof(Char),
			typeof(String),
			typeof(SByte),
			typeof(Byte),
			typeof(Int16),
			typeof(UInt16),
			typeof(Int32),
			typeof(UInt32),
			typeof(Int64),
			typeof(UInt64),
			typeof(Single),
			typeof(Double),
			typeof(Decimal),
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Guid),
			typeof(Math),
			typeof(Convert)
		};

		/// <summary>
		/// Literal que representa o valor verdadeiro.
		/// </summary>
		private static readonly System.Linq.Expressions.Expression trueLiteral = System.Linq.Expressions.Expression.Constant(true);

		/// <summary>
		/// Literal que representa o valor falso.
		/// </summary>
		private static readonly System.Linq.Expressions.Expression falseLiteral = System.Linq.Expressions.Expression.Constant(false);

		/// <summary>
		/// Literal que representa o valor nulo.
		/// </summary>
		private static readonly System.Linq.Expressions.Expression nullLiteral = System.Linq.Expressions.Expression.Constant(null);

		/// <summary>
		/// Instancia do método usado para compara as strings.
		/// </summary>
		private static readonly MethodInfo StringCompareMethod = typeof(Colosoft.Globalization.SystemStringComparer).GetMethod("GlobalCompare", BindingFlags.Static | BindingFlags.Public, null, new Type[] {
			typeof(object),
			typeof(object)
		}, null);

		private static readonly string keywordIt = "it";

		private static readonly string keywordIif = "iif";

		private static readonly string keywordNew = "new";

		/// <summary>
		/// Relação das palavras chave.
		/// </summary>
		private static Dictionary<string, object> keywords;

		private Dictionary<string, object> _symbols;

		private IDictionary<string, object> _externals;

		private Dictionary<System.Linq.Expressions.Expression, string> _literals;

		private System.Linq.Expressions.ParameterExpression _it;

		private string _text;

		private int _textPos;

		private int _textLen;

		private char _ch;

		private Token _token;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parameters"></param>
		/// <param name="expression">Expressão sobre onde será executado o parser.</param>
		/// <param name="values"></param>
		public ExpressionParser(ParameterExpression[] parameters, string expression, object[] values)
		{
			if(expression == null)
				throw new ArgumentNullException("expression");
			if(keywords == null)
				keywords = CreateKeywords();
			_symbols = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			_literals = new Dictionary<Expression, string>();
			if(parameters != null)
				ProcessParameters(parameters);
			if(values != null)
				ProcessValues(values);
			_text = expression;
			_textLen = _text.Length;
			SetTextPos(0);
			NextToken();
		}

		/// <summary>
		/// Processa os parametros informados.
		/// </summary>
		/// <param name="parameters"></param>
		private void ProcessParameters(ParameterExpression[] parameters)
		{
			foreach (ParameterExpression pe in parameters)
				if(!String.IsNullOrEmpty(pe.Name))
					AddSymbol(pe.Name, pe);
			if(parameters.Length == 1 && String.IsNullOrEmpty(parameters[0].Name))
				_it = parameters[0];
		}

		/// <summary>
		/// Processa os valores informados.
		/// </summary>
		/// <param name="values"></param>
		private void ProcessValues(object[] values)
		{
			for(int i = 0; i < values.Length; i++)
			{
				object value = values[i];
				if(i == values.Length - 1 && value is IDictionary<string, object>)
					_externals = (IDictionary<string, object>)value;
				else if(value is QueryParameter)
				{
					var parameter = (QueryParameter)value;
					AddSymbol(parameter.Name, parameter.Value);
				}
				else
					AddSymbol("@" + i.ToString(System.Globalization.CultureInfo.InvariantCulture), value);
			}
		}

		/// <summary>
		/// Adiciona o simbolo.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		private void AddSymbol(string name, object value)
		{
			if(_symbols.ContainsKey(name))
				throw ParseError(Res.DuplicateIdentifier, name);
			_symbols.Add(name, value);
		}

		/// <summary>
		/// Executa o parser do tipo do resultado.
		/// </summary>
		/// <param name="resultType"></param>
		/// <returns></returns>
		public Expression Parse(Type resultType)
		{
			int exprPos = _token.pos;
			Expression expr = ParseExpression();
			if(resultType != null)
				if((expr = PromoteExpression(expr, resultType, true)) == null)
					throw ParseError(exprPos, Res.ExpressionTypeMismatch, GetTypeName(resultType));
			ValidateToken(TokenId.End, Res.SyntaxError);
			return expr;
		}

		private Expression ParseExpression()
		{
			int errorPos = _token.pos;
			Expression expr = ParseLogicalOr();
			if(_token.id == TokenId.Question)
			{
				NextToken();
				Expression expr1 = ParseExpression();
				ValidateToken(TokenId.Colon, Res.ColonExpected);
				NextToken();
				Expression expr2 = ParseExpression();
				expr = GenerateConditional(expr, expr1, expr2, errorPos);
			}
			return expr;
		}

		private Expression ParseLogicalOr()
		{
			Expression left = ParseLogicalAnd();
			while (_token.id == TokenId.DoubleBar || TokenIdentifierIs("or"))
			{
				Token op = _token;
				NextToken();
				Expression right = ParseLogicalAnd();
				CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
				left = Expression.OrElse(left, right);
			}
			return left;
		}

		private Expression ParseLogicalAnd()
		{
			Expression left = ParseComparison();
			while (_token.id == TokenId.DoubleAmphersand || TokenIdentifierIs("and"))
			{
				Token op = _token;
				NextToken();
				Expression right = ParseComparison();
				CheckAndPromoteOperands(typeof(ILogicalSignatures), op.text, ref left, ref right, op.pos);
				left = Expression.AndAlso(left, right);
			}
			return left;
		}

		private Expression ParseComparison()
		{
			Expression left = ParseAdditive();
			while (_token.id == TokenId.Equal || _token.id == TokenId.DoubleEqual || _token.id == TokenId.ExclamationEqual || _token.id == TokenId.LessGreater || _token.id == TokenId.GreaterThan || _token.id == TokenId.GreaterThanEqual || _token.id == TokenId.LessThan || _token.id == TokenId.LessThanEqual)
			{
				Token op = _token;
				NextToken();
				Expression right = ParseAdditive();
				bool isEquality = op.id == TokenId.Equal || op.id == TokenId.DoubleEqual || op.id == TokenId.ExclamationEqual || op.id == TokenId.LessGreater;
				if(isEquality && !left.Type.IsValueType && !right.Type.IsValueType)
				{
					if(left.Type != right.Type)
					{
						if(left.Type.IsAssignableFrom(right.Type))
						{
							right = Expression.Convert(right, left.Type);
						}
						else if(right.Type.IsAssignableFrom(left.Type))
						{
							left = Expression.Convert(left, right.Type);
						}
						else
						{
							throw IncompatibleOperandsError(op.text, left, right, op.pos);
						}
					}
				}
				else if(IsEnumType(left.Type) || IsEnumType(right.Type))
				{
					if(left.Type != right.Type)
					{
						Expression e;
						if((e = PromoteExpression(right, left.Type, true)) != null)
						{
							right = e;
						}
						else if((e = PromoteExpression(left, right.Type, true)) != null)
						{
							left = e;
						}
						else
						{
							throw IncompatibleOperandsError(op.text, left, right, op.pos);
						}
					}
				}
				else
				{
					CheckAndPromoteOperands(isEquality ? typeof(IEqualitySignatures) : typeof(IRelationalSignatures), op.text, ref left, ref right, op.pos);
				}
				switch(op.id)
				{
				case TokenId.Equal:
				case TokenId.DoubleEqual:
					left = GenerateEqual(left, right);
					break;
				case TokenId.ExclamationEqual:
				case TokenId.LessGreater:
					left = GenerateNotEqual(left, right);
					break;
				case TokenId.GreaterThan:
					left = GenerateGreaterThan(left, right);
					break;
				case TokenId.GreaterThanEqual:
					left = GenerateGreaterThanEqual(left, right);
					break;
				case TokenId.LessThan:
					left = GenerateLessThan(left, right);
					break;
				case TokenId.LessThanEqual:
					left = GenerateLessThanEqual(left, right);
					break;
				}
			}
			return left;
		}

		private Expression ParseAdditive()
		{
			Expression left = ParseMultiplicative();
			while (_token.id == TokenId.Plus || _token.id == TokenId.Minus || _token.id == TokenId.Amphersand)
			{
				Token op = _token;
				NextToken();
				Expression right = ParseMultiplicative();
				switch(op.id)
				{
				case TokenId.Plus:
					if(left.Type == typeof(string) || right.Type == typeof(string))
						goto case TokenId.Amphersand;
					CheckAndPromoteOperands(typeof(IAddSignatures), op.text, ref left, ref right, op.pos);
					left = GenerateAdd(left, right);
					break;
				case TokenId.Minus:
					CheckAndPromoteOperands(typeof(ISubtractSignatures), op.text, ref left, ref right, op.pos);
					left = GenerateSubtract(left, right);
					break;
				case TokenId.Amphersand:
					left = GenerateStringConcat(left, right);
					break;
				}
			}
			return left;
		}

		private Expression ParseMultiplicative()
		{
			Expression left = ParseUnary();
			while (_token.id == TokenId.Asterisk || _token.id == TokenId.Slash || _token.id == TokenId.Percent || TokenIdentifierIs("mod"))
			{
				Token op = _token;
				NextToken();
				Expression right = ParseUnary();
				CheckAndPromoteOperands(typeof(IArithmeticSignatures), op.text, ref left, ref right, op.pos);
				switch(op.id)
				{
				case TokenId.Asterisk:
					left = Expression.Multiply(left, right);
					break;
				case TokenId.Slash:
					left = Expression.Divide(left, right);
					break;
				case TokenId.Percent:
				case TokenId.Identifier:
					left = Expression.Modulo(left, right);
					break;
				}
			}
			return left;
		}

		private Expression ParseUnary()
		{
			if(_token.id == TokenId.Minus || _token.id == TokenId.Exclamation || TokenIdentifierIs("not"))
			{
				Token op = _token;
				NextToken();
				if(op.id == TokenId.Minus && (_token.id == TokenId.IntegerLiteral || _token.id == TokenId.RealLiteral))
				{
					_token.text = "-" + _token.text;
					_token.pos = op.pos;
					return ParsePrimary();
				}
				Expression expr = ParseUnary();
				if(op.id == TokenId.Minus)
				{
					CheckAndPromoteOperand(typeof(INegationSignatures), op.text, ref expr, op.pos);
					expr = Expression.Negate(expr);
				}
				else
				{
					CheckAndPromoteOperand(typeof(INotSignatures), op.text, ref expr, op.pos);
					expr = Expression.Not(expr);
				}
				return expr;
			}
			return ParsePrimary();
		}

		private Expression ParsePrimary()
		{
			Expression expr = ParsePrimaryStart();
			while (true)
			{
				if(_token.id == TokenId.Dot)
				{
					NextToken();
					expr = ParseMemberAccess(null, expr);
				}
				else if(_token.id == TokenId.OpenBracket)
				{
					expr = ParseElementAccess(expr);
				}
				else
				{
					break;
				}
			}
			return expr;
		}

		private Expression ParsePrimaryStart()
		{
			switch(_token.id)
			{
			case TokenId.Identifier:
				return ParseIdentifier();
			case TokenId.StringLiteral:
				return ParseStringLiteral();
			case TokenId.IntegerLiteral:
				return ParseIntegerLiteral();
			case TokenId.RealLiteral:
				return ParseRealLiteral();
			case TokenId.OpenParen:
				return ParseParenExpression();
			default:
				throw ParseError(Res.ExpressionExpected);
			}
		}

		private Expression ParseStringLiteral()
		{
			ValidateToken(TokenId.StringLiteral);
			char quote = _token.text[0];
			string s = _token.text.Substring(1, _token.text.Length - 2);
			int start = 0;
			while (true)
			{
				int i = s.IndexOf(quote, start);
				if(i < 0)
					break;
				s = s.Remove(i, 1);
				start = i + 1;
			}
			if(quote == '\'')
			{
				if(s.Length != 1)
					throw ParseError(Res.InvalidCharacterLiteral);
				NextToken();
				return CreateLiteral(s[0], s);
			}
			NextToken();
			return CreateLiteral(s, s);
		}

		private Expression ParseIntegerLiteral()
		{
			ValidateToken(TokenId.IntegerLiteral);
			string text = _token.text;
			if(text[0] != '-')
			{
				ulong value;
				if(!UInt64.TryParse(text, out value))
					throw ParseError(Res.InvalidIntegerLiteral, text);
				NextToken();
				if(value <= (ulong)Int32.MaxValue)
					return CreateLiteral((int)value, text);
				if(value <= (ulong)UInt32.MaxValue)
					return CreateLiteral((uint)value, text);
				if(value <= (ulong)Int64.MaxValue)
					return CreateLiteral((long)value, text);
				return CreateLiteral(value, text);
			}
			else
			{
				long value;
				if(!Int64.TryParse(text, out value))
					throw ParseError(Res.InvalidIntegerLiteral, text);
				NextToken();
				if(value >= Int32.MinValue && value <= Int32.MaxValue)
					return CreateLiteral((int)value, text);
				return CreateLiteral(value, text);
			}
		}

		private Expression ParseRealLiteral()
		{
			ValidateToken(TokenId.RealLiteral);
			string text = _token.text;
			object value = null;
			char last = text[text.Length - 1];
			if(last == 'F' || last == 'f')
			{
				float f;
				if(Single.TryParse(text.Substring(0, text.Length - 1), out f))
					value = f;
			}
			else
			{
				double d;
				if(Double.TryParse(text, out d))
					value = d;
			}
			if(value == null)
				throw ParseError(Res.InvalidRealLiteral, text);
			NextToken();
			return CreateLiteral(value, text);
		}

		private Expression CreateLiteral(object value, string text)
		{
			ConstantExpression expr = Expression.Constant(value);
			_literals.Add(expr, text);
			return expr;
		}

		private Expression ParseParenExpression()
		{
			ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
			NextToken();
			Expression e = ParseExpression();
			ValidateToken(TokenId.CloseParen, Res.CloseParenOrOperatorExpected);
			NextToken();
			return e;
		}

		private Expression ParseIdentifier()
		{
			ValidateToken(TokenId.Identifier);
			object value;
			if(keywords.TryGetValue(_token.text, out value))
			{
				if(value is Type)
					return ParseTypeAccess((Type)value);
				if(value == (object)keywordIt)
					return ParseIt();
				if(value == (object)keywordIif)
					return ParseIif();
				if(value == (object)keywordNew)
					return ParseNew();
				NextToken();
				return (Expression)value;
			}
			if(_symbols.TryGetValue(_token.text, out value) || _externals != null && _externals.TryGetValue(_token.text, out value))
			{
				Expression expr = value as Expression;
				if(expr == null)
				{
					expr = Expression.Constant(value);
				}
				else
				{
					LambdaExpression lambda = expr as LambdaExpression;
					if(lambda != null)
						return ParseLambdaInvocation(lambda);
				}
				NextToken();
				return expr;
			}
			if(_it != null)
				return ParseMemberAccess(null, _it);
			throw ParseError(Res.UnknownIdentifier, _token.text);
		}

		private Expression ParseIt()
		{
			if(_it == null)
				throw ParseError(Res.NoItInScope);
			NextToken();
			return _it;
		}

		private Expression ParseIif()
		{
			int errorPos = _token.pos;
			NextToken();
			Expression[] args = ParseArgumentList();
			if(args.Length != 3)
				throw ParseError(errorPos, Res.IifRequiresThreeArgs);
			return GenerateConditional(args[0], args[1], args[2], errorPos);
		}

		private Expression GenerateConditional(Expression test, Expression expr1, Expression expr2, int errorPos)
		{
			if(test.Type != typeof(bool))
				throw ParseError(errorPos, Res.FirstExprMustBeBool);
			if(expr1.Type != expr2.Type)
			{
				Expression expr1as2 = expr2 != nullLiteral ? PromoteExpression(expr1, expr2.Type, true) : null;
				Expression expr2as1 = expr1 != nullLiteral ? PromoteExpression(expr2, expr1.Type, true) : null;
				if(expr1as2 != null && expr2as1 == null)
				{
					expr1 = expr1as2;
				}
				else if(expr2as1 != null && expr1as2 == null)
				{
					expr2 = expr2as1;
				}
				else
				{
					string type1 = expr1 != nullLiteral ? expr1.Type.Name : "null";
					string type2 = expr2 != nullLiteral ? expr2.Type.Name : "null";
					if(expr1as2 != null && expr2as1 != null)
						throw ParseError(errorPos, Res.BothTypesConvertToOther, type1, type2);
					throw ParseError(errorPos, Res.NeitherTypeConvertsToOther, type1, type2);
				}
			}
			return Expression.Condition(test, expr1, expr2);
		}

		private Expression ParseNew()
		{
			throw new NotSupportedException();
		}

		private Expression ParseLambdaInvocation(LambdaExpression lambda)
		{
			int errorPos = _token.pos;
			NextToken();
			Expression[] args = ParseArgumentList();
			MethodBase method;
			if(FindMethod(lambda.Type, "Invoke", false, args, out method) != 1)
				throw ParseError(errorPos, Res.ArgsIncompatibleWithLambda);
			return Expression.Invoke(lambda, args);
		}

		/// <summary>
		/// Executa parser para recupera a expressão do tipo de acesso.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private Expression ParseTypeAccess(Type type)
		{
			int errorPos = _token.pos;
			NextToken();
			if(_token.id == TokenId.Question)
			{
				if(!type.IsValueType || IsNullableType(type))
					throw ParseError(errorPos, Res.TypeHasNoNullableForm, GetTypeName(type));
				type = typeof(Nullable<>).MakeGenericType(type);
				NextToken();
			}
			if(_token.id == TokenId.OpenParen)
			{
				Expression[] args = ParseArgumentList();
				MethodBase method;
				switch(FindBestMethod(type.GetConstructors(), args, out method))
				{
				case 0:
					if(args.Length == 1)
						return GenerateConversion(args[0], type, errorPos);
					throw ParseError(errorPos, Res.NoMatchingConstructor, GetTypeName(type));
				case 1:
					return Expression.New((ConstructorInfo)method, args);
				default:
					throw ParseError(errorPos, Res.AmbiguousConstructorInvocation, GetTypeName(type));
				}
			}
			ValidateToken(TokenId.Dot, Res.DotOrOpenParenExpected);
			NextToken();
			return ParseMemberAccess(type, null);
		}

		/// <summary>
		/// Gera uma expressão de conversão.
		/// </summary>
		/// <param name="expr"></param>
		/// <param name="type"></param>
		/// <param name="errorPos"></param>
		/// <returns></returns>
		private Expression GenerateConversion(Expression expr, Type type, int errorPos)
		{
			Type exprType = expr.Type;
			if(exprType == type)
				return expr;
			if(exprType.IsValueType && type.IsValueType)
			{
				if((IsNullableType(exprType) || IsNullableType(type)) && GetNonNullableType(exprType) == GetNonNullableType(type))
					return Expression.Convert(expr, type);
				if((IsNumericType(exprType) || IsEnumType(exprType)) && (IsNumericType(type)) || IsEnumType(type))
					return Expression.ConvertChecked(expr, type);
			}
			if(exprType.IsAssignableFrom(type) || type.IsAssignableFrom(exprType) || exprType.IsInterface || type.IsInterface)
				return Expression.Convert(expr, type);
			throw ParseError(errorPos, Res.CannotConvertValue, GetTypeName(exprType), GetTypeName(type));
		}

		/// <summary>
		/// Executa o parser para recuperar o membro de acesso.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="instance"></param>
		/// <returns></returns>
		private Expression ParseMemberAccess(Type type, Expression instance)
		{
			if(instance != null)
				type = instance.Type;
			int errorPos = _token.pos;
			string id = GetIdentifier();
			NextToken();
			if(_token.id == TokenId.OpenParen)
			{
				if(instance != null && type != typeof(string))
				{
					Type enumerableType = FindGenericType(typeof(IEnumerable<>), type);
					if(enumerableType != null)
					{
						Type elementType = enumerableType.GetGenericArguments()[0];
						return ParseAggregate(instance, elementType, id, errorPos);
					}
				}
				Expression[] args = ParseArgumentList();
				MethodBase mb;
				switch(FindMethod(type, id, instance == null, args, out mb))
				{
				case 0:
					throw ParseError(errorPos, Res.NoApplicableMethod, id, GetTypeName(type));
				case 1:
					MethodInfo method = (MethodInfo)mb;
					if(method.ReturnType == typeof(void))
						throw ParseError(errorPos, Res.MethodIsVoid, id, GetTypeName(method.DeclaringType));
					return Expression.Call(instance, (MethodInfo)method, args);
				default:
					throw ParseError(errorPos, Res.AmbiguousMethodInvocation, id, GetTypeName(type));
				}
			}
			else
			{
				MemberInfo member = FindPropertyOrField(type, id, instance == null);
				if(member == null)
					throw ParseError(errorPos, Res.UnknownPropertyOrField, id, GetTypeName(type));
				return member is PropertyInfo ? Expression.Property(instance, (PropertyInfo)member) : Expression.Field(instance, (FieldInfo)member);
			}
		}

		/// <summary>
		/// Localiza o tipo genérico.
		/// </summary>
		/// <param name="generic"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Type FindGenericType(Type generic, Type type)
		{
			while (type != null && type != typeof(object))
			{
				if(type.IsGenericType && type.GetGenericTypeDefinition() == generic)
					return type;
				if(generic.IsInterface)
				{
					foreach (Type intfType in type.GetInterfaces())
					{
						Type found = FindGenericType(generic, intfType);
						if(found != null)
							return found;
					}
				}
				type = type.BaseType;
			}
			return null;
		}

		private Expression ParseAggregate(Expression instance, Type elementType, string methodName, int errorPos)
		{
			ParameterExpression outerIt = _it;
			ParameterExpression innerIt = Expression.Parameter(elementType, "");
			_it = innerIt;
			Expression[] args = ParseArgumentList();
			_it = outerIt;
			MethodBase signature;
			if(FindMethod(typeof(IEnumerableSignatures), methodName, false, args, out signature) != 1)
				throw ParseError(errorPos, Res.NoApplicableAggregate, methodName);
			Type[] typeArgs;
			if(signature.Name == "Min" || signature.Name == "Max")
			{
				typeArgs = new Type[] {
					elementType,
					args[0].Type
				};
			}
			else
			{
				typeArgs = new Type[] {
					elementType
				};
			}
			if(args.Length == 0)
			{
				args = new Expression[] {
					instance
				};
			}
			else
			{
				args = new Expression[] {
					instance,
					Expression.Lambda(args[0], innerIt)
				};
			}
			return Expression.Call(typeof(Enumerable), signature.Name, typeArgs, args);
		}

		/// <summary>
		/// Executa o parser da lista de argumentos.
		/// </summary>
		/// <returns></returns>
		private Expression[] ParseArgumentList()
		{
			ValidateToken(TokenId.OpenParen, Res.OpenParenExpected);
			NextToken();
			Expression[] args = _token.id != TokenId.CloseParen ? ParseArguments() : new Expression[0];
			ValidateToken(TokenId.CloseParen, Res.CloseParenOrCommaExpected);
			NextToken();
			return args;
		}

		/// <summary>
		/// Executa o parser dos argumentos.
		/// </summary>
		/// <returns></returns>
		private Expression[] ParseArguments()
		{
			List<Expression> argList = new List<Expression>();
			while (true)
			{
				argList.Add(ParseExpression());
				if(_token.id != TokenId.Comma)
					break;
				NextToken();
			}
			return argList.ToArray();
		}

		Expression ParseElementAccess(Expression expr)
		{
			int errorPos = _token.pos;
			ValidateToken(TokenId.OpenBracket, Res.OpenParenExpected);
			NextToken();
			Expression[] args = ParseArguments();
			ValidateToken(TokenId.CloseBracket, Res.CloseBracketOrCommaExpected);
			NextToken();
			if(expr.Type.IsArray)
			{
				if(expr.Type.GetArrayRank() != 1 || args.Length != 1)
					throw ParseError(errorPos, Res.CannotIndexMultiDimArray);
				Expression index = PromoteExpression(args[0], typeof(int), true);
				if(index == null)
					throw ParseError(errorPos, Res.InvalidIndex);
				return Expression.ArrayIndex(expr, index);
			}
			else
			{
				MethodBase mb;
				switch(FindIndexer(expr.Type, args, out mb))
				{
				case 0:
					throw ParseError(errorPos, Res.NoApplicableIndexer, GetTypeName(expr.Type));
				case 1:
					return Expression.Call(expr, (MethodInfo)mb, args);
				default:
					throw ParseError(errorPos, Res.AmbiguousIndexerInvocation, GetTypeName(expr.Type));
				}
			}
		}

		/// <summary>
		/// Verifica se o tipo informado é predifinido.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsPredefinedType(Type type)
		{
			foreach (Type t in predefinedTypes)
				if(t == type)
					return true;
			return false;
		}

		/// <summary>
		/// Verifica se o tipo informdo é nullable.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsNullableType(Type type)
		{
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}

		/// <summary>
		/// Recupera o tipo não nullable associado com o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static Type GetNonNullableType(Type type)
		{
			return IsNullableType(type) ? type.GetGenericArguments()[0] : type;
		}

		/// <summary>
		/// Recupera o nome do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static string GetTypeName(Type type)
		{
			Type baseType = GetNonNullableType(type);
			string s = baseType.Name;
			if(type != baseType)
				s += '?';
			return s;
		}

		/// <summary>
		/// Verific se o tipo informado é númerico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static bool IsNumericType(Type type)
		{
			return GetNumericTypeKind(type) != 0;
		}

		static bool IsSignedIntegralType(Type type)
		{
			return GetNumericTypeKind(type) == 2;
		}

		static bool IsUnsignedIntegralType(Type type)
		{
			return GetNumericTypeKind(type) == 3;
		}

		static int GetNumericTypeKind(Type type)
		{
			type = GetNonNullableType(type);
			if(type.IsEnum)
				return 0;
			switch(Type.GetTypeCode(type))
			{
			case TypeCode.Char:
			case TypeCode.Single:
			case TypeCode.Double:
			case TypeCode.Decimal:
				return 1;
			case TypeCode.SByte:
			case TypeCode.Int16:
			case TypeCode.Int32:
			case TypeCode.Int64:
				return 2;
			case TypeCode.Byte:
			case TypeCode.UInt16:
			case TypeCode.UInt32:
			case TypeCode.UInt64:
				return 3;
			default:
				return 0;
			}
		}

		static bool IsEnumType(Type type)
		{
			return GetNonNullableType(type).IsEnum;
		}

		void CheckAndPromoteOperand(Type signatures, string opName, ref Expression expr, int errorPos)
		{
			Expression[] args = new Expression[] {
				expr
			};
			MethodBase method;
			if(FindMethod(signatures, "F", false, args, out method) != 1)
				throw ParseError(errorPos, Res.IncompatibleOperand, opName, GetTypeName(args[0].Type));
			expr = args[0];
		}

		void CheckAndPromoteOperands(Type signatures, string opName, ref Expression left, ref Expression right, int errorPos)
		{
			Expression[] args = new Expression[] {
				left,
				right
			};
			MethodBase method;
			if(FindMethod(signatures, "F", false, args, out method) != 1)
				throw IncompatibleOperandsError(opName, left, right, errorPos);
			left = args[0];
			right = args[1];
		}

		Exception IncompatibleOperandsError(string opName, Expression left, Expression right, int pos)
		{
			return ParseError(pos, Res.IncompatibleOperands, opName, GetTypeName(left.Type), GetTypeName(right.Type));
		}

		MemberInfo FindPropertyOrField(Type type, string memberName, bool staticAccess)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
			foreach (Type t in SelfAndBaseTypes(type))
			{
				MemberInfo[] members = t.FindMembers(MemberTypes.Property | MemberTypes.Field, flags, Type.FilterNameIgnoreCase, memberName);
				if(members.Length != 0)
					return members[0];
			}
			return null;
		}

		int FindMethod(Type type, string methodName, bool staticAccess, Expression[] args, out MethodBase method)
		{
			BindingFlags flags = BindingFlags.Public | BindingFlags.DeclaredOnly | (staticAccess ? BindingFlags.Static : BindingFlags.Instance);
			foreach (Type t in SelfAndBaseTypes(type))
			{
				MemberInfo[] members = t.FindMembers(MemberTypes.Method, flags, Type.FilterNameIgnoreCase, methodName);
				int count = FindBestMethod(members.Cast<MethodBase>(), args, out method);
				if(count != 0)
					return count;
			}
			method = null;
			return 0;
		}

		int FindIndexer(Type type, Expression[] args, out MethodBase method)
		{
			foreach (Type t in SelfAndBaseTypes(type))
			{
				MemberInfo[] members = t.GetDefaultMembers();
				if(members.Length != 0)
				{
					IEnumerable<MethodBase> methods = members.OfType<PropertyInfo>().Select(p => (MethodBase)p.GetGetMethod()).Where(m => m != null);
					int count = FindBestMethod(methods, args, out method);
					if(count != 0)
						return count;
				}
			}
			method = null;
			return 0;
		}

		/// <summary>
		/// Recupera os tipos base do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<Type> SelfAndBaseTypes(Type type)
		{
			if(type.IsInterface)
			{
				List<Type> types = new List<Type>();
				AddInterface(types, type);
				return types;
			}
			return SelfAndBaseClasses(type);
		}

		/// <summary>
		/// Recupera as classe base do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private static IEnumerable<Type> SelfAndBaseClasses(Type type)
		{
			while (type != null)
			{
				yield return type;
				type = type.BaseType;
			}
		}

		private static void AddInterface(List<Type> types, Type type)
		{
			if(!types.Contains(type))
			{
				types.Add(type);
				foreach (Type t in type.GetInterfaces())
					AddInterface(types, t);
			}
		}

		/// <summary>
		/// Localiza o melho método.
		/// </summary>
		/// <param name="methods"></param>
		/// <param name="args"></param>
		/// <param name="method"></param>
		/// <returns></returns>
		private int FindBestMethod(IEnumerable<MethodBase> methods, Expression[] args, out MethodBase method)
		{
			MethodData[] applicable = methods.Select(m => new MethodData {
				MethodBase = m,
				Parameters = m.GetParameters()
			}).Where(m => IsApplicable(m, args)).ToArray();
			if(applicable.Length > 1)
			{
				applicable = applicable.Where(m => applicable.All(n => m == n || IsBetterThan(args, m, n))).ToArray();
			}
			if(applicable.Length == 1)
			{
				MethodData md = applicable[0];
				for(int i = 0; i < args.Length; i++)
					args[i] = md.Args[i];
				method = md.MethodBase;
			}
			else
			{
				method = null;
			}
			return applicable.Length;
		}

		/// <summary>
		/// Verifica se o argumentos se aplica ao método.
		/// </summary>
		/// <param name="method"></param>
		/// <param name="args"></param>
		/// <returns></returns>
		private bool IsApplicable(MethodData method, Expression[] args)
		{
			if(method.Parameters.Length != args.Length)
				return false;
			Expression[] promotedArgs = new Expression[args.Length];
			for(int i = 0; i < args.Length; i++)
			{
				ParameterInfo pi = method.Parameters[i];
				if(pi.IsOut)
					return false;
				Expression promoted = PromoteExpression(args[i], pi.ParameterType, false);
				if(promoted == null)
					return false;
				promotedArgs[i] = promoted;
			}
			method.Args = promotedArgs;
			return true;
		}

		private Expression PromoteExpression(Expression expr, Type type, bool exact)
		{
			if(expr.Type == type)
				return expr;
			if(expr is ConstantExpression)
			{
				ConstantExpression ce = (ConstantExpression)expr;
				if(ce == nullLiteral)
				{
					if(!type.IsValueType || IsNullableType(type))
						return Expression.Constant(null, type);
				}
				else
				{
					string text;
					if(_literals.TryGetValue(ce, out text))
					{
						Type target = GetNonNullableType(type);
						Object value = null;
						switch(Type.GetTypeCode(ce.Type))
						{
						case TypeCode.Int32:
						case TypeCode.UInt32:
						case TypeCode.Int64:
						case TypeCode.UInt64:
							value = ParseNumber(text, target);
							break;
						case TypeCode.Double:
							if(target == typeof(decimal))
								value = ParseNumber(text, target);
							break;
						case TypeCode.String:
							value = ParseEnum(text, target);
							break;
						}
						if(value != null)
							return Expression.Constant(value, type);
					}
				}
			}
			if(IsCompatibleWith(expr.Type, type))
			{
				if(type.IsValueType || exact)
					return Expression.Convert(expr, type);
				return expr;
			}
			return null;
		}

		/// <summary>
		/// Executa o parser de um número.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static object ParseNumber(string text, Type type)
		{
			switch(Type.GetTypeCode(GetNonNullableType(type)))
			{
			case TypeCode.SByte:
				sbyte sb;
				if(sbyte.TryParse(text, out sb))
					return sb;
				break;
			case TypeCode.Byte:
				byte b;
				if(byte.TryParse(text, out b))
					return b;
				break;
			case TypeCode.Int16:
				short s;
				if(short.TryParse(text, out s))
					return s;
				break;
			case TypeCode.UInt16:
				ushort us;
				if(ushort.TryParse(text, out us))
					return us;
				break;
			case TypeCode.Int32:
				int i;
				if(int.TryParse(text, out i))
					return i;
				break;
			case TypeCode.UInt32:
				uint ui;
				if(uint.TryParse(text, out ui))
					return ui;
				break;
			case TypeCode.Int64:
				long l;
				if(long.TryParse(text, out l))
					return l;
				break;
			case TypeCode.UInt64:
				ulong ul;
				if(ulong.TryParse(text, out ul))
					return ul;
				break;
			case TypeCode.Single:
				float f;
				if(float.TryParse(text, out f))
					return f;
				break;
			case TypeCode.Double:
				double d;
				if(double.TryParse(text, out d))
					return d;
				break;
			case TypeCode.Decimal:
				decimal e;
				if(decimal.TryParse(text, out e))
					return e;
				break;
			}
			return null;
		}

		/// <summary>
		/// Executa o parser para recupera o valor do enum.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		private static object ParseEnum(string name, Type type)
		{
			if(type.IsEnum)
			{
				MemberInfo[] memberInfos = type.FindMembers(MemberTypes.Field, BindingFlags.Public | BindingFlags.DeclaredOnly | BindingFlags.Static, Type.FilterNameIgnoreCase, name);
				if(memberInfos.Length != 0)
					return ((FieldInfo)memberInfos[0]).GetValue(null);
			}
			return null;
		}

		/// <summary>
		/// Verifica se o tipos são compatíveis.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="target"></param>
		/// <returns></returns>
		private static bool IsCompatibleWith(Type source, Type target)
		{
			if(source == target)
				return true;
			if(!target.IsValueType)
				return target.IsAssignableFrom(source);
			Type st = GetNonNullableType(source);
			Type tt = GetNonNullableType(target);
			if(st != source && tt == target)
				return false;
			TypeCode sc = st.IsEnum ? TypeCode.Object : Type.GetTypeCode(st);
			TypeCode tc = tt.IsEnum ? TypeCode.Object : Type.GetTypeCode(tt);
			switch(sc)
			{
			case TypeCode.SByte:
				switch(tc)
				{
				case TypeCode.SByte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.Byte:
				switch(tc)
				{
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.Int16:
				switch(tc)
				{
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.UInt16:
				switch(tc)
				{
				case TypeCode.UInt16:
				case TypeCode.Int32:
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.Int32:
				switch(tc)
				{
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.UInt32:
				switch(tc)
				{
				case TypeCode.UInt32:
				case TypeCode.Int64:
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.Int64:
				switch(tc)
				{
				case TypeCode.Int64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.UInt64:
				switch(tc)
				{
				case TypeCode.UInt64:
				case TypeCode.Single:
				case TypeCode.Double:
				case TypeCode.Decimal:
					return true;
				}
				break;
			case TypeCode.Single:
				switch(tc)
				{
				case TypeCode.Single:
				case TypeCode.Double:
					return true;
				}
				break;
			default:
				if(st == tt)
					return true;
				break;
			}
			return false;
		}

		static bool IsBetterThan(Expression[] args, MethodData m1, MethodData m2)
		{
			bool better = false;
			for(int i = 0; i < args.Length; i++)
			{
				int c = CompareConversions(args[i].Type, m1.Parameters[i].ParameterType, m2.Parameters[i].ParameterType);
				if(c < 0)
					return false;
				if(c > 0)
					better = true;
			}
			return better;
		}

		static int CompareConversions(Type s, Type t1, Type t2)
		{
			if(t1 == t2)
				return 0;
			if(s == t1)
				return 1;
			if(s == t2)
				return -1;
			bool t1t2 = IsCompatibleWith(t1, t2);
			bool t2t1 = IsCompatibleWith(t2, t1);
			if(t1t2 && !t2t1)
				return 1;
			if(t2t1 && !t1t2)
				return -1;
			if(IsSignedIntegralType(t1) && IsUnsignedIntegralType(t2))
				return 1;
			if(IsSignedIntegralType(t2) && IsUnsignedIntegralType(t1))
				return -1;
			return 0;
		}

		/// <summary>
		/// Cria uma expressão para tratar a comparação igual.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateEqual(Expression left, Expression right)
		{
			if(left.Type == typeof(string) || right.Type == typeof(string))
			{
				return Expression.Equal(Expression.Call(null, StringCompareMethod, new Expression[] {
					left,
					right
				}), Expression.Constant(0));
			}
			return Expression.Equal(left, right);
		}

		/// <summary>
		/// Cria a expressão para tratar a comparação diferente.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateNotEqual(Expression left, Expression right)
		{
			if(left.Type == typeof(string) || right.Type == typeof(string))
			{
				return Expression.NotEqual(Expression.Call(null, StringCompareMethod, new Expression[] {
					left,
					right
				}), Expression.Constant(0));
			}
			return Expression.NotEqual(left, right);
		}

		/// <summary>
		/// Gera uma expressão maior que.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateGreaterThan(Expression left, Expression right)
		{
			if(left.Type == typeof(string) || right.Type == typeof(string))
			{
				return Expression.GreaterThan(Expression.Call(null, StringCompareMethod, new Expression[] {
					left,
					right
				}), Expression.Constant(0));
			}
			return Expression.GreaterThan(left, right);
		}

		/// <summary>
		/// Gera uma expressão maior igual que.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateGreaterThanEqual(Expression left, Expression right)
		{
			if(left.Type == typeof(string) || right.Type == typeof(string))
			{
				return Expression.GreaterThanOrEqual(Expression.Call(null, StringCompareMethod, new Expression[] {
					left,
					right
				}), Expression.Constant(0));
			}
			return Expression.GreaterThanOrEqual(left, right);
		}

		/// <summary>
		/// Gera uma expressão menor que.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateLessThan(Expression left, Expression right)
		{
			if(left.Type == typeof(string) || right.Type == typeof(string))
			{
				return Expression.LessThan(Expression.Call(null, StringCompareMethod, new Expression[] {
					left,
					right
				}), Expression.Constant(0));
			}
			return Expression.LessThan(left, right);
		}

		/// <summary>
		/// Gera uma expressão de menor igual.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateLessThanEqual(Expression left, Expression right)
		{
			if(left.Type == typeof(string) || right.Type == typeof(string))
			{
				return Expression.LessThanOrEqual(Expression.Call(null, StringCompareMethod, new Expression[] {
					left,
					right
				}), Expression.Constant(0));
			}
			return Expression.LessThanOrEqual(left, right);
		}

		/// <summary>
		/// Gera uma operação de adição.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateAdd(Expression left, Expression right)
		{
			if(left.Type == typeof(string) && right.Type == typeof(string))
			{
				return GenerateStaticMethodCall("Concat", left, right);
			}
			return Expression.Add(left, right);
		}

		/// <summary>
		/// Gera uma expressão de subtração.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateSubtract(Expression left, Expression right)
		{
			return Expression.Subtract(left, right);
		}

		/// <summary>
		/// Gera uma expressão para concatenação de string.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateStringConcat(Expression left, Expression right)
		{
			return Expression.Call(null, typeof(string).GetMethod("Concat", new[] {
				typeof(object),
				typeof(object)
			}), new[] {
				left,
				right
			});
		}

		/// <summary>
		/// Recupera as informações de um método estático.
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private MethodInfo GetStaticMethod(string methodName, Expression left, Expression right)
		{
			return left.Type.GetMethod(methodName, new[] {
				left.Type,
				right.Type
			});
		}

		/// <summary>
		/// Recupera a chamada de um método estático.
		/// </summary>
		/// <param name="methodName">Nome do método.</param>
		/// <param name="left"></param>
		/// <param name="right"></param>
		/// <returns></returns>
		private Expression GenerateStaticMethodCall(string methodName, Expression left, Expression right)
		{
			return Expression.Call(null, GetStaticMethod(methodName, left, right), new[] {
				left,
				right
			});
		}

		/// <summary>
		/// Define a posição informada.
		/// </summary>
		/// <param name="pos"></param>
		private void SetTextPos(int pos)
		{
			_textPos = pos;
			_ch = _textPos < _textLen ? _text[_textPos] : '\0';
		}

		/// <summary>
		/// Move para o próximo caractere.
		/// </summary>
		private void NextChar()
		{
			if(_textPos < _textLen)
				_textPos++;
			_ch = _textPos < _textLen ? _text[_textPos] : '\0';
		}

		/// <summary>
		/// Move para o próximo token.
		/// </summary>
		private void NextToken()
		{
			while (Char.IsWhiteSpace(_ch))
				NextChar();
			TokenId t;
			int tokenPos = _textPos;
			switch(_ch)
			{
			case '!':
				NextChar();
				if(_ch == '=')
				{
					NextChar();
					t = TokenId.ExclamationEqual;
				}
				else
				{
					t = TokenId.Exclamation;
				}
				break;
			case '%':
				NextChar();
				t = TokenId.Percent;
				break;
			case '&':
				NextChar();
				if(_ch == '&')
				{
					NextChar();
					t = TokenId.DoubleAmphersand;
				}
				else
				{
					t = TokenId.Amphersand;
				}
				break;
			case '(':
				NextChar();
				t = TokenId.OpenParen;
				break;
			case ')':
				NextChar();
				t = TokenId.CloseParen;
				break;
			case '*':
				NextChar();
				t = TokenId.Asterisk;
				break;
			case '+':
				NextChar();
				t = TokenId.Plus;
				break;
			case ',':
				NextChar();
				t = TokenId.Comma;
				break;
			case '-':
				NextChar();
				t = TokenId.Minus;
				break;
			case '.':
				NextChar();
				t = TokenId.Dot;
				break;
			case '/':
				NextChar();
				t = TokenId.Slash;
				break;
			case ':':
				NextChar();
				t = TokenId.Colon;
				break;
			case '<':
				NextChar();
				if(_ch == '=')
				{
					NextChar();
					t = TokenId.LessThanEqual;
				}
				else if(_ch == '>')
				{
					NextChar();
					t = TokenId.LessGreater;
				}
				else
				{
					t = TokenId.LessThan;
				}
				break;
			case '=':
				NextChar();
				if(_ch == '=')
				{
					NextChar();
					t = TokenId.DoubleEqual;
				}
				else
				{
					t = TokenId.Equal;
				}
				break;
			case '>':
				NextChar();
				if(_ch == '=')
				{
					NextChar();
					t = TokenId.GreaterThanEqual;
				}
				else
				{
					t = TokenId.GreaterThan;
				}
				break;
			case '?':
				NextChar();
				if(Char.IsLetter(_ch))
				{
					do
					{
						NextChar();
					}
					while (Char.IsLetterOrDigit(_ch) || _ch == '_');
					t = TokenId.Identifier;
					break;
				}
				else
					t = TokenId.Question;
				break;
			case '[':
				NextChar();
				t = TokenId.OpenBracket;
				break;
			case ']':
				NextChar();
				t = TokenId.CloseBracket;
				break;
			case '|':
				NextChar();
				if(_ch == '|')
				{
					NextChar();
					t = TokenId.DoubleBar;
				}
				else
				{
					t = TokenId.Bar;
				}
				break;
			case '"':
			case '\'':
				char quote = _ch;
				do
				{
					NextChar();
					while (_textPos < _textLen && _ch != quote)
						NextChar();
					if(_textPos == _textLen)
						throw ParseError(_textPos, Res.UnterminatedStringLiteral);
					NextChar();
				}
				while (_ch == quote);
				t = TokenId.StringLiteral;
				break;
			default:
				if(Char.IsLetter(_ch) || _ch == '@' || _ch == '?' || _ch == '_')
				{
					do
					{
						NextChar();
					}
					while (Char.IsLetterOrDigit(_ch) || _ch == '_');
					t = TokenId.Identifier;
					break;
				}
				if(Char.IsDigit(_ch))
				{
					t = TokenId.IntegerLiteral;
					do
					{
						NextChar();
					}
					while (Char.IsDigit(_ch));
					if(_ch == '.')
					{
						t = TokenId.RealLiteral;
						NextChar();
						ValidateDigit();
						do
						{
							NextChar();
						}
						while (Char.IsDigit(_ch));
					}
					if(_ch == 'E' || _ch == 'e')
					{
						t = TokenId.RealLiteral;
						NextChar();
						if(_ch == '+' || _ch == '-')
							NextChar();
						ValidateDigit();
						do
						{
							NextChar();
						}
						while (Char.IsDigit(_ch));
					}
					if(_ch == 'F' || _ch == 'f')
						NextChar();
					break;
				}
				if(_textPos == _textLen)
				{
					t = TokenId.End;
					break;
				}
				throw ParseError(_textPos, Res.InvalidCharacter, _ch);
			}
			_token.id = t;
			_token.text = _text.Substring(tokenPos, _textPos - tokenPos);
			_token.pos = tokenPos;
		}

		private bool TokenIdentifierIs(string id)
		{
			return _token.id == TokenId.Identifier && String.Equals(id, _token.text, StringComparison.OrdinalIgnoreCase);
		}

		/// <summary>
		/// Recupera o identificador
		/// </summary>
		/// <returns></returns>
		private string GetIdentifier()
		{
			ValidateToken(TokenId.Identifier, Res.IdentifierExpected);
			string id = _token.text;
			if(id.Length > 1 && (id[0] == '@' || id[0] == '?'))
				id = id.Substring(1);
			return id;
		}

		/// <summary>
		/// Valida o digito.
		/// </summary>
		private void ValidateDigit()
		{
			if(!Char.IsDigit(_ch))
				throw ParseError(_textPos, Res.DigitExpected);
		}

		/// <summary>
		/// Valida o atual token, e mostra a mensagem de erro case seja inválido.
		/// </summary>
		/// <param name="t"></param>
		/// <param name="errorMessage"></param>
		private void ValidateToken(TokenId t, string errorMessage)
		{
			if(_token.id != t)
				throw ParseError(errorMessage);
		}

		/// <summary>
		/// Valida o atual toekn.
		/// </summary>
		/// <param name="t"></param>
		private void ValidateToken(TokenId t)
		{
			if(_token.id != t)
				throw ParseError(Res.SyntaxError);
		}

		/// <summary>
		/// Recupera um erro do parser.
		/// </summary>
		/// <param name="format">Formatação da mensagem do erro.</param>
		/// <param name="args">Argumentos que serão utilizados na formatação.</param>
		/// <returns></returns>
		private Exception ParseError(string format, params object[] args)
		{
			return ParseError(_token.pos, format, args);
		}

		/// <summary>
		/// Recupera um erro do parser.
		/// </summary>
		/// <param name="pos">Posição inicial do erro.</param>
		/// <param name="format">Formatação.</param>
		/// <param name="args">Argumentos que serão utilizados na formatação.</param>
		/// <returns></returns>
		private Exception ParseError(int pos, string format, params object[] args)
		{
			return new ParseException(string.Format(System.Globalization.CultureInfo.CurrentCulture, format, args), pos);
		}

		/// <summary>
		/// Recupera a relação das palavras chaves utilizados pelo parser.
		/// </summary>
		/// <returns></returns>
		private static Dictionary<string, object> CreateKeywords()
		{
			Dictionary<string, object> d = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
			d.Add("true", trueLiteral);
			d.Add("false", falseLiteral);
			d.Add("null", nullLiteral);
			d.Add(keywordIt, keywordIt);
			d.Add(keywordIif, keywordIif);
			d.Add(keywordNew, keywordNew);
			foreach (Type type in predefinedTypes)
				d.Add(type.Name, type);
			return d;
		}
	}
}
