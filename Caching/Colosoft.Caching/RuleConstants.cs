﻿/* 
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

namespace Colosoft.Caching.Queries
{
	/// <summary>
	/// Possíveis constantes
	/// </summary>
	internal enum RuleConstants
	{
		RULE_QUERY_SELECT = 0,
		RULE_QUERY_SELECT_WHERE = 1,
		RULE_EXPRESSION = 2,
		RULE_OREXPR_OR = 3,
		RULE_OREXPR = 4,
		RULE_ANDEXPR_AND = 5,
		RULE_ANDEXPR = 6,
		RULE_UNARYEXPR_NOT = 7,
		RULE_UNARYEXPR = 8,
		RULE_COMPAREEXPR_EQ = 9,
		RULE_COMPAREEXPR_EXCLAMEQ = 10,
		RULE_COMPAREEXPR_EQEQ = 11,
		RULE_COMPAREEXPR_LTGT = 12,
		RULE_COMPAREEXPR_LT = 13,
		RULE_COMPAREEXPR_GT = 14,
		RULE_COMPAREEXPR_LTEQ = 15,
		RULE_COMPAREEXPR_GTEQ = 16,
		RULE_COMPAREEXPR_LIKE_STRINGLITERAL = 17,
		RULE_COMPAREEXPR_LIKE_PARAMETER = 18,
		RULE_COMPAREEXPR_NOT_LIKE_STRINGLITERAL = 19,
		RULE_COMPAREEXPR_NOT_LIKE_PARAMETER = 20,
		RULE_COMPAREEXPR_IN = 21,
		RULE_COMPAREEXPR_NOT_IN = 22,
		RULE_COMPAREEXPR_IS_NULL = 23,
		RULE_COMPAREEXPR_IS_NOT_NULL = 24,
		RULE_COMPAREEXPR_LPARAN_RPARAN = 25,
		RULE_BITWISEEXPR_BITWISEOR = 26,
		RULE_BITWISEEXPR_BITWISEAND = 27,
		RULE_BITWISEEXPR_LPARAN_RPARAN = 28,
		RULE_ATTRIB = 29,
		RULE_VALUE_MINUS = 30,
		RULE_VALUE = 31,
		RULE_VALUE2 = 32,
		RULE_VALUE_TRUE = 33,
		RULE_VALUE_FALSE = 34,
		RULE_VALUE3 = 35,
		RULE_DATE_DATETIME_DOT_NOW = 36,
		RULE_DATE_DATETIME_LPARAN_STRINGLITERAL_RPARAN = 37,
		RULE_STRLITERAL_STRINGLITERAL = 38,
		RULE_STRLITERAL_NULL = 39,
		RULE_STRLITERAL_PARAMETER = 40,
		RULE_NUMLITERAL_INTEGERLITERAL = 41,
		RULE_NUMLITERAL_REALLITERAL = 42,
		RULE_OBJECTTYPE_TIMES = 43,
		RULE_OBJECTTYPE_DOLLARTEXTDOLLAR = 44,
		RULE_OBJECTTYPE = 45,
		RULE_OBJECTTYPE2 = 46,
		RULE_OBJECTATTRIBUTE_IDENTIFIER = 47,
		RULE_PROPERTY_DOT_IDENTIFIER = 48,
		RULE_PROPERTY_IDENTIFIER = 49,
		RULE_TYPEPLUSATTRIBUTE_DOT = 50,
		RULE_AGGREGATEFUNCTION = 51,
		RULE_AGGREGATEFUNCTION2 = 52,
		RULE_AGGREGATEFUNCTION3 = 53,
		RULE_AGGREGATEFUNCTION4 = 54,
		RULE_AGGREGATEFUNCTION5 = 55,
		RULE_SUMFUNCTION_SUMLPARAN_RPARAN = 56,
		RULE_COUNTFUNCTION_COUNTLPARAN_RPARAN = 57,
		RULE_MINFUNCTION_MINLPARAN_RPARAN = 58,
		RULE_MAXFUNCTION_MAXLPARAN_RPARAN = 59,
		RULE_AVERAGEFUNCTION_AVGLPARAN_RPARAN = 60,
		RULE_OBJECTVALUE_KEYWORD_DOT_IDENTIFIER = 61,
		RULE_OBJECTVALUE_KEYWORD_DOT_TAG = 62,
		RULE_OBJECTVALUE = 63,
		RULE_INLIST_LPARAN_RPARAN = 64,
		RULE_LISTTYPE = 65,
		RULE_LISTTYPE2 = 66,
		RULE_LISTTYPE3 = 67,
		RULE_NUMLITERALLIST_COMMA = 68,
		RULE_NUMLITERALLIST_COMMA2 = 69,
		RULE_NUMLITERALLIST = 70,
		RULE_STRLITERALLIST_COMMA = 71,
		RULE_STRLITERALLIST_COMMA2 = 72,
		RULE_STRLITERALLIST = 73,
		RULE_DATELIST_COMMA = 74,
		RULE_DATELIST = 75
	}
}
