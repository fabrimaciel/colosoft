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
	/// Implementação do provedor de binder para entidade.
	/// </summary>
	public class EntityModelBinderProvider : System.Web.Mvc.IModelBinderProvider
	{
		private Colosoft.Query.ISourceContext _sourceContext;

		private Colosoft.Business.IEntityTypeManager _entityTypeManager;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="sourceContext"></param>
		/// <param name="entityTypeManager"></param>
		public EntityModelBinderProvider(Colosoft.Business.IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext)
		{
			_sourceContext = sourceContext;
			_entityTypeManager = entityTypeManager;
		}

		/// <summary>
		/// Recupera o binder para o tipo informado.
		/// </summary>
		/// <param name="modelType"></param>
		/// <returns></returns>
		public System.Web.Mvc.IModelBinder GetBinder(Type modelType)
		{
			if(typeof(Colosoft.Business.Entity).IsAssignableFrom(modelType))
			{
				var modelBinderType = typeof(EntityModelBinder<>).MakeGenericType(modelType);
				var modelBinder = Activator.CreateInstance(modelBinderType, new object[] {
					_entityTypeManager,
					_sourceContext
				});
				return (System.Web.Mvc.IModelBinder)modelBinder;
			}
			else if(modelType.IsGenericType && modelType.GetGenericTypeDefinition().FindInterfaces((f, c) => f == typeof(Colosoft.Business.IEntityList), null).Any())
			{
				return new EntityCollectionModelBinder();
			}
			return null;
		}
	}
}
