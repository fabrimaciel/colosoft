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
using Colosoft.WebControls.Route.Security.Configuration;

namespace Colosoft.WebControls.Route.Security
{
	public static class Authorization
	{
		private static AuthorizationProvider _provider;

		private static AuthorizationProviderCollection _providers;

		static Authorization()
		{
			Initialize();
		}

		private static void Initialize()
		{
			AuthorizationSection section = System.Configuration.ConfigurationManager.GetSection("Colosoft.Route.Security") as AuthorizationSection;
			if(section != null)
			{
				_providers = new AuthorizationProviderCollection();
				System.Web.Configuration.ProvidersHelper.InstantiateProviders(section.Providers, _providers, typeof(AuthorizationProvider));
				_providers.SetReadOnly();
				_provider = _providers[section.DefaultProvider.Trim()];
				if(_provider != null)
					return;
			}
		}

		public static AuthorizationProvider Provider
		{
			get
			{
				return _provider;
			}
		}

		public static AuthorizationProviderCollection Providers
		{
			get
			{
				return _providers;
			}
		}

		/// <summary>
		/// Método responsável retorna coleção de  todos os elementos do tipo AuthorizationRule
		/// </summary>
		/// <returns>coleção de elementos do tipo AuthorizationRule</returns>
		public static AuthorizationRuleCollection GetAllRules()
		{
			return Provider.GetAllRules();
		}

		/// <summary>
		/// Método responsável retorna coleção de  dos elementos do tipo AuthorizationRule que pertencem ao path informado
		/// </summary>
		/// <param name="path">path </param>
		/// <returns>coleção de elementos do tipo AuthorizationRule</returns>
		public static AuthorizationRuleCollection GetAllRulesByPath(string path)
		{
			return Provider.GetAllRulesByPath(path);
		}

		/// <summary>
		/// Método responsável por retorna o elemento do tipo AuthorizationRule pertencente ao ruleId da regra informada.
		/// </summary>
		/// <param name="pathId">Número de identificação da caminho.</param>
		/// <returns> elemento do tipo AuthorizationRule</returns>
		public static AuthorizationRule GetRuleByPath(Guid pathId)
		{
			return Provider.GetRuleByPath(pathId);
		}

		/// <summary>
		/// Método responsável por adiciona o Path do tipo AuthorizationRule informado
		/// </summary>
		/// <param name="rule">Elemento do path</param>
		public static void AddPath(AuthorizationRule rule)
		{
			Provider.AddPath(rule);
		}

		/// <summary>
		/// Método responsável por adiciona o Path  e Ruledo tipo AuthorizationRule informado
		/// </summary>
		/// <param name="rule">Elemento do path</param>
		public static void AddPathRule(AuthorizationRule rule)
		{
			Provider.AddPathRule(rule);
		}

		/// <summary>
		/// Método responsável por adiciona o Rule do tipo AuthorizationRule informado
		/// </summary>
		/// <param name="rule">>Elemento da regra</param>
		public static void AddRule(AuthorizationRule rule)
		{
			Provider.AddRule(rule);
		}

		/// <summary>
		/// Método responsável por apagar o Path apartir do Id informado
		/// </summary>
		/// <param name="path">Número de identificação do path</param>
		public static void DeletePath(AuthorizationRule path)
		{
			Provider.DeletePath(path);
		}

		/// <summary>
		/// Método responsável por apagar a regra apartir do Id informado
		/// </summary>
		/// <param name="ruleId">Número de identificação da regra</param>
		public static void DeleteRule(Guid ruleId)
		{
			Provider.DeleteRule(ruleId);
		}

		/// <summary>
		/// Método responsável por apagar a regra apartir do AuthorizationRule informado
		/// </summary>
		/// <param name="rule">AuthorizationRule</param>
		public static void DeleteRule(AuthorizationRule rule)
		{
			Provider.DeleteRule(rule);
		}

		/// <summary>
		/// Método responsável por atualizar a regra apartir do AuthorizationRule informado
		/// </summary>
		/// <param name="rule">AuthorizationRule</param>
		public static void UpdateRule(AuthorizationRule rule)
		{
			Provider.UpdateRule(rule);
		}

		/// <summary>
		/// Método responsável por atualizar a regra apartir dos dados informados
		/// </summary>
		/// <param name="action">Ação</param>
		/// <param name="data">Valor adicional, que são os usuários, papéis ou verbos </param>
		/// <param name="type">Type</param>
		/// <param name="ruleId">Número de identificação da regra</param>
		public static void UpdateRule(string action, string data, string type, Guid ruleId)
		{
			Provider.UpdateRule(action, data, type, ruleId);
		}

		/// <summary>
		/// Método responsável por atualizar o path apartir do AuthorizationRule informado
		/// </summary>        
		public static void UpdatePath(string name, string path, Guid parentId, Guid pathId, string action, string data, string type, Guid ruleId, bool status)
		{
			Provider.UpdatePath(name, path, parentId, pathId, action, data, type, ruleId, status);
		}

		/// <summary>
		/// Método responsável por atualizar o path apartir do AuthorizationRule informado
		/// </summary>
		/// <param name="path">AuthorizationRule</param>
		public static void UpdatePath(AuthorizationRule path)
		{
			Provider.UpdatePath(path);
		}

		/// <summary>
		/// Método responsável por atualizar a regra apartir dos dados informados
		/// </summary>
		/// <param name="name">Nome do Path</param>
		/// <param name="path">Path</param>
		/// <param name="parentId">Id do pai</param>
		/// <param name="pathId">Número de identificação do Path</param>
		/// <param name="Status">status</param>
		public static void UpdatePath(string name, string path, Guid parentId, Guid pathId, bool status)
		{
			Provider.UpdatePath(name, path, parentId, pathId, status);
		}

		/// <summary>
		/// Método responsável por pesquisar a regra apartir dos dados informados
		/// </summary>
		/// <param name="pathId">Número de identificação do Path</param>
		/// <param name="action">Ação</param>
		/// <param name="type">Tipo</param>
		/// <returns>AuthorizationRule</returns>
		public static AuthorizationRule FindRule(Guid pathId, AuthorizationRuleAction action, AuthorizationRuleType type)
		{
			return Provider.FindRule(pathId, action, type);
		}

		/// <summary>
		/// Método responsável por pesquisar o Path 
		/// </summary>
		/// <param name="path">Path</param>
		/// <returns>AuthorizationRule</returns>
		public static AuthorizationRule FindPath(string path)
		{
			return Provider.FindPath(path);
		}

		/// <summary>
		/// Lista todas regras de forma hierarquica 
		/// </summary>
		/// <param name="namePathToMatch">Nome do path</param> 
		/// <param name="status">false- Exibe apenas os path permitidos, true -exibe todos os path </param>
		/// <returns>Lista de AuthorizationRule </returns>           
		public static List<AuthorizationRule> ListHierarchyRule(string namePathToMatch, bool status)
		{
			return Provider.ListHierarchyRule(namePathToMatch, status);
		}

		/// <summary>
		/// Recupera o nome do caminho.
		/// </summary>
		/// <param name="pathId"></param>
		/// <returns></returns>
		public static string FindNamePath(Guid pathId)
		{
			return Provider.FindNamePath(pathId);
		}
	}
}
