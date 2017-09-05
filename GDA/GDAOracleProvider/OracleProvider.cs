using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using GDA.Interfaces;
using System.Reflection;

#if UNMANAGED
using Oracle.DataAccess.Client;
#elif DEVART
using Devart.Data.Oracle;
#else
using Oracle.ManagedDataAccess.Client;

#endif
namespace GDA.Provider.Oracle
{
    public class OracleProvider : Provider, IParameterConverter, IParameterConverter2
    {
        public OracleProvider()
#if UNMANAGED
		            : base("Oracle", "Oracle.DataAccess.dll",
                   "Oracle.DataAccess.Client.OracleConnection",
                   "Oracle.DataAccess.Client.OracleDataAdapter",
                   "Oracle.DataAccess.Client.OracleCommand",
                   "Oracle.DataAccess.Client.OracleParameter", ":", true, "")
#elif DEVART
                : base("Oracle", "Devart.Data.Oracle.dll",
                   "Devart.Data.Oracle.OracleConnection",
                   "Devart.Data.Oracle.OracleDataAdapter",
                   "Devart.Data.Oracle.OracleCommand",
                   "Devart.Data.Oracle.OracleParameter", ":", true, "")
#else
        : base("Oracle", "Oracle.ManagedDataAccess.dll", "Oracle.ManagedDataAccess.Client.OracleConnection", "Oracle.ManagedDataAccess.Client.OracleDataAdapter", "Oracle.ManagedDataAccess.Client.OracleCommand", "Oracle.ManagedDataAccess.Client.OracleParameter", ":", true, "")
#endif

        {
            base.ExecuteCommandsOneAtATime = true;
            string a = Assembly.GetExecutingAssembly().EscapedCodeBase;
            Uri b = new Uri(a);
            string c = b.IsFile ? System.IO.Path.GetDirectoryName(b.LocalPath) : null;
#if UNMANAGED
			            providerAssembly = typeof(global::Oracle.DataAccess.Client.OracleCommand).Assembly;
#elif DEVART
            providerAssembly = typeof(Devart.Data.Oracle.OracleCommand).Assembly;
#else
            providerAssembly = typeof(global::Oracle.ManagedDataAccess.Client.OracleCommand).Assembly;
#endif
        }

        public override string SqlQueryReturnIdentity
        {
            get
            {
                return "SELECT {0}.currval FROM dual;";
            }
        }

        public override char QuoteCharacter
        {
            get
            {
                return '"';
            }
        }

        public override string QuoteExpressionBegin
        {
            get
            {
                return "\"";
            }
        }

        public override string QuoteExpressionEnd
        {
            get
            {
                return "\"";
            }
        }

        public override string QuoteExpression(string a)
        {
            string[] b = a.Split('.');
            string c = "";
            for (int d = 0; d < b.Length; d++)
                c += "\"" + b[d] + "\"" + ((d + 1) != b.Length ? "." : "");
            return c.ToUpper();
        }

        public override bool SupportSQLCommandLimit
        {
            get
            {
                return true;
            }
        }

        public override string ParameterPrefix
        {
            get
            {
                return ":";
            }
        }

        public override IDbConnection CreateConnection()
        {
            return base.CreateConnection();
        }

        public override string BuildTableName(Sql.TableName a)
        {
            var b = base.BuildTableName(a);
            if (b != null)
                b = b.ToUpper();
            return b;
        }

        public override string GetIdentitySelect(string a, string b)
        {
            string c = (a + "_seq").ToUpper();
            return String.Format(SqlQueryReturnIdentity, c);
        }

        public override long GetDbType(Type a)
        {
#if DEVART
			OracleDbType b = OracleDbType.Integer;
#else
            OracleDbType b = OracleDbType.Int32;
#endif
            if (a.Equals(typeof(byte)) || a.Equals(typeof(Byte)))
                b = OracleDbType.Byte;
            else if (a.Equals(typeof(short)) || a.Equals(typeof(Int16)))
                b = OracleDbType.Int16;
            else if (a.Equals(typeof(int)) || a.Equals(typeof(Int32)) || a.IsEnum)
#if DEVART
				b = OracleDbType.Integer;
#else
                b = OracleDbType.Int32;
#endif
            else if (a.Equals(typeof(long)) || a.Equals(typeof(Int64)))
                b = OracleDbType.Int64;
            else if (a.Equals(typeof(float)) || a.Equals(typeof(Single)))
                b = OracleDbType.Double;
            else if (a.Equals(typeof(double)))
                b = OracleDbType.Double;
            else if (a.Equals(typeof(decimal)) || a.Equals(typeof(Decimal)))
#if DEVART
				b = OracleDbType.Double;
#else
                b = OracleDbType.Decimal;
#endif
            else if (a.Equals(typeof(DateTime)))
                b = OracleDbType.Date;
            else if (a.Equals(typeof(bool)))
                b = OracleDbType.Byte;
            else if (a.Equals(typeof(string)))
#if DEVART
				b = OracleDbType.VarChar;
#else
                b = OracleDbType.Varchar2;
#endif
            else if (a.Equals(typeof(TimeSpan)))
                b = OracleDbType.IntervalDS;
            else if (a.Equals(typeof(byte[])))
                b = OracleDbType.Blob;
            else
                throw new GDAException("Unsupported Property Type");
            return (long)b;
        }

        public override Type GetSystemType(long a)
        {
            switch (a)
            {
                case (long)OracleDbType.Byte:
                    return typeof(bool);
                case (long)OracleDbType.Int16:
                    return typeof(Int16);
#if DEVART
			    case (long)OracleDbType.Integer:
#else
                case (long)OracleDbType.Int32:
#endif
                    return typeof(Int32);
                case (long)OracleDbType.Int64:
                case (long)OracleDbType.Long:
                    return typeof(Int64);
#if DEVART
			    case (long)OracleDbType.Float:
#else
                case (long)OracleDbType.Single:
#endif
                    return typeof(float);
                case (long)OracleDbType.Double:
                    return typeof(double);
                case (long)OracleDbType.Date:
                case (long)OracleDbType.TimeStamp:
                case (long)OracleDbType.TimeStampLTZ:
                case (long)OracleDbType.TimeStampTZ:
                    return typeof(DateTime);
#if !DEVART
                case (long)OracleDbType.Decimal:
                    return typeof(decimal);
#endif
#if DEVART
			    case (long)OracleDbType.NVarChar:
                case (long)OracleDbType.VarChar:
                case (long)OracleDbType.Xml:
#else
                case (long)OracleDbType.NVarchar2:
                case (long)OracleDbType.Varchar2:
                case (long)OracleDbType.XmlType:
#endif
                case (long)OracleDbType.NChar:
                case (long)OracleDbType.Char:
                case (long)OracleDbType.Clob:
                case (long)OracleDbType.NClob:
                    return typeof(string);
                case (long)OracleDbType.Raw:
                case (long)OracleDbType.LongRaw:
                case (long)OracleDbType.Blob:
                case (long)OracleDbType.BFile:
                    return typeof(byte[]);
                case (long)OracleDbType.IntervalDS:
                    return typeof(TimeSpan);
#if DEVART
			        case (long)OracleDbType.Cursor:
                    return typeof(Devart.Data.Oracle.OracleCursor);
#endif
                default:
                    return typeof(object);
            }
        }

        public override long GetDbType(string a, bool b)
        {
            string c = a.ToLower();
            switch (c)
            {
                case "bfile":
                    return (long)OracleDbType.BFile;
                case "blob":
                    return (long)OracleDbType.Blob;
                case "byte":
                    return (long)OracleDbType.Byte;
                case "char":
                    return (long)OracleDbType.Char;
                case "clob":
                    return (long)OracleDbType.Clob;
                case "date":
                case "datetime":
                    return (long)OracleDbType.Date;
                case "decimal":
                case "number":
#if DEVART
				                    return (long)OracleDbType.Double;
#else
                    return (long)OracleDbType.Decimal;
#endif
                case "double":
                case "float":
                    return (long)OracleDbType.Double;
                case "int16":
                    return (long)OracleDbType.Int16;
                case "int32":
#if DEVART
				                    return (long)OracleDbType.Integer;
#else
                    return (long)OracleDbType.Int32;
#endif
                case "int64":
                    return (long)OracleDbType.Int64;
                case "intervalds":
                case "intervaldaytosecond":
                case "interval day to second":
                    return (long)OracleDbType.IntervalDS;
                case "intervalym":
                case "intervalyeartomonth":
                case "interval year to month":
                    return (long)OracleDbType.IntervalYM;
                case "long":
                    return (long)OracleDbType.Long;
                case "longraw":
                case "long raw":
                    return (long)OracleDbType.LongRaw;
                case "nchar":
                    return (long)OracleDbType.NChar;
                case "nclob":
                    return (long)OracleDbType.NClob;
                case "nvarchar":
                case "nvarchar2":
#if DEVART
				                    return (long)OracleDbType.NVarChar;
#else
                    return (long)OracleDbType.NVarchar2;
#endif
                case "raw":
                    return (long)OracleDbType.Raw;
                case "cursor":
                case "ref cursor":
                case "refcursor":
#if DEVART
				                    return (long)OracleDbType.Cursor;
#else
                    return (long)OracleDbType.RefCursor;
#endif
                case "single":
#if DEVART
				                    return (long)OracleDbType.Float;
#else
                    return (long)OracleDbType.Single;
#endif
                case "timestamp":
                    return (long)OracleDbType.TimeStamp;
                case "timestamplocal":
                case "timestamp with local time zone":
                case "timestampltz":
                    return (long)OracleDbType.TimeStampLTZ;
                case "timestampwithtz":
                case "timestamp with time zone":
                case "timestamptz":
                    return (long)OracleDbType.TimeStampTZ;
                case "varchar":
                case "varchar2":
#if DEVART
				                    return (long)OracleDbType.VarChar;
#else
                    return (long)OracleDbType.Varchar2;
#endif
                case "xmltype":
#if DEVART
				                    return (long)OracleDbType.Xml;
#else
                    return (long)OracleDbType.XmlType;
#endif
                case "rowid":
#if DEVART
				                    return (long)OracleDbType.VarChar;
#else
                    return (long)OracleDbType.Varchar2;
#endif
                default:
                    return No_DbType;
            }
        }

        public override void SetParameterValue(System.Data.IDbDataParameter a, object b)
        {
            if (b != null && b.GetType().IsEnum)
                b = System.Convert.ChangeType(b, Enum.GetUnderlyingType(b.GetType()));
            if (b is DateTimeOffset)
            {
                var c = (DateTimeOffset)b;
#if UNMANAGED
				b = new global::Oracle.DataAccess.Types.OracleTimeStampTZ(c.Date, string.Format("{0}:{1}", c.Offset.Hours, c.Offset.Minutes));
#elif DEVART
                b = new Devart.Data.Oracle.OracleTimeStamp(date.Date, string.Format("{0}:{1}", date.Offset.Hours, date.Offset.Minutes));
#else
                b = new global::Oracle.ManagedDataAccess.Types.OracleTimeStampTZ(c.Date, string.Format("{0}:{1}", c.Offset.Hours, c.Offset.Minutes));
#endif
            }
            if (b is bool)
                b = ((bool)b) ? 1 : 0;
            if (b is Guid)
                b = b.ToString();
            try
            {
                base.SetParameterValue(a, b);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public override string SQLCommandLimit(List<Mapper> a, string b, int c, int d)
        {
            string e = "";
            string[] f = null;
            string[] g = null;
            if (b.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) < 0)
            {
                if (a == null)
                    throw new GDAException("On Oracle for paging is required at least one ordered field");
                Mapper h = a.Find(delegate (Mapper i) {
                    return (i.ParameterType == PersistenceParameterType.IdentityKey || i.ParameterType == PersistenceParameterType.Key);
                });
                if (h == null)
                    h = a[0];
                var j = b.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
                var k = b.IndexOf("FROM", 0, StringComparison.OrdinalIgnoreCase);
                if (j >= 0 && k >= "SELECT".Length + 1)
                {
                    var l = b.Substring(j + "SELECT".Length, k - (j + "SELECT".Length));
                    if (l.IndexOf("*", 0) < 0 && l.IndexOf(h.Name, 0, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        b = b.Substring(0, k) + ", " + QuoteExpression(h.Name) + " " + b.Substring(k);
                    }
                }
                var m = h.Name.LastIndexOf('.');
                if (m >= 0)
                    b += " ORDER BY " + QuoteExpression(h.Name.Substring(m + 1));
                else
                    b += " ORDER BY " + QuoteExpression(h.Name);
            }
            int n = b.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase);
            if (n >= 0)
            {
                e = b.Substring(n + "ORDER BY".Length, b.Length - (n + "ORDER BY".Length));
                e = e.Trim('\r', '\n');
                f = e.Split(',');
                g = new string[f.Length];
                for (int o = 0; o < f.Length; o++)
                {
                    int p = 0;
                    if (f[o].TrimEnd(' ').EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
                    {
                        p = f[o].TrimEnd(' ').LastIndexOf(" DESC", StringComparison.OrdinalIgnoreCase);
                        g[o] = "DESC";
                        f[o] = f[o].Substring(0, p).Trim();
                    }
                    else
                    {
                        g[o] = "ASC";
                        p = f[o].IndexOf(" ASC", 0, StringComparison.OrdinalIgnoreCase);
                        if (p >= 0)
                            f[o] = f[o].Substring(0, p).Trim();
                        else
                            f[o] = f[o].Trim();
                    }
                    int q = f[o].LastIndexOf('.');
                    if (q >= 0)
                        f[o] = f[o].Substring(q + 1);
                }
            }
            if (f.Length > 0)
            {
                string[] r = new string[f.Length];
                for (int o = 0; o < f.Length; o++)
                    r[o] = f[o] + " " + g[o];
                e = string.Join(", ", r);
            }
            int s = b.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
            b = string.Format("SELECT * FROM (SELECT a.*, rownum r__ FROM ({0}) a WHERE rownum < (({1} * {2}) + 1 ) ) WHERE r__ >= ((({1}-1) * {2}) + 1)", b, (c / d) + 1, d);
            return b;
        }

        public System.Data.IDbDataParameter Convert(GDAParameter a)
        {
            var b = this.CreateParameter() as OracleParameter;
            if (a.DbTypeIsDefined)
                b.DbType = a.DbType;
            if (a.NativeDbTypeIsDefined)
            {
                if (a.NativeDbType is int)
                    b.OracleDbType = (OracleDbType)(int)a.NativeDbType;
                else if (a.NativeDbType is OracleDbType)
                    b.OracleDbType = (OracleDbType)a.NativeDbType;
            }
            if (b.Direction != a.Direction)
                b.Direction = a.Direction;
            b.Size = a.Size;
            try
            {
                if (a.ParameterName[0] == '?')
                    b.ParameterName = ParameterPrefix + a.ParameterName.Substring(1) + ParameterSuffix;
                else
                    b.ParameterName = a.ParameterName;
            }
            catch (Exception ex)
            {
                throw new GDAException("Error on convert parameter name '" + a.ParameterName + "'.", ex);
            }
            SetParameterValue(b, a.Value == null ? DBNull.Value : a.Value);
            return b;
        }

        public override IDbCommand CreateCommand()
        {
            var a = new OracleCommand();
#if !DEVART
            a.BindByName = true;
#endif
            return a;
        }

        public IDbDataParameter Converter(IDbCommand a, GDAParameter b)
        {
            if (b == null)
                throw new ArgumentNullException("parameter");
#if !UNMANAGED && !DEVART
            if (a is DbCommandWrapper)
                a = ((DbCommandWrapper)a).Command;
#endif
            if (b.Value is byte[] && ((byte[])b.Value).Length >= 32768)
            {
                var c = (OracleCommand)a;
                var d = b.ParameterName;
                while (d.Length > 0 && !char.IsLetterOrDigit(d[0]))
                    d = d.Substring(1);
                var e = (OracleCommand)a.Connection.CreateCommand();
                e.CommandText = string.Format("declare xx{0} blob; begin dbms_lob.createtemporary(xx{0}, false, 0); :tempblob{0} := xx{0}; end;", d);
                var f = new OracleParameter(string.Format("tempblob{0}", d), OracleDbType.Blob);
                f.Direction = ParameterDirection.Output;
                e.Parameters.Add(f);
                e.ExecuteNonQuery();
                var g = (byte[])b.Value;
#if DEVART
				                var devartParameter = (Devart.Data.Oracle.OracleParameter)command2.Parameters[0];

                var tempLob = (OracleLob)devartParameter.OracleValue;

                // Escreve os dados no lob temporário
                for (int i = 0, length = (buffer.Length >= 32760 ? 32760 : buffer.Length);
                     length > 0;
                     i += length, length = buffer.Length - i >= 32760 ? 32760 : buffer.Length - i)
                {
                    tempLob.Write(buffer, i, length);
                }

                tempLob.Flush();
#else
#if UNMANAGED
				var h = (global::Oracle.DataAccess.Types.OracleBlob)e.Parameters[0].Value;
#else
                var h = (global::Oracle.ManagedDataAccess.Types.OracleBlob)e.Parameters[0].Value;
#endif
                h.BeginChunkWrite();
                for (int i = 0, j = (g.Length >= 32760 ? 32760 : g.Length); j > 0; i += j, j = g.Length - i >= 32760 ? 32760 : g.Length - i)
                {
                    h.Write(g, i, j);
                }
                h.EndChunkWrite();
#endif
                var k = (OracleParameter)Convert(b);
                k.OracleDbType = OracleDbType.Blob;
                k.Value = h;
                return k;
            }
            else if (b.Value is string && ((string)b.Value).Length > 32768)
            {
                var c = (OracleCommand)a;
                var d = b.ParameterName;
                while (d.Length > 0 && !char.IsLetterOrDigit(d[0]))
                    d = d.Substring(1);
                var e = (OracleCommand)a.Connection.CreateCommand();
                e.CommandText = string.Format("declare xx{0} nclob; begin dbms_lob.createtemporary(xx{0}, false, 0); :tempblob{0} := xx{0}; end;", d);
                var f = new OracleParameter(string.Format("tempblob{0}", d), OracleDbType.NClob);
                f.Direction = ParameterDirection.Output;
                e.Parameters.Add(f);
                e.ExecuteNonQuery();
                var g = (string)b.Value;
#if DEVART
				                var devartParameter = (Devart.Data.Oracle.OracleParameter)command2.Parameters[0];

                var tempLob = (OracleLob)devartParameter.OracleValue;

                var encoding = System.Text.Encoding.UTF8;

                // Escreve os dados no lob temporário
                for (int i = 0, length = (buffer.Length >= 32760 ? 32760 : buffer.Length);
                     length > 0;
                     i += length, length = buffer.Length - i >= 32760 ? 32760 : buffer.Length - i)
                {
                    var text = buffer.Substring(i, length);
                    tempLob.Write(encoding.GetBytes(text), 0, encoding.GetByteCount(text));
                }

                tempLob.Flush();
#else
#if UNMANAGED
				                var h = (global::Oracle.DataAccess.Types.OracleClob)e.Parameters[0].Value;
#else
                var h = (global::Oracle.ManagedDataAccess.Types.OracleClob)e.Parameters[0].Value;
#endif
                h.BeginChunkWrite();
                var l = System.Text.Encoding.UTF8;
                for (int i = 0, j = (g.Length >= 32760 ? 32760 : g.Length); j > 0; i += j, j = g.Length - i >= 32760 ? 32760 : g.Length - i)
                {
                    var m = g.Substring(i, j);
                    h.Write(l.GetBytes(m), 0, l.GetByteCount(m));
                }
                h.EndChunkWrite();
#endif
                var k = (OracleParameter)Convert(b);
                k.OracleDbType = OracleDbType.NClob;
                k.Value = h;
                return k;
            }
            return Convert(b);
        }
    }
}
