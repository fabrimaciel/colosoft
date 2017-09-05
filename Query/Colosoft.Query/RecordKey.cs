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

namespace Colosoft.Query
{
	/// <summary>
	/// Possíveis tipos de comparação que se
	/// pode aplicar sobre um RecordKey
	/// </summary>
	[Flags]
	public enum RecordKeyComparisonType
	{
		/// <summary>
		/// Identifica se é para comparar o chave.
		/// </summary>
		Key,
		/// <summary>
		/// Identifica se é para comparar o RowVersion.
		/// </summary>
		RowVersion
	}
	/// <summary>
	/// Classe que representa a chave de um registro.
	/// </summary>
	public class RecordKey : IComparable, IComparable<RecordKey>
	{
		private string _key;

		private long _rowVersion;

		/// <summary>
		/// Chave que identifica o registro.
		/// </summary>
		public string Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Versão da linha do registro.
		/// </summary>
		public long RowVersion
		{
			get
			{
				return _rowVersion;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="key">Chave do registro.</param>
		/// <param name="rowVersion">Versão do registro.</param>
		public RecordKey(string key, long rowVersion)
		{
			_key = key;
			_rowVersion = rowVersion;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return string.Format("[Key: {0}, RowVersion: {1}]", Key, RowVersion);
		}

		/// <summary>
		/// Compara a instancia com objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public int CompareTo(object obj)
		{
			var key2 = obj as RecordKey;
			if(key2 != null)
				return string.Compare(key2.Key, Key);
			return -1;
		}

		/// <summary>
		/// Compara a instancia informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public int CompareTo(RecordKey other)
		{
			if(other != null)
				return string.Compare(other.Key, Key);
			return -1;
		}
	}
}
