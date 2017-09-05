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
using System.Text;
using System.Collections;

namespace Colosoft.WebControls.Route
{
	/// <summary>
	/// Armazna oas informações para navegar na rota.
	/// </summary>
	public class RouteInfoNavigate
	{
		private Guid _uniqueID;

		private string _name;

		private RouteInfoNavigate _parent;

		private Hashtable _childrens = new Hashtable();

		private RouteInfo _info;

		private int _complexPartsCount;

		private string _complexRegex;

		/// <summary>
		/// Identificador único da instancia.
		/// </summary>
		public Guid UniqueID
		{
			get
			{
				return _uniqueID;
			}
		}

		/// <summary>
		/// Nome da navegação.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Nome completo da navegação.
		/// </summary>
		public string FullName
		{
			get
			{
				StringBuilder sb = new StringBuilder(_name);
				RouteInfoNavigate p = _parent;
				while (p != null)
				{
					sb.Insert(0, p.Name + "/");
					p = p.Parent;
				}
				return sb.ToString();
			}
		}

		/// <summary>
		/// Nome da navegação pai.
		/// </summary>
		internal RouteInfoNavigate Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		/// <summary>
		/// Filhos da navegação.
		/// </summary>
		public Hashtable Childrens
		{
			get
			{
				return _childrens;
			}
		}

		/// <summary>
		/// Informações da rota relacionada com a navegação.
		/// </summary>
		public RouteInfo Info
		{
			get
			{
				return _info;
			}
			set
			{
				_info = value;
			}
		}

		/// <summary>
		/// Quantidade de partes complexas no roteamento.
		/// </summary>
		public int ComplexPartsCount
		{
			get
			{
				return _complexPartsCount;
			}
			set
			{
				_complexPartsCount = value;
			}
		}

		/// <summary>
		/// Expressão regular usada para recuperar as partes complexas.
		/// </summary>
		public string ComplexRegex
		{
			get
			{
				return _complexRegex;
			}
			set
			{
				_complexRegex = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="parent"></param>
		/// <param name="info"></param>
		public RouteInfoNavigate(string name, RouteInfoNavigate parent, RouteInfo info)
		{
			_uniqueID = Guid.NewGuid();
			_name = name;
			_parent = parent;
			_info = info;
		}
	}
}
