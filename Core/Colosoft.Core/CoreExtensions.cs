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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft
{
	/// <summary>
	/// Extensões basicas do sistema.
	/// </summary>
	public static class CoreExtensions
	{
		private static MemberExpression RemoveUnary(Expression toUnwrap)
		{
			if(toUnwrap is UnaryExpression)
				return (MemberExpression)((UnaryExpression)toUnwrap).Operand;
			return toUnwrap as MemberExpression;
		}

		/// <summary>
		/// Recupera o fator de ajuste de data de acordo com especificador de frações de segundos da máscara.
		/// </summary>
		/// <param name="mask"></param>
		/// <returns></returns>
		private static long GetAdjustFactor(string mask)
		{
			var mul = 10000000L;
			foreach (var item in mask)
			{
				if(item == 'f')
				{
					mul /= 10L;
				}
			}
			return mul;
		}

		/// <summary>
		/// Mascara uma string.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		public static string MaskValue(this string str)
		{
			string[] SMaskValues =  {
				"",
				"",
				"0",
				"1",
				"12",
				"04",
				"015",
				"124",
				"0145",
				"1367",
				"02478",
				"0356A",
				"125689",
				"0123456789",
				"0123456789",
				"0123456789",
				"0123456789AB",
				"0123456789AB",
				"0123456789ABC",
				"0123456789ABCD",
				"0123456789ABCD"
			};
			string result = str;
			string indexes = SMaskValues[Math.Min(20, result.Length)];
			for(int index = 0; index < indexes.Length; index++)
			{
				int position;
				switch(indexes[index])
				{
				case 'A':
					position = 10;
					break;
				case 'B':
					position = 11;
					break;
				case 'C':
					position = 12;
					break;
				case 'D':
					position = 13;
					break;
				default:
					position = Convert.ToInt32(indexes[index].ToString());
					break;
				}
				result = result.Substring(0, position) + "*" + result.Substring(position + 1);
			}
			return result;
		}

		/// <summary>
		/// Compara uma string e suas máscaras.
		/// </summary>
		/// <param name="str1"></param>
		/// <param name="str2"></param>
		/// <param name="ret"></param>
		/// <returns></returns>
		public static int CompareMask(this string str1, string str2, int ret)
		{
			string p;
			int r;
			bool inv;
			int result = 0;
			string p1, p2;
			if(str1 == null)
			{
				return 1;
			}
			if(str2 == null)
			{
				return -1;
			}
			p1 = str1.ToUpper() + '\0';
			p2 = str2.ToUpper() + '\0';
			int l1 = str1.Length, index = 0;
			for(index = 0; index <= l1; index++)
			{
				result = ((int)(p1[0] - p2[0]));
				if(result == 0)
				{
					p1 = p1.Substring(1);
					p2 = p2.Substring(1);
					continue;
				}
				if((p1[0] != '\0') && (p2[0] != '\0') && (((p1[0] == '?') || (p2[0] == '?'))))
				{
					p1 = p1.Substring(1);
					p2 = p2.Substring(1);
					result = ret;
					continue;
				}
				if((p1[0] == '*') || (p2[0] == '*'))
				{
					inv = (p2[0] == '*');
					if(inv)
					{
						p = p1;
						p1 = p2;
						p2 = p;
					}
					p1 = p1.Substring(1);
					if(p1[0] == '\0')
					{
						result = ret;
					}
					else
					{
						r = 0;
						int Pos = p2.IndexOf(p1[0]);
						p = (Pos > -1) ? p2.Substring(Pos) : System.String.Empty + '\0';
						while (p != "\0")
						{
							r = CompareMask(p1, p, ret);
							if((r == 0) || (r == ret))
							{
								break;
							}
							Pos = p.IndexOf(p1[0], 1);
							p = (Pos > -1) ? p.Substring(Pos) : System.String.Empty + '\0';
						}
						if(p != "\0")
							result = r;
						else
						{
							if(ret != 0)
							{
								result = -ret;
							}
							else
							{
								if(r != 0)
								{
									result = r;
								}
								else
								{
									result = ((int)(p1[0] - p2[0]));
									if(result == 0)
									{
										result = -1;
									}
								}
								if(inv)
									result = -result;
							}
						}
					}
				}
				break;
			}
			return result;
		}

		/// <summary>
		/// Realiza a junção das mensagens informadas.
		/// </summary>
		/// <param name="left"></param>
		/// <param name="separator"></param>
		/// <param name="messages"></param>
		/// <returns></returns>
		public static IMessageFormattable Join(this IMessageFormattable left, string separator, params IMessageFormattable[] messages)
		{
			int index = 0;
			if(left == null)
			{
				if(messages == null || messages.Length == 0)
					return null;
				for(; index < messages.Length; index++)
					if(messages[index] != null)
					{
						left = messages[index];
						break;
					}
			}
			for(; index < messages.Length; index++)
				if(messages[index] != null)
					if(left == null)
						left = messages[index];
					else
						left = new Text.JoinMessageFormattable(left, separator, messages[index]);
			return left;
		}

		/// <summary>
		/// Indica que um tipo pode ser tratada como um campo de bit; ou seja, um conjunto de sinalizadores.
		/// </summary>
		/// <param name="type">Tipo a ser analisado.</param>
		/// <returns>Retorna verdadeiro se o tipo é um conjunto de sinalizadores.</returns>
		public static bool IsFlags(this Type type)
		{
			return type.IsEnum && (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0);
		}

		/// <summary>
		/// Cria uma ConditionalExpression que representa um bloco condicional com uma instrução 'if'.
		/// </summary>
		/// <param name="test">Uma expressão para definir a propriedade 'Test'.</param>
		/// <param name="ifTrue">Uma expressão para definir a propriedade 'ifTrue'.</param>
		/// <returns>A ConditionalExpression.</returns>
		public static ConditionalExpression IfThen(Expression test, Expression ifTrue)
		{
			if(ifTrue.NodeType == ExpressionType.Conditional)
			{
				ConditionalExpression conditional = (ConditionalExpression)ifTrue;
				if(conditional.IfFalse.NodeType == ExpressionType.Default && ((DefaultExpression)conditional.IfFalse).Type == typeof(void))
					return Expression.IfThen(Expression.AndAlso(test, conditional.Test), conditional.IfTrue);
			}
			return Expression.IfThen(test, ifTrue);
		}

		/// <summary>
		/// Recupera as informações do membro associado com a expressão informada.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns>Informações do membro contido na expressão informada.</returns>
		public static MemberInfo GetMember(this Expression<Func<string>> expression)
		{
			var memberExp = RemoveUnary(expression.Body);
			if(memberExp == null)
				return null;
			return memberExp.Member;
		}

		/// <summary>
		/// Recupera as informações do membro associado com a expressão informada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="expression"></param>
		/// <returns>Informações do membro contido na expressão informada.</returns>
		public static MemberInfo GetMember<T>(this Expression<Func<T, object>> expression)
		{
			var memberExp = RemoveUnary(expression.Body);
			if(memberExp == null)
				return null;
			return memberExp.Member;
		}

		/// <summary>
		/// Recupera as informações do membro associado com a expressão informada.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="Result"></typeparam>
		/// <param name="expression"></param>
		/// <returns>Informações do membro contido na expressão informada.</returns>
		public static MemberInfo GetMember<T, Result>(this Expression<Func<T, Result>> expression)
		{
			var memberExp = RemoveUnary(expression.Body);
			if(memberExp == null)
				return null;
			return memberExp.Member;
		}

		/// <summary>
		/// Recupera o formatador de texto para a string informada.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="parameters"></param>
		/// <returns></returns>
		public static IMessageFormattable GetFormatter(this string text, params object[] parameters)
		{
			if(text == null)
				return null;
			return new Text.TextMessageFormattable(text, parameters);
		}

		/// <summary>
		/// Recupera o formatador de texto para a exception informada.
		/// </summary>
		/// <param name="exception"></param>
		/// <returns></returns>
		public static IMessageFormattable GetFormatter(this Exception exception)
		{
			if(exception == null)
				return null;
			return new Logging.ExceptionMessageFormattable(exception);
		}

		/// <summary>
		/// Junta as mensagem informadas.
		/// </summary>
		/// <param name="messages"></param>
		/// <param name="separator">Separador.</param>
		/// <returns></returns>
		public static IMessageFormattable Join(this IEnumerable<IMessageFormattable> messages, string separator)
		{
			messages.Require("messages").NotNull();
			var enumerator = messages.GetEnumerator();
			IMessageFormattable current = null;
			if(!enumerator.MoveNext())
				return MessageFormattable.Empty;
			current = enumerator.Current;
			while (enumerator.MoveNext())
				if(current == null)
					current = enumerator.Current;
				else
					current = new Text.JoinMessageFormattable(current, separator, enumerator.Current);
			return current;
		}

		/// <summary>
		/// Formata o valor da mensagem, caso ela seja nula retorna o valor null.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static string FormatOrNull(this IMessageFormattable message)
		{
			return message == null ? null : message.Format();
		}

		/// <summary>
		/// Remove os caracteres especiais de uma string.
		/// </summary>
		/// <param name="inputString">String de entrada</param>
		/// <returns></returns>
		public static string RemoveDiacritics(this string inputString)
		{
			string stFormD = inputString.Normalize(System.Text.NormalizationForm.FormD);
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			for(int ich = 0; ich < stFormD.Length; ich++)
			{
				System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(stFormD[ich]);
				if(uc != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					sb.Append(stFormD[ich]);
				}
			}
			return (sb.ToString().Normalize(System.Text.NormalizationForm.FormC));
		}

		/// <summary>
		/// Limpeza de máscara simples.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ClearMask(string input)
		{
			return String.IsNullOrWhiteSpace(input) || (!input.Where(c => "0123456789".Contains(c)).Any()) ? String.Empty : new String(input.Trim().Where(c => !"_.-/ \n\r\t".Contains(c)).ToArray());
		}

		/// <summary>
		/// Valida um CPF(Brasil)
		/// </summary>
		/// <param name="inputString">palavra contendo o CPF</param>
		/// <returns></returns>
		public static bool ValidateCPF(this string inputString)
		{
			var data = ClearMask(inputString);
			return (data != null) && (data.Length == 11) && (data.ValidateAlg(10, 2, 11, 11)) && (data.ValidateAlg(11, 2, 11, 11));
		}

		/// <summary>
		/// Valida um CNPJ(Brasil)
		/// </summary>
		/// <param name="inputString">palavra contendo o CPF</param>
		/// <returns></returns>
		public static bool ValidateCNPJ(this string inputString)
		{
			var data = ClearMask(inputString);
			return (data != null) && (data.Length == 14) && (data.ValidateAlg(13, 2, 9, 11)) && (data.ValidateAlg(14, 2, 9, 11));
		}

		/// <summary>
		/// Valida um valor por dígitos verificadores
		/// </summary>
		/// <param name="inputString">palavra a ser validada</param>
		/// <param name="size">POsição do dígito a ser verificado</param>
		/// <param name="start">...</param>
		/// <param name="limit">...</param>
		/// <param name="baseValue">...</param>
		/// <returns></returns>
		public static bool ValidateAlg(this string inputString, int size, int start, int limit, int baseValue)
		{
			bool result;
			long sum, digit, factor;
			sum = 0;
			factor = start;
			for(int index = size - 2; index >= 0; index--)
			{
				digit = factor * (((long)inputString[index]) - ((long)'0'));
				if((baseValue < 0) && (digit > 9))
					digit = (digit % 10) + ((digit / 10) % 10) + ((digit / 100) % 10) + ((digit / 100) % 10);
				sum = sum + digit;
				if(start <= limit)
				{
					factor++;
					if(factor > limit)
						factor = start;
				}
				else
				{
					factor--;
					if(factor < limit)
						factor = start;
				}
			}
			result = sum != 0;
			sum = sum % Math.Abs(baseValue);
			sum = Math.Abs(baseValue) - sum;
			if(sum > 9)
				sum = 0;
			result = result && (sum == ((long)inputString[size - 1]) - ((long)'0'));
			return result;
		}

		/// <summary>
		/// Tratamento de strings para entrada no sistema. Limpa espaços e normaliza a înstância Unicode.
		/// </summary>
		/// <param name="inputString"></param>
		/// <returns></returns>
		public static string TrimAndNormalize(this string inputString)
		{
			return (inputString != null) ? inputString.Trim().Normalize(System.Text.NormalizationForm.FormC) : null;
		}

		/// <summary>
		/// Verifica se o valor da mensagem é nulo ou vazio.
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(this IMessageFormattable message)
		{
			return message == null || string.IsNullOrEmpty(message.Format());
		}

		/// <summary>
		/// Ajusta o valor de um ponto flutuante de acordo com a precisão recebida
		/// </summary>
		/// <param name="Value">Valor a ser ajustado</param>
		/// <param name="Precision">Precisão</param>
		/// <returns>Valor ajustado</returns>
		public static double AdjustValue(this double Value, double Precision)
		{
			if(Value < 0)
				return Precision * Math.Ceiling(Value / Precision - 0.501);
			else
				return Precision * Math.Floor(Value / Precision + 0.501);
		}

		/// <summary>
		/// Trunca o numero informado na quantidade de digitos informada.
		/// </summary>
		/// <param name="number"></param>
		/// <param name="digits"></param>
		/// <returns></returns>
		public static decimal Truncate(this decimal number, int digits)
		{
			decimal stepper = (decimal)(Math.Pow(10.0, (double)digits));
			int temp = (int)(stepper * number);
			return (decimal)temp / stepper;
		}

		/// <summary>
		/// Retorna o nome do assembly.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static string GetAssemblyName(this Type type)
		{
			return type.Assembly.FullName.Substring(0, type.Assembly.FullName.IndexOf(","));
		}

		/// <summary>
		/// Elimina os componentes da data que não são armazenados de acordo com a máscara especificada.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static DateTimeOffset FilterByMask(this DateTimeOffset value, string mask)
		{
			if(String.IsNullOrEmpty(mask))
			{
				return new DateTimeOffset(value.Date, value.Offset);
			}
			var input = mask.ToLower();
			if(input.IndexOf("f") >= 0)
			{
				var mul = GetAdjustFactor(mask);
				return new DateTimeOffset((value.Ticks / mul) * mul, value.Offset);
			}
			else if(input.IndexOf("s") >= 0)
			{
				var mul = 10000000L;
				return new DateTimeOffset((value.Ticks / mul) * mul, value.Offset);
			}
			else if(mask.IndexOf("m") >= 0)
			{
				var mul = 60L * 10000000L;
				return new DateTimeOffset((value.Ticks / mul) * mul, value.Offset);
			}
			else if(input.IndexOf("h") >= 0)
			{
				var mul = 60L * 60L * 10000000L;
				return new DateTimeOffset((value.Ticks / mul) * mul, value.Offset);
			}
			else if(input.IndexOf("d") >= 0)
			{
				return new DateTimeOffset(value.Date, value.Offset);
			}
			else if(mask.IndexOf("M") >= 0)
			{
				return new DateTimeOffset(value.Year, value.Month, 1, 0, 0, 0, value.Offset);
			}
			else if(input.IndexOf("y") >= 0)
			{
				return new DateTimeOffset(value.Year, 1, 1, 0, 0, 0, value.Offset);
			}
			return value;
		}

		/// <summary>
		/// Elimina os componentes da data que não são armazenados de acordo com a máscara especificada.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="mask"></param>
		/// <returns></returns>
		public static DateTime FilterByMask(this DateTime value, string mask)
		{
			if(String.IsNullOrEmpty(mask))
			{
				return value.Date;
			}
			var input = mask.ToLower();
			if(input.IndexOf("f") >= 0)
			{
				var mul = GetAdjustFactor(mask);
				return new DateTime((value.Ticks / mul) * mul);
			}
			else if(input.IndexOf("s") >= 0)
			{
				var mul = 10000000L;
				return new DateTime((value.Ticks / mul) * mul);
			}
			else if(mask.IndexOf("m") >= 0)
			{
				var mul = 60L * 10000000L;
				return new DateTime((value.Ticks / mul) * mul);
			}
			else if(input.IndexOf("h") >= 0)
			{
				var mul = 60L * 60L * 10000000L;
				return new DateTime((value.Ticks / mul) * mul);
			}
			else if(input.IndexOf("d") >= 0)
			{
				return value.Date;
			}
			else if(mask.IndexOf("M") >= 0)
			{
				return new DateTime(value.Year, value.Month, 1, 0, 0, 0);
			}
			else if(input.IndexOf("y") >= 0)
			{
				return new DateTime(value.Year, 1, 1, 0, 0, 0);
			}
			return value;
		}

		/// <summary>
		/// Processa o valor se for não nulo, retornando um valor default em caso contrário.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <param name="value"></param>
		/// <param name="call"></param>
		/// <param name="stdVal"></param>
		/// <returns></returns>
		public static TR IfNotNull<T, TR>(this T value, Func<T, TR> call, TR stdVal = default(TR)) where T : class
		{
			return (value != null) ? call(value) : stdVal;
		}

		/// <summary>
		/// Processa o valor se estiver definido, retornando um valor default em caso contrário.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <typeparam name="TR"></typeparam>
		/// <param name="value"></param>
		/// <param name="call"></param>
		/// <param name="stdVal"></param>
		/// <returns></returns>
		public static TR IfHasValue<T, TR>(this Nullable<T> value, Func<T, TR> call, TR stdVal = default(TR)) where T : struct
		{
			return value.HasValue ? call(value.Value) : stdVal;
		}

		/// <summary>
		/// Determina se a lista de strings parâmetro contem a string base.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static bool IsIn(this string instance, params string[] values)
		{
			return values.HasItems() && values.Contains(instance);
		}

		/// <summary>
		/// Enumerates through the individual set bits in a flag enum.
		/// </summary>
		/// <param name="flags">The flags enum value.</param>
		/// <returns>An enumeration of just the <i>set</i> bits in the flags enum.</returns>
		public static IEnumerable<long> GetIndividualFlags(this Enum flags)
		{
			long flagsLong = Convert.ToInt64(flags);
			for(int i = 0; i < sizeof(long) * 8; i++)
			{
				long individualFlagPosition = (long)Math.Pow(2, i);
				long individualFlag = flagsLong & individualFlagPosition;
				if(individualFlag == individualFlagPosition)
				{
					yield return individualFlag;
				}
			}
		}

		/// <summary>
		/// Recupera o flag do dia da semana com base na data informada.
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static WeekDay GetWeekDay(this DateTime date)
		{
			switch(date.DayOfWeek)
			{
			case DayOfWeek.Sunday:
				return WeekDay.Sunday;
			case DayOfWeek.Monday:
				return WeekDay.Monday;
			case DayOfWeek.Tuesday:
				return WeekDay.Tuesday;
			case DayOfWeek.Wednesday:
				return WeekDay.Wednesday;
			case DayOfWeek.Friday:
				return WeekDay.Friday;
			case DayOfWeek.Saturday:
				return WeekDay.Saturday;
			default:
				return 0;
			}
		}

		/// <summary>
		/// Aplica o distinc nos itens usando o comparador que é uma expressão Lambda.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="items"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IEnumerable<T> Distinct<T>(this IEnumerable<T> items, Func<T, T, bool> comparer)
		{
			return items.Distinct(new LambdaComparer<T>(comparer));
		}

		/// <summary>
		/// Agrupa a coleção de itens informados.
		/// </summary>
		/// <typeparam name="TSource">Tipo do item da coleção.</typeparam>
		/// <typeparam name="TKey">Tipo da chave de agrupamento.</typeparam>
		/// <param name="source">Origem dos itens.</param>
		/// <param name="keySelector"></param>
		/// <param name="comparer"></param>
		/// <returns></returns>
		public static IEnumerable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TKey, TKey, bool> comparer)
		{
			return source.GroupBy(keySelector, new LambdaComparer<TKey>(comparer));
		}
	}
}
