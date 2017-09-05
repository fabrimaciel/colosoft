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
using System.Collections;

namespace Colosoft.Caching
{
	/// <summary>
	/// Classe que armazena informações de metadados.
	/// </summary>
	[Serializable]
	internal class MetaInformation
	{
		private Hashtable _attributeValues;

		private string _cacheKey;

		private string _type;

		/// <summary>
		/// Valores dos atributos da instancia.
		/// </summary>
		public Hashtable AttributeValues
		{
			get
			{
				if(_attributeValues == null)
				{
					return _attributeValues;
				}
				Hashtable hashtable = new Hashtable();
				foreach (DictionaryEntry entry in _attributeValues)
				{
					if(entry.Value is string)
					{
						hashtable.Add(entry.Key, ((string)entry.Value).ToLower());
					}
					else
					{
						hashtable.Add(entry.Key, entry.Value);
					}
				}
				return hashtable;
			}
		}

		/// <summary>
		/// Chave do cache.
		/// </summary>
		public string CacheKey
		{
			get
			{
				return _cacheKey;
			}
			set
			{
				_cacheKey = value;
			}
		}

		/// <summary>
		/// Texto que representa o tipo da informação.
		/// </summary>
		public string Type
		{
			get
			{
				return _type;
			}
			set
			{
				_type = value;
			}
		}

		/// <summary>
		/// Recupera o valor da atributo indexado.
		/// </summary>
		/// <param name="key">Nome do atributo.</param>
		/// <returns></returns>
		public object this[string key]
		{
			get
			{
				if(_attributeValues != null)
					return _attributeValues[key];
				return null;
			}
			set
			{
				if(_attributeValues == null)
					_attributeValues = new Hashtable();
				_attributeValues[key] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="attributeValues">Atributos para iniciar os valores da instancia.</param>
		internal MetaInformation(Hashtable attributeValues)
		{
			_attributeValues = attributeValues;
		}

		/// <summary>
		/// Verifica se o atributo com o nome informado é indexado pela instancia.
		/// </summary>
		/// <param name="attribName">Nome do atributo que será verificado.</param>
		/// <returns></returns>
		public bool IsAttributeIndexed(string attribName)
		{
			return _attributeValues.ContainsKey(attribName);
		}

		/// <summary>
		/// Adiciona um conjunto de atributos para a instancia.
		/// </summary>
		/// <param name="attributeValues">Instancia com os atributos que serão adicionados.</param>
		public void Add(Hashtable attributeValues)
		{
			foreach (DictionaryEntry entry in attributeValues)
				_attributeValues.Add(entry.Key, entry.Value);
		}

		/// <summary>
		/// Compara a instancia o objeto informado.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool flag = false;
			MetaInformation information = obj as MetaInformation;
			if(information != null)
				flag = CacheKey.Equals(information.CacheKey);
			return flag;
		}

		/// <summary>
		/// Recupera a hash code da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this.CacheKey.GetHashCode();
		}
	}
}
