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
	/// Implementação base do vinculador de entidades.
	/// </summary>
	class EntityModelBinder<TEntity> : EntityModelBinderBase
	{
		private Colosoft.Query.ISourceContext _sourceContext;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityTypeManager"></param>
		/// <param name="sourceContext"></param>
		public EntityModelBinder(Colosoft.Business.IEntityTypeManager entityTypeManager, Colosoft.Query.ISourceContext sourceContext) : base(entityTypeManager)
		{
			_sourceContext = sourceContext;
		}

		/// <summary>
		/// Cria uma instancia da modelo para realizar a vinculação dos dados.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="modelType"></param>
		/// <returns></returns>
		protected override object CreateModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, Type modelType)
		{
			var entity = CreateEntity(modelType);
			if(entity.Uid == 0)
				entity.Uid = entity.TypeManager.GenerateInstanceUid(modelType);
			return entity;
		}

		/// <summary>
		/// Cria um nome para a propriedade da model.
		/// </summary>
		/// <param name="prefix"></param>
		/// <param name="propertyName"></param>
		/// <returns></returns>
		private static string CreatePropertyModelName(string prefix, string propertyName)
		{
			if(string.IsNullOrEmpty(prefix))
				return propertyName ?? String.Empty;
			else if(string.IsNullOrEmpty(propertyName))
				return prefix ?? string.Empty;
			else
				return prefix + "." + propertyName;
		}

		/// <summary>
		/// Cria a entidade.
		/// </summary>
		/// <param name="entityType"></param>
		/// <returns></returns>
		protected virtual Business.IEntity CreateEntity(Type entityType)
		{
			Colosoft.Business.IEntity entity = null;
			try
			{
				entity = (Colosoft.Business.IEntity)Activator.CreateInstance(entityType);
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			if(entity is Colosoft.Business.IConnectedEntity)
				((Colosoft.Business.IConnectedEntity)entity).Connect(_sourceContext);
			return entity;
		}
	}
}
