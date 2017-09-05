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
using System.Text;
using System.Collections;
using System.Xml;

namespace Colosoft.WebControls.dhtmlx
{
	public class dhtmlxAttributeCollection : IEnumerable
	{
		private System.Web.UI.StateBag _stateBag;

		/// <summary>
		/// Número de atributos na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _stateBag.Count;
			}
		}

		/// <summary>
		/// Chaves da coleção.
		/// </summary>
		public System.Collections.ICollection Keys
		{
			get
			{
				return _stateBag.Keys;
			}
		}

		/// <summary>
		/// Recupera o valor do atributo.
		/// </summary>
		/// <param name="key">Chave do atributo.</param>
		/// <returns>Valor do atributo.</returns>
		public string this[string key]
		{
			get
			{
				return (string)_stateBag[key];
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public dhtmlxAttributeCollection()
		{
			_stateBag = new System.Web.UI.StateBag(true);
		}

		/// <summary>
		/// Adiciona o atributo na coleção.
		/// </summary>
		/// <param name="key">Nome do atributo.</param>
		/// <param name="value">Valor do atributo.</param>
		public void Add(string key, string value)
		{
			_stateBag.Add(key, value);
		}

		/// <summary>
		/// Remove o atributo da coleção.
		/// </summary>
		/// <param name="key">Nome do atributo.</param>
		public void Remove(string key)
		{
			_stateBag.Remove(key);
		}

		/// <summary>
		/// Carrega os atributos no elemento XML.
		/// </summary>
		/// <param name="element"></param>
		internal void LoadAttributes(XmlElement element)
		{
			foreach (object obj in _stateBag)
			{
				System.Collections.DictionaryEntry entry = (System.Collections.DictionaryEntry)obj;
				element.SetAttribute((string)entry.Key, (string)((System.Web.UI.StateItem)entry.Value).Value);
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _stateBag.GetEnumerator();
		}
	}
}
