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

namespace Colosoft.Caching.Data
{
	/// <summary>
	/// Representa o ponteiro para a enumeração dos grupos.
	/// </summary>
	public class GroupEnumerationPointer : EnumerationPointer, ICompactSerializable
	{
		private string _group;

		private string _subGroup;

		/// <summary>
		/// Nome do grupo.
		/// </summary>
		public string Group
		{
			get
			{
				return _group;
			}
			set
			{
				_group = value;
			}
		}

		/// <summary>
		/// Nome do subgrupo.
		/// </summary>
		public string SubGroup
		{
			get
			{
				return _subGroup;
			}
			set
			{
				_subGroup = value;
			}
		}

		/// <summary>
		/// Identifica se é um ponteiro para um grupo.
		/// </summary>
		public override bool IsGroupPointer
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="group">Nome do grupo.</param>
		/// <param name="subGroup">Nome do subgrupo.</param>
		public GroupEnumerationPointer(string group, string subGroup)
		{
			_group = group;
			_subGroup = subGroup;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="chunkId"></param>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		public GroupEnumerationPointer(string id, int chunkId, string group, string subGroup) : base(id, chunkId)
		{
			_group = group;
			_subGroup = subGroup;
		}

		/// <summary>
		/// Desserializa os dados na instancia.
		/// </summary>
		/// <param name="reader"></param>
		void ICompactSerializable.Deserialize(CompactReader reader)
		{
			base.Deserialize(reader);
			_group = reader.ReadString();
			_subGroup = reader.ReadString();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		void ICompactSerializable.Serialize(CompactWriter writer)
		{
			base.Serialize(writer);
			writer.Write(_group);
			writer.Write(_subGroup);
		}

		/// <summary>
		/// Compara com outra instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			bool flag = false;
			if(obj is GroupEnumerationPointer)
			{
				GroupEnumerationPointer pointer = obj as GroupEnumerationPointer;
				if(base.Equals(obj))
				{
					flag = object.Equals(_group, pointer._group) && object.Equals(_subGroup, pointer._subGroup);
				}
			}
			return flag;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
