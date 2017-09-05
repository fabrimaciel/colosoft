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
	using Colosoft.Text.Jep.ConfigurableParser.Matchers;
	using Colosoft.Text.Jep.ConfigurableParser.Tokens;
	using Colosoft.Text.Jep.DataStructures;
	using Colosoft.Text.Jep.Parser;
	using System;
	using System.Collections.Generic;
	using System.IO;

	public class ConfigurableParserInstance : IParser, IJepComponent
	{
		protected List<ITokenFilter> filters = new List<ITokenFilter>();

		protected List<IGrammarMatcher> g = new List<IGrammarMatcher>();

		[NonSerialized]
		protected JepInstance jep;

		protected List<ITokenMatcher> m = new List<ITokenMatcher>();

		[NonSerialized]
		protected OperatorTokenMatcher otm = new OperatorTokenMatcher();

		[NonSerialized]
		protected SymbolTokenMatcher stm = new SymbolTokenMatcher();

		[NonSerialized]
		protected Tokenizer tk;

		public IGrammarMatcher AddArrayAccessMatcher(string open, string close)
		{
			SymbolToken symbolToken = this.GetSymbolToken(open);
			SymbolToken token2 = this.GetSymbolToken(close);
			IGrammarMatcher item = new ArrayAccessGrammarMatcher(symbolToken, token2);
			this.g.Add(item);
			return item;
		}

		public IGrammarMatcher AddBracketMatcher(string open, string close)
		{
			SymbolToken symbolToken = this.GetSymbolToken(open);
			SymbolToken token2 = this.GetSymbolToken(close);
			IGrammarMatcher item = new RoundBracketGrammarMatcher(symbolToken, token2);
			this.g.Add(item);
			return item;
		}

		public ITokenMatcher AddDoubleQuoteStrings()
		{
			ITokenMatcher item = StringTokenMatcher2.DoubleQuoteStringMatcher();
			this.m.Add(item);
			return item;
		}

		public ITokenMatcher AddExponentNumbers()
		{
			ITokenMatcher item = NumberTokenMatcher.ExponentNumberTokenMatcher();
			this.m.Add(item);
			return item;
		}

		public IGrammarMatcher AddFunctionMatcher(string open, string close, string sep)
		{
			SymbolToken symbolToken = this.GetSymbolToken(open);
			SymbolToken token2 = this.GetSymbolToken(close);
			SymbolToken comma = this.GetSymbolToken(sep);
			IGrammarMatcher item = new FunctionGrammarMatcher(symbolToken, token2, comma);
			this.g.Add(item);
			return item;
		}

		public IGrammarMatcher AddGrammarMatcher(IGrammarMatcher gm)
		{
			this.g.Add(gm);
			return gm;
		}

		public ITokenMatcher AddHashComments()
		{
			ITokenMatcher item = CommentTokenMatcher.HashCommentMatcher();
			this.m.Add(item);
			return item;
		}

		public ITokenMatcher AddIdentifiers()
		{
			ITokenMatcher item = IdentifierTokenMatcher.BasicIndetifierMatcher();
			this.m.Add(item);
			return item;
		}

		public IGrammarMatcher AddListMatcher(string open, string close, string sep)
		{
			SymbolToken symbolToken = this.GetSymbolToken(open);
			SymbolToken token2 = this.GetSymbolToken(close);
			SymbolToken comma = this.GetSymbolToken(sep);
			IGrammarMatcher item = new ListGrammarMatcher(symbolToken, token2, comma);
			this.g.Add(item);
			return item;
		}

		public IGrammarMatcher AddListOrBracketMatcher(string open, string close, string sep)
		{
			SymbolToken symbolToken = this.GetSymbolToken(open);
			SymbolToken token2 = this.GetSymbolToken(close);
			SymbolToken comma = this.GetSymbolToken(sep);
			IGrammarMatcher item = new ListOrBracketGrammarMatcher(symbolToken, token2, comma);
			this.g.Add(item);
			return item;
		}

		public ITokenMatcher AddOperatorTokenMatcher()
		{
			this.m.Add(this.otm);
			return this.otm;
		}

		public ITokenMatcher AddSemiColonTerminator()
		{
			TerminatorTokenMatcher item = TerminatorTokenMatcher.SemiColonTerminatorMatcher();
			this.m.Add(item);
			return item;
		}

		public ITokenMatcher AddSimpleNumbers()
		{
			ITokenMatcher item = NumberTokenMatcher.DefaultNumberTokenMatcher();
			this.m.Add(item);
			return item;
		}

		public ITokenMatcher AddSingleQuoteStrings()
		{
			ITokenMatcher item = StringTokenMatcher2.SingleQuoteStringMatcher();
			this.m.Add(item);
			return item;
		}

		public void AddSlashComments()
		{
			this.m.Add(CommentTokenMatcher.SlashStarCommentMatcher());
			this.m.Add(CommentTokenMatcher.MultiLineSlashStarCommentMatcher());
			this.m.Add(CommentTokenMatcher.SlashSlashCommentMatcher());
		}

		public ITokenMatcher AddSymbols(params string[] symbols)
		{
			this.m.Add(this.stm);
			for(int i = 0; i < symbols.Length; i++)
			{
				SymbolToken t = new SymbolToken(symbols[i]);
				this.stm.Add(t);
			}
			return this.stm;
		}

		public ITokenFilter AddTokenFilter(ITokenFilter tf)
		{
			this.filters.Add(tf);
			return tf;
		}

		public ITokenMatcher AddTokenMatcher(ITokenMatcher tm)
		{
			this.m.Add(tm);
			return tm;
		}

		public ITokenMatcher AddWhiteSpace()
		{
			ITokenMatcher item = WhiteSpaceTokenMatcher.DefaultWhiteSpaceTokenMatcher();
			this.m.Add(item);
			return item;
		}

		public ITokenFilter AddWhiteSpaceCommentFilter()
		{
			ITokenFilter item = new WhiteSpaceCommentFilter();
			this.filters.Add(item);
			return item;
		}

		public INode ContinueParse()
		{
			List<Token> list;
			Label_0000:
			list = this.Scan();
			if(list == null)
			{
				return null;
			}
			IExtendedEnumerator<Token> it = this.Filter(list);
			if(!it.MoveNext())
			{
				goto Label_0000;
			}
			INode node = this.Parse(it);
			if(node == null)
			{
				goto Label_0000;
			}
			return node;
		}

		public IExtendedEnumerator<Token> Filter(List<Token> input)
		{
			IExtendedEnumerator<Token> tokens = new ExtendedEnumerator<Token>(input);
			foreach (ITokenFilter filter in this.filters)
			{
				tokens = filter.Filter(tokens);
			}
			return tokens;
		}

		public List<IGrammarMatcher> GetGrammarMatchers()
		{
			return this.g;
		}

		public IJepComponent GetLightWeightInstance()
		{
			return null;
		}

		public ITokenMatcher GetOperatorTokenMatcher()
		{
			this.m.Add(this.otm);
			return this.otm;
		}

		public SymbolToken GetSymbolToken(string sym)
		{
			return this.stm.GetSymbolToken(sym);
		}

		public SymbolTokenMatcher GetSymbolTokenMatcher()
		{
			return this.stm;
		}

		public List<ITokenFilter> GetTokenFilters()
		{
			return this.filters;
		}

		public List<ITokenMatcher> GetTokenMatchers()
		{
			return this.m;
		}

		public void Init(JepInstance jep)
		{
			this.jep = jep;
			foreach (ITokenMatcher matcher in this.m)
			{
				matcher.Init(jep);
			}
			foreach (ITokenFilter filter in this.filters)
			{
				filter.Init(jep);
			}
			foreach (IGrammarMatcher matcher2 in this.g)
			{
				matcher2.Init(jep);
			}
		}

		public INode Parse(IExtendedEnumerator<Token> it)
		{
			ShuntingYard yard = new ShuntingYard(this.jep, this.g);
			return yard.Parse(it);
		}

		public INode Parse(TextReader stream)
		{
			List<Token> input = this.Scan(stream);
			IExtendedEnumerator<Token> it = this.Filter(input);
			return this.Parse(it);
		}

		public void Restart(TextReader stream)
		{
			this.tk = new Tokenizer(stream, this.m);
		}

		public List<Token> Scan()
		{
			return this.tk.Scan();
		}

		public List<Token> Scan(TextReader stream)
		{
			this.Restart(stream);
			return this.tk.Scan();
		}

		public void SetImplicitMultiplicationSymbols(params string[] symbols)
		{
			for(int i = 0; i < symbols.Length; i++)
			{
				this.GetSymbolToken(symbols[i]).SetRhsImpMul(true);
			}
		}
	}
}
