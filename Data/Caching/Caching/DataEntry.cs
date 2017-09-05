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

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Representa uma entrada de dados do cache.
	/// </summary>
	[Serializable]
	public class DataEntry : Colosoft.Serialization.ICompactSerializable, System.Runtime.Serialization.ISerializable, IDisposable
	{
		/// <summary>
		/// Stream dos dados da entrada.
		/// </summary>
		private Query.IQueryResult _result;

		private Colosoft.Reflection.TypeName _typeName;

		private DateTime _version;

		/// <summary>
		/// Identifica se a leitura já foi processada.
		/// </summary>
		private bool _processed = false;

		/// <summary>
		/// Nome do tipo armazenado na entrada.
		/// </summary>
		public Colosoft.Reflection.TypeName TypeName
		{
			get
			{
				return _typeName;
			}
		}

		/// <summary>
		/// Versão da entrada.
		/// </summary>
		public DateTime Version
		{
			get
			{
				return _version;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DataEntry()
		{
		}

		/// <summary>
		/// Cria uma instancia com os dados da entrada.
		/// </summary>
		/// <param name="typeName">Nome do tipo da entrada.</param>
		/// <param name="version"></param>
		/// <param name="queryResult">Resultado dos itens da entrada.</param>
		public DataEntry(Colosoft.Reflection.TypeName typeName, DateTime version, Query.IQueryResult queryResult)
		{
			typeName.Require("typeName").NotNull();
			_typeName = typeName;
			_version = version;
			_result = queryResult;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected DataEntry(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			_typeName = info.GetValue("typeName", typeof(Colosoft.Reflection.TypeName)) as Colosoft.Reflection.TypeName;
			_version = info.GetDateTime("version");
			_result = info.GetValue("result", typeof(Query.QueryResult)) as Query.QueryResult;
		}

		/// <summary>
		/// Recupera os registros da entrada.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Colosoft.Query.Record> GetRecords()
		{
			if(_result != null)
				return _result;
			if(_processed)
				throw new InvalidOperationException("Record has processed");
			throw new InvalidOperationException();
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		void Colosoft.Serialization.ICompactSerializable.Deserialize(Serialization.IO.CompactReader reader)
		{
			if(reader.ReadByte() == 1)
			{
				_typeName = new Reflection.TypeName();
				_typeName.Deserialize(reader);
			}
			else
				_typeName = null;
			_version = reader.ReadDateTime();
			if(reader.ReadByte() == 1)
			{
				_result = new Query.QueryResult();
				((Colosoft.Serialization.ICompactSerializable)_result).Deserialize(reader);
			}
			else
				_result = new Query.QueryResult();
		}

		/// <summary>
		/// Recuepra os registro do enumerador.
		/// </summary>
		/// <param name="enumerator"></param>
		/// <returns></returns>
		private IEnumerable<Colosoft.Query.Record> GetRecords(IEnumerator<Colosoft.Query.Record> enumerator)
		{
			while (enumerator.MoveNext())
				yield return enumerator.Current;
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void Colosoft.Serialization.ICompactSerializable.Serialize(Serialization.IO.CompactWriter writer)
		{
			if(_typeName == null)
				writer.Write((byte)0);
			else
			{
				writer.Write((byte)1);
				_typeName.Serialize(writer);
			}
			writer.Write(_version);
			var serializable = _result as Colosoft.Serialization.ICompactSerializable;
			if(serializable == null && _result != null)
			{
				var recordEnumerator = _result.GetEnumerator();
				if(recordEnumerator.MoveNext())
				{
					var record = recordEnumerator.Current;
					if(serializable == null)
						serializable = new Colosoft.Query.QueryResult(record.Descriptor, new Colosoft.Query.Record[] {
							record
						}.Concat(GetRecords(recordEnumerator)));
				}
			}
			if(serializable != null)
			{
				writer.Write((byte)1);
				serializable.Serialize(writer);
			}
			else
				writer.Write((byte)0);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "_result")]
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("typeName", _typeName);
			info.AddValue("version", _version);
			info.AddValue("result", _result);
		}
	}
}
