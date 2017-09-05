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
using Colosoft.Logging;
using Colosoft.Text.Parser;
using System.IO;
using Colosoft.Caching.Queries.Filters;

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Representa o parser para a linguagem de consulta do cache (CQL).
	/// </summary>
	public class CQLParser : GoldParser
	{
		private ILogger _logger;

		/// <summary>
		/// Identifica se as informações para log estão habilitadas.
		/// </summary>
		private bool IsInfoEnabled
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Instancia do logger associado.
		/// </summary>
		private ILogger Logger
		{
			get
			{
				return _logger;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="stream">Stream onde está a definição da linguagem.</param>
		public CQLParser(Stream stream) : base(stream)
		{
		}

		/// <summary>
		/// Cria uma instancia recupera a definição da linguagem de um arquivo de resource.
		/// </summary>
		/// <param name="resourceName"></param>
		/// <param name="Logger"></param>
		public CQLParser(string resourceName, ILogger Logger)
		{
			_logger = Logger;
			var manifestResourceStream = base.GetType().Assembly.GetManifestResourceStream(resourceName);
			base.LoadGrammar(manifestResourceStream);
		}

		/// <summary>
		/// Cria uma lista de inclusão
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected Reduction CreateInclusionList(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(2).Data).Tag;
			IsInListPredicate predicate = null;
			if(tag is IsInListPredicate)
				predicate = tag as IsInListPredicate;
			else
			{
				predicate = new IsInListPredicate();
				predicate.Append(tag);
			}
			predicate.Append(((Reduction)reduction.GetToken(0).Data).Tag);
			reduction.Tag = predicate;
			return null;
		}

		/// <summary>
		/// Cria um novo objeto com base no tipo da regra.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		private Reduction CreateNewObject(Reduction reduction)
		{
			Reduction reduction2 = null;
			switch(((RuleConstants)Enum.ToObject(typeof(RuleConstants), reduction.ParentRule.TableIndex)))
			{
			case RuleConstants.RULE_QUERY_SELECT:
				reduction2 = this.CreateRULE_QUERY_SELECT(reduction);
				break;
			case RuleConstants.RULE_QUERY_SELECT_WHERE:
				reduction2 = this.CreateRULE_QUERY_SELECT_WHERE(reduction);
				break;
			case RuleConstants.RULE_EXPRESSION:
				reduction2 = this.CreateRULE_EXPRESSION(reduction);
				break;
			case RuleConstants.RULE_OREXPR_OR:
				reduction2 = this.CreateRULE_OREXPR_OR(reduction);
				break;
			case RuleConstants.RULE_OREXPR:
				reduction2 = this.CreateRULE_OREXPR(reduction);
				break;
			case RuleConstants.RULE_ANDEXPR_AND:
				reduction2 = this.CreateRULE_ANDEXPR_AND(reduction);
				break;
			case RuleConstants.RULE_ANDEXPR:
				reduction2 = this.CreateRULE_ANDEXPR(reduction);
				break;
			case RuleConstants.RULE_UNARYEXPR_NOT:
				reduction2 = this.CreateRULE_UNARYEXPR_NOT(reduction);
				break;
			case RuleConstants.RULE_UNARYEXPR:
				reduction2 = this.CreateRULE_UNARYEXPR(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_EQ:
				reduction2 = this.CreateRULE_COMPAREEXPR_EQ(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_EXCLAMEQ:
				reduction2 = this.CreateRULE_COMPAREEXPR_EXCLAMEQ(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_EQEQ:
				reduction2 = this.CreateRULE_COMPAREEXPR_EQEQ(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_LTGT:
				reduction2 = this.CreateRULE_COMPAREEXPR_LTGT(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_LT:
				reduction2 = this.CreateRULE_COMPAREEXPR_LT(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_GT:
				reduction2 = this.CreateRULE_COMPAREEXPR_GT(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_LTEQ:
				reduction2 = this.CreateRULE_COMPAREEXPR_LTEQ(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_GTEQ:
				reduction2 = this.CreateRULE_COMPAREEXPR_GTEQ(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_LIKE_STRINGLITERAL:
				throw new NotSupportedException();
			case RuleConstants.RULE_COMPAREEXPR_LIKE_PARAMETER:
				reduction2 = this.CreateRULE_COMPAREEXPR_LIKE_PARAMETER(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_NOT_LIKE_STRINGLITERAL:
				throw new NotSupportedException();
			case RuleConstants.RULE_COMPAREEXPR_NOT_LIKE_PARAMETER:
				reduction2 = this.CreateRULE_COMPAREEXPR_NOT_LIKE_PARAMETER(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_IN:
				reduction2 = this.CreateRULE_COMPAREEXPR_IN(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_NOT_IN:
				reduction2 = this.CreateRULE_COMPAREEXPR_NOT_IN(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_IS_NULL:
				reduction2 = this.CreateRULE_COMPAREEXPR_IS_NULL(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_IS_NOT_NULL:
				reduction2 = this.CreateRULE_COMPAREEXPR_IS_NOT_NULL(reduction);
				break;
			case RuleConstants.RULE_COMPAREEXPR_LPARAN_RPARAN:
				reduction2 = this.CreateRULE_COMPAREEXPR_LPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_ATTRIB:
				reduction2 = this.CreateRULE_ATTRIB(reduction);
				break;
			case RuleConstants.RULE_VALUE_MINUS:
				reduction2 = this.CreateRULE_VALUE_MINUS(reduction);
				break;
			case RuleConstants.RULE_VALUE:
				reduction2 = this.CreateRULE_VALUE(reduction);
				break;
			case RuleConstants.RULE_VALUE2:
				reduction2 = this.CreateRULE_VALUE2(reduction);
				break;
			case RuleConstants.RULE_VALUE_TRUE:
				reduction2 = this.CreateRULE_VALUE_TRUE(reduction);
				break;
			case RuleConstants.RULE_VALUE_FALSE:
				reduction2 = this.CreateRULE_VALUE_FALSE(reduction);
				break;
			case RuleConstants.RULE_BITWISEEXPR_LPARAN_RPARAN:
				reduction2 = this.CreateRULE_BITWISEEXPR_LPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_BITWISEEXPR_BITWISEAND:
				reduction2 = this.CreateRULE_BITWISEEXPR_BITWISEAND(reduction);
				break;
			case RuleConstants.RULE_BITWISEEXPR_BITWISEOR:
				reduction2 = this.CreateRULE_BITWISEEXPR_BITWISEOR(reduction);
				break;
			case RuleConstants.RULE_VALUE3:
				reduction2 = this.CreateRULE_VALUE3(reduction);
				break;
			case RuleConstants.RULE_DATE_DATETIME_DOT_NOW:
				reduction2 = this.CreateRULE_DATE_DATETIME_DOT_NOW(reduction);
				break;
			case RuleConstants.RULE_DATE_DATETIME_LPARAN_STRINGLITERAL_RPARAN:
				reduction2 = this.CreateRULE_DATE_DATETIME_LPARAN_STRINGLITERAL_RPARAN(reduction);
				break;
			case RuleConstants.RULE_STRLITERAL_STRINGLITERAL:
				reduction2 = this.CreateRULE_STRLITERAL_STRINGLITERAL(reduction);
				break;
			case RuleConstants.RULE_STRLITERAL_NULL:
				reduction2 = this.CreateRULE_STRLITERAL_NULL(reduction);
				break;
			case RuleConstants.RULE_STRLITERAL_PARAMETER:
				reduction2 = this.CreateRULE_STRLITERAL_PARAMETER(reduction);
				break;
			case RuleConstants.RULE_NUMLITERAL_INTEGERLITERAL:
				reduction2 = this.CreateRULE_NUMLITERAL_INTEGERLITERAL(reduction);
				break;
			case RuleConstants.RULE_NUMLITERAL_REALLITERAL:
				reduction2 = this.CreateRULE_NUMLITERAL_REALLITERAL(reduction);
				break;
			case RuleConstants.RULE_OBJECTTYPE_TIMES:
				reduction2 = this.CreateRULE_OBJECTTYPE_TIMES(reduction);
				break;
			case RuleConstants.RULE_OBJECTTYPE_DOLLARTEXTDOLLAR:
				reduction2 = this.CreateRULE_OBJECTTYPE_DOLLARTEXTDOLLAR(reduction);
				break;
			case RuleConstants.RULE_OBJECTTYPE2:
				reduction2 = this.CreateRULE_OBJECTTYPE2(reduction);
				break;
			case RuleConstants.RULE_OBJECTATTRIBUTE_IDENTIFIER:
				reduction2 = this.CreateRULE_OBJECTATTRIBUTE_IDENTIFIER(reduction);
				break;
			case RuleConstants.RULE_PROPERTY_DOT_IDENTIFIER:
				reduction2 = this.CreateRULE_OBJECTTYPE_IDENTIFIER_DOT(reduction);
				break;
			case RuleConstants.RULE_PROPERTY_IDENTIFIER:
				reduction2 = this.CreateRULE_OBJECTTYPE_IDENTIFIER(reduction);
				break;
			case RuleConstants.RULE_SUMFUNCTION_SUMLPARAN_RPARAN:
				reduction2 = this.CreateRULE_SUMFUNCTION_SUMLPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_COUNTFUNCTION_COUNTLPARAN_RPARAN:
				reduction2 = this.CreateRULE_COUNTFUNCTION_COUNTLPARAN_TIMES_RPARAN(reduction);
				break;
			case RuleConstants.RULE_MINFUNCTION_MINLPARAN_RPARAN:
				reduction2 = this.CreateRULE_MINFUNCTION_MINLPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_MAXFUNCTION_MAXLPARAN_RPARAN:
				reduction2 = this.CreateRULE_MAXFUNCTION_MAXLPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_AVERAGEFUNCTION_AVGLPARAN_RPARAN:
				reduction2 = this.CreateRULE_AVERAGEFUNCTION_AVGLPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_OBJECTVALUE_KEYWORD_DOT_IDENTIFIER:
				reduction2 = this.CreateRULE_OBJECTVALUE_KEYWORD_DOT_IDENTIFIER(reduction);
				break;
			case RuleConstants.RULE_OBJECTVALUE_KEYWORD_DOT_TAG:
				reduction2 = this.CreateRULE_OBJECTVALUE_KEYWORD_DOT_TAG(reduction);
				break;
			case RuleConstants.RULE_INLIST_LPARAN_RPARAN:
				reduction2 = this.CreateRULE_INLIST_LPARAN_RPARAN(reduction);
				break;
			case RuleConstants.RULE_LISTTYPE:
				reduction2 = this.CreateRULE_LISTTYPE(reduction);
				break;
			case RuleConstants.RULE_LISTTYPE2:
				reduction2 = this.CreateRULE_LISTTYPE2(reduction);
				break;
			case RuleConstants.RULE_LISTTYPE3:
				reduction2 = this.CreateRULE_LISTTYPE3(reduction);
				break;
			case RuleConstants.RULE_NUMLITERALLIST_COMMA:
			case RuleConstants.RULE_NUMLITERALLIST_COMMA2:
				reduction2 = this.CreateRULE_NUMLITERALLIST_COMMA(reduction);
				break;
			case RuleConstants.RULE_NUMLITERALLIST:
				reduction2 = this.CreateRULE_NUMLITERALLIST(reduction);
				break;
			case RuleConstants.RULE_STRLITERALLIST_COMMA:
			case RuleConstants.RULE_STRLITERALLIST_COMMA2:
				reduction2 = this.CreateRULE_STRLITERALLIST_COMMA(reduction);
				break;
			case RuleConstants.RULE_STRLITERALLIST:
				reduction2 = this.CreateRULE_STRLITERALLIST(reduction);
				break;
			case RuleConstants.RULE_DATELIST_COMMA:
				reduction2 = this.CreateRULE_DATELIST_COMMA(reduction);
				break;
			case RuleConstants.RULE_DATELIST:
				reduction2 = this.CreateRULE_DATELIST(reduction);
				break;
			}
			if(reduction2 == null)
				reduction2 = reduction;
			return reduction2;
		}

		/// <summary>
		/// Cria a regra para comparação NOT LIKE com parametro.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		private Reduction CreateRULE_COMPAREEXPR_NOT_LIKE_PARAMETER(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			var parameterName = ((Reduction)reduction.GetToken(2).Data).Tag as string;
			var parameter = new Parameter(parameterName);
			var predicate = ExpressionBuilder.CreateLikePatternPredicate(tag, parameter);
			predicate.Invert();
			reduction.Tag = predicate;
			return null;
		}

		/// <summary>
		/// Cria a regra para comparação LIKE com parametro.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		private Reduction CreateRULE_COMPAREEXPR_LIKE_PARAMETER(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			var parameterName = reduction.GetToken(2).Data as string;
			var parameter = new Parameter(parameterName);
			reduction.Tag = ExpressionBuilder.CreateLikePatternPredicate(tag, parameter);
			return null;
		}

		/// <summary>
		/// Cria a redição para a regra.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		private Reduction CreateRULE_BITWISEEXPR_LPARAN_RPARAN(Reduction reduction)
		{
			reduction.Tag = ((Reduction)reduction.GetToken(1).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a reduction para o BitwiseAnd
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		private Reduction CreateRULE_BITWISEEXPR_BITWISEAND(Reduction reduction)
		{
			object leftTag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object rightTag = ((Reduction)reduction.GetToken(2).Data).Tag;
			bool flag = leftTag is IGenerator;
			bool flag2 = rightTag is IGenerator;
			if(flag && flag2)
			{
				var leftValue = Convert.ToInt32(((IGenerator)leftTag).Evaluate());
				var rightValue = Convert.ToInt32(((IGenerator)rightTag).Evaluate());
				reduction.Tag = new IntegerConstantValue(leftValue & rightValue);
			}
			else
			{
				IFunctor functor = !flag ? (IFunctor)leftTag : (IFunctor)rightTag;
				reduction.Tag = new BitwiseFunction(functor, flag ? (IGenerator)leftTag : (IGenerator)rightTag, BitwiseOperator.And);
			}
			return null;
		}

		/// <summary>
		/// Cria a reduction para o BitwiseOr
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		private Reduction CreateRULE_BITWISEEXPR_BITWISEOR(Reduction reduction)
		{
			object leftTag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object rightTag = ((Reduction)reduction.GetToken(2).Data).Tag;
			bool flag = leftTag is IGenerator;
			bool flag2 = rightTag is IGenerator;
			if(flag && flag2)
			{
				var leftValue = Convert.ToInt32(((IGenerator)leftTag).Evaluate());
				var rightValue = Convert.ToInt32(((IGenerator)rightTag).Evaluate());
				reduction.Tag = new IntegerConstantValue(leftValue | rightValue);
			}
			else
			{
				IFunctor functor = !flag ? (IFunctor)leftTag : (IFunctor)rightTag;
				reduction.Tag = new BitwiseFunction(functor, flag ? (IGenerator)leftTag : (IGenerator)rightTag, BitwiseOperator.Or);
			}
			return null;
		}

		/// <summary>
		/// Cria a regra para AND + Expression.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_ANDEXPR(Reduction reduction)
		{
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_ANDEXPR".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para AND + Expression + AND
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_ANDEXPR_AND(Reduction reduction)
		{
			Predicate tag = (Predicate)((Reduction)reduction.GetToken(0).Data).Tag;
			Predicate rhsPred = (Predicate)((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateLogicalAndPredicate(tag, rhsPred);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_ANDEXPR_AND".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para um atributo.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_ATTRIB(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_ATRRIB".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra do valor médio de dois parametros
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_AVERAGEFUNCTION_AVGLPARAN_RPARAN(Reduction reduction)
		{
			Reduction data = (Reduction)reduction.GetToken(1).Data;
			string tag = ((Reduction)data.GetToken(0).Data).Tag as string;
			string attributeName = ((Reduction)data.GetToken(2).Data).Tag as string;
			Predicate predicate = new IsOfTypePredicate(tag);
			AggregateFunctionPredicate predicate2 = ExpressionBuilder.CreateAverageFunctionPredicate(attributeName) as AggregateFunctionPredicate;
			predicate2.ChildPredicate = predicate;
			reduction.Tag = predicate2;
			return null;
		}

		/// <summary>
		/// Cria a regra para comparação expressão igual.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_EQ(Reduction reduction)
		{
			return this.CreateRULE_COMPAREEXPR_EQEQ(reduction);
		}

		/// <summary>
		/// Cria a regra para comparação igual-igual
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_EQEQ(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object v = ((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateEqualsPredicate(tag, v);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_EQEQ".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a comparação !=
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_EXCLAMEQ(Reduction reduction)
		{
			return this.CreateRULE_COMPAREEXPR_LTGT(reduction);
		}

		/// <summary>
		/// Cria a regra para a comparação "Maior que".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_GT(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object v = ((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateGreaterPredicate(tag, v);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_GT".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a comparação "Maior igual".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_GTEQ(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object v = ((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateGreaterEqualsPredicate(tag, v);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_GTEQ".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a comparação IN.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_IN(Reduction reduction)
		{
			object leftTag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object rightTag = ((Reduction)reduction.GetToken(2).Data).Tag;
			if(rightTag is IsInListPredicate)
			{
				var predicate = rightTag as IsInListPredicate;
				predicate.Functor = leftTag as IFunctor;
				reduction.Tag = predicate;
			}
			else
			{
				reduction.Tag = ExpressionBuilder.CreateEqualsPredicate(leftTag, rightTag);
			}
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_IN".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a comparação "IS NOT NULL".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_IS_NOT_NULL(Reduction reduction)
		{
			Predicate predicate = new IsNullPredicate(((Reduction)reduction.GetToken(0).Data).Tag as IFunctor);
			predicate.Invert();
			reduction.Tag = predicate;
			if(IsInfoEnabled)
				this.Logger.Info("RULE_COMPAREEXPR_IS_NOT_NULL".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a comparação "IS NULL".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_IS_NULL(Reduction reduction)
		{
			Predicate predicate = new IsNullPredicate(((Reduction)reduction.GetToken(0).Data).Tag as IFunctor);
			reduction.Tag = predicate;
			if(IsInfoEnabled)
				this.Logger.Info("RULE_COMPAREEXPR_IS_NULL".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a comparação "LIKE" com uma string.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_LIKE_STRINGLITERAL(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			RuntimeValue pattern = new RuntimeValue();
			Predicate predicate = ExpressionBuilder.CreateLikePatternPredicate(tag, pattern);
			reduction.Tag = predicate;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_LIKE_STRINGLITERAL".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra de comparação expressão.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_LPARAN_RPARAN(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_LPARAN_RPARAN".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(1).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra de comparação "Menor".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_LT(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object v = ((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateLesserPredicate(tag, v);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_LT".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra de comparação "Menor que".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_LTEQ(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object v = ((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateLesserEqualsPredicate(tag, v);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_LTEQ".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra de comparação diferente.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_LTGT(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			object v = ((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateNotEqualsPredicate(tag, v);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_LTGT".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra de comparação "NOT IN".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_NOT_IN(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			IsInListPredicate predicate = (IsInListPredicate)((Reduction)reduction.GetToken(3).Data).Tag;
			predicate.Invert();
			predicate.Functor = tag as IFunctor;
			reduction.Tag = predicate;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_NOT_IN".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra de comparação "NOT LIKE" para string.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COMPAREEXPR_NOT_LIKE_STRINGLITERAL(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			var pattern = new RuntimeValue();
			Predicate predicate = ExpressionBuilder.CreateLikePatternPredicate(tag, pattern);
			predicate.Invert();
			reduction.Tag = predicate;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_COMPAREEXPR_LIKE_STRINGLITERAL".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para "COUNT"
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_COUNTFUNCTION_COUNTLPARAN_TIMES_RPARAN(Reduction reduction)
		{
			string tag = ((Reduction)reduction.GetToken(1).Data).Tag as string;
			Predicate predicate = new IsOfTypePredicate(tag);
			AggregateFunctionPredicate predicate2 = ExpressionBuilder.CreateCountFunctionPredicate() as AggregateFunctionPredicate;
			predicate2.ChildPredicate = predicate;
			reduction.Tag = predicate2;
			return null;
		}

		/// <summary>
		/// Cria a regra para "NOT NOW".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_DATE_DATETIME_DOT_NOW(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_DATE_DATETIME_DOT_NOW".GetFormatter());
			reduction.Tag = new DateTimeConstantValue();
			return null;
		}

		/// <summary>
		/// Cria a regra para "DATE".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_DATE_DATETIME_LPARAN_RPARAN(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_DATE_DATETIME_LPARAN_RPARAN".GetFormatter());
			reduction.Tag = new DateTimeConstantValue(reduction.GetToken(2).Data.ToString());
			return null;
		}

		/// <summary>
		/// Cria a regra para comparação de datetime com string.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_DATE_DATETIME_LPARAN_STRINGLITERAL_RPARAN(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("RULE_DATE_DATETIME_LPARAN_STRINGLITERAL_RPARAN".GetFormatter());
			string lexeme = reduction.GetToken(2).Data.ToString().Trim(new char[] {
				'\''
			});
			reduction.Tag = new DateTimeConstantValue(lexeme);
			return null;
		}

		/// <summary>
		/// Cria a regra para comparação com lista da datas.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_DATELIST(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_DATELIST".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para comparação com lista de datas separadas por virgula.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_DATELIST_COMMA(Reduction reduction)
		{
			return this.CreateInclusionList(reduction);
		}

		/// <summary>
		/// Cria a regra para uma expressão.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_EXPRESSION(Reduction reduction)
		{
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_EXPRESSION".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para um identificador.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_IDENTIFIER_IDENTIFIER(Reduction reduction)
		{
			reduction.Tag = reduction.GetToken(0).Data.ToString();
			if(IsInfoEnabled)
				this.Logger.Info(("RULE_IDENTIFIER_IDENTIFIER -> " + reduction.Tag).GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para uma palavra chave.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_IDENTIFIER_KEYWORD(Reduction reduction)
		{
			reduction.Tag = reduction.GetToken(0).Data;
			if(IsInfoEnabled)
				this.Logger.Info(("RULE_IDENTIFIER_KEYWORD -> " + reduction.Tag).GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para "IN LIST".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_INLIST_LPARAN_RPARAN(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_INLIST_LPARAN_RPARAN".GetFormatter());
			object tag = ((Reduction)reduction.GetToken(1).Data).Tag;
			if((tag is ConstantValue) || (tag is RuntimeValue))
			{
				IsInListPredicate predicate = new IsInListPredicate();
				predicate.Append(tag);
				reduction.Tag = predicate;
			}
			else
				reduction.Tag = ((Reduction)reduction.GetToken(1).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para uma lista de tipos.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_LISTTYPE(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_LISTTYPE".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para uma lista de tipos.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_LISTTYPE2(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_LISTTYPE2".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para uma lista de tipos.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_LISTTYPE3(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_LISTTYPE3".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para a função "MAX".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_MAXFUNCTION_MAXLPARAN_RPARAN(Reduction reduction)
		{
			Reduction data = (Reduction)reduction.GetToken(1).Data;
			string tag = ((Reduction)data.GetToken(0).Data).Tag as string;
			string attributeName = ((Reduction)data.GetToken(2).Data).Tag as string;
			Predicate predicate = new IsOfTypePredicate(tag);
			AggregateFunctionPredicate predicate2 = ExpressionBuilder.CreateMaxFunctionPredicate(attributeName) as AggregateFunctionPredicate;
			predicate2.ChildPredicate = predicate;
			reduction.Tag = predicate2;
			return null;
		}

		/// <summary>
		/// Cria a regra para a função "MIN".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_MINFUNCTION_MINLPARAN_RPARAN(Reduction reduction)
		{
			Reduction data = (Reduction)reduction.GetToken(1).Data;
			string tag = ((Reduction)data.GetToken(0).Data).Tag as string;
			string attributeName = ((Reduction)data.GetToken(2).Data).Tag as string;
			Predicate predicate = new IsOfTypePredicate(tag);
			AggregateFunctionPredicate predicate2 = ExpressionBuilder.CreateMinFunctionPredicate(attributeName) as AggregateFunctionPredicate;
			predicate2.ChildPredicate = predicate;
			reduction.Tag = predicate2;
			return null;
		}

		/// <summary>
		/// Cria a regra para um valor constante inteiro.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_NUMLITERAL_INTEGERLITERAL(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_NUMLITERAL_INTEGERLITERAL".GetFormatter());
			reduction.Tag = new IntegerConstantValue(reduction.GetToken(0).Data.ToString());
			return null;
		}

		/// <summary>
		/// Cria a regra para um valor real.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_NUMLITERAL_REALLITERAL(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_NUMLITERAL_REALLITERAL".GetFormatter());
			reduction.Tag = new DoubleConstantValue(reduction.GetToken(0).Data.ToString());
			return null;
		}

		/// <summary>
		/// Cria a regra para uma lista de números
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_NUMLITERALLIST(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_NUMLITERALLIST".GetFormatter());
			IsInListPredicate predicate = new IsInListPredicate();
			predicate.Append(((Reduction)reduction.GetToken(0).Data).Tag);
			reduction.Tag = predicate;
			return null;
		}

		/// <summary>
		/// Cria a regra para uma lista de valores numéricos separados por virgula.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_NUMLITERALLIST_COMMA(Reduction reduction)
		{
			return this.CreateInclusionList(reduction);
		}

		/// <summary>
		/// Cria a regra para um atributo de objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTATTRIBUTE_IDENTIFIER(Reduction reduction)
		{
			string str = reduction.GetToken(0).Data.ToString();
			reduction.Tag = str;
			return null;
		}

		/// <summary>
		/// Cria a regra para o tipo do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTTYPE_DOLLARTEXTDOLLAR(Reduction reduction)
		{
			reduction.Tag = "System.String";
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_OBJECTTYPE_DOLLARTEXTDOLLAR".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para o identificador do tipo do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTTYPE_IDENTIFIER(Reduction reduction)
		{
			reduction.Tag = reduction.GetToken(0).Data;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_TYPEIDENTIFIER_IDENTIFIER".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para o "." do identificador do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTTYPE_IDENTIFIER_DOT(Reduction reduction)
		{
			string str = ((Reduction)reduction.GetToken(0).Data).Tag.ToString();
			string str2 = reduction.GetToken(2).Data.ToString();
			reduction.Tag = str + "." + str2;
			return null;
		}

		/// <summary>
		/// Cria a regra para o "*".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTTYPE_TIMES(Reduction reduction)
		{
			reduction.Tag = "*";
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_OBJECTTYPE_TIMES".GetFormatter());
			return null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTTYPE2(Reduction reduction)
		{
			return null;
		}

		/// <summary>
		/// Cria a regra para palavra chave do valor do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTVALUE_KEYWORD(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_OBJECTVALUE_KEYWORD".GetFormatter());
			reduction.Tag = new IdentityFunction();
			return null;
		}

		/// <summary>
		/// Cria a regra para o "." da palavra chave do valor do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTVALUE_KEYWORD_DOT(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(2).Data).Tag;
			if(tag is IFunctor)
				reduction.Tag = tag;
			else
				reduction.Tag = new MemberFunction(tag.ToString());
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_IDENTIFIER_KEYWORD".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para o identificador "." da palavra chave do valor do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTVALUE_KEYWORD_DOT_IDENTIFIER(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("RULE_OBJECTVALUE_KEYWORD_DOT_IDENTIFIER".GetFormatter());
			string memname = reduction.GetToken(2).Data.ToString();
			reduction.Tag = new MemberFunction(memname);
			return null;
		}

		/// <summary>
		/// Cria a regra para o tag "." da palavra chave do valor do objeto.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OBJECTVALUE_KEYWORD_DOT_TAG(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("RULE_OBJECTVALUE_KEYWORD_DOT_TAG".GetFormatter());
			string memname = "$Tag$";
			reduction.Tag = new MemberFunction(memname);
			return null;
		}

		/// <summary>
		/// Cria a regra para a "OR".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OREXPR(Reduction reduction)
		{
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_OREXPR".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a expressão "OR".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_OREXPR_OR(Reduction reduction)
		{
			Predicate tag = (Predicate)((Reduction)reduction.GetToken(0).Data).Tag;
			Predicate rhsPred = (Predicate)((Reduction)reduction.GetToken(2).Data).Tag;
			reduction.Tag = ExpressionBuilder.CreateLogicalOrPredicate(tag, rhsPred);
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_OREXPR_OR".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra de uma propriedade.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_PROPERTY(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_PROPERTY".GetFormatter());
			reduction.Tag = reduction.GetToken(0).Data;
			return null;
		}

		/// <summary>
		/// Cria a regra para "." da propriedade.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_PROPERTY_DOT(Reduction reduction)
		{
			IFunctor functor = new MemberFunction(((Reduction)reduction.GetToken(2).Data).Tag.ToString());
			IFunctor func = new MemberFunction(((Reduction)reduction.GetToken(0).Data).Tag.ToString());
			reduction.Tag = new CompositeFunction(func, functor);
			if(IsInfoEnabled)
				this.Logger.Info(("RULE_PROPERTY_DOT -> " + reduction.Tag).GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a seleção da consulta.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_QUERY_SELECT(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(1).Data).Tag;
			Predicate predicate = tag as Predicate;
			if(predicate == null)
				reduction.Tag = new IsOfTypePredicate(tag.ToString());
			else
				reduction.Tag = predicate;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_QUERY_SELECT".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a condicional da consulta.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_QUERY_SELECT_WHERE(Reduction reduction)
		{
			object tag = ((Reduction)reduction.GetToken(1).Data).Tag;
			Predicate lhsPred = null;
			Predicate rhsPred = (Predicate)((Reduction)reduction.GetToken(3).Data).Tag;
			Predicate predicate3 = tag as Predicate;
			Predicate predicate4 = null;
			if(predicate3 == null)
			{
				lhsPred = new IsOfTypePredicate(tag.ToString());
				predicate4 = ExpressionBuilder.CreateLogicalAndPredicate(lhsPred, rhsPred);
			}
			else if(predicate3 is AggregateFunctionPredicate)
			{
				AggregateFunctionPredicate predicate5 = predicate3 as AggregateFunctionPredicate;
				lhsPred = predicate5.ChildPredicate;
				predicate5.ChildPredicate = ExpressionBuilder.CreateLogicalAndPredicate(lhsPred, rhsPred);
				predicate4 = predicate5;
			}
			else
			{
				lhsPred = predicate3;
				predicate4 = ExpressionBuilder.CreateLogicalAndPredicate(lhsPred, rhsPred);
			}
			reduction.Tag = predicate4;
			return null;
		}

		/// <summary>
		/// Cria a regra para a string NULL.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_STRLITERAL_NULL(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_STRLITERAL_NULL".GetFormatter());
			reduction.Tag = new NullValue();
			return null;
		}

		/// <summary>
		/// Cria a regra para "?"
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_STRLITERAL_PARAMETER(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_STRLITERAL_PARAMETER".GetFormatter());
			string name = reduction.GetToken(0).Data as string;
			reduction.Tag = new Parameter(name);
			return null;
		}

		/// <summary>
		/// Cria a regra para string.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_STRLITERAL_STRINGLITERAL(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_STRLITERAL_STRINGLITERAL".GetFormatter());
			reduction.Tag = new StringConstantValue(reduction.GetToken(0).Data.ToString());
			return null;
		}

		/// <summary>
		/// Cria a regra para uma lista de string.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_STRLITERALLIST(Reduction reduction)
		{
			if(IsInfoEnabled)
			{
				this.Logger.Info("CreateRULE_STRLITERALLIST".GetFormatter());
			}
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para ",".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_STRLITERALLIST_COMMA(Reduction reduction)
		{
			return this.CreateInclusionList(reduction);
		}

		/// <summary>
		/// Cria a regra para a função SUM.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_SUMFUNCTION_SUMLPARAN_RPARAN(Reduction reduction)
		{
			Reduction data = (Reduction)reduction.GetToken(1).Data;
			string tag = ((Reduction)data.GetToken(0).Data).Tag as string;
			string attributeName = ((Reduction)data.GetToken(2).Data).Tag as string;
			Predicate predicate = new IsOfTypePredicate(tag);
			AggregateFunctionPredicate predicate2 = ExpressionBuilder.CreateSumFunctionPredicate(attributeName) as AggregateFunctionPredicate;
			predicate2.ChildPredicate = predicate;
			reduction.Tag = predicate2;
			return null;
		}

		/// <summary>
		/// Cria a regra para uma expressão unária.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_UNARYEXPR(Reduction reduction)
		{
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_UNARYEXPR".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para a negação de uma expressão unária.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_UNARYEXPR_NOT(Reduction reduction)
		{
			Predicate tag = (Predicate)((Reduction)reduction.GetToken(1).Data).Tag;
			tag.Invert();
			reduction.Tag = tag;
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_UNARYEXPR_NOT".GetFormatter());
			return null;
		}

		/// <summary>
		/// Cria a regra para um valor.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_VALUE".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para um valor falso.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE_FALSE(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_VALUE_FALSE".GetFormatter());
			reduction.Tag = new FalseValue();
			return null;
		}

		/// <summary>
		/// Cria a regra para a expressão "-".
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE_MINUS(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("RULE_VALUE_MINUS".GetFormatter());
			if(((Reduction)reduction.GetToken(0).Data).Tag is IntegerConstantValue)
				reduction.Tag = new IntegerConstantValue("-" + reduction.GetToken(1).Data.ToString());
			else
				reduction.Tag = new DoubleConstantValue("-" + reduction.GetToken(1).Data.ToString());
			return null;
		}

		/// <summary>
		/// Cria a regra para o valor verdadeiro.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE_TRUE(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_VALUE_TRUE".GetFormatter());
			reduction.Tag = new TrueValue();
			return null;
		}

		/// <summary>
		/// Cria a regra para o valor.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE2(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_VALUE2".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para o valor.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE3(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_VALUE3".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Cria a regra para o valor.
		/// </summary>
		/// <param name="reduction"></param>
		/// <returns></returns>
		protected virtual Reduction CreateRULE_VALUE4(Reduction reduction)
		{
			if(IsInfoEnabled)
				this.Logger.Info("CreateRULE_VALUE4".GetFormatter());
			reduction.Tag = ((Reduction)reduction.GetToken(0).Data).Tag;
			return null;
		}

		/// <summary>
		/// Executa a parse para o texto informado.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="generateContext"></param>
		/// <returns></returns>
		public ParseMessage Parse(TextReader source, bool generateContext)
		{
			ParseMessage message;
			bool flag = false;
			base.OpenStream(source);
			base.TrimReductions = true;
			do
			{
				message = base.Parse();
				switch(message)
				{
				case ParseMessage.Reduction:
					if(generateContext)
						base.CurrentReduction = this.CreateNewObject(base.CurrentReduction);
					break;
				case ParseMessage.Accept:
					flag = true;
					break;
				case ParseMessage.LexicalError:
					flag = true;
					break;
				case ParseMessage.SyntaxError:
					foreach (Token token in base.GetTokens())
					{
						if(IsInfoEnabled)
							this.Logger.Info(token.Name.GetFormatter());
					}
					flag = true;
					break;
				case ParseMessage.CommentError:
					flag = true;
					break;
				case ParseMessage.InternalError:
					flag = true;
					break;
				}
			}
			while (!flag);
			base.CloseFile();
			return message;
		}
	}
}
