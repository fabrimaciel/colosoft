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
	/// Implementação padrão da factory das chave dos 
	/// registros.
	/// </summary>
	public class RecordKeyFactory : IRecordKeyFactory
	{
		private static IRecordKeyFactory _instance;

		private static readonly string[] _emptyFields = new string[0];

		private readonly Generator _generator = new Generator();

		/// <summary>
		/// Instancia da factory padrão.
		/// </summary>
		public static IRecordKeyFactory Instance
		{
			get
			{
				if(_instance == null)
					_instance = new RecordKeyFactory();
				return _instance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Cria o gerador associado com o tipo informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IRecordKeyGenerator CreateGenerator(Colosoft.Reflection.TypeName typeName)
		{
			return _generator;
		}

		/// <summary>
		/// Recupera os nomes dos campos que fazem parte da chave do registro.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IEnumerable<string> GetKeyFields(Colosoft.Reflection.TypeName typeName)
		{
			return _emptyFields;
		}

		/// <summary>
		/// Cria a chave para o registro informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="record"></param>
		/// <returns></returns>
		public RecordKey Create(Reflection.TypeName typeName, IRecord record)
		{
			return new RecordKey(string.Empty, 0);
		}

		class Generator : IRecordKeyGenerator
		{
			public string GetSimpleKey(IRecord record)
			{
				return string.Empty;
			}

			public string GetKey(IRecord record)
			{
				return string.Empty;
			}

			public string GetSimpleKeyFromFullKey(string key)
			{
				return string.Empty;
			}
		}
	}
}
