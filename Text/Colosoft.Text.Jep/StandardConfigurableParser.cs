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
	using System;

	public class StandardConfigurableParser : ConfigurableParserInstance
	{
		public StandardConfigurableParser()
		{
			base.AddHashComments();
			base.AddSlashComments();
			base.AddSingleQuoteStrings();
			base.AddDoubleQuoteStrings();
			base.AddWhiteSpace();
			base.AddExponentNumbers();
			base.AddOperatorTokenMatcher();
			base.AddSymbols(new string[] {
				"(",
				")",
				"[",
				"]",
				","
			});
			base.SetImplicitMultiplicationSymbols(new string[] {
				"(",
				"["
			});
			base.AddIdentifiers();
			base.AddSemiColonTerminator();
			base.AddWhiteSpaceCommentFilter();
			base.AddBracketMatcher("(", ")");
			base.AddFunctionMatcher("(", ")", ",");
			base.AddListMatcher("[", "]", ",");
			base.AddArrayAccessMatcher("[", "]");
		}
	}
}
