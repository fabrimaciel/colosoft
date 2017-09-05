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

namespace Colosoft.Business
{
	/// <summary>
	/// Implementação padrão do gerenciador dos tipos de entitidade.
	/// </summary>
	public class BusinessEntityTypeManager : IEntityTypeManager
	{
		private object _objLock = new object();

		private int _currentUid = -1;

		private Dictionary<string, BusinessEntityTypeVersion> _typeVersions = new Dictionary<string, BusinessEntityTypeVersion>();

		private Dictionary<Colosoft.Reflection.TypeName, IEntityLoader> _entityLoaders = new Dictionary<Colosoft.Reflection.TypeName, IEntityLoader>(Colosoft.Reflection.TypeName.TypeNameFullNameComparer.Instance);

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="loader">Tipos das entidades que são trabalhadas no sistema.</param>
		public BusinessEntityTypeManager(IBusinessEntityTypeLoader loader)
		{
			loader.Require("loader").NotNull();
			Initialize(loader.GetEntityTypes());
		}

		/// <summary>
		/// Inicializa a instancia.
		/// </summary>
		/// <param name="types"></param>
		private void Initialize(IEnumerable<BusinessEntityType> types)
		{
			foreach (var i in types)
			{
				foreach (var version in i.Versions)
				{
					var key = CreateTypeVersionKey(version);
					if(!_typeVersions.ContainsKey(key))
						_typeVersions.Add(key, version);
				}
			}
		}

		/// <summary>
		/// Cria uma chave para identificar a versão do tipo.
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		private static string CreateTypeVersionKey(BusinessEntityTypeVersion version)
		{
			return string.Format("{0}.{1}, {2}", version.TypeNamespace, version.TypeName, version.TypeAssembly);
		}

		/// <summary>
		/// Cria uma chave para identificar a versão do tipo.
		/// </summary>
		/// <param name="typeName"></param>
		/// <returns></returns>
		private static string CreateTypeVersionKey(Colosoft.Reflection.TypeName typeName)
		{
			return string.Format("{0}.{1}, {2}", string.Join(".", typeName.Namespace), typeName.Name, typeName.AssemblyName.FullName);
		}

		/// <summary>
		/// Tenta recupera os dados da versão do tipo da entidade com base
		/// no tipo informado.
		/// </summary>
		/// <param name="typeName">Tipo que será pesquisado.</param>
		/// <returns>Versão do tipo da entidade correspondente ou null.</returns>
		public BusinessEntityTypeVersion GetTypeVersion(Colosoft.Reflection.TypeName typeName)
		{
			var key = CreateTypeVersionKey(typeName);
			BusinessEntityTypeVersion entityTypeVersion = null;
			if(_typeVersions.TryGetValue(key, out entityTypeVersion))
				return entityTypeVersion;
			return null;
		}

		/// <summary>
		/// Tenta recupera as propriedade da versão do tipo da entidade com base no tipo informado.
		/// </summary>
		/// <param name="typeName">Tipo da versão da entidade.</param>
		/// <param name="uiContext">Nome do contexto visual usado para filtrar os dados.</param>
		/// <returns></returns>
		public IEnumerable<BusinessEntityTypeVersionProperty> GetTypeProperties(Colosoft.Reflection.TypeName typeName, string uiContext = null)
		{
			var key = CreateTypeVersionKey(typeName);
			BusinessEntityTypeVersion entityTypeVersion = null;
			if(_typeVersions.TryGetValue(key, out entityTypeVersion))
			{
				return entityTypeVersion.Properties.Where(f => uiContext == null || f.UIConfigurations.Where(x => x.UIContext == uiContext).Any());
			}
			return new BusinessEntityTypeVersionProperty[0];
		}

		/// <summary>
		/// Verifica se o tipo da entidade possui um identificador unico.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool HasUid(Type type)
		{
			IEntityLoader loader = null;
			if(TryGetLoader(type, out loader))
				return loader.HasUid;
			return false;
		}

		/// <summary>
		/// Verifica se o tipo informado possui chaves.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public bool HasKeys(Type type)
		{
			IEntityLoader loader = null;
			if(TryGetLoader(type, out loader))
				return loader.HasKeys;
			return false;
		}

		/// <summary>
		/// Gera um novo identificador unico para uma instancia do tipo informado.
		/// </summary>
		/// <param name="type">Tipo da entidade de dados.</param>
		/// <returns></returns>
		public int GenerateInstanceUid(Type type)
		{
			lock (_objLock)
				return _currentUid--;
		}

		/// <summary>
		/// Recupera o loader para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="loader"></param>
		/// <returns></returns>
		public bool TryGetLoader(Type type, out IEntityLoader loader)
		{
			type.Require("type").NotNull();
			var typeName = Colosoft.Reflection.TypeName.Get(type);
			lock (_entityLoaders)
			{
				if(!_entityLoaders.TryGetValue(typeName, out loader))
				{
					var loaderAttribute = type.GetCustomAttributes(typeof(EntityLoaderAttribute), false).LastOrDefault() as EntityLoaderAttribute;
					if(loaderAttribute == null)
					{
						_entityLoaders.Add(typeName, null);
					}
					else
					{
						try
						{
							loader = (IEntityLoader)Activator.CreateInstance(loaderAttribute.EntityLoaderType);
						}
						catch(System.Reflection.TargetInvocationException ex)
						{
							throw ex.InnerException;
						}
						_entityLoaders.Add(typeName, loader);
						return true;
					}
				}
				else
				{
					return loader != null;
				}
			}
			loader = null;
			return false;
		}

		/// <summary>
		/// Recupera o loader para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEntityLoader GetLoader(Type type)
		{
			IEntityLoader loader = null;
			if(!TryGetLoader(type, out loader))
				throw new LoaderNotFoundException(string.Format("Not found loader for type '{0}'", type.FullName));
			return loader;
		}
	}
}
