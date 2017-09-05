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
using Colosoft.Data.Schema;
using Colosoft.Query;

namespace Colosoft.Data.Caching
{
	/// <summary>
	/// Objeto que representa o índice para o estágio de carga do cache
	/// </summary>
	public class CacheLoaderIndex
	{
		/// <summary>
		/// Construtor padrão recebe um <see cref="System.Collections.IEnumerator"/> do tipo <see cref="ITypeMetadata"/>
		/// </summary>
		/// <param name="enumerator"><see cref="System.Collections.IEnumerator"/> do tipo <see cref="ITypeMetadata"/></param>
		public CacheLoaderIndex(IEnumerator<ITypeMetadata> enumerator)
		{
			TypeMetadataEnumerator = enumerator;
			LoadNextInstanceCount = 0;
		}

		/// <summary>
		/// IEnumerator para percorrer todos os tipos registrados
		/// </summary>
		public IEnumerator<ITypeMetadata> TypeMetadataEnumerator
		{
			get;
			private set;
		}

		/// <summary>
		/// IEnumerator para a última consulta realizada
		/// </summary>
		public IEnumerator<Record> RecordEnumerator
		{
			get;
			set;
		}

		/// <summary>
		/// Contagem das instâncias já recuperadas para uma chamada da função LoadNext
		/// </summary>
		public int LoadNextInstanceCount
		{
			get;
			set;
		}

		/// <summary>
		/// Objeto que contém a lógica para criar uma instância do tipo de record atual
		/// </summary>
		public TypeBindStrategy BindStrategy
		{
			get;
			set;
		}
	}
}
