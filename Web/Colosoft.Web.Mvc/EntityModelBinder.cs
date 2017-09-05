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
	/// Representa a vinculação de dados com a entidade.
	/// </summary>
	class EntityModelBinder : EntityModelBinderBase
	{
		private Type _flowType;

		private string _getMethodName;

		private string _createMethodName;

		private string[] _clearPropertyNames;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entityTypeManager"></param>
		/// <param name="flowType"></param>
		/// <param name="getMethodName"></param>
		/// <param name="createMethodName"></param>
		/// <param name="clearPropertyNames">Nome das propriedades que serão limpas na vinculação.</param>
		public EntityModelBinder(Business.IEntityTypeManager entityTypeManager, Type flowType, string getMethodName, string createMethodName, string[] clearPropertyNames) : base(entityTypeManager)
		{
			_flowType = flowType;
			_getMethodName = getMethodName;
			_createMethodName = createMethodName;
			_clearPropertyNames = clearPropertyNames ?? new string[0];
		}

		/// <summary>
		/// Recupera a instancia do fluxo.
		/// </summary>
		/// <returns></returns>
		protected virtual object GetFlowInstance()
		{
			return Microsoft.Practices.ServiceLocation.ServiceLocator.Current.GetInstance(_flowType);
		}

		/// <summary>
		/// Cria a model que será usada na vinculação.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <param name="modelType"></param>
		/// <returns></returns>
		protected override object CreateModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext, Type modelType)
		{
			var getMethodName = _getMethodName ?? string.Format("Get{0}", modelType.Name);
			var methods = _flowType.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
			var getMethods = methods.Where(f => f.Name == getMethodName).ToArray();
			var loader = EntityTypeManager.GetLoader(modelType);
			System.Reflection.MethodInfo getMethod = null;
			if(getMethods.Length == 1)
				getMethod = getMethods[0];
			else if(loader.HasUid)
			{
				foreach (var method in getMethods)
				{
					var methodParameters = method.GetParameters();
					if(methodParameters.Length == 1 && methodParameters[0].ParameterType == typeof(int))
					{
						getMethod = method;
						break;
					}
				}
			}
			if(getMethod == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityBinder_GetMethodNotFound, getMethodName, _flowType.FullName).Format());
			var descriptor = GetTypeDescriptor(controllerContext, bindingContext);
			var record = GetRecordOfKey(controllerContext, bindingContext, modelType);
			var flow = GetFlowInstance();
			if(loader.HasUid && record.GetInt32(loader.UidPropertyName) <= 0)
				return CreateEntity(flow, modelType);
			object[] parameters = null;
			if(record.Descriptor.Count == 1)
				parameters = new object[] {
					record.GetInt32(0)
				};
			else
			{
				var methodParameters = getMethod.GetParameters();
				parameters = new object[methodParameters.Length];
				for(var i = 0; i < parameters.Length; i++)
				{
					var key = record.Descriptor.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, methodParameters[i].Name));
					if(key == null && methodParameters[i].Name == "id" && loader.HasUid)
						key = record.Descriptor.FirstOrDefault(f => StringComparer.InvariantCultureIgnoreCase.Equals(f.Name, loader.UidPropertyName));
					if(key != null)
						parameters[i] = Colosoft.Reflection.TypeConverter.Get(methodParameters[i].ParameterType, record.GetValue(key.Name));
					;
				}
			}
			Business.IEntity entity = null;
			try
			{
				entity = (Business.IEntity)getMethod.Invoke(flow, parameters);
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
			if(entity == null)
				entity = CreateEntity(flow, modelType);
			if(entity.Uid == 0)
				entity.Uid = entity.TypeManager.GenerateInstanceUid(modelType);
			return entity;
		}

		/// <summary>
		/// Realiza a vinculação dos dados.
		/// </summary>
		/// <param name="controllerContext"></param>
		/// <param name="bindingContext"></param>
		/// <returns></returns>
		public override object BindModel(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext)
		{
			var properties = controllerContext.Controller.ViewData.ContainsKey(EntityModelMetadataProvider.ClearPropertyNamesKey) ? controllerContext.Controller.ViewData[EntityModelMetadataProvider.ClearPropertyNamesKey] as IEnumerable<string> : null;
			if(properties == null)
				controllerContext.Controller.ViewData.Add(EntityModelMetadataProvider.ClearPropertyNamesKey, _clearPropertyNames);
			else
				controllerContext.Controller.ViewData[EntityModelMetadataProvider.ClearPropertyNamesKey] = properties.Concat(_clearPropertyNames);
			var entity = (Colosoft.Business.IEntity)base.BindModel(controllerContext, bindingContext);
			if(entity != null && entity.Uid == 0)
				entity.Uid = entity.TypeManager.GenerateInstanceUid(entity.GetType());
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
		/// <param name="flow"></param>
		/// <param name="modelType"></param>
		/// <returns></returns>
		private Business.IEntity CreateEntity(object flow, Type modelType)
		{
			var createMethodName = _createMethodName ?? string.Format("Create{0}", modelType.Name);
			var createMethod = _flowType.GetMethod(createMethodName);
			if(createMethod == null)
				throw new InvalidOperationException(ResourceMessageFormatter.Create(() => Properties.Resources.EntityBinder_CreatedMethodNotFound, createMethodName, _flowType.FullName).Format());
			try
			{
				return (Business.IEntity)createMethod.Invoke(flow, null);
			}
			catch(System.Reflection.TargetInvocationException ex)
			{
				throw ex.InnerException;
			}
		}
	}
}
