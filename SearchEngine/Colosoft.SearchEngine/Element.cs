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

namespace Colosoft.SearchEngine
{
	/// <summary>
	/// Elementos que retornarão da base de dados
	/// </summary>
	public class Element
	{
		private object[] _values;

		/// <summary>
		/// Construtor de elementos
		/// </summary>
		/// <param name="uid">Id do elemento</param>
		/// <param name="channelId">Id do canal</param>
		/// <param name="values">campos do elemento</param>
		public Element(int uid, byte channelId, params object[] values)
		{
			Uid = uid;
			ChannelId = channelId;
			Deleted = false;
			_values = values;
		}

		/// <summary>
		/// Identificador do elemento
		/// </summary>
		public int Uid
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do canal
		/// </summary>
		public byte ChannelId
		{
			get;
			set;
		}

		/// <summary>
		/// Lista de valores do elemento
		/// </summary>
		public object[] Values
		{
			get
			{
				return _values;
			}
			set
			{
				_values = value;
			}
		}

		/// <summary>
		/// Informa se o elemento está ou não deletado
		/// </summary>
		public bool Deleted
		{
			get;
			set;
		}

		/// <summary>
		/// Retorna o código hash
		/// </summary>
		/// <returns>Uid</returns>
		public override int GetHashCode()
		{
			return Uid;
		}

		/// <summary>
		/// Verifica se os elementos são iguais
		/// </summary>
		/// <param name="obj">elemento</param>
		/// <returns>verdadeiro se os Uid forem iguais</returns>
		public override bool Equals(object obj)
		{
			if(obj is Element)
				return Uid.Equals(((Element)obj).Uid);
			return false;
		}
	}
}
