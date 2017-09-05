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
using System.Reflection;
using Colosoft.Runtime;
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação básica do substituto de serialização.
	/// </summary>
	public class SerializationSurrogate : ISerializationSurrogate
	{
		private short _handle;

		private short _subTypeHandle;

		private Type _type;

		/// <summary>
		/// Tipo associado.
		/// </summary>
		public Type ActualType
		{
			get
			{
				return _type;
			}
		}

		/// <summary>
		/// Identificador do manipulador do sub-tipo.
		/// </summary>
		public short SubTypeHandle
		{
			get
			{
				return _subTypeHandle;
			}
			set
			{
				_subTypeHandle = value;
			}
		}

		/// <summary>
		/// Identificador do manipulador do tipo.
		/// </summary>
		public short TypeHandle
		{
			get
			{
				return _handle;
			}
			set
			{
				_handle = value;
			}
		}

		/// <summary>
		/// Cria uma nova instancia definindo o tipo que será tratado pela instancia.
		/// </summary>
		/// <param name="t">Tipo que será trato pela instancia.</param>
		public SerializationSurrogate(Type t)
		{
			_type = t;
		}

		/// <summary>
		/// Cria uma instancia do tipo associado.
		/// </summary>
		/// <returns></returns>
		public virtual object CreateInstance()
		{
			return Activator.CreateInstance(this.ActualType, BindingFlags.CreateInstance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, null, null, null);
		}

		/// <summary>
		/// Recupera a instancia associada do gerenciador informado.
		/// </summary>
		/// <param name="objManager">Gerenciador de objetos de onde a instancia será recuperada.</param>
		/// <returns></returns>
		public virtual object GetInstance(MemoryManager objManager)
		{
			object obj2 = null;
			if(objManager != null)
			{
				var provider = objManager.GetProvider(this.ActualType);
				if(provider != null)
					obj2 = provider.RentAnObject();
			}
			return obj2;
		}

		/// <summary>
		/// Recupera a instancia de um objeto contido no leitor informado.
		/// </summary>
		/// <param name="reader">Leitor onde estão os dados.</param>
		/// <returns>Instancia do objeto recuperado.</returns>
		public virtual object Read(CompactBinaryReader reader)
		{
			return null;
		}

		/// <summary>
		/// Salva os dados do objeto do leitor informado.
		/// </summary>
		/// <param name="reader">Leitor onde estão os dados.</param>
		public virtual void Skip(CompactBinaryReader reader)
		{
		}

		/// <summary>
		/// Salva os dados do objeto no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph">Instancia que será registrada no escritor.</param>
		public virtual void Write(CompactBinaryWriter writer, object graph)
		{
		}
	}
}
