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

namespace Colosoft.Web.Mvc
{
	/// <summary>
	/// Informações do controller.
	/// </summary>
	public class ControllerInfo
	{
		private Type _controllerType;

		private string _area;

		private string _name;

		/// <summary>
		/// Tipo do controller.
		/// </summary>
		public Type ControllerType
		{
			get
			{
				return _controllerType;
			}
		}

		/// <summary>
		/// Área do controler.
		/// </summary>
		public string Area
		{
			get
			{
				return _area;
			}
		}

		/// <summary>
		/// Nome do controller.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="controllerType"></param>
		/// <param name="name"></param>
		/// <param name="area"></param>
		public ControllerInfo(Type controllerType, string name, string area = null)
		{
			_controllerType = controllerType;
			_area = area;
			_name = name;
		}
	}
	/// <summary>
	/// Assinatura de um provedor de controllers.
	/// </summary>
	public interface IControllerProvider
	{
		/// <summary>
		/// Recupera as tuplas com o nome do controller e o tipo.
		/// </summary>
		/// <returns></returns>
		IEnumerable<ControllerInfo> GetControllers();
	}
}
