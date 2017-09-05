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
using Colosoft.Serialization;
using Colosoft.Serialization.IO;

namespace Colosoft.Caching.Expiration
{
	/// <summary>
	/// Representa um dica de expiração para dependencia.
	/// </summary>
	[Serializable]
	public abstract class DependencyHint : ExpirationHint, ICompactSerializable
	{
		/// <summary>
		/// Identifica apartir de qual data a dependencia irá valer.
		/// </summary>
		protected int _startAfter;

		/// <summary>
		/// Identifica se a instancia foi alterada.
		/// </summary>
		public abstract bool HasChanged
		{
			get;
		}

		/// <summary>
		/// Chave de ordenação.
		/// </summary>
		internal sealed override int SortKey
		{
			get
			{
				return _startAfter;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		protected DependencyHint()
		{
			base.HintType = ExpirationHintType.DependencyHint;
			_startAfter = CachingUtils.DiffSeconds(DateTime.Now);
		}

		/// <summary>
		/// Cria uma instancia com data de inicio.
		/// </summary>
		/// <param name="startAfter"></param>
		protected DependencyHint(DateTime startAfter)
		{
			_startAfter = CachingUtils.DiffSeconds(startAfter);
		}

		/// <summary>
		/// Verifica se a instancia já expirou no contexto.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool CheckExpired(CacheRuntimeContext context)
		{
			return this.DetermineExpiration(context);
		}

		/// <summary>
		/// Determina da expiração no contexto informado.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		internal override bool DetermineExpiration(CacheRuntimeContext context)
		{
			if(_startAfter.CompareTo(CachingUtils.DiffSeconds(DateTime.Now)) > 0)
				return false;
			if(!base.HasExpired && HasChanged)
				base.NotifyExpiration(this, null);
			return base.HasExpired;
		}

		/// <summary>
		/// Deserializa os dados para a instancia.
		/// </summary>
		/// <param name="reader"></param>
		public override void Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_startAfter = reader.ReadInt32();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public override void Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_startAfter);
		}
	}
}
