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
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Collections
{
	/// <summary>
	/// Representa um dicionário de entradas Json.
	/// </summary>
	[Serializable]
	public class JsonDictionary : ISerializable
	{
		private Dictionary<string, object> _entries;

		/// <summary>
		/// Entradas.
		/// </summary>
		public IEnumerable<KeyValuePair<string, object>> Entries
		{
			get
			{
				return _entries;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public JsonDictionary()
		{
			_entries = new Dictionary<string, object>();
		}

		/// <summary>
		/// Cria a instancia com base no dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected JsonDictionary(SerializationInfo info, StreamingContext context)
		{
			_entries = new Dictionary<string, object>();
			foreach (var entry in info)
			{
				_entries.Add(entry.Name, entry.Value);
			}
		}

		/// <summary>
		/// Recupera os dados serializados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			foreach (var entry in _entries)
				info.AddValue(entry.Key, entry.Value);
		}
	///// <summary>
	///// Adiciona uma nova entrada.
	///// </summary>
	///// <param name="key"></param>
	///// <param name="value"></param>
	///// <summary>
	///// Verifica se contém a chave informada.
	///// </summary>
	///// <param name="key"></param>
	///// <returns></returns>
	///// <summary>
	///// Chaves.
	///// </summary>
	///// <summary>
	///// Remove a entrada pela chave informada.
	///// </summary>
	///// <param name="key"></param>
	///// <returns></returns>
	///// <summary>
	///// Tenta recupera o valor.
	///// </summary>
	///// <param name="key"></param>
	///// <param name="value"></param>
	///// <returns></returns>
	///// <summary>
	///// Valores.
	///// </summary>
	///// <summary>
	///// Recupera e define a entrada pela chave informada.
	///// </summary>
	///// <param name="key"></param>
	///// <returns></returns>
	///// <summary>
	///// Adiciona uma entrada.
	///// </summary>
	///// <param name="item"></param>
	///// <summary>
	///// Limpa os itens.
	///// </summary>
	///// <summary>
	///// Verifica se contém o item informado.
	///// </summary>
	///// <param name="item"></param>
	///// <returns></returns>
	///// <summary>
	///// Copia para.
	///// </summary>
	///// <param name="array"></param>
	///// <param name="arrayIndex"></param>
	///// <summary>
	///// Quantidade de itens.
	///// </summary>
	///// <summary>
	///// Identifica se é somente leitura.
	///// </summary>
	///// <summary>
	///// Remove o item informado.
	///// </summary>
	///// <param name="item"></param>
	///// <returns></returns>
	///// <summary>
	///// Recupera o enumerator dos itens.
	///// </summary>
	///// <returns></returns>
	}
}
