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

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Representa um substituto de serialização.
	/// </summary>
	public interface ISerializationSurrogate
	{
		/// <summary>
		/// Tipo atual associado.
		/// </summary>
		Type ActualType
		{
			get;
		}

		/// <summary>
		/// Identificador do manipulador do sub-tipo.
		/// </summary>
		short SubTypeHandle
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do manipulador do tipo.
		/// </summary>
		short TypeHandle
		{
			get;
			set;
		}

		/// <summary>
		/// Recupera a instancia de um objeto contido no leitor informado.
		/// </summary>
		/// <param name="reader">Leitor onde estão os dados.</param>
		/// <returns>Instancia do objeto recuperado.</returns>
		object Read(CompactBinaryReader reader);

		/// <summary>
		/// Salva os dados do objeto do leitor informado.
		/// </summary>
		/// <param name="reader">Leitor onde estão os dados.</param>
		void Skip(CompactBinaryReader reader);

		/// <summary>
		/// Salva os dados do objeto no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph">Instancia que será registrada no escritor.</param>
		void Write(CompactBinaryWriter writer, object graph);
	}
}
