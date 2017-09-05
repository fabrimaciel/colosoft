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

namespace Colosoft.Reflection.Dynamic
{
	/// <summary>
	/// Representa a assinatura de cuma classe.
	/// </summary>
	class Signature : IEquatable<Signature>
	{
		private DynamicProperty[] _properties;

		private int _hashCode;

		/// <summary>
		/// Propriedade associadas.
		/// </summary>
		public DynamicProperty[] Properties
		{
			get
			{
				return _properties;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties"></param>
		public Signature(IEnumerable<DynamicProperty> properties)
		{
			this._properties = properties.ToArray();
			_hashCode = 0;
			foreach (DynamicProperty p in properties)
				_hashCode ^= p.Name.GetHashCode() ^ p.Type.GetHashCode();
		}

		/// <summary>
		/// Recupera o hash code da instancia.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return _hashCode;
		}

		/// <summary>
		/// Compara com a instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return obj is Signature ? Equals((Signature)obj) : false;
		}

		/// <summary>
		/// Compara com outra assinatura.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Signature other)
		{
			if(_properties.Length != other._properties.Length)
				return false;
			for(int i = 0; i < _properties.Length; i++)
			{
				if(_properties[i].Name != other._properties[i].Name || _properties[i].Type != other._properties[i].Type)
					return false;
			}
			return true;
		}
	}
}
