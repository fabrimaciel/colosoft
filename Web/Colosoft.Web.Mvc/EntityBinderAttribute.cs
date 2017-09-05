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
	/// Atributo que define o vinculador de uma entidade.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
	public class EntityBinderAttribute : System.Web.Mvc.CustomModelBinderAttribute
	{
		private Type _flowType;

		private string _getMethodName;

		private string _createMethodName;

		private string[] _clearPropertyNames;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="flowType">Tipo do fluxo usado.</param>
		/// <param name="getMethodName">Nome do método que recupera a instancia.</param>
		/// <param name="createMethodName">Nome do método que cria a instancia.</param>
		/// <param name="clearPropertyNames"></param>
		public EntityBinderAttribute(Type flowType, string getMethodName = null, string createMethodName = null, string[] clearPropertyNames = null)
		{
			_flowType = flowType;
			_getMethodName = getMethodName;
			_createMethodName = createMethodName;
			_clearPropertyNames = clearPropertyNames;
		}

		/// <summary>
		/// Recupera o vinculador.
		/// </summary>
		/// <returns></returns>
		public override System.Web.Mvc.IModelBinder GetBinder()
		{
			return new EntityModelBinder(Business.EntityTypeManager.Instance, _flowType, _getMethodName, _createMethodName, _clearPropertyNames);
		}
	}
}
