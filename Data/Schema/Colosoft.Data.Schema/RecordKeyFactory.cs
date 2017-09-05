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

namespace Colosoft.Data.Schema
{
	/// <summary>
	/// Implementação do factory da chaves dos registros.
	/// </summary>
	public class RecordKeyFactory : Query.IRecordKeyFactory
	{
		private Colosoft.Data.Schema.ITypeSchema _typeSchema;

		private System.Globalization.CultureInfo _cultureInfo;

		private Dictionary<string, Query.IRecordKeyGenerator> _generators = new Dictionary<string, Query.IRecordKeyGenerator>();

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeSchema"></param>
		public RecordKeyFactory(Colosoft.Data.Schema.ITypeSchema typeSchema)
		{
			typeSchema.Require("typeSchema").NotNull();
			_typeSchema = typeSchema;
			_cultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-US");
		}

		/// <summary>
		/// Cria o gerador associado com o tipo informado.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public Query.IRecordKeyGenerator CreateGenerator(Colosoft.Reflection.TypeName typeName)
		{
			typeName.Require("typeName").NotNull();
			Query.IRecordKeyGenerator result = null;
			var key = typeName.FullName;
			lock (_generators)
				if(_generators.TryGetValue(key, out result))
					return result;
			var typeMetadata = _typeSchema.GetTypeMetadata(key);
			if(typeMetadata == null)
			{
				result = new DefaultRecordKeyGenerator(typeName);
			}
			else
			{
				var keys = typeMetadata.GetKeyProperties().Select(f => f.Name).ToArray();
				result = new Generator(typeName, _cultureInfo, keys);
			}
			lock (_generators)
				if(!_generators.ContainsKey(key))
					_generators.Add(key, result);
			return result;
		}

		/// <summary>
		/// Classe responsável por gerar a chave.
		/// </summary>
		public class Generator : Query.IRecordKeyGenerator
		{
			private System.Globalization.CultureInfo _cultureInfo;

			private Colosoft.Reflection.TypeName _typeName;

			private string[] _fieldNames;

			/// <summary>
			/// Nome dos campos associados.
			/// </summary>
			internal string[] FieldNames
			{
				get
				{
					return _fieldNames;
				}
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="typeName"></param>
			/// <param name="cultureInfo"></param>
			/// <param name="fieldNames"></param>
			internal Generator(Colosoft.Reflection.TypeName typeName, System.Globalization.CultureInfo cultureInfo, string[] fieldNames)
			{
				typeName.Require("typeName").NotNull();
				fieldNames.Require("fieldNames").NotNull();
				_typeName = typeName;
				_cultureInfo = cultureInfo;
				_fieldNames = fieldNames;
			}

			/// <summary>
			/// Recupera a chave simples que representa o registro.
			/// </summary>
			/// <param name="record">Instancia do registro.</param>
			/// <returns></returns>
			public string GetSimpleKey(Query.IRecord record)
			{
				record.Require("record").NotNull();
				record.Descriptor.Require("record.Descriptor").NotNull();
				var values = new string[_fieldNames.Length];
				for(var i = 0; i < values.Length; i++)
				{
					var indexOf = 0;
					for(; indexOf < record.Descriptor.Count; indexOf++)
						if(StringComparer.InvariantCultureIgnoreCase.Equals(record.Descriptor[indexOf].Name, _fieldNames[i]))
						{
							var value = record.GetValue(indexOf);
							if(value != null)
								values[i] = value.ToString();
							break;
						}
				}
				return string.Join("|", values);
			}

			/// <summary>
			/// Recupera a chave com base nos dados do registro informado.
			/// </summary>
			/// <param name="record"></param>
			/// <returns></returns>
			public string GetKey(Colosoft.Query.IRecord record)
			{
				return string.Format("{0}:{1}", _typeName.FullName, GetSimpleKey(record));
			}

			/// <summary>
			/// Recupera uma chave simples a partir de uma chave completa.
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public string GetSimpleKeyFromFullKey(string key)
			{
				if(!string.IsNullOrEmpty(key))
				{
					var indexOf = key.IndexOf(':');
					if(indexOf > 0)
						return key.Substring(indexOf);
				}
				return key;
			}
		}

		/// <summary>
		/// Gerador padrão.
		/// </summary>
		public class DefaultRecordKeyGenerator : Query.IRecordKeyGenerator
		{
			private Colosoft.Reflection.TypeName _typeName;

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="typeName">Nome do tipo associado.</param>
			public DefaultRecordKeyGenerator(Colosoft.Reflection.TypeName typeName)
			{
				_typeName = typeName;
			}

			/// <summary>
			/// Recupera a chave com base nos dados do registro informado.
			/// </summary>
			/// <param name="record"></param>
			/// <returns></returns>
			public string GetKey(Query.IRecord record)
			{
				return string.Format("{0}:{1}", _typeName.FullName, GetSimpleKey(record));
			}

			/// <summary>
			/// Recupera a chave simples que representa o registro.
			/// </summary>
			/// <param name="record">Instancia do registro.</param>
			/// <returns></returns>
			public string GetSimpleKey(Query.IRecord record)
			{
				if(record != null)
					return record.GetHashCode().ToString();
				return string.Empty;
			}

			/// <summary>
			/// Recupera uma chave simples a partir de uma chave completa.
			/// </summary>
			/// <param name="key"></param>
			/// <returns></returns>
			public string GetSimpleKeyFromFullKey(string key)
			{
				if(!string.IsNullOrEmpty(key))
				{
					var indexOf = key.IndexOf(':');
					if(indexOf > 0)
						return key.Substring(indexOf);
				}
				return key;
			}
		}

		/// <summary>
		/// Cria uma chave para o registro.
		/// </summary>
		/// <param name="typeName">Nome do tipo que representa os dados contidos no registro.</param>
		/// <param name="record">Instancia do registro com os dados.</param>
		/// <returns>Chave que representa o registro.</returns>
		public Query.RecordKey Create(Reflection.TypeName typeName, Query.IRecord record)
		{
			typeName.Require("typeName").NotNull();
			var generator = CreateGenerator(typeName);
			var key = generator.GetSimpleKey(record);
			long rowVersion = 0;
			var index = 0;
			for(; index < record.Descriptor.Count; index++)
				if(StringComparer.InvariantCultureIgnoreCase.Equals(record.Descriptor[index].Name, "RowVersion"))
					break;
			if(index < record.Descriptor.Count)
				rowVersion = (long)record.GetInt64(index);
			return new Query.RecordKey(key, rowVersion);
		}

		/// <summary>
		/// Recupera os nomes dos campos que representam a chave do registro.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		public IEnumerable<string> GetKeyFields(Reflection.TypeName typeName)
		{
			typeName.Require("typeName").NotNull();
			var generator = CreateGenerator(typeName) as Generator;
			return generator.FieldNames;
		}
	}
}
