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

namespace Colosoft.Mef
{
	/// <summary>
	/// Representa um parametro 
	/// </summary>
	class ImportingConstructorParameterDescription : ImportDescription
	{
		/// <summary>
		/// Nome do parametro.
		/// </summary>
		public string Name
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Representa a descrição da importação do construtor.
	/// </summary>
	class ImportingConstructorDescription : ImportDescription
	{
		private Colosoft.Reflection.TypeName _typeName;

		private Type _type;

		private ImportingConstructorParameterDescription[] _parameters;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeName">Tipo que contem o construtor.</param>
		/// <param name="parameters"></param>        
		public ImportingConstructorDescription(Colosoft.Reflection.TypeName typeName, ImportingConstructorParameterDescription[] parameters)
		{
			typeName.Require("type").NotNull();
			_typeName = typeName;
			_parameters = parameters;
		}

		/// <summary>
		/// Recupera os parametros.
		/// </summary>
		/// <param name="assemblyContainer"></param>
		/// <returns></returns>
		private ImportingConstructorParameterDescription[] GetParameters(IAssemblyContainer assemblyContainer)
		{
			if(_parameters == null || _parameters.Length == 0)
			{
				var type = GetType(assemblyContainer);
				var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
				foreach (var ctor in constructors)
				{
					var ctorParameters = ctor.GetParameters();
					if(ctorParameters.Length > 0)
					{
						_parameters = ctorParameters.Select(f => new ImportingConstructorParameterDescription {
							Name = f.Name,
							ContractType = f.ParameterType
						}).ToArray();
						continue;
					}
				}
			}
			return _parameters;
		}

		/// <summary>
		/// Recupera o tipo associado com a instancia.
		/// </summary>
		/// <param name="assemblyContainer"></param>
		/// <returns></returns>
		private Type GetType(IAssemblyContainer assemblyContainer)
		{
			if(_type == null)
			{
				System.Reflection.Assembly assembly = null;
				if(assemblyContainer.TryGet(_typeName.AssemblyName.Name, out assembly))
					_type = assembly.GetType(_typeName.FullName, true);
				else
					throw new InvalidOperationException(string.Format("Assembly '{0}' not found", _typeName.AssemblyName.Name));
			}
			return _type;
		}

		/// <summary>
		/// Recupera a instancia do construtor associado.
		/// </summary>
		/// <param name="assemblyContainer"></param>
		/// <returns></returns>
		public System.Reflection.ConstructorInfo GetConstructor(IAssemblyContainer assemblyContainer)
		{
			var type = GetType(assemblyContainer);
			var constructors = type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			var parameters = GetParameters(assemblyContainer) ?? new ImportingConstructorParameterDescription[0];
			foreach (var i in constructors)
			{
				var ctorParameters = i.GetParameters();
				if(ctorParameters.Length == parameters.Length)
				{
					var equals = true;
					for(var j = 0; equals && j < ctorParameters.Length; j++)
					{
						equals = ctorParameters[j].Name == parameters[j].Name && (parameters[j].ContractType == null || (parameters[j].ContractType == ctorParameters[j].ParameterType));
					}
					if(equals)
						return i;
				}
			}
			return null;
		}

		/// <summary>
		/// Recupera os parametros do construtor.
		/// </summary>
		/// <param name="assemblyContainer"></param>
		/// <returns></returns>
		public IEnumerable<Tuple<ImportingConstructorParameterDescription, System.Reflection.ParameterInfo>> GetParameterImportDefinitions(IAssemblyContainer assemblyContainer)
		{
			var constructor = GetConstructor(assemblyContainer);
			if(constructor != null)
			{
				var parameters = GetParameters(assemblyContainer);
				var ctorParameters = constructor.GetParameters();
				for(var i = 0; i < ctorParameters.Length; i++)
					yield return new Tuple<ImportingConstructorParameterDescription, System.Reflection.ParameterInfo>(parameters[i], ctorParameters[i]);
			}
		}
	}
}
