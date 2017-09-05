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
using System.Linq.Expressions;
using System.Text;

namespace Colosoft.Reflection
{
	/// <summary>
	/// Representa um esquema de conversão de tipo.
	/// </summary>
	public class TypeConverterScheme<T1, T2>
	{
		private List<ISchemeItem> _items = new List<ISchemeItem>();

		/// <summary>
		/// Construtor privado.
		/// </summary>
		private TypeConverterScheme()
		{
		}

		/// <summary>
		/// Cria o configurar o esquema.
		/// </summary>
		/// <returns></returns>
		public static FluentTypeConverterSchema Create()
		{
			return new FluentTypeConverterSchema(new TypeConverterScheme<T1, T2>());
		}

		/// <summary>
		/// Aplica os valores da origem para o destino.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void Apply(T1 source, T2 destination)
		{
			foreach (var item in _items)
				item.Apply(source, destination);
		}

		/// <summary>
		/// Aplica os valores da origem para o destino.
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public void Apply(T2 source, T1 destination)
		{
			foreach (var item in _items)
				item.Apply(source, destination);
		}

		/// <summary>
		/// Assinatura de um item do esquema.
		/// </summary>
		interface ISchemeItem
		{
			/// <summary>
			/// Aplica o item da origem na classe de destinho.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			void Apply(T1 source, T2 destination);

			/// <summary>
			/// Aplica o item do destinho na classe de origem.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			void Apply(T2 source, T1 destination);
		}

		/// <summary>
		/// Implementação de um item do esquema.
		/// </summary>
		/// <typeparam name="TValue"></typeparam>
		class SchemeItem<TValue> : ISchemeItem
		{
			private Expression<Func<T1, TValue>> _t1Expression;

			private Expression<Func<T2, TValue>> _t2Expression;

			private Lazy<Func<T1, TValue>> _t1Getter;

			private Lazy<Func<T2, TValue>> _t2Getter;

			private Lazy<System.Reflection.MemberInfo> _t1Member;

			private Lazy<System.Reflection.MemberInfo> _t2Member;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="t1Expression"></param>
			/// <param name="t2Expression"></param>
			public SchemeItem(Expression<Func<T1, TValue>> t1Expression, Expression<Func<T2, TValue>> t2Expression)
			{
				_t1Expression = t1Expression;
				_t2Expression = t2Expression;
				_t1Getter = new Lazy<Func<T1, TValue>>(() => _t1Expression.Compile());
				_t2Getter = new Lazy<Func<T2, TValue>>(() => _t2Expression.Compile());
				_t1Member = new Lazy<System.Reflection.MemberInfo>(() => _t1Expression.GetMember());
				_t2Member = new Lazy<System.Reflection.MemberInfo>(() => _t2Expression.GetMember());
			}

			/// <summary>
			/// Define o valor para a instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="value"></param>
			private void SetValue(T1 instance, TValue value)
			{
				SetValue(instance, value, _t1Member.Value);
			}

			/// <summary>
			/// Define o valor para a instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="value"></param>
			private void SetValue(T2 instance, TValue value)
			{
				SetValue(instance, value, _t2Member.Value);
			}

			/// <summary>
			/// Define o valor para a instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="value"></param>
			/// <param name="member"></param>
			private static void SetValue<T>(T instance, TValue value, System.Reflection.MemberInfo member)
			{
				try
				{
					if(instance == null || !member.ReflectedType.IsAssignableFrom(instance.GetType()))
						return;
					if(member is System.Reflection.PropertyInfo && ((System.Reflection.PropertyInfo)member).CanWrite)
						((System.Reflection.PropertyInfo)member).SetValue(instance, value, null);
					else if(member is System.Reflection.FieldInfo)
						((System.Reflection.FieldInfo)member).SetValue(instance, value);
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}

			/// <summary>
			/// Aplica o item da origem na classe de destinho.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			public void Apply(T1 source, T2 destination)
			{
				SetValue(destination, _t1Getter.Value(source));
			}

			/// <summary>
			/// Aplica o item do destinho na classe de origem.
			/// </summary>
			/// <param name="destination"></param>
			/// <param name="source"></param>
			public void Apply(T2 source, T1 destination)
			{
				SetValue(destination, _t2Getter.Value(source));
			}
		}

		/// <summary>
		/// Implementação de um item do esquema com conversão.
		/// </summary>
		/// <typeparam name="TLeft"></typeparam>
		/// <typeparam name="TRight"></typeparam>
		class CastSchemeItem<TLeft, TRight> : ISchemeItem
		{
			private Expression<Func<T1, TLeft>> _leftExpression;

			private Expression<Func<T2, TRight>> _rightExpression;

			private Lazy<Func<T1, TLeft>> _leftGetter;

			private Lazy<Func<T2, TRight>> _rightGetter;

			private Func<TLeft, TRight> _leftRightCast;

			private Func<TRight, TLeft> _rightLeftCast;

			private Lazy<System.Reflection.MemberInfo> _t1Member;

			private Lazy<System.Reflection.MemberInfo> _t2Member;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="leftExpression"></param>
			/// <param name="rightExpression"></param>
			/// <param name="leftRightCast"></param>
			/// <param name="rightLeftCast"></param>
			public CastSchemeItem(Expression<Func<T1, TLeft>> leftExpression, Expression<Func<T2, TRight>> rightExpression, Func<TLeft, TRight> leftRightCast, Func<TRight, TLeft> rightLeftCast)
			{
				_leftExpression = leftExpression;
				_rightExpression = rightExpression;
				_leftGetter = new Lazy<Func<T1, TLeft>>(() => _leftExpression.Compile());
				_rightGetter = new Lazy<Func<T2, TRight>>(() => _rightExpression.Compile());
				_t1Member = new Lazy<System.Reflection.MemberInfo>(() => _leftExpression.GetMember());
				_t2Member = new Lazy<System.Reflection.MemberInfo>(() => _rightExpression.GetMember());
				_leftRightCast = leftRightCast;
				_rightLeftCast = rightLeftCast;
			}

			/// <summary>
			/// Define o valor para a instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="value"></param>
			private void SetValue(T1 instance, TRight value)
			{
				var value2 = _rightLeftCast(value);
				SetValue(instance, value2, _t1Member.Value);
			}

			/// <summary>
			/// Define o valor para a instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="value"></param>
			private void SetValue(T2 instance, TLeft value)
			{
				var value2 = _leftRightCast(value);
				SetValue(instance, value2, _t2Member.Value);
			}

			/// <summary>
			/// Define o valor para a instancia.
			/// </summary>
			/// <param name="instance"></param>
			/// <param name="value"></param>
			/// <param name="member"></param>
			private static void SetValue<T, TValue>(T instance, TValue value, System.Reflection.MemberInfo member)
			{
				try
				{
					if(instance == null || !member.ReflectedType.IsAssignableFrom(instance.GetType()))
						return;
					if(member is System.Reflection.PropertyInfo && ((System.Reflection.PropertyInfo)member).CanWrite)
						((System.Reflection.PropertyInfo)member).SetValue(instance, value, null);
					else if(member is System.Reflection.FieldInfo)
						((System.Reflection.FieldInfo)member).SetValue(instance, value);
				}
				catch(System.Reflection.TargetInvocationException ex)
				{
					throw ex.InnerException;
				}
			}

			/// <summary>
			/// Aplica o item da origem na classe de destinho.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			public void Apply(T1 source, T2 destination)
			{
				SetValue(destination, _leftGetter.Value(source));
			}

			/// <summary>
			/// Aplica o item do destinho na classe de origem.
			/// </summary>
			/// <param name="destination"></param>
			/// <param name="source"></param>
			public void Apply(T2 source, T1 destination)
			{
				SetValue(destination, _rightGetter.Value(source));
			}
		}

		class SchemaItem : ISchemeItem
		{
			private Action<T1, T2> _t1ToT2;

			private Action<T2, T1> _t2ToT1;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="t1ToT2"></param>
			/// <param name="t2ToT1"></param>
			public SchemaItem(Action<T1, T2> t1ToT2, Action<T2, T1> t2ToT1)
			{
				_t1ToT2 = t1ToT2;
				_t2ToT1 = t2ToT1;
			}

			/// <summary>
			/// Aplica os valores da origem para o destino.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			public void Apply(T1 source, T2 destination)
			{
				_t1ToT2(source, destination);
			}

			/// <summary>
			/// Aplica os valores da origem para o destino.
			/// </summary>
			/// <param name="source"></param>
			/// <param name="destination"></param>
			public void Apply(T2 source, T1 destination)
			{
				_t2ToT1(source, destination);
			}
		}

		/// <summary>
		/// Classe usada para configurar o esquema.
		/// </summary>
		public class FluentTypeConverterSchema
		{
			private TypeConverterScheme<T1, T2> _schema;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="scheme"></param>
			internal FluentTypeConverterSchema(TypeConverterScheme<T1, T2> scheme)
			{
				_schema = scheme;
			}

			/// <summary>
			/// Configura uma propriedade.
			/// </summary>
			/// <typeparam name="TValue">Tipo da propriedade.</typeparam>
			/// <param name="sourceProperty">Propriedade da origem.</param>
			/// <param name="destinationProperty">Propriedade do destino.</param>
			/// <returns></returns>
			public FluentTypeConverterSchema Property<TValue>(Expression<Func<T1, TValue>> sourceProperty, Expression<Func<T2, TValue>> destinationProperty)
			{
				sourceProperty.Require("sourceProperty").NotNull();
				destinationProperty.Require("destinationProperty").NotNull();
				_schema._items.Add(new SchemeItem<TValue>(sourceProperty, destinationProperty));
				return this;
			}

			/// <summary>
			/// Configura uma propriedade.
			/// </summary>
			/// <typeparam name="TLeft">Tipo da propriedade da esquerda.</typeparam>
			/// <typeparam name="TRight">Tipo da propriedade da direita.</typeparam>
			/// <param name="sourceProperty">Propriedade de origem.</param>
			/// <param name="destinationProperty">Propriedade de destino.</param>
			/// <param name="leftRightCast">Cast da esquerda para a direita.</param>
			/// <param name="rightLeftCast">Cast da direita para esquerda.</param>
			/// <returns></returns>
			public FluentTypeConverterSchema Property<TLeft, TRight>(Expression<Func<T1, TLeft>> sourceProperty, Expression<Func<T2, TRight>> destinationProperty, Func<TLeft, TRight> leftRightCast, Func<TRight, TLeft> rightLeftCast)
			{
				sourceProperty.Require("sourceProperty").NotNull();
				destinationProperty.Require("destinationProperty").NotNull();
				_schema._items.Add(new CastSchemeItem<TLeft, TRight>(sourceProperty, destinationProperty, leftRightCast, rightLeftCast));
				return this;
			}

			/// <summary>
			/// Configura a aplicação da conversão de valores de T1 para T2 e vice versa.
			/// </summary>
			/// <param name="t1ToT2"></param>
			/// <param name="t2ToT1"></param>
			/// <returns></returns>
			public FluentTypeConverterSchema Apply(Action<T1, T2> t1ToT2, Action<T2, T1> t2ToT1)
			{
				t1ToT2.Require("t1ToT2").NotNull();
				t2ToT1.Require("t2ToT1").NotNull();
				_schema._items.Add(new SchemaItem(t1ToT2, t2ToT1));
				return this;
			}

			/// <summary>
			/// Finaliza a configuração.
			/// </summary>
			/// <returns></returns>
			public TypeConverterScheme<T1, T2> Finalize()
			{
				return _schema;
			}
		}
	}
}
