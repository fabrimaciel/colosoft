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
	/// Formatos EasyComm
	/// </summary>
	public enum MaskFormat
	{
		/// <summary>
		/// Caractere fixo
		/// </summary>
		Separator,
		/// <summary>
		/// Caractere de escape
		/// </summary>
		Escape,
		/// <summary>
		/// [0-9].
		/// </summary>
		Digit,
		/// <summary>
		/// [0-9] ou espaço.
		/// </summary>
		DigitOption,
		/// <summary>
		/// [A-Za-z].
		/// </summary>
		Ascii,
		/// <summary>
		/// [A-Za-z] ou espaço.
		/// </summary>
		AsciiOption,
		/// <summary>
		/// Qualquer caractere exceto espaço.
		/// </summary>
		Unicode,
		/// <summary>
		/// Qualquer caractere.
		/// </summary>
		UnicodeOption,
		/// <summary>
		/// [A-Za-z0-9].
		/// </summary>
		AlphaNumeric,
		/// <summary>
		/// [A-Za-z0-9] ou espaço.
		/// </summary>
		AlphaNumericOption,
		/// <summary>
		/// Separador de fracionários.
		/// </summary>
		Fractional,
		/// <summary>
		/// Separador de milhares.
		/// </summary>
		Thousands,
		/// <summary>
		/// Os caracteres seguintes serão convertidos para minúsculos.
		/// </summary>
		Lowercase,
		/// <summary>
		/// Os caracteres seguintes serão convertdos para maiúsculas.
		/// </summary>
		Uppercase
	}
	/// <summary>
	/// Valores para definições de maiúsculas/minúsculas.
	/// </summary>
	public enum MaskCase
	{
		/// <summary>
		/// Normal.
		/// </summary>
		Normal,
		/// <summary>
		/// Minúsculas.
		/// </summary>
		Lower,
		/// <summary>
		/// Maiúsculas.
		/// </summary>
		Upper
	}
	/// <summary>
	/// Formato associado a uma máscara completa.
	/// </summary>
	public enum GroupFormat
	{
		/// <summary>
		/// Sem formato definido.
		/// </summary>
		Empty,
		/// <summary>
		/// Caracteres diversos.
		/// </summary>
		Free,
		/// <summary>
		/// Somente dígitos.
		/// </summary>
		Numeric,
		/// <summary>
		/// Somente letras do alfabeto.
		/// </summary>
		Alphabetic,
		/// <summary>
		/// Somente letras ou dígitos.
		/// </summary>
		AlphaNumeric
	}
	/// <summary>
	/// Item de definição de formato.
	/// </summary>
	public struct FormatToken
	{
		private MaskFormat _format;

		private MaskCase _itemCase;

		private char _value;

		/// <summary>
		/// O formato associado.
		/// </summary>
		public MaskFormat Format
		{
			get
			{
				return _format;
			}
		}

		/// <summary>
		/// O formato maiúsculas/minúsculas associado.
		/// </summary>
		public MaskCase ItemCase
		{
			get
			{
				return _itemCase;
			}
		}

		/// <summary>
		/// O valor do caractere na posição.
		/// </summary>
		public char Value
		{
			get
			{
				return _value;
			}
		}

		/// <summary>
		/// Indica se o item representa uma posição de preenchimento obrigatório.
		/// </summary>
		public bool IsRequired
		{
			get
			{
				return _format.IsRequired();
			}
		}

		/// <summary>
		/// Indica se o item representa uma posição de preenchimento opcional.
		/// </summary>
		public bool IsOptional
		{
			get
			{
				return _format.IsOptional();
			}
		}

		/// <summary>
		/// Indica se o item representa uma posição que é possível preencher.
		/// </summary>
		public bool IsFillable
		{
			get
			{
				return _format.IsPositionToFill();
			}
		}

		/// <summary>
		/// Indica se o item representa um caractere fixo no formato.
		/// </summary>
		public bool IsSeparator
		{
			get
			{
				return _format.IsSeparator();
			}
		}

		/// <summary>
		/// Indica se o item representa um caractere alfanumérico.
		/// </summary>
		public bool IsAlphaNumeric
		{
			get
			{
				return _format.IsAlphaNumeric();
			}
		}

		/// <summary>
		/// Indica se o item está com formato de maiúsculas selecionado.
		/// </summary>
		public bool IsUpperCase
		{
			get
			{
				return _itemCase == MaskCase.Upper;
			}
		}

		/// <summary>
		/// Indica se o item está com formato de minúsculas selecionado.
		/// </summary>
		public bool IsLowerCase
		{
			get
			{
				return _itemCase == MaskCase.Lower;
			}
		}

		/// <summary>
		/// Indica se o item não tem formato maiúsculas/minúsculas selecionado.
		/// </summary>
		public bool IsNormalCase
		{
			get
			{
				return _itemCase == MaskCase.Normal;
			}
		}

		/// <summary>
		/// Indica se o caractere é válido na posição do token.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="whitespace"></param>
		/// <returns></returns>
		public bool IsValid(char value, char whitespace = ' ')
		{
			switch(_format)
			{
			case MaskFormat.AlphaNumeric:
				return value.IsAlphaNumeric(_itemCase);
			case MaskFormat.AlphaNumericOption:
				return value.IsAlphaNumeric(_itemCase) || (value == whitespace);
			case MaskFormat.Ascii:
				return value.IsAlphabetic(_itemCase);
			case MaskFormat.AsciiOption:
				return value.IsAlphabetic(_itemCase) || (value == whitespace);
			case MaskFormat.Digit:
				return value.IsNumeric();
			case MaskFormat.DigitOption:
				return value.IsNumeric() || (value == whitespace);
			case MaskFormat.Fractional:
				return (value == ',');
			case MaskFormat.Separator:
				return (value == Value);
			case MaskFormat.Thousands:
				return (value == '.');
			case MaskFormat.Unicode:
				return (value != whitespace) && value.IsCaseMatch(_itemCase);
			case MaskFormat.UnicodeOption:
				return value.IsCaseMatch(_itemCase);
			}
			return false;
		}

		/// <summary>
		/// Construtor parametrizado.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="itemCase"></param>
		public FormatToken(char value, MaskCase itemCase) : this(value, value.GetMaskFormat(), itemCase)
		{
		}

		/// <summary>
		/// Construtor parametrizado.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="format"></param>
		/// <param name="itemCase"></param>
		public FormatToken(char value, MaskFormat format, MaskCase itemCase)
		{
			_value = value;
			_format = format;
			_itemCase = itemCase;
		}

		/// <summary>
		/// Exibição da instância como texto.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var caseIdentifier = 'N';
			switch(_itemCase)
			{
			case MaskCase.Lower:
				caseIdentifier = '<';
				break;
			case MaskCase.Upper:
				caseIdentifier = '>';
				break;
			}
			return String.Format("{0}{1}'{2}'", caseIdentifier, _format.GetFormatChar(), _value);
		}

		/// <summary>
		/// Obtém o código hash associado com a instância.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.ToString().GetHashCode();
		}

		/// <summary>
		/// Determina se os objetos são iguais.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if((obj == null) || (!(obj is FormatToken)))
			{
				return false;
			}
			var cObj = (FormatToken)obj;
			return (_format == cObj._format) && (_itemCase == cObj._itemCase) && (_value == cObj._value);
		}
	}
	/// <summary>
	/// Especificação completa de máscara.
	/// </summary>
	public struct FormatMask
	{
		private string _content;

		private IEnumerable<FormatToken> _tokens;

		private IList<Tuple<FormatToken, int>> _required;

		private IList<Tuple<FormatToken, int>> _fillable;

		private IList<Tuple<FormatToken, int>> _separators;

		private string _initialValue;

		private string _formatted;

		private GroupFormat _format;

		private static readonly FormatMask _empty = new FormatMask(null);

		/// <summary>
		/// O valor original da máscara;
		/// </summary>
		public string Content
		{
			get
			{
				return _content;
			}
		}

		/// <summary>
		/// Os componentes da máscara.
		/// </summary>
		public IEnumerable<FormatToken> Tokens
		{
			get
			{
				return _tokens;
			}
		}

		/// <summary>
		/// O formato associado ao grupo de tokens.
		/// </summary>
		public GroupFormat Format
		{
			get
			{
				return _format;
			}
		}

		/// <summary>
		/// Tokens com preenchimento obrigatório com suas respectivas posições.
		/// </summary>
		public IEnumerable<Tuple<FormatToken, int>> Required
		{
			get
			{
				return _required;
			}
		}

		/// <summary>
		/// Tokens passíveis de preenchimento com suas respectivas posições.
		/// </summary>
		public IEnumerable<Tuple<FormatToken, int>> Fillable
		{
			get
			{
				return _fillable;
			}
		}

		/// <summary>
		/// Retorna uma sequência dos caracteres fixos com suas respectivas posições.
		/// </summary>
		public IEnumerable<Tuple<FormatToken, int>> Separators
		{
			get
			{
				return _separators;
			}
		}

		/// <summary>
		/// Número de posições passíveis de preenchimento.
		/// </summary>
		public int FillableCount
		{
			get
			{
				return _fillable.Count;
			}
		}

		/// <summary>
		/// O valor inicial da máscara.
		/// </summary>
		public string InitialValue
		{
			get
			{
				return _initialValue;
			}
		}

		/// <summary>
		/// Indica se a máscara não possui formato.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return String.IsNullOrEmpty(_content);
			}
		}

		/// <summary>
		/// Máscara vazia.
		/// </summary>
		public static FormatMask Empty
		{
			get
			{
				return _empty;
			}
		}

		/// <summary>
		/// Conta o número de posições passíveis de preenchimento a partir do índice fornecido.
		/// </summary>
		/// <param name="fromIndex"></param>
		/// <returns></returns>
		public int CountToFill(int fromIndex = 0)
		{
			var items = (fromIndex > 0) ? _tokens.Skip(fromIndex) : _tokens;
			return items.Where(t => t.IsFillable).Count();
		}

		/// <summary>
		/// Indica se o valor está corretamente formatado.
		/// </summary>
		/// <param name="value"></param>
		/// <param name="whitespace"></param>
		/// <returns></returns>
		public bool IsAppliedTo(string value, char whitespace = ' ')
		{
			if(_tokens.IsNullOrEmpty())
			{
				return false;
			}
			if(String.IsNullOrEmpty(value))
			{
				return true;
			}
			var index = 0;
			foreach (var token in _tokens)
			{
				var selected = value[index++];
				if(!token.IsValid(selected, whitespace))
				{
					return false;
				}
				if(index >= value.Length)
				{
					return true;
				}
			}
			return true;
		}

		/// <summary>
		/// Formata o valor.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="padding"></param>
		/// <returns></returns>
		public string FormatValue(string input, char padding = '_')
		{
			if(String.IsNullOrEmpty(input))
			{
				input = String.Empty;
			}
			if(_tokens.IsNullOrEmpty())
			{
				return input ?? String.Empty;
			}
			var result = new StringBuilder();
			var consumed = 0;
			var filled = 0;
			var total = input.Length;
			foreach (var token in _tokens)
			{
				var isRequired = token.IsRequired;
				var isOptional = token.IsOptional;
				if(isRequired || isOptional)
				{
					if(isOptional)
					{
						var rest = CountToFill(consumed);
						var left = total - filled;
						if(rest > left)
						{
							result.Append(padding);
							continue;
						}
					}
					if(total == filled)
					{
						result.Append(padding);
					}
					else
					{
						result.Append(input[filled++]);
					}
				}
				else
				{
					result.Append(token.Value);
				}
				++consumed;
			}
			return result.ToString();
		}

		/// <summary>
		/// Formata um valor que pode conter espaços nas posições não requeridas.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="padding"></param>
		/// <returns></returns>
		public string FormatWithOptional(string input, char padding = '_')
		{
			if(String.IsNullOrEmpty(input))
			{
				input = String.Empty;
			}
			if(_tokens.IsNullOrEmpty())
			{
				return input ?? String.Empty;
			}
			var result = new StringBuilder();
			var sequence = input.GetEnumerator();
			var available = sequence.MoveNext();
			foreach (var token in _tokens)
			{
				if(token.IsFillable)
				{
					if(available)
					{
						var next = (sequence.Current == ' ') ? padding : sequence.Current;
						result.Append(next);
						available = sequence.MoveNext();
					}
					else
					{
						result.Append(padding);
					}
				}
				else
				{
					result.Append(token.Value);
				}
			}
			return result.ToString();
		}

		/// <summary>
		/// Detecta se o valor não é capaz de preencher todas as posições requeridas
		/// do formato. Valores vazios são considerados válidos.
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public bool IsIncompleteValue(string input)
		{
			if(IsEmpty || String.IsNullOrEmpty(input))
			{
				return false;
			}
			var reqSize = _required.Count;
			var value = ClearValue(input);
			return reqSize > value.Length;
		}

		/// <summary>
		/// Remove a formatação do valor.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="whitespace"></param>
		/// <returns></returns>
		public string ClearValue(string input, char whitespace = ' ')
		{
			if(String.IsNullOrEmpty(input))
			{
				return String.Empty;
			}
			if(_tokens.IsNullOrEmpty())
			{
				return input ?? String.Empty;
			}
			if(IsAppliedTo(input, whitespace))
			{
				var tks = _tokens;
				var siz = _tokens.Count();
				var data = input.Where((c, i) => (i < siz) && tks.ElementAt(i).IsFillable && (c != whitespace));
				return data.IsNullOrEmpty() || Enumerable.SequenceEqual(data, _initialValue) ? String.Empty : new String(data.ToArray());
			}
			else
			{
				return input.Strip(Format).LimitTo(FillableCount);
			}
		}

		/// <summary>
		/// Limpa o formato do valor mantendo espaços nas posições opcionais não preenchidas.
		/// </summary>
		/// <param name="input"></param>
		/// <param name="padding"></param>
		/// <returns></returns>
		public string ClearWithOptional(string input, char padding = '_')
		{
			if(String.IsNullOrEmpty(input))
			{
				return String.Empty;
			}
			if(_tokens.IsNullOrEmpty())
			{
				return input ?? String.Empty;
			}
			if(IsAppliedTo(input, padding))
			{
				var tks = _tokens;
				var siz = _tokens.Count();
				var data = input.Where((c, i) => (i < siz) && tks.ElementAt(i).IsFillable).Select(c => (c == padding) ? ' ' : c);
				return data.IsNullOrEmpty() || Enumerable.SequenceEqual(data, _initialValue) ? String.Empty : new String(data.ToArray());
			}
			else
			{
				return input.Strip(Format).LimitTo(FillableCount);
			}
		}

		/// <summary>
		/// Construtor parametrizado.
		/// </summary>
		/// <param name="content"></param>
		public FormatMask(string content)
		{
			_initialValue = content.GetMaskIntialValue();
			_content = content.GetMaskContent();
			_tokens = _content.GetTokens();
			_required = _tokens.Select((v, i) => Tuple.Create(v, i)).Where(t => t.Item1.IsRequired).ToList();
			_fillable = _tokens.Select((v, i) => Tuple.Create(v, i)).Where(t => t.Item1.IsFillable).ToList();
			_separators = _tokens.Select((v, i) => Tuple.Create(v, i)).Where(t => t.Item1.IsSeparator).ToList();
			_formatted = new StringBuilder(_content).Append('|').Append(_initialValue).ToString();
			_format = GroupFormat.Empty;
			foreach (var token in _tokens)
			{
				var newFormat = token.Format.GetGroupFormat();
				if((_format == GroupFormat.Empty) || (newFormat == GroupFormat.Free))
				{
					_format = newFormat;
				}
				if(_format == GroupFormat.Free)
				{
					break;
				}
				if(_format == GroupFormat.Numeric)
				{
					if((newFormat == GroupFormat.Alphabetic) || (newFormat == GroupFormat.AlphaNumeric))
					{
						_format = GroupFormat.AlphaNumeric;
					}
				}
				if(_format == GroupFormat.Alphabetic)
				{
					if((newFormat == GroupFormat.Numeric) || (newFormat == GroupFormat.AlphaNumeric))
					{
						_format = GroupFormat.AlphaNumeric;
					}
				}
			}
		}

		/// <summary>
		/// Exibição da instância como texto.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _formatted;
		}

		/// <summary>
		/// Obtém o código hash associado com a instância.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _formatted.GetHashCode();
		}

		/// <summary>
		/// Determina se os objetos são iguais.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			if((obj == null) || (!(obj is FormatMask)))
			{
				return false;
			}
			var cObj = (FormatMask)obj;
			return String.Equals(_formatted, cObj._formatted);
		}
	}
	/// <summary>
	/// Classe que encapsula a formatação de um valor com máscara.
	/// </summary>
	public class MaskedValue
	{
		private FormatMask _mask;

		private string _content;

		private char _padding;

		/// <summary>
		/// A máscara a ser aplicada ao valor.
		/// </summary>
		public FormatMask Mask
		{
			get
			{
				return _mask;
			}
		}

		/// <summary>
		/// O valor contendo apenas os caracteres que não sejam formatadores
		/// e os possíveis espaços em branco para posições opcionais.
		/// </summary>
		public string Content
		{
			get
			{
				return _content;
			}
			set
			{
				_content = _mask.ClearWithOptional(value, _padding);
			}
		}

		/// <summary>
		/// O caractere a ser utilizado como indicador de posição não preenchida.
		/// </summary>
		public char Padding
		{
			get
			{
				return _padding;
			}
		}

		/// <summary>
		/// Caso não seja uma máscara de formato liver, retorna o valor sem os espaços para campos opcionais.
		/// Em caso contrário, retorna o valor do conteúdo sem alteração.
		/// </summary>
		public string ClearContent
		{
			get
			{
				return _mask.Format.IsFreeFormat() ? _content : _content.Replace(" ", String.Empty);
			}
		}

		/// <summary>
		/// O valor formatado pela máscara.
		/// </summary>
		public string FormattedContent
		{
			get
			{
				return _mask.FormatWithOptional(_content, _padding);
			}
		}

		/// <summary>
		/// Construtor especificando a máscara e o caractere de preenchimento.
		/// </summary>
		/// <param name="mask"></param>
		/// <param name="padding"></param>
		public MaskedValue(string mask, char padding = '_')
		{
			_mask = mask.GetMask();
			_padding = padding;
			_content = _mask.FormatWithOptional(String.Empty, _padding);
		}
	}
}
