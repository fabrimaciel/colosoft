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
using System.Collections.Specialized;
using System.Xml;
using System.Text.RegularExpressions;

namespace Colosoft.WebControls.Route.Security.XmlAuthorization
{
	/// <summary>
	/// Armazena os dados da regra.
	/// </summary>
	public class XmlAuthorizationRule
	{
		/// <summary>
		/// Ação da regra.
		/// </summary>
		public AuthorizationRuleAction Action
		{
			get;
			set;
		}

		/// <summary>
		/// Lista dos usuários que se aplicam a regra.
		/// </summary>
		public StringCollection Users
		{
			get;
			private set;
		}

		/// <summary>
		/// Lista dos perfis que se aplicam.
		/// </summary>
		public StringCollection Roles
		{
			get;
			private set;
		}

		/// <summary>
		/// Lista dos verbos Http que se aplicam.
		/// </summary>
		public StringCollection HttpVerbs
		{
			get;
			private set;
		}

		/// <summary>
		/// Define se a regra implica no redicionamento para a página de login.
		/// </summary>
		public bool LoginPageRedirect
		{
			get;
			private set;
		}

		/// <summary>
		/// Identifica se a regra está vazia.
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				return Users.Count == 0 && Roles.Count == 0 && HttpVerbs.Count == 0;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="element"></param>
		internal XmlAuthorizationRule(XmlElement element)
		{
			LoginPageRedirect = true;
			var name = element.Name.ToLower();
			if(name == "allow")
				Action = AuthorizationRuleAction.Allow;
			else if(name == "deny")
				Action = AuthorizationRuleAction.Deny;
			else
				throw new Exception(string.Format("Invalid tag {0} in authorization", name));
			Users = new StringCollection();
			Roles = new StringCollection();
			HttpVerbs = new StringCollection();
			var usersAttr = element.GetAttribute("users");
			var rolesAttr = element.GetAttribute("roles");
			var verbsAttr = element.GetAttribute("verbs");
			var loginPageRedirectAttr = element.GetAttribute("loginPageRedirect");
			if(!string.IsNullOrEmpty(usersAttr))
				foreach (var i in usersAttr.Split(','))
					Users.Add(i);
			if(!string.IsNullOrEmpty(rolesAttr))
				foreach (var i in rolesAttr.Split(','))
					Roles.Add(i);
			if(!string.IsNullOrEmpty(verbsAttr))
				foreach (var i in verbsAttr.Split(','))
					HttpVerbs.Add(i);
			if(!string.IsNullOrEmpty(loginPageRedirectAttr))
			{
				var r1 = true;
				if(bool.TryParse(loginPageRedirectAttr, out r1))
					LoginPageRedirect = r1;
			}
		}

		internal XmlAuthorizationRule(AuthorizationRuleAction action)
		{
			Users = new StringCollection();
			Roles = new StringCollection();
			HttpVerbs = new StringCollection();
		}

		/// <summary>
		/// Recupera as regras equivalentes.
		/// </summary>
		/// <param name="path"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public IEnumerable<AuthorizationRule> GetAuthorizationRules(XmlAuthorizationPath path)
		{
			if(Users.Count > 0)
				yield return new AuthorizationRule {
					Path = path.FullUrl,
					Action = Action,
					Type = AuthorizationRuleType.Users,
					Status = true,
					Data = Users,
					LoginPageRedirect = LoginPageRedirect
				};
			if(Roles.Count > 0)
				yield return new AuthorizationRule {
					Path = path.FullUrl,
					Action = Action,
					Type = AuthorizationRuleType.Roles,
					Status = true,
					Data = Roles,
					LoginPageRedirect = LoginPageRedirect
				};
			if(HttpVerbs.Count > 0)
				yield return new AuthorizationRule {
					Path = path.FullUrl,
					Action = Action,
					Type = AuthorizationRuleType.HttpVerbs,
					Status = true,
					Data = HttpVerbs,
					LoginPageRedirect = LoginPageRedirect
				};
		}
	}
	/// <summary>
	/// Armazena os dados do caminho de autorização.
	/// </summary>
	public class XmlAuthorizationPath
	{
		/// <summary>
		/// Coleção das permissões. Ela é usada somente para garantir a ordem definição
		/// </summary>
		private List<XmlAuthorizationRule> _permissions = new List<XmlAuthorizationRule>();

		/// <summary>
		/// Definição da regra de permissão.
		/// </summary>
		public XmlAuthorizationRule Allow
		{
			get;
			private set;
		}

		/// <summary>
		/// Definição da regra de negação.
		/// </summary>
		public XmlAuthorizationRule Deny
		{
			get;
			private set;
		}

		/// <summary>
		/// Url do caminho.
		/// </summary>
		public string Url
		{
			get;
			set;
		}

		/// <summary>
		/// Url do caminho completo
		/// </summary>
		public string FullUrl
		{
			get;
			set;
		}

		/// <summary>
		/// Match usado para comprar o caminho complexo.
		/// </summary>
		public Regex Complex
		{
			get;
			set;
		}

		/// <summary>
		/// Caminhos filhos.
		/// </summary>
		public List<XmlAuthorizationPath> Children
		{
			get;
			private set;
		}

		/// <summary>
		/// Elemento pai.
		/// </summary>
		public XmlAuthorizationPath Parent
		{
			get;
			private set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="parent">Elemento pai.</param>
		/// <param name="element"></param>
		public XmlAuthorizationPath(XmlAuthorizationPath parent, XmlElement element)
		{
			if(element == null)
				throw new ArgumentNullException("element");
			Parent = parent;
			Children = new List<XmlAuthorizationPath>();
			Url = element.GetAttribute("url");
			FullUrl = (parent != null && !string.IsNullOrEmpty(parent.FullUrl) ? parent.FullUrl + "/" : "") + Url;
			var complexParts = Regex.Matches(FullUrl, "{[0-9]*?}", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
			if(complexParts.Count > 0)
			{
				var complexRegex = FullUrl;
				var partNumber = 0;
				foreach (Match i in complexParts)
					complexRegex = complexRegex.Replace(i.Value, "(?<complex" + (partNumber++) + ">[\\s\\S]*?)");
				Complex = new Regex(complexRegex);
			}
			if(element.HasChildNodes)
			{
				foreach (var i in element.ChildNodes)
				{
					var node = i as XmlElement;
					if(node == null)
						continue;
					var name = node.Name.ToLower();
					if(name == "allow")
					{
						Allow = new XmlAuthorizationRule(node);
						_permissions.Add(Allow);
					}
					else if(name == "deny")
					{
						Deny = new XmlAuthorizationRule(node);
						_permissions.Add(Deny);
					}
					else if(name == "paths" && node.HasChildNodes)
					{
						foreach (var j in node.ChildNodes)
						{
							var n1 = j as XmlElement;
							if(n1.Name.ToLower() == "path")
								Children.Add(new XmlAuthorizationPath(this, n1));
						}
					}
				}
			}
			if(Allow == null)
				Allow = new XmlAuthorizationRule(AuthorizationRuleAction.Allow);
			if(Deny == null)
				Deny = new XmlAuthorizationRule(AuthorizationRuleAction.Deny);
		}

		public XmlAuthorizationPath()
		{
			Allow = new XmlAuthorizationRule(AuthorizationRuleAction.Allow);
			Deny = new XmlAuthorizationRule(AuthorizationRuleAction.Deny);
			Children = new List<XmlAuthorizationPath>();
		}

		/// <summary>
		/// Recupera as regras que se aplicam ao caminho.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<AuthorizationRule> GetRules()
		{
			foreach (var i in _permissions)
				foreach (var j in i.GetAuthorizationRules(this))
					yield return j;
		}
	}
}
