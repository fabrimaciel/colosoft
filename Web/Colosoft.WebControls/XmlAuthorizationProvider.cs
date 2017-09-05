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
using System.Xml;
using Colosoft.Configuration.Handlers;
using System.IO;

namespace Colosoft.WebControls.Route.Security.XmlAuthorization
{
	public class XmlAuthorizationProvider : AuthorizationProvider
	{
		delegate TResult Func<T, TResult> (T arg);

		/// <summary>
		/// Versão do arquivo de autorização.
		/// </summary>
		private static string authorizationFileVersion = "";

		private static FileConfigHandler _configHandler;

		private static XmlAuthorizationPath _root;

		/// <summary>
		/// Formata o diretório virtual.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static string FormatVirtualPath(string path)
		{
			string loc = "";
			if(!string.IsNullOrEmpty(path) && path[0] == '~')
				loc = path.Substring(1);
			else
				loc = path;
			if(!string.IsNullOrEmpty(loc) && loc[0] == '/')
				loc = loc.Substring(1);
			loc = loc.ToLower();
			return loc;
		}

		/// <summary>
		/// Recupera a instancia do caminho raiz
		/// </summary>
		public static XmlAuthorizationPath Root
		{
			get
			{
				if(_configHandler == null)
					try
					{
						_configHandler = new Colosoft.Configuration.Handlers.FileConfigHandler("authorization.config");
					}
					catch(Exception ex)
					{
						throw new Exception("Fail on load authorization.config.", ex);
					}
				var fi = new FileInfo(_configHandler.LocalConfigFilePath);
				if(_root == null || authorizationFileVersion != fi.LastWriteTime.ToString("ddMMyyyyhhmmss"))
				{
					_root = null;
					lock (_configHandler)
					{
						if(_root != null)
							return _root;
						authorizationFileVersion = fi.LastWriteTime.ToString("ddMMyyyyhhmmss");
						_configHandler.Refresh();
						var first = _configHandler.GetNode("authorization");
						if(first != null && first.HasChildNodes)
						{
							_root = new XmlAuthorizationPath();
							foreach (var node in first.ChildNodes)
							{
								var n1 = node as XmlElement;
								if(n1 != null)
									_root.Children.Add(new XmlAuthorizationPath(_root, n1));
							}
						}
					}
				}
				return _root;
			}
		}

		/// <summary>
		/// Navega por todos os elementos filhos do pai.
		/// </summary>
		/// <param name="parent"></param>
		/// <returns></returns>
		private static IEnumerable<XmlAuthorizationPath> Navigate(XmlAuthorizationPath parent)
		{
			foreach (var i in parent.Children)
			{
				yield return i;
				foreach (var x in Navigate(i))
					yield return x;
			}
		}

		/// <summary>
		/// Pesquisa os caminhos aplicando um filtro.
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		private static IEnumerable<XmlAuthorizationPath> Where(XmlAuthorizationPath parent, Func<XmlAuthorizationPath, bool> filter)
		{
			foreach (var i in parent.Children)
			{
				if(filter(i))
				{
					yield return i;
					foreach (var x in Navigate(i))
						yield return x;
				}
			}
		}

		/// <summary>
		/// Recupera todas as regras cadastradas.
		/// </summary>
		/// <returns></returns>
		public override AuthorizationRuleCollection GetAllRules()
		{
			var collection = new AuthorizationRuleCollection();
			foreach (var i in Navigate(Root))
				foreach (var j in i.GetRules())
					collection.Add(j);
			return collection;
		}

		public override AuthorizationRuleCollection GetAllRulesByPath(string path)
		{
			string loc = FormatVirtualPath(path);
			var validPathsQueue = new Queue<XmlAuthorizationPath>();
			XmlAuthorizationPath validPath = null;
			var queuePaths = new Queue<XmlAuthorizationPath>();
			queuePaths.Enqueue(Root);
			while (queuePaths.Count > 0)
			{
				var p1 = queuePaths.Dequeue();
				foreach (var i in p1.Children.ToArray())
				{
					if(i == null)
						continue;
					if((i.Complex != null && i.Complex.Match(loc).Success) || (loc.Length >= i.FullUrl.Length && loc.IndexOf(i.FullUrl, 0, i.FullUrl.Length, StringComparison.InvariantCultureIgnoreCase) == 0 && (validPath == null || i.FullUrl.Length > validPath.FullUrl.Length)))
					{
						validPathsQueue.Enqueue(i);
						validPath = i;
						queuePaths.Enqueue(i);
					}
				}
				if(p1 == null)
					break;
			}
			var result = new AuthorizationRuleCollection();
			foreach (var i in validPathsQueue)
				foreach (var j in i.GetRules())
					result.Add(j);
			return result;
		}

		public override AuthorizationRule GetRuleByPath(Guid ruleId)
		{
			throw new NotImplementedException();
		}

		public override void DeletePath(AuthorizationRule rule)
		{
			throw new NotImplementedException();
		}

		public override void AddRule(AuthorizationRule rule)
		{
			throw new NotImplementedException();
		}

		public override void AddPath(AuthorizationRule rule)
		{
			throw new NotImplementedException();
		}

		public override void AddPathRule(AuthorizationRule rule)
		{
			throw new NotImplementedException();
		}

		public override void UpdateRule(AuthorizationRule rule)
		{
			throw new NotImplementedException();
		}

		public override void UpdateRule(string action, string data, string type, Guid ruleId)
		{
			throw new NotImplementedException();
		}

		public override void UpdatePath(string fullname, string path, Guid parentId, Guid pathId, string action, string data, string type, Guid ruleId, bool status)
		{
			throw new NotImplementedException();
		}

		public override void UpdatePath(string fullname, string path, Guid parentId, Guid pathId, bool status)
		{
			throw new NotImplementedException();
		}

		public override void UpdatePath(AuthorizationRule path)
		{
			throw new NotImplementedException();
		}

		public override void DeleteRule(Guid ruleId)
		{
			throw new NotImplementedException();
		}

		public override void DeleteRule(AuthorizationRule rule)
		{
			throw new NotImplementedException();
		}

		public override AuthorizationRule FindPath(string path)
		{
			throw new NotImplementedException();
		}

		public override AuthorizationRule FindRule(Guid pathId, AuthorizationRuleAction action, AuthorizationRuleType type)
		{
			throw new NotImplementedException();
		}

		public override List<AuthorizationRule> ListHierarchyRule(string fullnamePath, bool status)
		{
			throw new NotImplementedException();
		}

		public override string FindNamePath(Guid pathId)
		{
			throw new NotImplementedException();
		}

		public override bool StoreRulesInCache
		{
			get
			{
				return true;
			}
		}
	}
}
