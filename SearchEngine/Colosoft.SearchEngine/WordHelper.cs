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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Classe que auxilia no tratamento de palavras.
	/// </summary>
	public static class WordHelper
	{
		/// <summary>
		/// Recupera as palavras contidas no texto.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string[] GetWords(this string text)
		{
			StringBuilder currentWord = new StringBuilder();
			List<string> result = new List<string>();
			foreach (char currentChar in text ?? "")
			{
				if(IsEndOfWord(currentChar))
				{
					string word = ClearWord(currentWord.ToString());
					if(IsValidWord(word))
					{
						result.Add(word);
					}
					currentWord = new StringBuilder();
				}
				else
					currentWord.Append(currentChar);
			}
			if(currentWord.ToString().Length > 0)
				result.Add(currentWord.ToString());
			return result.ToArray<string>();
		}

		/// <summary>
		/// Recupera palavras distintas do texto.
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string[] GetDistinctWords(this string text)
		{
			StringBuilder currentWord = new StringBuilder();
			List<string> result = new List<string>();
			foreach (char currentChar in text ?? "")
			{
				if(IsEndOfWord(currentChar))
				{
					string word = ClearWord(currentWord.ToString());
					if((IsValidWord(word)) && (!result.Contains(word)))
					{
						result.Add(word);
					}
					currentWord = new StringBuilder();
				}
				else
					currentWord.Append(currentChar);
			}
			if((currentWord.ToString().Length > 0) && (IsValidWord(currentWord.ToString())) && (!result.Contains(currentWord.ToString())))
				result.Add(currentWord.ToString());
			return result.ToArray<string>();
		}

		/// <summary>
		/// Verifica se é o final de uma palavra.
		/// </summary>
		/// <param name="ch"></param>
		/// <returns></returns>
		private static bool IsEndOfWord(char ch)
		{
			return ((Char.IsPunctuation(ch)) || (Char.IsSymbol(ch)) || (Char.IsWhiteSpace(ch)));
		}

		/// <summary>
		/// Verifica ser a palavra informada é válida.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private static bool IsValidWord(string word)
		{
			return ((word.Trim().Length > 1) && (!word.Contains("#")) && (!word.Contains("$")) && (!word.Contains("&")) && (!word.Contains("*")) && (!word.Contains("=")) && (!word.Contains("+")));
		}

		/// <summary>
		/// Limpa a palavra de caracters invalidos.
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private static string ClearWord(string word)
		{
			return ClearString(word.ToUpper().Trim());
		}

		private static string ClearString(string text)
		{
			string s = text.Normalize(NormalizationForm.FormD);
			StringBuilder sb = new StringBuilder();
			for(int k = 0; k < s.Length; k++)
			{
				System.Globalization.UnicodeCategory uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(s[k]);
				if(uc != System.Globalization.UnicodeCategory.NonSpacingMark)
				{
					sb.Append(s[k]);
				}
			}
			return sb.ToString();
		}
	}
}
