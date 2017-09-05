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
	/// Representa um substituto de serialização sensivel ao contexto.
	/// </summary>
	public abstract class ContextSensitiveSerializationSurrogate : SerializationSurrogate
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="t">Tipo que será utilizado pela instancia.</param>
		public ContextSensitiveSerializationSurrogate(Type t) : base(t)
		{
		}

		/// <summary>
		/// Salta os dados diretamente.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="graph"></param>
		public abstract void SkipDirect(CompactBinaryReader reader, object graph);

		/// <summary>
		/// Lê os dados diretamente.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph"></param>
		public abstract void WriteDirect(CompactBinaryWriter writer, object graph);

		/// <summary>
		/// Lê os dados diretamente.
		/// </summary>
		/// <param name="reader"></param>
		/// <param name="graph"></param>
		/// <returns></returns>
		public abstract object ReadDirect(CompactBinaryReader reader, object graph);

		/// <summary>
		/// Cria um instancia do tipo associado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public virtual object Instantiate(CompactBinaryReader reader)
		{
			return base.CreateInstance();
		}

		/// <summary>
		/// Lê os dados do leitor e recupera um instancia preenchida do tipo associado.
		/// </summary>
		/// <param name="reader">Leitor onde estão os dados.</param>
		/// <returns></returns>
		public sealed override object Read(CompactBinaryReader reader)
		{
			int key = reader.ReadInt32();
			object graph = reader.Context.GetObject(key);
			if(graph == null)
			{
				bool flag = false;
				graph = this.Instantiate(reader);
				if(graph != null)
				{
					reader.Context.RememberObject(graph);
					flag = true;
				}
				graph = this.ReadDirect(reader, graph);
				if(!flag)
					reader.Context.RememberObject(graph);
			}
			return graph;
		}

		/// <summary>
		/// Salva os dados do objeto do leitor informado.
		/// </summary>
		/// <param name="reader">Leitor onde estão os dados.</param>
		public sealed override void Skip(CompactBinaryReader reader)
		{
			int key = reader.ReadInt32();
			if(reader.Context.GetObject(key) == null)
			{
				object graph = this.Instantiate(reader);
				this.SkipDirect(reader, graph);
			}
		}

		/// <summary>
		/// Salva os dados do objeto no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph">Instancia que será registrada no escritor.</param>
		public sealed override void Write(CompactBinaryWriter writer, object graph)
		{
			int cookie = writer.Context.GetCookie(graph);
			if(cookie != -1)
				writer.Write(cookie);
			else
			{
				cookie = writer.Context.RememberObject(graph);
				writer.Write(cookie);
				this.WriteDirect(writer, graph);
			}
		}
	}
}
