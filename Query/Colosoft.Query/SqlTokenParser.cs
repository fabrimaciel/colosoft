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
using Colosoft.Text.InterpreterExpression;

namespace Colosoft.Query.Parser
{
	/// <summary>
	/// Implementação do <see cref="ITokenParser"/> para comandos SQL.
	/// </summary>
	public class SqlTokenParser : DefaultTokenParser
	{
		/// <summary>
		/// Palavras chave do sql.
		/// </summary>
		internal static SortedList<string, SqlTokenID> Keywords = new SortedList<string, SqlTokenID>();

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static SqlTokenParser()
		{
			Keywords.Add("absolute", SqlTokenID.kAbsolute);
			Keywords.Add("action", SqlTokenID.kAction);
			Keywords.Add("add", SqlTokenID.kAdd);
			Keywords.Add("all", SqlTokenID.kAll);
			Keywords.Add("allocate", SqlTokenID.kAllocate);
			Keywords.Add("alter", SqlTokenID.kAlter);
			Keywords.Add("and", SqlTokenID.kAnd);
			Keywords.Add("any", SqlTokenID.kAny);
			Keywords.Add("are", SqlTokenID.kAre);
			Keywords.Add("as", SqlTokenID.kAs);
			Keywords.Add("asc", SqlTokenID.kAsc);
			Keywords.Add("assertion", SqlTokenID.kAssertion);
			Keywords.Add("at", SqlTokenID.kAt);
			Keywords.Add("authorization", SqlTokenID.kAuthorization);
			Keywords.Add("avg", SqlTokenID.kAvg);
			Keywords.Add("begin", SqlTokenID.kBegin);
			Keywords.Add("between", SqlTokenID.kBetween);
			Keywords.Add("bit", SqlTokenID.kBit);
			Keywords.Add("bit_length", SqlTokenID.kBit_Length);
			Keywords.Add("both", SqlTokenID.kBoth);
			Keywords.Add("by", SqlTokenID.kBy);
			Keywords.Add("cascade", SqlTokenID.kCascade);
			Keywords.Add("cascaded", SqlTokenID.kCascaded);
			Keywords.Add("case", SqlTokenID.kCase);
			Keywords.Add("cast", SqlTokenID.kCast);
			Keywords.Add("catalog", SqlTokenID.kCatalog);
			Keywords.Add("char", SqlTokenID.kChar);
			Keywords.Add("char_length", SqlTokenID.kChar_Length);
			Keywords.Add("character", SqlTokenID.kCharacter);
			Keywords.Add("character_length", SqlTokenID.kCharacter_Length);
			Keywords.Add("check", SqlTokenID.kCheck);
			Keywords.Add("close", SqlTokenID.kClose);
			Keywords.Add("coalesce", SqlTokenID.kCoalesce);
			Keywords.Add("collate", SqlTokenID.kCollate);
			Keywords.Add("collation", SqlTokenID.kCollation);
			Keywords.Add("column", SqlTokenID.kColumn);
			Keywords.Add("commit", SqlTokenID.kCommit);
			Keywords.Add("connect", SqlTokenID.kConnect);
			Keywords.Add("connection", SqlTokenID.kConnection);
			Keywords.Add("constraint", SqlTokenID.kConstraint);
			Keywords.Add("constraints", SqlTokenID.kConstraints);
			Keywords.Add("continue", SqlTokenID.kContinue);
			Keywords.Add("convert", SqlTokenID.kConvert);
			Keywords.Add("corresponding", SqlTokenID.kCorresponding);
			Keywords.Add("count", SqlTokenID.kCount);
			Keywords.Add("create", SqlTokenID.kCreate);
			Keywords.Add("cross", SqlTokenID.kCross);
			Keywords.Add("current", SqlTokenID.kCurrent);
			Keywords.Add("current_date", SqlTokenID.kCurrent_Date);
			Keywords.Add("current_time", SqlTokenID.kCurrent_Time);
			Keywords.Add("current_timestamp", SqlTokenID.kCurrent_Timestamp);
			Keywords.Add("current_user", SqlTokenID.kCurrent_User);
			Keywords.Add("cursor", SqlTokenID.kCursor);
			Keywords.Add("date", SqlTokenID.kDate);
			Keywords.Add("day", SqlTokenID.kDay);
			Keywords.Add("deallocate", SqlTokenID.kDeallocate);
			Keywords.Add("dec", SqlTokenID.kDec);
			Keywords.Add("decimal", SqlTokenID.kDecimal);
			Keywords.Add("declare", SqlTokenID.kDeclare);
			Keywords.Add("default", SqlTokenID.kDefault);
			Keywords.Add("deferrable", SqlTokenID.kDeferrable);
			Keywords.Add("deferred", SqlTokenID.kDeferred);
			Keywords.Add("delete", SqlTokenID.kDelete);
			Keywords.Add("desc", SqlTokenID.kDesc);
			Keywords.Add("describe", SqlTokenID.kDescribe);
			Keywords.Add("descriptor", SqlTokenID.kDescriptor);
			Keywords.Add("diagnostics", SqlTokenID.kDiagnostics);
			Keywords.Add("disconnect", SqlTokenID.kDisconnect);
			Keywords.Add("distinct", SqlTokenID.kDistinct);
			Keywords.Add("double", SqlTokenID.kDouble);
			Keywords.Add("drop", SqlTokenID.kDrop);
			Keywords.Add("else", SqlTokenID.kElse);
			Keywords.Add("end", SqlTokenID.kEnd);
			Keywords.Add("end_exec", SqlTokenID.kEnd_Exec);
			Keywords.Add("escape", SqlTokenID.kEscape);
			Keywords.Add("except", SqlTokenID.kExcept);
			Keywords.Add("exception", SqlTokenID.kException);
			Keywords.Add("exec", SqlTokenID.kExec);
			Keywords.Add("execute", SqlTokenID.kExecute);
			Keywords.Add("exists", SqlTokenID.kExists);
			Keywords.Add("external", SqlTokenID.kExternal);
			Keywords.Add("extract", SqlTokenID.kExtract);
			Keywords.Add("false", SqlTokenID.kFalse);
			Keywords.Add("fetch", SqlTokenID.kFetch);
			Keywords.Add("first", SqlTokenID.kFirst);
			Keywords.Add("float", SqlTokenID.kFloat);
			Keywords.Add("for", SqlTokenID.kFor);
			Keywords.Add("foreign", SqlTokenID.kForeign);
			Keywords.Add("fortran", SqlTokenID.kFortran);
			Keywords.Add("found", SqlTokenID.kFound);
			Keywords.Add("from", SqlTokenID.kFrom);
			Keywords.Add("full", SqlTokenID.kFull);
			Keywords.Add("get", SqlTokenID.kGet);
			Keywords.Add("global", SqlTokenID.kGlobal);
			Keywords.Add("go", SqlTokenID.kGo);
			Keywords.Add("goto", SqlTokenID.kGoto);
			Keywords.Add("grant", SqlTokenID.kGrant);
			Keywords.Add("group", SqlTokenID.kGroup);
			Keywords.Add("having", SqlTokenID.kHaving);
			Keywords.Add("hour", SqlTokenID.kHour);
			Keywords.Add("identity", SqlTokenID.kIdentity);
			Keywords.Add("immediate", SqlTokenID.kImmediate);
			Keywords.Add("in", SqlTokenID.kIn);
			Keywords.Add("include", SqlTokenID.kInclude);
			Keywords.Add("index", SqlTokenID.kIndex);
			Keywords.Add("indicator", SqlTokenID.kIndicator);
			Keywords.Add("initially", SqlTokenID.kInitially);
			Keywords.Add("inner", SqlTokenID.kInner);
			Keywords.Add("input", SqlTokenID.kInput);
			Keywords.Add("insensitive", SqlTokenID.kInsensitive);
			Keywords.Add("insert", SqlTokenID.kInsert);
			Keywords.Add("int", SqlTokenID.kInt);
			Keywords.Add("integer", SqlTokenID.kInteger);
			Keywords.Add("intersect", SqlTokenID.kIntersect);
			Keywords.Add("interval", SqlTokenID.kInterval);
			Keywords.Add("into", SqlTokenID.kInto);
			Keywords.Add("is", SqlTokenID.kIs);
			Keywords.Add("isolation", SqlTokenID.kIsolation);
			Keywords.Add("join", SqlTokenID.kJoin);
			Keywords.Add("key", SqlTokenID.kKey);
			Keywords.Add("language", SqlTokenID.kLanguage);
			Keywords.Add("last", SqlTokenID.kLast);
			Keywords.Add("leading", SqlTokenID.kLeading);
			Keywords.Add("left", SqlTokenID.kLeft);
			Keywords.Add("level", SqlTokenID.kLevel);
			Keywords.Add("like", SqlTokenID.kLike);
			Keywords.Add("local", SqlTokenID.kLocal);
			Keywords.Add("lower", SqlTokenID.kLower);
			Keywords.Add("match", SqlTokenID.kMatch);
			Keywords.Add("max", SqlTokenID.kMax);
			Keywords.Add("min", SqlTokenID.kMin);
			Keywords.Add("minute", SqlTokenID.kMinute);
			Keywords.Add("module", SqlTokenID.kModule);
			Keywords.Add("month", SqlTokenID.kMonth);
			Keywords.Add("names", SqlTokenID.kNames);
			Keywords.Add("national", SqlTokenID.kNational);
			Keywords.Add("natural", SqlTokenID.kNatural);
			Keywords.Add("nchar", SqlTokenID.kNChar);
			Keywords.Add("next", SqlTokenID.kNext);
			Keywords.Add("no", SqlTokenID.kNo);
			Keywords.Add("none", SqlTokenID.kNone);
			Keywords.Add("not", SqlTokenID.kNot);
			Keywords.Add("null", SqlTokenID.kNull);
			Keywords.Add("nullif", SqlTokenID.kNullIf);
			Keywords.Add("nulls", SqlTokenID.kNulls);
			Keywords.Add("numeric", SqlTokenID.kNumeric);
			Keywords.Add("octect_length", SqlTokenID.kOctect_Length);
			Keywords.Add("of", SqlTokenID.kOf);
			Keywords.Add("on", SqlTokenID.kOn);
			Keywords.Add("only", SqlTokenID.kOnly);
			Keywords.Add("open", SqlTokenID.kOpen);
			Keywords.Add("option", SqlTokenID.kOption);
			Keywords.Add("or", SqlTokenID.kOr);
			Keywords.Add("order", SqlTokenID.kOrder);
			Keywords.Add("outer", SqlTokenID.kOuter);
			Keywords.Add("output", SqlTokenID.kOutput);
			Keywords.Add("overlaps", SqlTokenID.kOverlaps);
			Keywords.Add("pad", SqlTokenID.kPad);
			Keywords.Add("partial", SqlTokenID.kPartial);
			Keywords.Add("pascal", SqlTokenID.kPascal);
			Keywords.Add("percent", SqlTokenID.kPercent);
			Keywords.Add("position", SqlTokenID.kPosition);
			Keywords.Add("precision", SqlTokenID.kPrecision);
			Keywords.Add("prepare", SqlTokenID.kPrepare);
			Keywords.Add("preserve", SqlTokenID.kPreserve);
			Keywords.Add("primary", SqlTokenID.kPrimary);
			Keywords.Add("prior", SqlTokenID.kPrior);
			Keywords.Add("privileges", SqlTokenID.kPrivileges);
			Keywords.Add("procedure", SqlTokenID.kProcedure);
			Keywords.Add("public", SqlTokenID.kPublic);
			Keywords.Add("read", SqlTokenID.kRead);
			Keywords.Add("real", SqlTokenID.kReal);
			Keywords.Add("references", SqlTokenID.kReferences);
			Keywords.Add("relative", SqlTokenID.kRelative);
			Keywords.Add("restrict", SqlTokenID.kRestrict);
			Keywords.Add("revoke", SqlTokenID.kRevoke);
			Keywords.Add("right", SqlTokenID.kRight);
			Keywords.Add("rollback", SqlTokenID.kRollback);
			Keywords.Add("rows", SqlTokenID.kRows);
			Keywords.Add("schema", SqlTokenID.kSchema);
			Keywords.Add("scroll", SqlTokenID.kScroll);
			Keywords.Add("second", SqlTokenID.kSecond);
			Keywords.Add("select", SqlTokenID.kSelect);
			Keywords.Add("set", SqlTokenID.kSet);
			Keywords.Add("smallint", SqlTokenID.kSmallint);
			Keywords.Add("some", SqlTokenID.kSome);
			Keywords.Add("space", SqlTokenID.kSpace);
			Keywords.Add("sql", SqlTokenID.kSql);
			Keywords.Add("sqlca", SqlTokenID.kSqlCa);
			Keywords.Add("sqlcode", SqlTokenID.kSqlCode);
			Keywords.Add("sqlerror", SqlTokenID.kSqlError);
			Keywords.Add("sqlstate", SqlTokenID.kSqlState);
			Keywords.Add("sqlwarning", SqlTokenID.kSqlWarning);
			Keywords.Add("substring", SqlTokenID.kSubstring);
			Keywords.Add("sum", SqlTokenID.kSum);
			Keywords.Add("system_user", SqlTokenID.kSystem_User);
			Keywords.Add("table", SqlTokenID.kTable);
			Keywords.Add("temporary", SqlTokenID.kTemporary);
			Keywords.Add("then", SqlTokenID.kThen);
			Keywords.Add("time", SqlTokenID.kTime);
			Keywords.Add("timestamp", SqlTokenID.kTimestamp);
			Keywords.Add("timezone_hour", SqlTokenID.kTimezone_Hour);
			Keywords.Add("timezone_minute", SqlTokenID.kTimezone_Minute);
			Keywords.Add("to", SqlTokenID.kTo);
			Keywords.Add("top", SqlTokenID.kTop);
			Keywords.Add("trailing", SqlTokenID.kTrailing);
			Keywords.Add("transaction", SqlTokenID.kTransaction);
			Keywords.Add("translate", SqlTokenID.kTranslate);
			Keywords.Add("translation", SqlTokenID.kTranslation);
			Keywords.Add("trim", SqlTokenID.kTrim);
			Keywords.Add("true", SqlTokenID.kTrue);
			Keywords.Add("union", SqlTokenID.kUnion);
			Keywords.Add("unique", SqlTokenID.kUnique);
			Keywords.Add("unknown", SqlTokenID.kUnknown);
			Keywords.Add("update", SqlTokenID.kUpdate);
			Keywords.Add("upper", SqlTokenID.kUpper);
			Keywords.Add("usage", SqlTokenID.kUsage);
			Keywords.Add("using", SqlTokenID.kUsing);
			Keywords.Add("value", SqlTokenID.kValue);
			Keywords.Add("values", SqlTokenID.kValues);
			Keywords.Add("varchar", SqlTokenID.kVarchar);
			Keywords.Add("varying", SqlTokenID.kVarying);
			Keywords.Add("view", SqlTokenID.kView);
			Keywords.Add("when", SqlTokenID.kWhen);
			Keywords.Add("whenever", SqlTokenID.kWhenever);
			Keywords.Add("where", SqlTokenID.kWhere);
			Keywords.Add("with", SqlTokenID.kWith);
			Keywords.Add("work", SqlTokenID.kWork);
			Keywords.Add("write", SqlTokenID.kWrite);
			Keywords.Add("year", SqlTokenID.kYear);
			Keywords.Add("zone", SqlTokenID.kZone);
		}

		/// <summary>
		/// Recupera o identificador do token.
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		protected override TokenID ParseTokenID(char character)
		{
			var token = base.ParseTokenID(character);
			if(token == TokenID.InvalidExpression)
			{
				SqlTokenID sqlToken = 0;
				if(Keywords.TryGetValue(char.ToLower(character).ToString(), out sqlToken))
					return (TokenID)(int)sqlToken;
			}
			return token;
		}

		/// <summary>
		/// Recupera o identificador do token.
		/// </summary>
		/// <param name="term"></param>
		/// <returns></returns>
		protected override TokenID ParseTokenID(string term)
		{
			if(term == "<>")
				return TokenID.NotEqual;
			var token = base.ParseTokenID(term);
			if(token == TokenID.InvalidExpression && term != null)
			{
				SqlTokenID sqlToken = 0;
				if(Keywords.TryGetValue(term.ToLower(), out sqlToken))
					return (TokenID)(int)sqlToken;
			}
			return token;
		}

		/// <summary>
		/// Recupera o termo com base no token.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public override string GetTerm(int token)
		{
			var term = base.GetTerm(token);
			if(term == null)
			{
				var index = Keywords.IndexOfValue((SqlTokenID)token);
				if(index >= 0)
					return Keywords.Keys[index];
				return null;
			}
			return term;
		}

		/// <summary>
		/// Verifica se o token informado é uma função Ansi do SQL.
		/// </summary>
		/// <param name="token"></param>
		/// <returns></returns>
		public static bool IsSqlAnsiFunction(int token)
		{
			var token2 = (SqlTokenID)token;
			switch(token2)
			{
			case SqlTokenID.kAvg:
			case SqlTokenID.kSum:
			case SqlTokenID.kCount:
			case SqlTokenID.kMin:
			case SqlTokenID.kMax:
			case SqlTokenID.kSecond:
			case SqlTokenID.kMinute:
			case SqlTokenID.kHour:
			case SqlTokenID.kDay:
			case SqlTokenID.kMonth:
			case SqlTokenID.kYear:
			case SqlTokenID.kDate:
			case SqlTokenID.kLower:
			case SqlTokenID.kConvert:
			case SqlTokenID.kSubstring:
			case SqlTokenID.kTranslate:
			case SqlTokenID.kTrim:
			case SqlTokenID.kUpper:
			case SqlTokenID.kAbsolute:
			case SqlTokenID.kCast:
				return true;
			}
			return false;
		}
	}
}
