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
using System.Threading.Tasks;

namespace Colosoft.Web.Mvc.Infrastructure
{
	/// <summary>
	/// Representa a assinatura de uma classe.
	/// </summary>
	class Signature : IEquatable<Signature>
	{
		public int _hashCode;

		public Implementation.DynamicProperty[] _properties;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="properties"></param>
		public Signature(IEnumerable<Implementation.DynamicProperty> properties)
		{
			_properties = properties.ToArray();
			_hashCode = 0;
			foreach (var property in properties)
			{
				this._hashCode ^= property.Name.GetHashCode() ^ property.Type.GetHashCode();
			}
		}

		/// <summary>
		/// Verifica se a assinatura é iguala assinatura informada.
		/// </summary>
		/// <param name="other"></param>
		/// <returns></returns>
		public bool Equals(Signature other)
		{
			if(_properties.Length != other._properties.Length)
			{
				return false;
			}
			for(int i = 0; i < _properties.Length; i++)
			{
				if((_properties[i].Name != other._properties[i].Name) || (_properties[i].Type != other._properties[i].Type))
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Verifica se a instancia é igual a instancia informada.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			return ((obj is Signature) && this.Equals((Signature)obj));
		}

		/// <summary>
		/// Recupera o hashcode que representa a assinatura.
		/// </summary>
		/// <returns></returns>
		public override int GetHashCode()
		{
			return this._hashCode;
		}
	}
}
