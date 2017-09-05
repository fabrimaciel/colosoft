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
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Representa uma dica de expiração para a dependecia de uma chave.
	/// </summary>
	[Serializable]
	public class KeyDependency : DependencyHint
	{
		private string[] _cacheKeys;

		private long _startAfterTicks;

		/// <summary>
		/// Chaves do cache.
		/// </summary>
		public string[] CacheKeys
		{
			get
			{
				return _cacheKeys;
			}
		}

		/// <summary>
		/// Identifica se a instancia foi alterada.
		/// </summary>
		public override bool HasChanged
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Ticks
		/// </summary>
		internal long StartAfterTicks
		{
			get
			{
				return _startAfterTicks;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public KeyDependency()
		{
			base.HintType = ExpirationHintType.KeyDependency;
		}

		/// <summary>
		/// Cria uma instancia já com o valor da chave.
		/// </summary>
		/// <param name="key"></param>
		public KeyDependency(string key) : this(new string[] {
			key
		}, DateTime.Now)
		{
			base.HintType = ExpirationHintType.KeyDependency;
		}

		/// <summary>
		/// Cria a instancia informando as chaves associadas.
		/// </summary>
		/// <param name="keys">Chaves que serão associadas.</param>
		public KeyDependency(string[] keys) : this(keys, DateTime.Now)
		{
			base.HintType = ExpirationHintType.KeyDependency;
		}

		/// <summary>
		/// Cria a instancia adicionando a chave e a data de inicio da dependencia.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="startAfter"></param>
		public KeyDependency(string key, DateTime startAfter) : this(new string[] {
			key
		}, startAfter)
		{
			base.HintType = ExpirationHintType.KeyDependency;
		}

		/// <summary>
		/// Cria a instancia adicionando as chaves e a data de inicio da dependencia.
		/// </summary>
		/// <param name="keys"></param>
		/// <param name="startAfter"></param>
		public KeyDependency(string[] keys, DateTime startAfter) : base(startAfter)
		{
			base.HintType = ExpirationHintType.KeyDependency;
			_cacheKeys = keys;
			_startAfterTicks = startAfter.Ticks;
		}

		/// <summary>
		/// Verifica se já está expirado no contexto informado.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool CheckExpired(CacheRuntimeContext context)
		{
			return this.DetermineExpiration(context);
		}

		/// <summary>
		/// Determina a expiração para o contexto.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool DetermineExpiration(CacheRuntimeContext context)
		{
			if(_startAfter.CompareTo(CachingUtils.DiffSeconds(DateTime.Now)) > 0)
				return false;
			return base.HasExpired;
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_cacheKeys = (string[])reader.ReadObject();
			_startAfterTicks = reader.ReadInt64();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.WriteObject(_cacheKeys);
			writer.Write(_startAfterTicks);
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			string str = "KEYDEPENDENCY \"";
			for(int i = 0; i < _cacheKeys.Length; i++)
				str = str + _cacheKeys[i] + "\"";
			return (str + "STARTAFTER\"" + _startAfterTicks.ToString() + "\"\r\n");
		}
	}
}
