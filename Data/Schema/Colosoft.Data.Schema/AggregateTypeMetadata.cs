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
	/// Representa um agregador de metadados de tipos.
	/// </summary>
	public class AggregateTypeMetadata : ITypeMetadata
	{
		private IEnumerable<ITypeMetadata> _types;

		/// <summary>
		/// Código do tipo.
		/// </summary>
		public int TypeCode
		{
			get
			{
				return _types.First().TypeCode;
			}
			set
			{
				var typeMetadata = _types.First();
				if(typeMetadata is Local.TypeMetadata)
					((Local.TypeMetadata)typeMetadata).TypeCode = value;
			}
		}

		/// <summary>
		/// Nome do tipo.
		/// </summary>
		public string Name
		{
			get
			{
				return _types.First().Name;
			}
		}

		/// <summary>
		/// Espaço de nome onde o tipo está inserido.
		/// </summary>
		public string Namespace
		{
			get
			{
				return _types.First().Namespace;
			}
		}

		/// <summary>
		/// Nome completo do tipo.
		/// </summary>
		public string FullName
		{
			get
			{
				return _types.First().FullName;
			}
		}

		/// <summary>
		/// Nome do assembly onde o tipo está inserido.
		/// </summary>
		public string Assembly
		{
			get
			{
				return _types.First().Assembly;
			}
		}

		/// <summary>
		/// Nome da tabela associada.
		/// </summary>
		public TableName TableName
		{
			get
			{
				return _types.First().TableName;
			}
		}

		/// <summary>
		/// Recupera os dados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será recuperada.</param>
		/// <returns></returns>
		public IPropertyMetadata this[string propertyName]
		{
			get
			{
				foreach (var i in _types)
				{
					var result = i[propertyName];
					if(result != null)
						return result;
				}
				return null;
			}
		}

		/// <summary>
		/// Recupera a quantidade de propriedades da tabela.
		/// </summary>
		public int Count
		{
			get
			{
				return _types.Sum(f => f.Count);
			}
		}

		/// <summary>
		/// Define se o tipo pode ser persistido em cache.
		/// </summary>
		public bool IsCache
		{
			get
			{
				return _types.First().IsCache;
			}
		}

		/// <summary>
		/// Define se a coluna é versionada ou não.
		/// </summary>
		public bool IsVersioned
		{
			get
			{
				return _types.First().IsVersioned;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="types">Tipos que serão agregados.</param>
		public AggregateTypeMetadata(IEnumerable<ITypeMetadata> types)
		{
			types.Require("types").NotNull();
			_types = types;
		}

		/// <summary>
		/// Recupera os metadados da propriedade com base no código informado.
		/// </summary>
		/// <param name="propertyCode"></param>
		/// <returns></returns>
		public IPropertyMetadata GetProperty(int propertyCode)
		{
			foreach (var i in _types)
			{
				var result = i.GetProperty(propertyCode);
				if(result != null)
					return result;
			}
			return null;
		}

		/// <summary>
		/// Recupera as propriedades chave do tipo.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<IPropertyMetadata> GetKeyProperties()
		{
			foreach (var i in _types)
				foreach (var j in i.GetKeyProperties())
					yield return j;
		}

		/// <summary>
		/// Recupera todas as propriedades que são recuperadas após a consulta.
		/// </summary>
		/// <returns>Propriedades voláteis</returns>
		public IEnumerable<IPropertyMetadata> GetVolatileProperties()
		{
			foreach (var i in _types)
				foreach (var j in i.GetVolatileProperties())
					yield return j;
		}

		/// <summary>
		/// Tenta recupera os metadados da propriedade pelo nome informado.
		/// </summary>
		/// <param name="propertyName">Nome da propriedade que será pesquisada.</param>
		/// <param name="propertyMetadata">Metadados da propriedade encontrada.</param>
		/// <returns></returns>
		public bool TryGet(string propertyName, out IPropertyMetadata propertyMetadata)
		{
			foreach (var i in _types)
			{
				if(i.TryGet(propertyName, out propertyMetadata))
					return true;
			}
			propertyMetadata = null;
			return false;
		}

		/// <summary>
		/// Recupera as proprieades do tipo.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IPropertyMetadata> GetEnumerator()
		{
			foreach (var i in _types)
				foreach (var j in i)
					yield return j;
		}

		/// <summary>
		/// Recupera as proprieades do tipo.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			foreach (var i in _types)
				foreach (var j in i)
					yield return j;
		}
	}
}
