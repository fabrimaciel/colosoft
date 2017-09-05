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
	/// Representa o resultado de uma préconfiguração.
	/// </summary>
	public class PreConfigureResult
	{
		/// <summary>
		/// Identifica se a operação foi realizada com sucesso.
		/// </summary>
		public bool Success
		{
			get;
			private set;
		}

		/// <summary>
		/// Relação dos tipos que estão com problema.
		/// </summary>
		public Type[] Types
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="success"></param>
		/// <param name="types"></param>
		public PreConfigureResult(bool success, Type[] types = null)
		{
			this.Success = success;
			this.Types = types ?? new Type[0];
		}
	}
	/// <summary>
	/// Assinatura da classe responsável pela configuração dos Exports do sistema.
	/// </summary>
	public interface IExportConfigurator
	{
		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		/// <param name="result">Informações do resultado da pré-configuração.</param>
		void PreConfigure(Type type, string contractName, out PreConfigureResult result);

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		void PreConfigure(Type type, string contractName);

		/// <summary>
		/// Pré-configura os exports do tipo associado com o contrato informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será pré configurado.</typeparam>
		/// <param name="contractName"></param>
		/// <param name="result">Informações do resultado da pré-configuração.</param>
		void PreConfigure<T>(string contractName, out PreConfigureResult result);

		/// <summary>
		/// Pré-configura os exports do tipo associado com o contrato informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será pré configurado.</typeparam>
		/// <param name="contractName"></param>
		void PreConfigure<T>(string contractName);

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result">Informações do resultado da pré-configuração.</param>
		void PreConfigure<T>(out PreConfigureResult result);

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		void PreConfigure<T>();

		/// <summary>
		/// Recupera os tipos que implementam os exports do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será analisado.</param>
		/// <returns></returns>
		IEnumerable<Type> GetExportTypes(Type type);

		/// <summary>
		/// Recupera os tipos que implementam os exports do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será analisado.</param>
		/// <param name="contractName"></param>
		/// <returns></returns>
		IEnumerable<Type> GetExportTypes(Type type, string contractName);
	}
	/// <summary>
	/// Implementação fake do configurador.
	/// </summary>
	class ExportConfiguratorFake : IExportConfigurator
	{
		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		/// <param name="result"></param>
		public void PreConfigure(Type type, string contractName, out PreConfigureResult result)
		{
			result = new PreConfigureResult(true);
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		public void PreConfigure(Type type, string contractName)
		{
		}

		/// <summary>
		/// Pré-configura os exports do tipo associado com o contrato informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será pré configurado.</typeparam>
		/// <param name="contractName"></param>
		/// <param name="result"></param>
		public void PreConfigure<T>(string contractName, out PreConfigureResult result)
		{
			result = new PreConfigureResult(true);
		}

		/// <summary>
		/// Pré-configura os exports do tipo associado com o contrato informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será pré configurado.</typeparam>
		/// <param name="contractName"></param>
		public void PreConfigure<T>(string contractName)
		{
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		public void PreConfigure<T>(out PreConfigureResult result)
		{
			result = new PreConfigureResult(true);
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void PreConfigure<T>()
		{
		}

		/// <summary>
		/// Recupera os tipos que implementam os exports do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será analisado.</param>
		/// <returns></returns>
		public IEnumerable<Type> GetExportTypes(Type type)
		{
			return new Type[0];
		}

		/// <summary>
		/// Recupera os tipos que implementam os exports do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será analisado.</param>
		/// <param name="contractName"></param>
		/// <returns></returns>
		public IEnumerable<Type> GetExportTypes(Type type, string contractName)
		{
			return new Type[0];
		}
	}
	/// <summary>
	/// Classe responsável pela configuração dos exports do sistema.
	/// </summary>
	public class ExportConfigurator : IExportConfigurator
	{
		private System.ComponentModel.Composition.Hosting.ExportProvider _exportProvider;

		private static IExportConfigurator _instance;

		private readonly static IExportConfigurator _fakeInstance = new ExportConfiguratorFake();

		/// <summary>
		/// Instancia global do configurador.
		/// </summary>
		public static IExportConfigurator Instance
		{
			get
			{
				return _instance ?? _fakeInstance;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="exportProvider"></param>
		public ExportConfigurator(System.ComponentModel.Composition.Hosting.ExportProvider exportProvider)
		{
			exportProvider.Require("exportProvider").NotNull();
			_exportProvider = exportProvider;
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		/// <param name="result"></param>
		public void PreConfigure(Type type, string contractName, out PreConfigureResult result)
		{
			var notFoundTypes = new List<Type>();
			var types = new List<Type>() {
				type
			};
			for(var i = 0; i < types.Count; i++)
			{
				IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> definitions = null;
				if(ExportProviderExtensions.TryGetExportDefintions(_exportProvider, types[i], contractName, out definitions))
				{
					foreach (var definition in definitions)
					{
						var providerDefintion = definition as ProviderExportDefinition;
						if(providerDefintion != null)
						{
							var member = providerDefintion.Member;
							if(member is System.Reflection.ConstructorInfo)
							{
								var ci = (System.Reflection.ConstructorInfo)member;
								foreach (var parameter in ci.GetParameters())
									if(!types.Contains(parameter.ParameterType))
										types.Add(parameter.ParameterType);
							}
						}
					}
				}
				else
				{
					notFoundTypes.Add(types[i]);
				}
				contractName = null;
			}
			result = new PreConfigureResult(notFoundTypes.Count == 0, notFoundTypes.ToArray());
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="contractName"></param>
		public void PreConfigure(Type type, string contractName)
		{
			PreConfigureResult result = null;
			PreConfigure(type, contractName, out result);
			if(!result.Success)
				throw new PreConfigureException(ResourceMessageFormatter.Create(() => Properties.Resources.ExportConfigurator_NotFoundExportForType, result.Types[0].FullName));
		}

		/// <summary>
		/// Pré-configura os exports do tipo associado com o contrato informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será pré configurado.</typeparam>
		/// <param name="contractName"></param>
		/// <param name="result"></param>
		public void PreConfigure<T>(string contractName, out PreConfigureResult result)
		{
			PreConfigure(typeof(T), contractName, out result);
		}

		/// <summary>
		/// Pré-configura os exports do tipo associado com o contrato informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será pré configurado.</typeparam>
		/// <param name="contractName"></param>
		public void PreConfigure<T>(string contractName)
		{
			PreConfigure(typeof(T), contractName);
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="result"></param>
		public void PreConfigure<T>(out PreConfigureResult result)
		{
			PreConfigure(typeof(T), null, out result);
		}

		/// <summary>
		/// Pré-configura os exports do tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		public void PreConfigure<T>()
		{
			PreConfigure(typeof(T), null);
		}

		/// <summary>
		/// Recupera os tipos que implementam os exports do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será analisado.</param>
		/// <returns></returns>
		public IEnumerable<Type> GetExportTypes(Type type)
		{
			return GetExportTypes(type, null);
		}

		/// <summary>
		/// Recupera os tipos que implementam os exports do tipo informado.
		/// </summary>
		/// <param name="type">Tipo que será analisado.</param>
		/// <param name="contractName"></param>
		/// <returns></returns>
		public IEnumerable<Type> GetExportTypes(Type type, string contractName)
		{
			IEnumerable<System.ComponentModel.Composition.Primitives.ExportDefinition> definitions = null;
			if(ExportProviderExtensions.TryGetExportDefintions(_exportProvider, type, contractName, out definitions))
			{
				foreach (var definition in definitions)
				{
					var providerDefintion = definition as ProviderExportDefinition;
					if(providerDefintion != null)
					{
						var member = providerDefintion.Member;
						switch(member.MemberType)
						{
						case System.Reflection.MemberTypes.Field:
							yield return ((System.Reflection.FieldInfo)member).FieldType;
							break;
						case System.Reflection.MemberTypes.Method:
							yield return ((System.Reflection.MethodInfo)member).ReturnType;
							break;
						case System.Reflection.MemberTypes.Property:
							yield return ((System.Reflection.PropertyInfo)member).PropertyType;
							break;
						case System.Reflection.MemberTypes.Constructor:
							yield return ((System.Reflection.ConstructorInfo)member).DeclaringType;
							break;
						case System.Reflection.MemberTypes.NestedType:
						case System.Reflection.MemberTypes.TypeInfo:
							yield return (Type)member;
							break;
						}
					}
				}
			}
		}
	}
}
namespace Colosoft
{
	/// <summary>
	/// Classe com método de extensão para o IExportConfigurator.
	/// </summary>
	public static class IExportConfiguratorExtensions
	{
		/// <summary>
		/// Verifica se existem algum exportação para o tipo informado.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="configurator"></param>
		/// <param name="contractName"></param>
		/// <returns></returns>
		public static bool Valid<T>(this Colosoft.Mef.IExportConfigurator configurator, string contractName)
		{
			return configurator != null && configurator.GetExportTypes(typeof(T), contractName).Any();
		}
	}
}
