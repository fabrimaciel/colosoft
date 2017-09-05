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

namespace Colosoft
{
	/// <summary>
	/// Extensões para formatos.
	/// </summary>
	public static class MaskFormatExtensions
	{
		/// <summary>
		/// Identifica se a posição tem preenchimento obrigatório.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsRequired(this MaskFormat instance)
		{
			return (instance == MaskFormat.AlphaNumeric) || (instance == MaskFormat.Ascii) || (instance == MaskFormat.Digit) || (instance == MaskFormat.Unicode);
		}

		/// <summary>
		/// Indica se o formato é de preenchimento opcional.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsOptional(this MaskFormat instance)
		{
			return (instance == MaskFormat.AlphaNumericOption) || (instance == MaskFormat.AsciiOption) || (instance == MaskFormat.DigitOption) || (instance == MaskFormat.UnicodeOption);
		}

		/// <summary>
		/// Indica a possibilidade da posição ser preenchida.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsPositionToFill(this MaskFormat instance)
		{
			return instance.IsOptional() || instance.IsRequired();
		}

		/// <summary>
		/// Indica se o formato é um caractere fixo.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsSeparator(this MaskFormat instance)
		{
			return (instance == MaskFormat.Separator) || (instance == MaskFormat.Fractional) || (instance == MaskFormat.Thousands);
		}

		/// <summary>
		/// Recupera o caractere indicador de formato para exibição.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static char GetFormatChar(this MaskFormat instance)
		{
			switch(instance)
			{
			case MaskFormat.Digit:
				return '0';
			case MaskFormat.DigitOption:
				return '#';
			case MaskFormat.Ascii:
				return 'L';
			case MaskFormat.AsciiOption:
				return '?';
			case MaskFormat.Unicode:
				return '&';
			case MaskFormat.UnicodeOption:
				return 'C';
			case MaskFormat.AlphaNumeric:
				return 'A';
			case MaskFormat.AlphaNumericOption:
				return 'a';
			case MaskFormat.Fractional:
				return '.';
			case MaskFormat.Thousands:
				return ',';
			case MaskFormat.Lowercase:
				return '<';
			case MaskFormat.Uppercase:
				return '>';
			case MaskFormat.Escape:
				return '\\';
			}
			return ' ';
		}

		/// <summary>
		/// Indicador de formato alfanumérico.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsAlphaNumeric(this MaskFormat instance)
		{
			return (instance == MaskFormat.AlphaNumeric) || (instance == MaskFormat.AlphaNumericOption) || (instance == MaskFormat.Ascii) || (instance == MaskFormat.AsciiOption) || (instance == MaskFormat.Digit) || (instance == MaskFormat.DigitOption);
		}

		/// <summary>
		/// Recupera o formato de grupo definido pelo item de formato.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static GroupFormat GetGroupFormat(this MaskFormat instance)
		{
			switch(instance)
			{
			case MaskFormat.Digit:
				return GroupFormat.Numeric;
			case MaskFormat.Ascii:
				return GroupFormat.Alphabetic;
			case MaskFormat.AlphaNumeric:
				return GroupFormat.AlphaNumeric;
			case MaskFormat.Unicode:
				return GroupFormat.Free;
			case MaskFormat.DigitOption:
				goto case MaskFormat.Digit;
			case MaskFormat.AsciiOption:
				goto case MaskFormat.Ascii;
			case MaskFormat.AlphaNumericOption:
				goto case MaskFormat.AlphaNumeric;
			case MaskFormat.UnicodeOption:
				goto case MaskFormat.Unicode;
			}
			return GroupFormat.Empty;
		}
	}
	/// <summary>
	/// Extensões para caracteres.
	/// </summary>
	public static class CharMaskExtensions
	{
		/// <summary>
		/// Recupera a definição de formato associada ao caractere.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static MaskFormat GetMaskFormat(this char instance)
		{
			switch(instance)
			{
			case '0':
				return MaskFormat.Digit;
			case '9':
				return MaskFormat.DigitOption;
			case '#':
				return MaskFormat.DigitOption;
			case 'L':
				return MaskFormat.Ascii;
			case '?':
				return MaskFormat.AsciiOption;
			case '&':
				return MaskFormat.Unicode;
			case 'C':
				return MaskFormat.UnicodeOption;
			case 'A':
				return MaskFormat.AlphaNumeric;
			case 'a':
				return MaskFormat.AlphaNumericOption;
			case '.':
				return MaskFormat.Fractional;
			case ',':
				return MaskFormat.Thousands;
			case '<':
				return MaskFormat.Lowercase;
			case '>':
				return MaskFormat.Uppercase;
			case '\\':
				return MaskFormat.Escape;
			}
			return MaskFormat.Separator;
		}

		/// <summary>
		/// Indica se se trata de um dígito.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsNumeric(this char instance)
		{
			return ('0' <= instance) && ('9' >= instance);
		}

		/// <summary>
		/// Indica se se trata de um caractere no intervalo A a Z.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="itemCase"></param>
		/// <returns></returns>
		public static bool IsAlphabetic(this char instance, MaskCase itemCase = MaskCase.Normal)
		{
			switch(itemCase)
			{
			case MaskCase.Lower:
				return ('a' <= instance) && ('z' >= instance);
			case MaskCase.Upper:
				return ('A' <= instance) && ('Z' >= instance);
			}
			return (('a' <= instance) && ('z' >= instance)) || (('A' <= instance) && ('Z' >= instance));
		}

		/// <summary>
		/// Indica se se trata de um caractere do alfabeto ou de dígito.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="itemCase"></param>
		/// <returns></returns>
		public static bool IsAlphaNumeric(this char instance, MaskCase itemCase = MaskCase.Normal)
		{
			return instance.IsNumeric() || instance.IsAlphabetic(itemCase);
		}

		/// <summary>
		/// Determina se o item atende a especificação de maiísculas/minúsculas fornecida.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="itemCase"></param>
		/// <returns></returns>
		public static bool IsCaseMatch(this char instance, MaskCase itemCase)
		{
			switch(itemCase)
			{
			case MaskCase.Lower:
				return !Char.IsUpper(instance);
			case MaskCase.Upper:
				return !Char.IsLower(instance);
			default:
				return true;
			}
		}
	}
	/// <summary>
	/// Extensões para formato de grupo.
	/// </summary>
	public static class GroupFormatExtensions
	{
		/// <summary>
		/// Indica se o formato é livre.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsFreeFormat(this GroupFormat instance)
		{
			return instance == GroupFormat.Free;
		}

		/// <summary>
		/// Indica se o formato é numérico.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsNumericFormat(this GroupFormat instance)
		{
			return instance == GroupFormat.Numeric;
		}

		/// <summary>
		/// Indica se o formato de letras do alfabeto.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsAlphabeticFormat(this GroupFormat instance)
		{
			return instance == GroupFormat.Alphabetic;
		}

		/// <summary>
		/// Indica se o formato de caracteres alfanuméricos.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static bool IsAlphaNumericFormat(this GroupFormat instance)
		{
			return instance == GroupFormat.AlphaNumeric;
		}
	}
	/// <summary>
	/// Extensões para strings.
	/// </summary>
	public static class StringMaskExtensions
	{
		/// <summary>
		/// Troca as ocorrências do primeiro caractere pelo segundo e vice-versa.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="first"></param>
		/// <param name="second"></param>
		/// <returns></returns>
		public static string Exchange(this string instance, char first, char second)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			if((first == '\0') || (second == '\0'))
			{
				throw new ArgumentException("Character '\\0' invalid in exchange o characters.");
			}
			return instance.Replace(first, '\0').Replace(second, first).Replace('\0', second);
		}

		/// <summary>
		/// Substitui todas as ocrrências de caracteres da string de valores pelo caractere de substituição.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="values"></param>
		/// <param name="replacement"></param>
		/// <returns></returns>
		public static string MultiReplace(this string instance, string values, char replacement)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			if(String.IsNullOrEmpty(values))
			{
				return instance ?? String.Empty;
			}
			var result = instance;
			foreach (var item in values)
			{
				result = result.Replace(item, replacement);
			}
			return result;
		}

		/// <summary>
		/// Remove as ocorrências dos caracteres o valor parâmetro da instância.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="values"></param>
		/// <returns></returns>
		public static string MultiRemove(this string instance, string values)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			if(String.IsNullOrEmpty(values))
			{
				return instance;
			}
			var result = new StringBuilder();
			foreach (var item in instance)
			{
				if(values.Contains(item))
				{
					continue;
				}
				result.Append(item);
			}
			return result.ToString();
		}

		/// <summary>
		/// Retorna apenas os caracteres alfanuméricos da instância.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string Strip(this string instance, GroupFormat format = GroupFormat.Free)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			switch(format)
			{
			case GroupFormat.Alphabetic:
				return new StringBuilder(instance.Length).Append(instance.Where(v => v.IsAlphabetic()).ToArray()).ToString();
			case GroupFormat.AlphaNumeric:
				return new StringBuilder(instance.Length).Append(instance.Where(v => v.IsAlphaNumeric()).ToArray()).ToString();
			case GroupFormat.Numeric:
				return new StringBuilder(instance.Length).Append(instance.Where(v => v.IsNumeric()).ToArray()).ToString();
			default:
				return instance;
			}
		}

		/// <summary>
		/// Limita o tamanho do valor retornado a um número de caracteres inferior
		/// ao parâmetro fornecido.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public static string LimitTo(this string instance, int size)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			if(size == 0)
			{
				return String.Empty;
			}
			if(size < 0)
			{
				return instance ?? String.Empty;
			}
			return (instance.Length > size) ? instance.Substring(0, size) : instance;
		}

		/// <summary>
		/// Transforma a string para o formato aceito pelo EasyComm.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static string GetEasyCommMask(this string instance)
		{
			return String.IsNullOrEmpty(instance) ? String.Empty : instance.Exchange('.', ',');
		}

		/// <summary>
		/// Converte um formato de data personalizado do CLR por um formato do EasyComm.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static string DateFormatToEasyCommMask(this string instance)
		{
			if(String.IsNullOrWhiteSpace(instance))
			{
				return String.Empty;
			}
			return instance.GetEasyCommMask().MultiReplace("dDmMyYhHsSfFkKtTzZ", '0');
		}

		/// <summary>
		/// Recupera o índice do primeiro caractere igual ao fornecido que não seja precedido
		/// por um caractere de escape.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="value"></param>
		/// <param name="escape"></param>
		/// <returns></returns>
		public static int IndexOfNotEscaped(this string instance, char value, char escape = '\\')
		{
			if(String.IsNullOrEmpty(instance))
			{
				return -1;
			}
			var position = instance.IndexOf(value);
			if(position <= 0)
			{
				return position;
			}
			var total = instance.Length - 1;
			while ((instance[position - 1] == escape) && (position < total))
			{
				position = instance.IndexOf(value, position + 1);
				if(position <= 0)
				{
					return position;
				}
			}
			return (instance[position - 1] == escape) ? -1 : position;
		}

		/// <summary>
		/// Elimina caracteres de marcação e escape para deixar a máscara apenas com definições
		/// que permitam identificar se um valor tem o formato correto.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static string GetMaskContent(this string instance)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			var pos = instance.IndexOfNotEscaped('"');
			var lim = instance.Length - 1;
			if((pos < lim) && (instance[lim] == '"'))
			{
				return instance.Substring(0, pos);
			}
			return instance;
		}

		/// <summary>
		/// Recupera o valor inicial definido em uma máscara (se existir).
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static string GetMaskIntialValue(this string instance)
		{
			if(String.IsNullOrEmpty(instance))
			{
				return String.Empty;
			}
			var pos = instance.IndexOfNotEscaped('"');
			var lim = instance.Length - 1;
			if((pos < lim) && (instance[lim] == '"'))
			{
				++pos;
				return instance.Substring(pos, lim - pos);
			}
			return String.Empty;
		}

		/// <summary>
		/// Decomposição da máscara em elementos.
		/// </summary>
		/// <param name="instance"></param>
		/// <param name="fractional"></param>
		/// <param name="thousands"></param>
		/// <returns></returns>
		public static IEnumerable<FormatToken> GetTokens(this string instance, char fractional = '.', char thousands = ',')
		{
			if(String.IsNullOrEmpty(instance))
			{
				return Enumerable.Empty<FormatToken>();
			}
			var result = new List<FormatToken>();
			var escaped = false;
			var itemCase = MaskCase.Normal;
			foreach (var item in instance)
			{
				if(escaped)
				{
					result.Add(new FormatToken(item, MaskFormat.Separator, itemCase));
					escaped = false;
					continue;
				}
				var format = item.GetMaskFormat();
				switch(format)
				{
				case MaskFormat.Escape:
					escaped = true;
					break;
				case MaskFormat.Lowercase:
					itemCase = MaskCase.Lower;
					break;
				case MaskFormat.Uppercase:
					itemCase = MaskCase.Upper;
					break;
				case MaskFormat.Fractional:
					result.Add(new FormatToken(fractional, MaskFormat.Fractional, itemCase));
					break;
				case MaskFormat.Thousands:
					result.Add(new FormatToken(thousands, MaskFormat.Thousands, itemCase));
					break;
				default:
					result.Add(new FormatToken(item, itemCase));
					break;
				}
			}
			return result.AsReadOnly();
		}

		/// <summary>
		/// Recupera a definição de máscara a partir de seu valor textual.
		/// </summary>
		/// <param name="instance"></param>
		/// <returns></returns>
		public static FormatMask GetMask(this string instance)
		{
			return String.IsNullOrWhiteSpace(instance) ? FormatMask.Empty : new FormatMask(instance);
		}
	}
}
