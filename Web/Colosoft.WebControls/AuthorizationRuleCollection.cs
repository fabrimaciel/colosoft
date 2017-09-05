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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Security.Principal;

namespace Colosoft.WebControls.Route.Security
{
	public class AuthorizationRuleCollection : Collection<AuthorizationRule>
	{
		/// <summary>
		/// Registra as informações da regra no requisição.
		/// </summary>
		/// <param name="rule"></param>
		private void RegisterAuthRuleInfo(AuthorizationRule rule)
		{
			System.Web.HttpContext.Current.Items[AuthorizationRule.AUTH_RULE_CONTEXT_ID] = rule;
		}

		/// <summary>
		/// Verifica se o usuário é permitido dentro das regras aplicadas.
		/// </summary>
		/// <param name="principal"></param>
		/// <param name="requestedPath"></param>
		/// <param name="verb"></param>
		/// <returns></returns>
		internal bool IsUserAllowed(IPrincipal principal, string requestedPath, string verb)
		{
			bool result = this.Count == 0;
			if(!result)
			{
				foreach (AuthorizationRule rule in this)
				{
					if(rule.Type == AuthorizationRuleType.Roles)
					{
						if(!rule.Everyone)
						{
							foreach (var se in rule.Data)
							{
								if(rule.Everyone || (principal != null && (principal is GenericPrincipal && System.Web.Security.Roles.Provider != null ? System.Web.Security.Roles.IsUserInRole(se) : principal.IsInRole(se))))
								{
									RegisterAuthRuleInfo(rule);
									result = rule.Action == AuthorizationRuleAction.Allow;
								}
							}
						}
						else
						{
							RegisterAuthRuleInfo(rule);
							result = rule.Action == AuthorizationRuleAction.Allow;
						}
					}
					else if(rule.Type == AuthorizationRuleType.Users)
					{
						if((rule.Anonymous && (principal == null || principal.Identity == null || !principal.Identity.IsAuthenticated)) || rule.Everyone)
						{
							RegisterAuthRuleInfo(rule);
							result = rule.Action == AuthorizationRuleAction.Allow;
						}
						else if(principal != null && principal.Identity != null && principal.Identity.IsAuthenticated)
						{
							result = true;
							if(!rule.Everyone)
							{
								foreach (var se in rule.Data)
								{
									if(string.Equals(se, principal.Identity.Name, StringComparison.OrdinalIgnoreCase))
									{
										RegisterAuthRuleInfo(rule);
										result = rule.Action == AuthorizationRuleAction.Allow;
									}
								}
							}
							else
							{
								RegisterAuthRuleInfo(rule);
								result = rule.Action == AuthorizationRuleAction.Allow;
							}
						}
					}
					else if(rule.Type == AuthorizationRuleType.HttpVerbs)
					{
						foreach (var se in rule.Data)
						{
							if(string.Equals(se, verb, StringComparison.OrdinalIgnoreCase))
							{
								RegisterAuthRuleInfo(rule);
								result = rule.Action == AuthorizationRuleAction.Allow;
							}
						}
					}
				}
			}
			return result;
		}
	}
}
