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
using System.Runtime.Serialization;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação do substituto de serializa para um instancia do tipo <see cref="ICompactSerializable"/>.
	/// </summary>
	internal sealed class ICompactSerializableSerializationSurrogate : ContextSensitiveSerializationSurrogate
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type"></param>
		public ICompactSerializableSerializationSurrogate(Type type) : base(type)
		{
		}

		/// <summary>
		/// Cria uma nova instancia para o objeto.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
		public override object Instantiate(CompactBinaryReader reader)
		{
			object instance = null;
			if(reader.Context.MemManager != null)
				instance = this.GetInstance(reader.Context.MemManager);
			if(instance == null)
			{
				instance = Activator.CreateInstance(base.ActualType);
			}
			return instance;
		}

		public override object ReadDirect(CompactBinaryReader reader, object graph)
		{
			((ICompactSerializable)graph).Deserialize(reader);
			return graph;
		}

		public override void SkipDirect(CompactBinaryReader reader, object graph)
		{
			((ICompactSerializable)graph).Deserialize(reader);
		}

		public override void WriteDirect(CompactBinaryWriter writer, object graph)
		{
			((ICompactSerializable)graph).Serialize(writer);
		}
	}
}
