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

namespace Colosoft.Web.Mvc.Infrastructure.Implementation.Expressions
{
	/// <summary>
	/// MemberAccessTokenizer.
	/// </summary>
	static class MemberAccessTokenizer
	{
		/// <summary>
		/// Converte o argumento indexado
		/// </summary>
		/// <param name="argument"></param>
		/// <returns></returns>
		private static object ConvertIndexerArgument(string argument)
		{
			int num;
			if(int.TryParse(argument, out num))
			{
				return num;
			}
			if(argument.StartsWith("\"", StringComparison.Ordinal))
			{
				return argument.Trim(new char[] {
					'"'
				});
			}
			if(!argument.StartsWith("'", StringComparison.Ordinal))
			{
				return argument;
			}
			string str = argument.Trim(new char[] {
				'\''
			});
			if(str.Length == 1)
			{
				return str[0];
			}
			return str;
		}

		/// <summary>
		/// Extrai os argumentos do indexador.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		private static IEnumerable<string> ExtractIndexerArguments(string member)
		{
			string iteratorVariable0 = member.TrimEnd(new char[] {
				']'
			});
			foreach (string iteratorVariable1 in iteratorVariable0.Split(new char[] {
				','
			}))
			{
				yield return iteratorVariable1;
			}
		}

		/// <summary>
		/// Recupera os tokens do caminho do membro.
		/// </summary>
		/// <param name="memberPath"></param>
		/// <returns></returns>
		public static IEnumerable<IMemberAccessToken> GetTokens(string memberPath)
		{
			string[] iteratorVariable0 = memberPath.Split(new char[] {
				'.',
				'['
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string iteratorVariable1 in iteratorVariable0)
			{
				IndexerToken iteratorVariable2;
				if(TryParseIndexerToken(iteratorVariable1, out iteratorVariable2))
				{
					yield return iteratorVariable2;
				}
				else
				{
					yield return new PropertyToken(iteratorVariable1);
				}
			}
		}

		/// <summary>
		/// Verifica se o membro possui um indexador válido.
		/// </summary>
		/// <param name="member"></param>
		/// <returns></returns>
		private static bool IsValidIndexer(string member)
		{
			return member.EndsWith("]", StringComparison.Ordinal);
		}

		/// <summary>
		/// Tenta executar o parse para o token do indexador.
		/// </summary>
		/// <param name="member"></param>
		/// <param name="token"></param>
		/// <returns></returns>
		private static bool TryParseIndexerToken(string member, out IndexerToken token)
		{
			token = null;
			if(!IsValidIndexer(member))
			{
				return false;
			}
			List<object> arguments = new List<object>();
			arguments.AddRange(from a in ExtractIndexerArguments(member)
			select ConvertIndexerArgument(a));
			token = new IndexerToken(arguments);
			return true;
		}
	}
}
