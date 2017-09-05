using Colosoft.Data.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Query.Database.Oracle{
	public class OracleQueryParser : SqlQueryParser	{
		private StringBuilder _sqlQuery;
		private Dictionary<string, QueryInfo> _queryParameters = new Dictionary<string, QueryInfo> ();
		private ITakeParametersParser _takeParametersParser;
		private Colosoft.Threading.SimpleMonitor _columnNameNoCheckMonitor = new Threading.SimpleMonitor ();
		private object _objLock = new object ();
		public override string Text {
			get {
				return _sqlQuery.ToString ();
			}
		}
		public OracleQueryParser (IQueryTranslator a, ITypeSchema b, ITakeParametersParser c) : base (a, b)		{
			_takeParametersParser = c;
		}
		private void BuildText (SqlQueryPart a)		{
			EntityAliasDictionary = null;
			if (a.HasFlag (SqlQueryPart.Select))
				SelectParser ();
			if (a.HasFlag (SqlQueryPart.Joins) && Query.Joins.Length > 0)
				JoinParser ();
			if (a.HasFlag (SqlQueryPart.Where))
				WhereParser ();
			if (a.HasFlag (SqlQueryPart.GroupBy) && Query.GroupBy != null)
				GroupByParser ();
			if (a.HasFlag (SqlQueryPart.Having) && Query.Having != null)
				HavingParser ();
			if (a.HasFlag (SqlQueryPart.OrderBy) && Query.Sort != null)
				SortParser ();
			if (Query.Unions != null) {
				var b = Query;
				foreach (var union in Query.Unions) {
					Append (string.Format (" UNION{0} ", union.All ? " ALL" : null));
					Query = union.Query;
					BuildText (SqlQueryPart.All);
				}
				Query = b;
			}
			if (UseTakeParameter && Query.TakeParameters != null && (Query.TakeParameters.Skip > 0 || Query.TakeParameters.Take > 0)) {
			}
		}
		public override string GetText ()		{
			return GetText (SqlQueryPart.All);
		}
		public override string GetText (SqlQueryPart a)		{
			_sqlQuery = new StringBuilder ();
			_queryParameters.Clear ();
			BuildText (a);
			return _sqlQuery.ToString ();
		}
		private OracleQueryParser SelectParser ()		{
			if (Query.IsSelectDistinct)
				Append ("SELECT DISTINCT ");
			else
				Append ("SELECT ");
			if (Query.Entities == null)
				throw new InvalidOperationException (string.Format ("Not found entities in query '{0}'", Query));
			ITypeMetadata a = null;
			for (int b = 0; b < Query.Entities.Length; b++) {
				if (string.IsNullOrEmpty (Query.Entities [b].FullName))
					continue;
				ITypeMetadata c = null;
				if (!Query.IgnoreTypeSchema) {
					c = TypeSchema.GetTypeMetadata (Query.Entities [b].FullName);
					if (c == null)
						throw new InvalidOperationException (ResourceMessageFormatter.Create (() => Properties.Resources.InvalidOperationException_TypeMetadataNotFoundByFullName, Query.Entities [b].FullName).Format ());
				}
				if (b == 0) {
					a = c;
					if (string.IsNullOrEmpty (Query.Entities [b].Alias))
						Query.Entities [b].Alias = "main";
				}
				else if (string.IsNullOrEmpty (Query.Entities [b].Alias))
					Query.Entities [b].Alias = "main" + b.ToString ();
			}
			bool d = false;
			EntityInfo e = Query.Entities [0];
			if (!string.IsNullOrEmpty (e.FullName)) {
				if (Query.Projection == null) {
					if (a != null && a.IsVersioned)
						Query.HasRowVersion = true;
					var f = a != null ? a.Where (g => g.Direction == Data.Schema.DirectionParameter.Input || g.Direction == Data.Schema.DirectionParameter.InputOutput).ToArray () : new IPropertyMetadata[0];
					if (f.Length == 0)
						throw new InvalidOperationException (ResourceMessageFormatter.Create (() => Properties.Resources.InvalidOperationException_NotFoundPropertiesForTypeMetadata, e.FullName).Format ());
					Query.Projection = new Projection ();
					for (var b = 0; b < f.Length; b++) {
						var h = f [b];
						if (h.Name == DataAccessConstants.RowVersionPropertyName)
							d = true;
						Query.Projection.Add (new ProjectionEntry (string.Format ("{0}.{1}", e.Alias, h.Name), h.Name));
						var j = Translator.GetName (e, h.Name, Query.IgnoreTypeSchema);
						var k = j as TranslatedColumnName;
						if (k != null) {
							if (!string.IsNullOrEmpty (k.TableAlias))
								AppendQuoteExpression (k.TableAlias).Append ('.');
							AppendQuoteExpression (k.Name);
						}
						if (h.ColumnName != h.Name) {
							Append (" AS ").AppendQuoteExpression (FormatProjectionAlias (h.Name));
						}
						if (b + 1 < f.Length)
							Append (',');
					}
					Append (' ');
				}
				else
					ProjectionParser ();
				if (Query.HasRowVersion && !d) {
					Append (", ").Append ("CAST(ORA_ROWSCN AS NUMBER(18,0)) AS \"").Append (DataAccessConstants.RowVersionColumnName.ToUpper ()).Append ("\" ");
				}
				var l = Translator.GetName (e, Query.IgnoreTypeSchema);
				if (l == null)
					throw new InvalidOperationException (string.Format ("Not found table name for entity '{0}'", e.FullName));
				Append ("FROM ").AppendTranslatedName (l).Append (' ').AppendQuoteExpression (e.Alias).Append (' ');
			}
			else {
				var m = new OracleQueryParser (Translator, TypeSchema, _takeParametersParser) {
					Query = e.SubQuery,
					UseTakeParameter = true
				};
				var n = m.GetText ();
				if (Query.Projection == null) {
					Query.Projection = new Projection ();
					foreach (var column in e.SubQuery.Projection)
						Query.Projection.Add (new ProjectionEntry (!string.IsNullOrEmpty (e.Alias) ? string.Format ("{0}.{1}", e.Alias, string.IsNullOrEmpty (column.Alias) ? (string.IsNullOrEmpty (column.GetColumnInfo ().Alias) ? column.GetColumnInfo ().Name : column.GetColumnInfo ().Alias) : column.Alias) : column.Alias, column.Alias));
				}
				ProjectionParser ();
				Append ("FROM (").Append (n).Append (") ").AppendQuoteExpression (e.Alias);
			}
			return this;
		}
		private OracleQueryParser JoinParser ()		{
			foreach (var join in Query.Joins) {
				switch (join.Type) {
				case JoinType.Inner:
					Append ("INNER JOIN ");
					break;
				case JoinType.Right:
					Append ("RIGHT JOIN ");
					break;
				case JoinType.Left:
					Append ("LEFT JOIN ");
					break;
				case JoinType.Cross:
					Append ("CROSS JOIN ");
					break;
				default:
					throw new InvalidOperationException ("Invalid join type");
				}
				var a = GetEntity (join.Right);
				if (a.SubQuery != null) {
					Format (a.SubQuery);
					Append (' ');
				}
				else {
					var b = Translator.GetName (a, Query.IgnoreTypeSchema);
					if (b == null)
						throw new InvalidOperationException (string.Format ("Not found table name for entity '{0}'", a.FullName));
					AppendTranslatedName (b).Append (' ');
				}
				AppendQuoteExpression (a.Alias);
				if (join.Conditional != null)
					Append (" ON (").Format (join.Conditional).Append (')');
			}
			return this;
		}
		private OracleQueryParser WhereParser ()		{
			if (Query.WhereClause.ConditionalsCount > 0)
				Append (" WHERE ").Format (Query.WhereClause);
			return this;
		}
		private OracleQueryParser HavingParser ()		{
			Append (" HAVING ");
			_columnNameNoCheckMonitor.Enter ();
			using (_columnNameNoCheckMonitor)
				Format (Query.Having);
			return this;
		}
		private OracleQueryParser GroupByParser ()		{
			Append (" GROUP BY ");
			int a = 1;
			foreach (var groupby in Query.GroupBy) {
				Format (groupby);
				if (a != Query.GroupBy.Count) {
					Append (',');
					a++;
				}
			}
			return this;
		}
		private OracleQueryParser SortParser ()		{
			Append (" ORDER BY ");
			int a = 1;
			foreach (var sort in Query.Sort) {
				Format (sort);
				if (a != Query.Sort.Count) {
					Append (',');
					a++;
				}
			}
			return this;
		}
		private OracleQueryParser ProjectionParser ()		{
			int a = 1;
			foreach (var proj in Query.Projection) {
				Format (proj);
				if (a != Query.Projection.Count) {
					Append (',');
					a++;
				}
				else
					Append (' ');
			}
			return this;
		}
		protected override string FormatProjectionAlias (string a)		{
			return !string.IsNullOrEmpty (a) && a.Length > 30 ? a.Substring (0, 30) : a;
		}
		private OracleQueryParser Format (ProjectionEntry a)		{
			Format (a.Term);
			var b = a.GetColumnInfo ();
			if (!string.IsNullOrEmpty (a.Alias))
				Append (" AS ").AppendQuoteExpression (FormatProjectionAlias (a.Alias));
			else if (b != null)
				Append (" AS ").AppendQuoteExpression (FormatProjectionAlias (b.Name));
			return this;
		}
		private OracleQueryParser Format (ConditionalTerm a, bool b = false)		{
			if (a is Constant)
				Append (((Constant)a).Text);
			else if (a is Column)
				Format ((Column)a);
			else if (a is Variable) {
				var c = (Variable)a;
				var d = Query != null ? Query.Parameters.Where (e => e.Name == c.Name).FirstOrDefault () : null;
				QueryInfo f = null;
				if (d != null && (d.Value is QueryInfo || d.Value is Queryable)) {
					Query.Parameters.Remove (d);
					var g = d.Value;
					if (g is Queryable)
						g = ((Queryable)g).CreateQueryInfo ();
					try {
						_queryParameters.Add (d.Name, (QueryInfo)g);
					}
					catch (ArgumentException) {
						throw new ConditionalParserException (ResourceMessageFormatter.Create (() => Properties.Resources.ConditionalParserException_DuplicateParameter, d.Name).Format ());
					}
					Format (new QueryTerm ((QueryInfo)g), b);
				}
				else if (_queryParameters.TryGetValue (c.Name, out f))
					Format (new QueryTerm (f), b);
				else
					Append (((Variable)a).Name);
			}
			else if (a is ValuesArray) {
				var h = (ValuesArray)a;
				if (h.Values != null && h.Values.Length == 1 && h.Values [0] is Variable) {
					var c = (Variable)h.Values [0];
					var d = Query != null ? Query.Parameters.Where (e => e.Name == c.Name).FirstOrDefault () : null;
					QueryInfo f = null;
					if (d != null && (d.Value is QueryInfo || d.Value is Queryable)) {
						Query.Parameters.Remove (d);
						var g = d.Value;
						if (g is Queryable)
							g = ((Queryable)g).CreateQueryInfo ();
						try {
							_queryParameters.Add (d.Name, (QueryInfo)g);
						}
						catch (ArgumentException) {
							throw new ConditionalParserException (ResourceMessageFormatter.Create (() => Properties.Resources.ConditionalParserException_DuplicateParameter, d.Name).Format ());
						}
						Format (new QueryTerm ((QueryInfo)g), b);
						h = null;
					}
					else if (_queryParameters.TryGetValue (c.Name, out f)) {
						Format (new QueryTerm (f), b);
						h = null;
					}
				}
				if (h != null) {
					Append ('(');
					var j = false;
					foreach (var i in h.Values) {
						if (j)
							Append (", ");
						else
							j = true;
						if (i != null)
							Format (i, b);
						else
							Append ("NULL");
					}
					Append (')');
				}
			}
			else if (a is ConditionalContainer)
				Format ((ConditionalContainer)a);
			else if (a is QueryTerm)
				Format ((QueryTerm)a, b);
			else if (a is FunctionCall) {
				var e = (FunctionCall)a;
				var k = e.Call.ToString ().Replace ("'", String.Empty);
				if (StringComparer.InvariantCultureIgnoreCase.Equals (k, "CAST") && e.Parameters.Length == 2) {
					Append (k).Append ('(').Format (e.Parameters [0]).Append (" AS ").Format (e.Parameters [1]).Append (')');
				}
				else if (StringComparer.InvariantCultureIgnoreCase.Equals (k, "DATEFORMAT") && e.Parameters.Length == 2) {
					throw new Exception ("DATEFORMAT function not implemented for this database");
				}
				else {
					if (StringComparer.InvariantCultureIgnoreCase.Equals (k, "ISNULL"))
						k = "NVL";
					Append (k);
					Append ('(');
					if (e.Options.HasFlag (FunctionCallOptions.Distinct))
						Append ("DISTINCT ");
					var j = false;
					foreach (var paramTerm in e.Parameters) {
						if (j)
							Append (", ");
						else
							j = true;
						if (paramTerm != null)
							Format (paramTerm, true);
						else
							Append ("NULL");
					}
					Append (')');
				}
			}
			else if (a is Formula)
				Format ((Formula)a);
			else if (a is MinusTerm) {
				Append ("-");
				Format (((MinusTerm)a).Term);
			}
			else if (a is Conditional)
				Format ((Conditional)a);
			else if (a is CaseConditional)
				Format ((CaseConditional)a);
			else
				throw new NotSupportedException (ResourceMessageFormatter.Create (() => Properties.Resources.NotSupportedException_TypeOfConditionalTermNotSupported, a.GetType ().ToString ()).Format ());
			return this;
		}
		private OracleQueryParser Format (CaseConditional a)		{
			Append ("CASE");
			if (a.InputExpression != null)
				Append (' ').Format (a.InputExpression);
			foreach (var i in a.WhenExpressions)
				Append (" WHEN ").Format (i.Expression).Append (" THEN ").Format (i.ResultExpression);
			if (a.ElseResultExpression != null)
				Append (" ELSE ").Format (a.ElseResultExpression);
			Append (" END ");
			return this;
		}
		private OracleQueryParser Format (Column a)		{
			Append (GetColumnName (a));
			return this;
		}
		private OracleQueryParser Format (Conditional a)		{
			var b = a.Operator.Op;
			if (b == "&" || b == "|") {
				if (b == "&")
					Append ("BITAND(");
				else
					Append ("BITOR(");
				if (!(a.Left is Query.Variable) || !FormatConditionalVariable ((Query.Variable)a.Left))
					Format (a.Left);
				Append (", ");
				if (!(a.Right is Query.Variable) || !FormatConditionalVariable ((Query.Variable)a.Right))
					Format (a.Right);
				Append (")");
			}
			else {
				Format ((ConditionalTerm)a.Left);
				if (a.Operator != null && (StringComparer.InvariantCultureIgnoreCase.Equals (a.Operator.Op, "EXISTS") || StringComparer.InvariantCultureIgnoreCase.Equals (a.Operator.Op, "NOT EXISTS")))
					return this;
				var c = GetSqlOperator (a.Operator.Op);
				if (a.Right is Variable && (c == "=" || c == "<>")) {
					var d = (Variable)a.Right;
					var e = Query != null ? Query.Parameters.Where (f => f.Name == d.Name).FirstOrDefault () : null;
					if (e != null && (e.Value == null || (e.Value is string && string.IsNullOrEmpty ((string)e.Value)))) {
						if (c == "=")
							Append (" IS NULL");
						else
							Append (" IS NOT NULL");
						return this;
					}
				}
				if (!string.IsNullOrEmpty (a.Operator.Op))
					Append (' ').AppendSqlOperator (a.Operator.Op).Append (' ');
				else
					Append (' ');
				if (a.Right != null)
					Format ((ConditionalTerm)a.Right);
				return this;
			}
			return this;
		}
		private bool FormatConditionalVariable (Variable a)		{
			var b = Query.Parameters.Where (c => c.Name == a.Name).FirstOrDefault ();
			if (b != null && b.Value != null) {
				if (b.Value is string)
					Append ("'").Append (b.Value.ToString ()).Append ("'");
				else
					Append (b.Value.ToString ());
				return true;
			}
			return false;
		}
		private OracleQueryParser Format (ConditionalContainer a)		{
			int b = 1;
			Append ("(");
			var c = a.LogicalOperators.ToList ();
			foreach (var conditionalItem in a.Conditionals) {
				Format (conditionalItem);
				if (b != a.ConditionalsCount) {
					if (c [b - 1] == LogicalOperator.And)
						Append (" AND ");
					else
						Append (" OR ");
					b++;
				}
			}
			Append (")");
			return this;
		}
		private OracleQueryParser Format (GroupByEntry a)		{
			if (a.Term == null)
				throw new InvalidOperationException (ResourceMessageFormatter.Create (() => Properties.Resources.InvalidOperationException_FoundEmptyGroupByEntry).Format ());
			return Format (a.Term);
		}
		private OracleQueryParser Format (SortEntry a)		{
			_columnNameNoCheckMonitor.Enter ();
			using (_columnNameNoCheckMonitor) {
				var b = a.Term as ConditionalContainer;
				if (b != null && b.ConditionalsCount == 1)
					Format (b.Conditionals.First ());
				else
					Format (a.Term);
			}
			if (a.Reverse)
				Append (" DESC");
			else
				Append (" ASC");
			return this;
		}
		private OracleQueryParser Format (QueryTerm a, bool b = false)		{
			var c = a.QueryInfo;
			return Format (c, b);
		}
		private OracleQueryParser Format (QueryInfo a, bool b = false)		{
			var c = new OracleQueryParser (this.Translator, this.TypeSchema, _takeParametersParser) {
				Owner = this,
				UseTakeParameter = true
			};
			c.Query = a;
			foreach (var parameter in a.Parameters)
				if (!this.Query.Parameters.Any (d => d.Name == parameter.Name))
					this.Query.Parameters.Add (parameter);
			if (!b)
				Append ('(');
			Append (c.GetText ());
			if (!b)
				Append (')');
			return this;
		}
		public override string GetColumnName (Column a)		{
			EntityInfo b = (!string.IsNullOrEmpty (a.Owner)) ? GetEntity (a.Owner) : Query.Entities [0];
			ITranslatedName c = null;
			if (string.IsNullOrEmpty (a.Owner) && !Translator.TryGetName (b, a.Name, Query.IgnoreTypeSchema, out c)) {
				return QuoteExpression (a.Name);
			}
			var d = new StringBuilder ();
			if (a.Name != DataAccessConstants.RowVersionPropertyName)
				d.Append (QuoteExpression (b.Alias)).Append ('.');
			if (!string.IsNullOrEmpty (b.FullName))
				d.Append (GetTranslatedName (Translator.GetName (b, a.Name, Query.IgnoreTypeSchema)));
			else
				d.Append (QuoteExpression (a.Name));
			return d.ToString ();
		}
		private OracleQueryParser Append (char a)		{
			_sqlQuery.Append (a);
			return this;
		}
		private OracleQueryParser Append (string a)		{
			_sqlQuery.Append (a);
			return this;
		}
		private string GetTranslatedName (ITranslatedName a)		{
			if (a is TranslatedTableName) {
				var b = (TranslatedTableName)a;
				var c = new StringBuilder ();
				if (!string.IsNullOrEmpty (b.Schema))
					c.Append (QuoteExpression (b.Schema)).Append (".");
				return c.Append (QuoteExpression (b.Name)).ToString ();
			}
			else if (a is TranslatedColumnName) {
				var d = (TranslatedColumnName)a;
				return QuoteExpression (d.Name);
			}
			else if (a is TranslatedSelectPart) {
				return a.ToString ();
			}
			return null;
		}
		private OracleQueryParser AppendTranslatedName (ITranslatedName a)		{
			Append (GetTranslatedName (a));
			return this;
		}
		private OracleQueryParser AppendQuoteExpression (string a)		{
			return Append ('"').Append (a).Append ('"');
		}
		protected virtual string QuoteExpression (string a)		{
			return string.Format ("\"{0}\"", a);
		}
		private OracleQueryParser AppendSqlOperator (string a)		{
			Append (GetSqlOperator (a));
			return this;
		}
		private string GetSqlOperator (string a)		{
			switch (a) {
			case "==":
				return "=";
			case "!=":
				return "<>";
			}
			if (String.Compare ("like", a, StringComparison.CurrentCultureIgnoreCase) == 0)
				return "LIKE";
			else if (String.Compare ("not like", a, StringComparison.CurrentCultureIgnoreCase) == 0)
				return "NOT LIKE";
			else
				return a;
		}
	}
}
