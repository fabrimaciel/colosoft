using Colosoft.Data.Schema;
using Colosoft.Query.Database.Generic;
using GDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace Colosoft.Query.Database.Oracle{
	public class OracleGenericSqlQueryDataSource : GenericSqlQueryDataSource	{
		protected override bool IgnoreLastFieldWithPaging {
			get {
				return false;
			}
		}
		public OracleGenericSqlQueryDataSource (Microsoft.Practices.ServiceLocation.IServiceLocator a, ITypeSchema b, IProviderLocator c) : base (a, b, new OracleQueryTranslator (b), c)		{
		}
		static OracleGenericSqlQueryDataSource ()		{
			#if !DEVART
			#if UNMANAGED
			            var a = typeof(global::Oracle.DataAccess.Client.OracleConnection).Assembly.GetTypes().FirstOrDefault(f => f.Name == "OraDb_DbTypeTable");
#else
			var a = typeof(global::Oracle.ManagedDataAccess.Client.OracleConnection).Assembly.GetTypes ().FirstOrDefault (b => b.Name == "OraDb_DbTypeTable");
			#endif
			var c = a.GetField ("dbTypeToOracleDbTypeMapping", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			var d = (int[])c.GetValue (null);
			d [13] = 0x79;
			d [6] = 0x7C;
			#endif
		}
		protected override void RegisterSession (GDASession a)		{
			var b = new DataAccess (a.ProviderConfiguration);
			b.ExecuteCommand (a, "ALTER SESSION SET NLS_SORT=BINARY_AI");
			b.ExecuteCommand (a, "ALTER SESSION SET NLS_COMP=LINGUISTIC");
		}
		protected override void RegisterUserInfo (IStoredProcedureTransaction a)		{
		}
		protected override SqlQueryParser CreateParser (QueryInfo a)		{
			return new OracleQueryParser (Translator, TypeSchema, new OracleTakeParametersParser ()) {
				Query = a,
				UseTakeParameter = true
			};
		}
		[System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Reliability", "CA2000:Dispose objects before losing scope"), System.Diagnostics.CodeAnalysis.SuppressMessage ("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
		protected override QueryResult ExecuteStoredProcedure (GDASession a, QueryInfo b)		{
			DataAccess c = a != null ? new DataAccess (a.ProviderConfiguration) : new DataAccess ();
			var d = new List<int> ();
			var e = (TranslatedStoredProcedureName)Translator.GetName (b.StoredProcedureName);
			var f = "BEGIN " + (!string.IsNullOrEmpty (e.Schema) ? string.Format ("\"{0}\".\"{1}\"", e.Schema, e.Name) : string.Format ("\"{0}\"", e.Name)) + " (";
			for (var g = 1; g <= b.Parameters.Count + 1; g++)
				f += ":" + g + ",";
			f = f.Substring (0, f.Length - 1) + "); END;";
			var h = new GDA.Collections.GDAParameterCollection ();
			for (int g = 0; g < b.Parameters.Count; g++) {
				var i = (System.Data.ParameterDirection)((int)b.Parameters [g].Direction);
				h.Add (new GDAParameter (":" + (g + 1), b.Parameters [g].Value, i));
				if (b.Parameters [g].Direction != ParameterDirection.Input)
					d.Add (g);
			}
			#if DEVART
			            // Cria o parametro é que referência do cursor
            var j = new GDAParameter(":" + (b.Parameters.Count + 1), null) { NativeDbType = Devart.Data.Oracle.OracleDbType.Cursor, Direction = System.Data.ParameterDirection.Output };
#else
			var j = new GDAParameter (":" + (b.Parameters.Count + 1), null) {
				DbType = (System.Data.DbType)13,
				Direction = System.Data.ParameterDirection.Output
			};
			#endif
			h.Add (j);
			c.ExecuteCommand (a, System.Data.CommandType.Text, b.CommandTimeout, f, h.ToArray ());
			IEnumerator<GDADataRecord> k = NavigateRefCursor (j).GetEnumerator ();
			IEnumerable<Record> l = null;
			Record.RecordDescriptor m = null;
			foreach (int index in d)
				b.Parameters [index].Value = h [index].Value;
			try {
				if (k.MoveNext ()) {
					var n = k.Current;
					var o = new List<Record.Field> ();
					for (int g = 0; g < n.FieldCount; g++) {
						var p = n [g].GetValue ();
						var q = n.GetName (g);
						if (string.IsNullOrEmpty (q))
							q = "return_value";
						o.Add (new Record.Field (q, n.GetFieldType (g)));
					}
					m = new Record.RecordDescriptor ("descriptor", o);
					var r = GetRecord (m, n);
					l = GetRecords (m, r, k);
				}
				else {
					k.Dispose ();
					l = new Record[0];
				}
			}
			catch {
				k.Dispose ();
				throw;
			}
			var s = new QueryResult (m, l, b, t => Execute (a, t));
			s.Validate ().ThrowInvalid ();
			return s;
		}
		private static IEnumerable<GDADataRecord> NavigateRefCursor (GDAParameter a)		{
			if (a.Value != null) {
				#if UNMANAGED
				                var b = a.Value as global::Oracle.DataAccess.Types.OracleRefCursor;
#elif DEVART
				                var b = a.Value as Devart.Data.Oracle.OracleCursor;
#else
				var b = a.Value as global::Oracle.ManagedDataAccess.Types.OracleRefCursor;
				#endif
				System.Data.IDataReader c = null;
				if (b != null)
					c = b.GetDataReader ();
				else if (a.Value is System.Data.IDataReader)
					c = (System.Data.IDataReader)a.Value;
				if (c != null)
					while (c.Read ()) {
						yield return new GDADataRecord (c, null);
					}
			}
		}
		protected override GDAStoredProcedure GenerateGDAStoredProcedure (QueryInfo a, out IEnumerable<int> b)		{
			var c = new List<int> ();
			var d = (TranslatedStoredProcedureName)Translator.GetName (a.StoredProcedureName);
			var e = new GDAStoredProcedure (!string.IsNullOrEmpty (d.Schema) ? string.Format ("\"{0}\".\"{1}\"", d.Schema, d.Name) : string.Format ("\"{0}\"", d.Name));
			for (int f = 0; f < a.Parameters.Count; f++) {
				var g = (System.Data.ParameterDirection)((int)a.Parameters [f].Direction);
				e.AddParameter (":" + (f + 1), a.Parameters [f].Value, g);
				if (a.Parameters [f].Direction != ParameterDirection.Input)
					c.Add (f);
			}
			#if DEVART
			            e.AddParameter(new GDAParameter(":" + (a.Parameters.Count + 1), null)            {
                Direction = System.Data.ParameterDirection.Output,
                NativeDbType = Devart.Data.Oracle.OracleDbType.Cursor
            });
#else
			e.AddOutputParameter (":" + (a.Parameters.Count + 1), (System.Data.DbType)13);
			#endif
			b = c;
			return e;
		}
		protected override object GetValue (int a, GDADataRecord b, Type c)		{
			#if UNMANAGED
			            var d = (global::Oracle.DataAccess.Client.OracleDataReader)b.BaseDataRecord;
#elif DEVART
			            var d = (Devart.Data.Oracle.OracleDataReader)b.BaseDataRecord;
#else
			var d = (global::Oracle.ManagedDataAccess.Client.OracleDataReader)b.BaseDataRecord;
			#endif
			var e = a;
			var f = d.GetDataTypeName (e);
			if (f == "Date" && !d.IsDBNull (e))
				return b.GetDateTime (e);
			else if ((f == "TimeStampTZ" || c == typeof(DateTime)) && !d.IsDBNull (e))
				#if DEVART
				                return d.GetOracleTimeStamp(e).ToDateTimeOffset();
#else
				return d.GetOracleTimeStampTZ (e).ToDateTimeOffset ();
			#endif
			return b [e].GetValue ();
		}
		protected override Type GetFieldType (int a, GDADataRecord b)		{
			#if UNMANAGED
			            var c = (global::Oracle.DataAccess.Client.OracleDataReader)b.BaseDataRecord;
#elif DEVART
			            var c = (Devart.Data.Oracle.OracleDataReader)b.BaseDataRecord;
#else
			var c = (global::Oracle.ManagedDataAccess.Client.OracleDataReader)b.BaseDataRecord;
			#endif
			var d = c.GetFieldType (a);
			return (d == typeof(DateTime)) ? typeof(DateTimeOffset) : d;
		}
		protected override GDAParameter CreateParameter (string a, object b)		{
			if (b is QueryInfo || b is Queryable)
				b = null;
			if (b is DateTime) {
				var c = (DateTime)b;
				if (c.Kind == DateTimeKind.Unspecified)
					b = new DateTime (c.Year, c.Month, c.Day, c.Hour, c.Minute, c.Second, c.Millisecond, DateTimeKind.Local);
			}
			if (b != null && b.GetType () == typeof(DateTimeOffset)) {
				var c = (DateTimeOffset)b;
				#if UNMANAGED
#elif DEVART
				                b = new Devart.Data.Oracle.OracleTimeStamp(c.DateTime, c.Offset.ToString());
#else
				b = new global::Oracle.ManagedDataAccess.Types.OracleTimeStampTZ (c.DateTime, c.Offset.ToString ());
				#endif
				GDAParameter d = new GDAParameter (a, b);
				d.DbType = System.Data.DbType.DateTime;
				return d;
			}
			else
				return new GDAParameter (a, b);
		}
		protected override Record.RecordDescriptor GetRecordDescriptor (QueryInfo a, GDA.Sql.NativeQuery b, GDADataRecord c)		{
			if (a.Projection.Count > 0) {
				var d = new List<Record.Field> ();
				int e;
				if (IgnoreLastFieldWithPaging && b.TakeCount > 0 && b.SkipCount > 0)
					e = c.FieldCount - 1;
				else
					e = c.FieldCount;
				var f = a.Projection.Select (g => g.Alias).ToArray ();
				if (e >= f.Length) {
					for (int h = 0; h < e; h++) {
						var i = (h < f.Length ? f [h] : c.GetName (h)) ?? c.GetName (h);
						var j = GetFieldType (h, c);
						d.Add (new Record.Field (i, j));
					}
					return new Record.RecordDescriptor ("descriptor", d);
				}
			}
			return base.GetRecordDescriptor (a, b, c);
		}
	}
}
