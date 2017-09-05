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

namespace Colosoft.Text.Jep.ConfigurableParser
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using Colosoft.Text.Jep.DataStructures;
	using System;
	using System.Collections;
	using System.Collections.Generic;

	[Serializable]
	public class WhiteSpaceCommentFilter : ITokenFilter
	{
		public IExtendedEnumerator<Token> Filter(IExtendedEnumerator<Token> tokens)
		{
			return new WSCEnumerator(tokens);
		}

		public void Init(JepInstance j)
		{
		}

		[Serializable]
		private class WSCEnumerator : IExtendedEnumerator<Token>, IEnumerator<Token>, IDisposable, IEnumerator
		{
			private Token _curTok;

			private IExtendedEnumerator<Token> input;

			public WSCEnumerator(IExtendedEnumerator<Token> input)
			{
				this.input = input;
				this.Grab();
			}

			public void Dispose()
			{
			}

			private void Grab()
			{
				while (this.input.HasNext())
				{
					Token token = this.input.Next();
					if(!token.IsComment() && !token.IsWhiteSpace())
					{
						this._curTok = token;
						return;
					}
				}
				this._curTok = null;
			}

			public bool HasNext()
			{
				return (this._curTok != null);
			}

			public bool MoveNext()
			{
				return this.HasNext();
			}

			public Token Next()
			{
				Token token = this._curTok;
				this.Grab();
				return token;
			}

			public void Reset()
			{
				throw new NotSupportedException("Reset function is not supported in WSCEnumerator");
			}

			public Token Current
			{
				get
				{
					return this._curTok;
				}
			}

			object IEnumerator.Current
			{
				get
				{
					return this.Current;
				}
			}
		}
	}
}
