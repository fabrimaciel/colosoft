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
	/// <summary>
	/// Implementação do provedor do Oracle.
	/// </summary>
	public class OracleProvider : Provider, IParameterConverter, IParameterConverter2
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
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
			string uriString = Assembly.GetExecutingAssembly().EscapedCodeBase;
			Uri uri = new Uri(uriString);
			string path = uri.IsFile ? System.IO.Path.GetDirectoryName(uri.LocalPath) : null;
			#if UNMANAGED
			            providerAssembly = typeof(global::Oracle.DataAccess.Client.OracleCommand).Assembly;
#elif DEVART
			            providerAssembly = typeof(Devart.Data.Oracle.OracleCommand).Assembly;
#else
			providerAssembly = typeof(global::Oracle.ManagedDataAccess.Client.OracleCommand).Assembly;
			#endif
		}

		/// <summary>
		/// Recupera a string da consulta para recuperar a identidade do registro inserido.
		/// </summary>
		public override string SqlQueryReturnIdentity
		{
			get
			{
				return "SELECT {0}.currval FROM dual;";
			}
		}

		/// <summary>
		/// Obtem o caracter usado para delimitar os parametros de string.
		/// </summary>
		/// <returns>The quote character.</returns>
		public override char QuoteCharacter
		{
			get
			{
				return '"';
			}
		}

		/// <summary>
		/// Quote inicial da expressão.
		/// </summary>
		public override string QuoteExpressionBegin
		{
			get
			{
				return "\"";
			}
		}

		/// <summary>
		/// Quote final da expressão.
		/// </summary>
		public override string QuoteExpressionEnd
		{
			get
			{
				return "\"";
			}
		}

		/// <summary>
		/// Adiciona a expressão sobre quote
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		public override string QuoteExpression(string word)
		{
			string[] parts = word.Split('.');
			string result = "";
			for(int i = 0; i < parts.Length; i++)
				result += "\"" + parts[i] + "\"" + ((i + 1) != parts.Length ? "." : "");
			return result.ToUpper();
		}

		/// <summary>
		/// Identifica que o provider suporta o comando limit
		/// </summary>
		public override bool SupportSQLCommandLimit
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Prefixo dos parametros do provedor.
		/// </summary>
		public override string ParameterPrefix
		{
			get
			{
				return ":";
			}
		}

		/// <summary>
		/// Cria uma conexão do banco de dados.
		/// </summary>
		/// <returns></returns>
		public override IDbConnection CreateConnection()
		{
			return base.CreateConnection();
		}

		/// <summary>
		/// Constrói o nome da tabela
		/// </summary>
		/// <param name="tableName"></param>
		/// <returns></returns>
		public override string BuildTableName(Sql.TableName tableName)
		{
			var name = base.BuildTableName(tableName);
			if(name != null)
				name = name.ToUpper();
			return name;
		}

		/// <summary>
		/// Esse método com base no nome da tabela e na coluna identidade da tabela 
		/// recupera a consulta SQL que irá recupera o valor da chave identidade gerado
		/// para o registro recentemente inserido.
		/// </summary>
		/// <param name="tableName">Nome da tabela onde o registro será inserido.</param>
		/// <param name="identityColumnName">Nome da coluna identidade da tabela.</param>
		/// <returns>The modified sql string which also retrieves the identity value</returns>
		public override string GetIdentitySelect(string tableName, string identityColumnName)
		{
			string seqName = (tableName + "_seq").ToUpper();
			return String.Format(SqlQueryReturnIdentity, seqName);
		}

		/// <summary>
		/// Obtem um número inteiro que corresponde ao tipo da base de dados que representa o tipo
		/// informado. O valor de retorno pode ser convertido em um tipo válido (enum value) para 
		/// o atual provider. Esse method é chamado para traduzir os tipos do sistema para os tipos
		/// do banco de dados que não são convertidos explicitamento.
		/// </summary>
		/// <param name="type">Tipo do sistema.</param>
		/// <returns>Tipo correspondente da base de dados.</returns>
		public override long GetDbType(Type type)
		{
			#if DEVART
			            OracleDbType result = OracleDbType.Integer;
#else
			OracleDbType result = OracleDbType.Int32;
			#endif
			if(type.Equals(typeof(byte)) || type.Equals(typeof(Byte)))
				result = OracleDbType.Byte;
			else if(type.Equals(typeof(short)) || type.Equals(typeof(Int16)))
				result = OracleDbType.Int16;
			else if(type.Equals(typeof(int)) || type.Equals(typeof(Int32)) || type.IsEnum)
				#if DEVART
				                result = OracleDbType.Integer;
#else
				result = OracleDbType.Int32;
			#endif
			else if(type.Equals(typeof(long)) || type.Equals(typeof(Int64)))
				result = OracleDbType.Int64;
			else if(type.Equals(typeof(float)) || type.Equals(typeof(Single)))
				result = OracleDbType.Double;
			else if(type.Equals(typeof(double)))
				result = OracleDbType.Double;
			else if(type.Equals(typeof(decimal)) || type.Equals(typeof(Decimal)))
				#if DEVART
				                result = OracleDbType.Double;
#else
				result = OracleDbType.Decimal;
			#endif
			else if(type.Equals(typeof(DateTime)))
				result = OracleDbType.Date;
			else if(type.Equals(typeof(bool)))
				result = OracleDbType.Byte;
			else if(type.Equals(typeof(string)))
				#if DEVART
				                result = OracleDbType.VarChar;
#else
				result = OracleDbType.Varchar2;
			#endif
			else if(type.Equals(typeof(TimeSpan)))
				result = OracleDbType.IntervalDS;
			else if(type.Equals(typeof(byte[])))
				result = OracleDbType.Blob;
			else
				throw new GDAException("Unsupported Property Type");
			return (long)result;
		}

		/// <summary>
		/// Esse método retorna o tipo do sistema correspodente ao tipo specifico indicado no long.
		/// A implementação padrão não retorna exception, mas sim null.
		/// </summary>
		/// <param name="dbType">Tipo especifico do provider.</param>
		/// <returns>Tipo do sistema correspondente.</returns>
		public override Type GetSystemType(long dbType)
		{
			switch(dbType)
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

		/// <summary>
		/// Esse método converte a string (extraída da tabelas do banco de dados) para o tipo do system
		/// correspondente.
		/// </summary>
		/// <param name="dbType">Nome do tipo usado no banco de dados.</param>
		/// <param name="isUnsigned">Valor boolean que identifica se o tipo é unsigned.</param>
		/// <returns>Valor do enumerator do tipo correspondente do banco de dados. O retorno é um número
		/// inteiro por causa que em alguns provider o enumerations não seguem o padrão do DbType definido
		/// no System.Data.</returns>
		public override long GetDbType(string dbType, bool isUnsigned)
		{
			string tmp = dbType.ToLower();
			switch(tmp)
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

		/// <summary>
		/// Define o valor do parametro.
		/// </summary>
		/// <param name="parameter"></param>
		/// <param name="value"></param>
		public override void SetParameterValue(System.Data.IDbDataParameter parameter, object value)
		{
			if(value != null && value.GetType().IsEnum)
				value = System.Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType()));
			if(value is DateTimeOffset)
			{
				var date = (DateTimeOffset)value;
				#if UNMANAGED
				                value = new global::Oracle.DataAccess.Types.OracleTimeStampTZ(date.Date, string.Format("{0}:{1}", date.Offset.Hours, date.Offset.Minutes));
#elif DEVART
				                value = new Devart.Data.Oracle.OracleTimeStamp(date.Date, string.Format("{0}:{1}", date.Offset.Hours, date.Offset.Minutes));
#else
				value = new global::Oracle.ManagedDataAccess.Types.OracleTimeStampTZ(date.Date, string.Format("{0}:{1}", date.Offset.Hours, date.Offset.Minutes));
				#endif
			}
			if(value is bool)
				value = ((bool)value) ? 1 : 0;
			if(value is Guid)
				value = value.ToString();
			try
			{
				base.SetParameterValue(parameter, value);
			}
			catch(Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Processa o comando de limite.
		/// </summary>
		/// <param name="mapping"></param>
		/// <param name="sqlQuery"></param>
		/// <param name="startRecord"></param>
		/// <param name="size"></param>
		/// <returns></returns>
		public override string SQLCommandLimit(List<Mapper> mapping, string sqlQuery, int startRecord, int size)
		{
			string order = "";
			string[] fieldOrderBy = null;
			string[] directionFieldOrderBy = null;
			if(sqlQuery.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase) < 0)
			{
				if(mapping == null)
					throw new GDAException("On Oracle for paging is required at least one ordered field");
				Mapper field = mapping.Find(delegate(Mapper m) {
					return (m.ParameterType == PersistenceParameterType.IdentityKey || m.ParameterType == PersistenceParameterType.Key);
				});
				if(field == null)
					field = mapping[0];
				var selectIndex = sqlQuery.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
				var fromIndex = sqlQuery.IndexOf("FROM", 0, StringComparison.OrdinalIgnoreCase);
				if(selectIndex >= 0 && fromIndex >= "SELECT".Length + 1)
				{
					var columnsPart = sqlQuery.Substring(selectIndex + "SELECT".Length, fromIndex - (selectIndex + "SELECT".Length));
					if(columnsPart.IndexOf("*", 0) < 0 && columnsPart.IndexOf(field.Name, 0, StringComparison.OrdinalIgnoreCase) < 0)
					{
						sqlQuery = sqlQuery.Substring(0, fromIndex) + ", " + QuoteExpression(field.Name) + " " + sqlQuery.Substring(fromIndex);
					}
				}
				var lastIndex = field.Name.LastIndexOf('.');
				if(lastIndex >= 0)
					sqlQuery += " ORDER BY " + QuoteExpression(field.Name.Substring(lastIndex + 1));
				else
					sqlQuery += " ORDER BY " + QuoteExpression(field.Name);
			}
			int orderBy = sqlQuery.IndexOf("ORDER BY", 0, StringComparison.OrdinalIgnoreCase);
			if(orderBy >= 0)
			{
				order = sqlQuery.Substring(orderBy + "ORDER BY".Length, sqlQuery.Length - (orderBy + "ORDER BY".Length));
				order = order.Trim('\r', '\n');
				fieldOrderBy = order.Split(',');
				directionFieldOrderBy = new string[fieldOrderBy.Length];
				for(int i = 0; i < fieldOrderBy.Length; i++)
				{
					int posDi = 0;
					if(fieldOrderBy[i].TrimEnd(' ').EndsWith(" DESC", StringComparison.OrdinalIgnoreCase))
					{
						posDi = fieldOrderBy[i].TrimEnd(' ').LastIndexOf(" DESC", StringComparison.OrdinalIgnoreCase);
						directionFieldOrderBy[i] = "DESC";
						fieldOrderBy[i] = fieldOrderBy[i].Substring(0, posDi).Trim();
					}
					else
					{
						directionFieldOrderBy[i] = "ASC";
						posDi = fieldOrderBy[i].IndexOf(" ASC", 0, StringComparison.OrdinalIgnoreCase);
						if(posDi >= 0)
							fieldOrderBy[i] = fieldOrderBy[i].Substring(0, posDi).Trim();
						else
							fieldOrderBy[i] = fieldOrderBy[i].Trim();
					}
					int dotPos = fieldOrderBy[i].LastIndexOf('.');
					if(dotPos >= 0)
						fieldOrderBy[i] = fieldOrderBy[i].Substring(dotPos + 1);
				}
			}
			if(fieldOrderBy.Length > 0)
			{
				string[] orderParts = new string[fieldOrderBy.Length];
				for(int i = 0; i < fieldOrderBy.Length; i++)
					orderParts[i] = fieldOrderBy[i] + " " + directionFieldOrderBy[i];
				order = string.Join(", ", orderParts);
			}
			int pos = sqlQuery.IndexOf("SELECT", 0, StringComparison.OrdinalIgnoreCase);
			sqlQuery = string.Format("SELECT * FROM (SELECT a.*, rownum r__ FROM ({0}) a WHERE rownum < (({1} * {2}) + 1 ) ) WHERE r__ >= ((({1}-1) * {2}) + 1)", sqlQuery, (startRecord / size) + 1, size);
			return sqlQuery;
		}

		/// <summary>
		/// Converte o parametro do GDA.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public System.Data.IDbDataParameter Convert(GDAParameter parameter)
		{
			var p = this.CreateParameter() as OracleParameter;
			if(parameter.DbTypeIsDefined)
				p.DbType = parameter.DbType;
			if(parameter.NativeDbTypeIsDefined)
			{
				if(parameter.NativeDbType is int)
					p.OracleDbType = (OracleDbType)(int)parameter.NativeDbType;
				else if(parameter.NativeDbType is OracleDbType)
					p.OracleDbType = (OracleDbType)parameter.NativeDbType;
			}
			if(p.Direction != parameter.Direction)
				p.Direction = parameter.Direction;
			p.Size = parameter.Size;
			try
			{
				if(parameter.ParameterName[0] == '?')
					p.ParameterName = ParameterPrefix + parameter.ParameterName.Substring(1) + ParameterSuffix;
				else
					p.ParameterName = parameter.ParameterName;
			}
			catch(Exception ex)
			{
				throw new GDAException("Error on convert parameter name '" + parameter.ParameterName + "'.", ex);
			}
			SetParameterValue(p, parameter.Value == null ? DBNull.Value : parameter.Value);
			return p;
		}

		/// <summary>
		/// Cria um comando.
		/// </summary>
		/// <returns></returns>
		public override IDbCommand CreateCommand()
		{
			var oracleCommand = new OracleCommand();
			#if !DEVART
			oracleCommand.BindByName = true;
			#endif
			return oracleCommand;
		}

		/// <summary>
		/// Converte um parametro do GDA para um parametro de dados.
		/// </summary>
		/// <param name="command">Instancia do comando onde o parametro será utilizado.</param>
		/// <param name="parameter">Instancia do parametro que será convertido.</param>
		/// <returns></returns>
		public IDbDataParameter Converter(IDbCommand command, GDAParameter parameter)
		{
			if(parameter == null)
				throw new ArgumentNullException("parameter");
			#if !UNMANAGED && !DEVART
			if(command is DbCommandWrapper)
				command = ((DbCommandWrapper)command).Command;
			#endif
			if(parameter.Value is byte[] && ((byte[])parameter.Value).Length >= 32768)
			{
				var oraCommand = (OracleCommand)command;
				var lobName = parameter.ParameterName;
				while (lobName.Length > 0 && !char.IsLetterOrDigit(lobName[0]))
					lobName = lobName.Substring(1);
				var command2 = (OracleCommand)command.Connection.CreateCommand();
				command2.CommandText = string.Format("declare xx{0} blob; begin dbms_lob.createtemporary(xx{0}, false, 0); :tempblob{0} := xx{0}; end;", lobName);
				var tmpParameter = new OracleParameter(string.Format("tempblob{0}", lobName), OracleDbType.Blob);
				tmpParameter.Direction = ParameterDirection.Output;
				command2.Parameters.Add(tmpParameter);
				command2.ExecuteNonQuery();
				var buffer = (byte[])parameter.Value;
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
				                var tempLob = (global::Oracle.DataAccess.Types.OracleBlob)command2.Parameters[0].Value;
#else
				var tempLob = (global::Oracle.ManagedDataAccess.Types.OracleBlob)command2.Parameters[0].Value;
				#endif
				tempLob.BeginChunkWrite();
				for(int i = 0, length = (buffer.Length >= 32760 ? 32760 : buffer.Length); length > 0; i += length, length = buffer.Length - i >= 32760 ? 32760 : buffer.Length - i)
				{
					tempLob.Write(buffer, i, length);
				}
				tempLob.EndChunkWrite();
				#endif
				var parameter2 = (OracleParameter)Convert(parameter);
				parameter2.OracleDbType = OracleDbType.Blob;
				parameter2.Value = tempLob;
				return parameter2;
			}
			else if(parameter.Value is string && ((string)parameter.Value).Length > 32768)
			{
				var oraCommand = (OracleCommand)command;
				var lobName = parameter.ParameterName;
				while (lobName.Length > 0 && !char.IsLetterOrDigit(lobName[0]))
					lobName = lobName.Substring(1);
				var command2 = (OracleCommand)command.Connection.CreateCommand();
				command2.CommandText = string.Format("declare xx{0} nclob; begin dbms_lob.createtemporary(xx{0}, false, 0); :tempblob{0} := xx{0}; end;", lobName);
				var tmpParameter = new OracleParameter(string.Format("tempblob{0}", lobName), OracleDbType.NClob);
				tmpParameter.Direction = ParameterDirection.Output;
				command2.Parameters.Add(tmpParameter);
				command2.ExecuteNonQuery();
				var buffer = (string)parameter.Value;
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
				                var tempLob = (global::Oracle.DataAccess.Types.OracleClob)command2.Parameters[0].Value;
#else
				var tempLob = (global::Oracle.ManagedDataAccess.Types.OracleClob)command2.Parameters[0].Value;
				#endif
				tempLob.BeginChunkWrite();
				var encoding = System.Text.Encoding.UTF8;
				for(int i = 0, length = (buffer.Length >= 32760 ? 32760 : buffer.Length); length > 0; i += length, length = buffer.Length - i >= 32760 ? 32760 : buffer.Length - i)
				{
					var text = buffer.Substring(i, length);
					tempLob.Write(encoding.GetBytes(text), 0, encoding.GetByteCount(text));
				}
				tempLob.EndChunkWrite();
				#endif
				var parameter2 = (OracleParameter)Convert(parameter);
				parameter2.OracleDbType = OracleDbType.NClob;
				parameter2.Value = tempLob;
				return parameter2;
			}
			return Convert(parameter);
		}
	}
}
