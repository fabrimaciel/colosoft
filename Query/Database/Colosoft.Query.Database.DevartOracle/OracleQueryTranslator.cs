using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Query.Database.Oracle{
	public class OracleQueryTranslator : SqlQueryTranslator	{
		public OracleQueryTranslator (Data.Schema.ITypeSchema a) : base (a)		{
		}
		public override ITranslatedName GetName (EntityInfo a)		{
			return this.GetName (a, false);
		}
		public override ITranslatedName GetName (EntityInfo a, bool b)		{
			var c = (TranslatedTableName)base.GetName (a, b);
			return new TranslatedTableName ((c.Schema ?? "").ToUpper (), c.Name.ToUpper ());
		}
		public override ITranslatedName GetName (EntityInfo a, string b)		{
			return this.GetName (a, b, false);
		}
		public override ITranslatedName GetName (EntityInfo a, string b, bool c)		{
			if (b == Query.DataAccessConstants.RowVersionPropertyName)
				return new TranslatedSelectPart (string.Format ("CAST({0}ORA_ROWSCN AS NUMBER(18,0))", !string.IsNullOrEmpty (a.Alias) ? string.Format ("\"{0}\".", a.Alias) : ""));
			var d = base.GetName (a, b, c) as TranslatedColumnName;
			return new TranslatedColumnName (d.Name.ToUpper (), d.TableAlias, d.PropertyType);
		}
		public override ITranslatedName GetName (StoredProcedureName a)		{
			return new TranslatedStoredProcedureName ((a.Name ?? "").ToUpper (), (a.Schema ?? "").ToUpper ());
		}
	}
}
