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
using System.Web.UI.WebControls;

namespace Colosoft.WebControls.Route.Security
{
	public class AuthorizationHierarchy
	{
		private List<AuthorizationRule> _authorizationRuleCollection;

		/// <summary>
		/// Lista todas regras de forma hierarquica dentro de um TreeNode
		/// </summary>
		/// <param name="status">0 - Exibe todos os node Visualizar, 1 - Exibe todos os node  Nao Visualizar, 2- Exibe todos os nodes </param>
		/// <returns></returns>
		public TreeNode ListHierarchyPathTreeNode(bool status)
		{
			if(status == false)
			{
				_authorizationRuleCollection = new List<AuthorizationRule>();
				foreach (AuthorizationRule ar in Authorization.GetAllRules())
					if(ar.Status == status)
						_authorizationRuleCollection.Add(ar);
			}
			else
				_authorizationRuleCollection = new List<AuthorizationRule>(Authorization.GetAllRules());
			TreeNode node = new TreeNode();
			node.Text = "Hierarchy";
			node.SelectAction = TreeNodeSelectAction.None;
			int verify = 1;
			var result = new List<AuthorizationRule>();
			foreach (var i in _authorizationRuleCollection)
				if(i.ParentId == Guid.Empty)
					result.Add(i);
			result.Sort(delegate(AuthorizationRule ar1, AuthorizationRule ar2) {
				return string.Compare(ar1.Name, ar2.Name);
			});
			foreach (var path in result)
			{
				int count = 0;
				_authorizationRuleCollection.ForEach(f => count += (f.PathId == path.PathId ? 1 : 0));
				if(count > verify)
					verify++;
				else
				{
					node.ChildNodes.Add(RecursiveChildNodesHierarchy(path.PathId, path.Name));
					verify = 1;
				}
			}
			return node;
		}

		private TreeNode RecursiveChildNodesHierarchy(Guid? id, string fullname)
		{
			int verify = 1;
			TreeNode node = new TreeNode();
			var fieldpaths = new List<KeyValuePair<Guid, string>>();
			foreach (AuthorizationRule ar in _authorizationRuleCollection)
				if(ar.ParentId == id)
					fieldpaths.Add(new KeyValuePair<Guid, string>(ar.PathId, ar.Name));
			fieldpaths.Sort(delegate(KeyValuePair<Guid, string> p1, KeyValuePair<Guid, string> p2) {
				return string.Compare(p1.Value, p2.Value);
			});
			node.Text = fullname;
			node.Value = id.ToString();
			foreach (var fPath in fieldpaths)
			{
				int count = 0;
				_authorizationRuleCollection.ForEach(f => count += (f.PathId == fPath.Key ? 1 : 0));
				if(count > verify)
					verify++;
				else
				{
					node.ChildNodes.Add(RecursiveChildNodesHierarchy(fPath.Key, fPath.Value));
					verify = 1;
				}
			}
			return node;
		}
	}
}
