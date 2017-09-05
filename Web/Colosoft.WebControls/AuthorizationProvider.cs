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
using System.Web;
using System.Configuration.Provider;
using System.Security.Permissions;

namespace Colosoft.WebControls.Route.Security
{
	[AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal)]
	public abstract class AuthorizationProvider : ProviderBase
	{
		protected bool _storeRulesInCache = true;

		/// <summary>
		/// Esse evento é disparado sempre quando uma nova política é criada ou excluída, e tem a finalidade de informar à 
		/// aplicação que algo foi mudado, dando oportunidade para ela decidir se mantém ou não o cache.
		/// Possui um argumento específico, chamado CachingRulesEventArgs. Essa classe tem uma propriedade booleana chamada
		/// ClearCache que, por padrão, é True e determina a limpeza do cache. 
		/// </summary>
		public event EventHandler<CachingRulesEventArgs> DataChanged;

		/// <summary>
		/// Recupera todas as regras cadastradas.
		/// </summary>
		/// <returns></returns>
		public abstract AuthorizationRuleCollection GetAllRules();

		/// <summary>
		/// Recupera as regras que ser aplicam ao caminho informado.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public abstract AuthorizationRuleCollection GetAllRulesByPath(string path);

		/// <summary>
		/// Recupera a regra com base no identificador informado.
		/// </summary>
		/// <param name="pathId"></param>
		/// <returns></returns>
		public abstract AuthorizationRule GetRuleByPath(Guid pathId);

		/// <summary>
		/// Apaga um caminho da regra.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void DeletePath(AuthorizationRule rule);

		/// <summary>
		/// Adiciona uma nova regra.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void AddRule(AuthorizationRule rule);

		/// <summary>
		/// Adiciona um novo caminho.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void AddPath(AuthorizationRule rule);

		/// <summary>
		/// Adiciona um novo caminho com regra.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void AddPathRule(AuthorizationRule rule);

		/// <summary>
		/// Atualiza uma regra.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void UpdateRule(AuthorizationRule rule);

		/// <summary>
		/// Atualiza uma regra.
		/// </summary>
		/// <param name="action"></param>
		/// <param name="data"></param>
		/// <param name="type"></param>
		/// <param name="ruleId"></param>
		public abstract void UpdateRule(string action, string data, string type, Guid ruleId);

		public abstract void UpdatePath(string name, string path, Guid parentId, Guid pathId, string action, string data, string type, Guid ruleId, bool status);

		public abstract void UpdatePath(string name, string path, Guid parentId, Guid pathId, bool status);

		public abstract void UpdatePath(AuthorizationRule path);

		/// <summary>
		/// Apagar uma regra existente.
		/// </summary>
		/// <param name="ruleId"></param>
		public abstract void DeleteRule(Guid ruleId);

		/// <summary>
		/// Apaga a regra informada.
		/// </summary>
		/// <param name="rule"></param>
		public abstract void DeleteRule(AuthorizationRule rule);

		public abstract AuthorizationRule FindPath(string path);

		/// <summary>
		/// Pesquisa um regra.
		/// </summary>
		/// <param name="pathId"></param>
		/// <param name="action"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public abstract AuthorizationRule FindRule(Guid pathId, AuthorizationRuleAction action, AuthorizationRuleType type);

		/// <summary>
		/// Carrega a lista com a hierarquia das regras.
		/// </summary>
		/// <param name="namePathToMatch"></param>
		/// <param name="status"></param>
		/// <returns></returns>
		public abstract List<AuthorizationRule> ListHierarchyRule(string namePathToMatch, bool status);

		/// <summary>
		/// Recupera o nome do caminho.
		/// </summary>
		/// <param name="pathId"></param>
		/// <returns></returns>
		public abstract string FindNamePath(Guid pathId);

		/// <summary> 
		/// Este método  ela analisa se a aplicação se vinculou ou não ao evento, e caso verdadeiro o dispara.
		/// Se a propriedade ClearCache retornar True, essa classe efetuará a limpeza do cache.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="args">determina a limpeza do cache</param>
		protected virtual void OnDataChanged(object sender, CachingRulesEventArgs args)
		{
			try
			{
				if(this.DataChanged != null)
					this.DataChanged(sender, args);
			}
			finally
			{
				if(args.ClearCache)
					this.ClearCache();
			}
		}

		/// <summary>
		/// Limpa o cache.
		/// </summary>
		private void ClearCache()
		{
			if(CacheManager.Rules != null)
				CacheManager.Rules = null;
		}

		/// <summary>
		/// Indica ao runtime se os dados devem ou não serem armazenados no cache, sendo o padrão True.
		/// </summary>
		public virtual bool StoreRulesInCache
		{
			get
			{
				return _storeRulesInCache;
			}
		}

		public override void Initialize(string name, System.Collections.Specialized.NameValueCollection config)
		{
			var store = false;
			if(bool.TryParse(config["storeRulesInCache"], out store))
				_storeRulesInCache = store;
			base.Initialize(name, config);
		}
	}
}
