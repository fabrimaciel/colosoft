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
	/// Representa um grupo de informações.
	/// </summary>
	[Serializable]
	public class GroupInfo : ICloneable, ICompactSerializable
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
				if(!string.IsNullOrEmpty(value))
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
				if(!string.IsNullOrEmpty(value))
					_subGroup = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public GroupInfo()
		{
		}

		/// <summary>
		/// Cria uma instancia já definindo o nome do grupo e do subgrupo.
		/// </summary>
		/// <param name="group"></param>
		/// <param name="subGroup"></param>
		public GroupInfo(string group, string subGroup)
		{
			if(!string.IsNullOrEmpty(group))
				_group = group;
			if(!string.IsNullOrEmpty(subGroup))
				_subGroup = subGroup;
		}

		/// <summary>
		/// Cria um clone da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new GroupInfo(_group, _subGroup);
		}

		/// <summary>
		/// Deserializa os dados da instancia.
		/// </summary>
		/// <param name="reader"></param>
		public void Deserialize(CompactReader reader)
		{
			_group = (string)reader.ReadObject();
			_subGroup = (string)reader.ReadObject();
		}

		/// <summary>
		/// Serializa os dados da instancia.
		/// </summary>
		/// <param name="writer"></param>
		public void Serialize(CompactWriter writer)
		{
			writer.WriteObject(_group);
			writer.WriteObject(_subGroup);
		}

		/// <summary>
		/// Lê os dados de um grupo da leitor informado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public static GroupInfo ReadGrpInfo(CompactReader reader)
		{
			if(reader.ReadByte() == 1)
			{
				return null;
			}
			GroupInfo info = new GroupInfo();
			info.Deserialize(reader);
			return info;
		}

		/// <summary>
		/// Registra os dados do grupo no escritor informado.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="grpInfo"></param>
		public static void WriteGrpInfo(CompactWriter writer, GroupInfo grpInfo)
		{
			byte num = 1;
			if(grpInfo == null)
			{
				writer.Write(num);
			}
			else
			{
				num = 0;
				writer.Write(num);
				grpInfo.Serialize(writer);
			}
		}
	}
}
