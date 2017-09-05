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

namespace Colosoft.Caching.Queries.Filters
{
	/// <summary>
	/// Representa um construtor de expressão.
	/// </summary>
	internal class ExpressionBuilder
	{
		/// <summary>
		/// Representa o predicado para o valor falso.
		/// </summary>
		public static readonly Predicate FALSE_PREDICATE = new AlwaysFalsePredicate();

		/// <summary>
		/// Representa o predicado para o valor verdadeiro.
		/// </summary>
		public static readonly Predicate TRUE_PREDICATE = new AlwaysTruePredicate();

		/// <summary>
		/// Cria um predicado para a função do valor médio.
		/// </summary>
		/// <param name="attributeName"></param>
		/// <returns></returns>
		public static Predicate CreateAverageFunctionPredicate(string attributeName)
		{
			AggregateFunctionPredicate predicate = new AveragePredicate();
			predicate.AttributeName = attributeName;
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para a função COUNT.
		/// </summary>
		/// <returns></returns>
		public static Predicate CreateCountFunctionPredicate()
		{
			return new CountPredicate();
		}

		/// <summary>
		/// Cria um predicado para comparação igual.
		/// </summary>
		/// <param name="o">Objeto em questão.</param>
		/// <param name="v">Valor que será comparado.</param>
		/// <returns></returns>
		public static Predicate CreateEqualsPredicate(object o, object v)
		{
			bool flag = o is IGenerator;
			bool flag2 = v is IGenerator;
			if(!flag && !flag2)
				return new FunctorEqualsFunctorPredicate((IFunctor)o, (IFunctor)v);
			if(flag && flag2)
			{
				object obj2 = ((IGenerator)o).Evaluate();
				object obj3 = ((IGenerator)v).Evaluate();
				if(!obj2.Equals(obj3))
					return FALSE_PREDICATE;
				return TRUE_PREDICATE;
			}
			IFunctor functor = flag ? ((IFunctor)v) : ((IFunctor)o);
			return new FunctorEqualsGeneratorPredicate(functor, flag ? ((IGenerator)o) : ((IGenerator)v));
		}

		/// <summary>
		/// Cria um predicado para a comparação maior igual.
		/// </summary>
		/// <param name="o">Objeto em questão.</param>
		/// <param name="v">Valor que será comparado.</param>
		/// <returns></returns>
		public static Predicate CreateGreaterEqualsPredicate(object o, object v)
		{
			Predicate predicate = CreateLesserPredicate(o, v);
			if(predicate.Equals(TRUE_PREDICATE))
				return FALSE_PREDICATE;
			if(predicate.Equals(FALSE_PREDICATE))
				return TRUE_PREDICATE;
			predicate.Invert();
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para a comparação maior.
		/// </summary>
		/// <param name="o">Objeto em questão.</param>
		/// <param name="v">Valor que será comparado.</param>
		/// <returns></returns>
		public static Predicate CreateGreaterPredicate(object o, object v)
		{
			bool flag = o is IGenerator;
			bool flag2 = v is IGenerator;
			if(!flag && !flag2)
				return new FunctorGreaterFunctorPredicate((IFunctor)o, (IFunctor)v);
			if(flag && flag2)
			{
				object a = ((IGenerator)o).Evaluate();
				object b = ((IGenerator)v).Evaluate();
				if(System.Collections.Comparer.Default.Compare(a, b) <= 0)
					return FALSE_PREDICATE;
				return TRUE_PREDICATE;
			}
			IFunctor lhs = flag ? ((IFunctor)v) : ((IFunctor)o);
			return new FunctorGreaterGeneratorPredicate(lhs, flag ? ((IGenerator)o) : ((IGenerator)v));
		}

		/// <summary>
		/// Cria um predicado para a comparação menor igual.
		/// </summary>
		/// <param name="o">Objeto em questão.</param>
		/// <param name="v">Valor que será comparado.</param>
		/// <returns></returns>
		public static Predicate CreateLesserEqualsPredicate(object o, object v)
		{
			Predicate predicate = CreateGreaterPredicate(o, v);
			if(predicate.Equals(TRUE_PREDICATE))
				return FALSE_PREDICATE;
			if(predicate.Equals(FALSE_PREDICATE))
				return TRUE_PREDICATE;
			predicate.Invert();
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para comparação de menor.
		/// </summary>
		/// <param name="o">Objeto em questão.</param>
		/// <param name="v">Valor que será comparado.</param>
		/// <returns></returns>
		public static Predicate CreateLesserPredicate(object o, object v)
		{
			bool flag = o is IGenerator;
			bool flag2 = v is IGenerator;
			if(!flag && !flag2)
				return new FunctorLesserFunctorPredicate((IFunctor)o, (IFunctor)v);
			if(flag && flag2)
			{
				object a = ((IGenerator)o).Evaluate();
				object b = ((IGenerator)v).Evaluate();
				if(System.Collections.Comparer.Default.Compare(a, b) >= 0)
					return FALSE_PREDICATE;
				return TRUE_PREDICATE;
			}
			IFunctor lhs = flag ? ((IFunctor)v) : ((IFunctor)o);
			return new FunctorLesserGeneratorPredicate(lhs, flag ? ((IGenerator)o) : ((IGenerator)v));
		}

		/// <summary>
		/// Cria um predicado para uma estrutura de comparação baseado no padrão informado.
		/// </summary>
		/// <param name="o">Instancia que será comparada.</param>
		/// <param name="pattern">Padrão que será usado na comparação.</param>
		/// <returns></returns>
		public static Predicate CreateLikePatternPredicate(object o, object pattern)
		{
			return new FunctorLikePatternPredicate(o as IFunctor, pattern as IGenerator);
		}

		/// <summary>
		/// Cria um predicado para a lógica AND.
		/// </summary>
		/// <param name="lhsPred"></param>
		/// <param name="rhsPred"></param>
		/// <returns></returns>
		public static Predicate CreateLogicalAndPredicate(Predicate lhsPred, Predicate rhsPred)
		{
			if(lhsPred.Equals(FALSE_PREDICATE) || rhsPred.Equals(FALSE_PREDICATE))
				return FALSE_PREDICATE;
			if(lhsPred.Equals(TRUE_PREDICATE))
				return rhsPred;
			if(rhsPred.Equals(TRUE_PREDICATE))
				return lhsPred;
			LogicalAndPredicate predicate = null;
			if(lhsPred is LogicalAndPredicate)
				predicate = lhsPred as LogicalAndPredicate;
			if((predicate == null) || predicate.Inverse)
			{
				predicate = new LogicalAndPredicate();
				predicate.Children.Add(lhsPred);
			}
			predicate.Children.Add(rhsPred);
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para a lógica OR.
		/// </summary>
		/// <param name="lhsPred">Predicado da esquerda.</param>
		/// <param name="rhsPred">Predicado da direita.</param>
		/// <returns></returns>
		public static Predicate CreateLogicalOrPredicate(Predicate lhsPred, Predicate rhsPred)
		{
			if(lhsPred.Equals(TRUE_PREDICATE) || rhsPred.Equals(TRUE_PREDICATE))
				return TRUE_PREDICATE;
			if(lhsPred.Equals(FALSE_PREDICATE))
				return rhsPred;
			if(rhsPred.Equals(FALSE_PREDICATE))
				return lhsPred;
			LogicalAndPredicate predicate = null;
			if(lhsPred is LogicalAndPredicate)
				predicate = lhsPred as LogicalAndPredicate;
			if((predicate == null) || !predicate.Inverse)
			{
				predicate = new LogicalAndPredicate();
				predicate.Invert();
				predicate.Children.Add(lhsPred);
			}
			predicate.Children.Add(rhsPred);
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para a função de maior valor.
		/// </summary>
		/// <param name="attributeName">Nome do atributo que será atribuído.</param>
		/// <returns></returns>
		public static Predicate CreateMaxFunctionPredicate(string attributeName)
		{
			AggregateFunctionPredicate predicate = new MaxPredicate();
			predicate.AttributeName = attributeName;
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para a função de menor valor.
		/// </summary>
		/// <param name="attributeName">Nome do atributo que será atribuído.</param>
		/// <returns></returns>
		public static Predicate CreateMinFunctionPredicate(string attributeName)
		{
			AggregateFunctionPredicate predicate = new MinPredicate();
			predicate.AttributeName = attributeName;
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para uma comparação de diferente entre os dois objetos informados.
		/// </summary>
		/// <param name="o"></param>
		/// <param name="v"></param>
		/// <returns></returns>
		public static Predicate CreateNotEqualsPredicate(object o, object v)
		{
			Predicate predicate = CreateEqualsPredicate(o, v);
			if(predicate.Equals(TRUE_PREDICATE))
				return FALSE_PREDICATE;
			if(predicate.Equals(FALSE_PREDICATE))
				return TRUE_PREDICATE;
			predicate.Invert();
			return predicate;
		}

		/// <summary>
		/// Cria um predicado para a função de soma.
		/// </summary>
		/// <param name="attributeName">Nome do atributo do predicao.</param>
		/// <returns></returns>
		public static Predicate CreateSumFunctionPredicate(string attributeName)
		{
			AggregateFunctionPredicate predicate = new SumPredicate();
			predicate.AttributeName = attributeName;
			return predicate;
		}
	}
}
