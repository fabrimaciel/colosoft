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
using Colosoft.Security.Util;

namespace Colosoft.Security.Permissions
{
	/// <summary>
	/// Classe que gerencia o acesso ao sistema.
	/// </summary>
	[Serializable]
	internal sealed class SystemAccess
	{
		private bool _allItems;

		private bool _ignoreCase;

		private bool _pathDiscovery;

		private StringExpressionSet _set;

		/// <summary>
		/// Identifica se engloba todos os itens.
		/// </summary>
		public bool AllItems
		{
			get
			{
				return _allItems;
			}
			set
			{
				_allItems = value;
			}
		}

		/// <summary>
		/// Caminho do acesso.
		/// </summary>
		public bool PathDiscovery
		{
			set
			{
				_pathDiscovery = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public SystemAccess()
		{
			_ignoreCase = true;
			_set = new StringExpressionSet(this._ignoreCase, true);
			_allItems = false;
			_pathDiscovery = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="pathDiscovery">Caminho descoberto.</param>
		public SystemAccess(bool pathDiscovery)
		{
			_ignoreCase = true;
			_set = new StringExpressionSet(this._ignoreCase, true);
			_allItems = false;
			_pathDiscovery = pathDiscovery;
		}

		/// <summary>
		/// Cria um nova instancia com base em dados da outro acesso.
		/// </summary>
		/// <param name="operand"></param>
		private SystemAccess(SystemAccess operand)
		{
			_ignoreCase = true;
			_set = operand._set.Copy();
			_allItems = operand._allItems;
			_pathDiscovery = operand._pathDiscovery;
		}

		/// <summary>
		/// Cria um novo acesso paara o sistema definindo a expressão para validar.
		/// </summary>
		/// <param name="value"></param>
		public SystemAccess(string value)
		{
			_ignoreCase = true;
			if(value == null)
			{
				_set = new StringExpressionSet(_ignoreCase, true);
				_allItems = false;
			}
			else if((value.Length >= "*AllItems*".Length) && (string.Compare("*AllItems*", value, StringComparison.Ordinal) == 0))
			{
				_set = new StringExpressionSet(this._ignoreCase, true);
				_allItems = true;
			}
			else
			{
				_set = new StringExpressionSet(_ignoreCase, value, true);
				_allItems = false;
			}
			this._pathDiscovery = false;
		}

		public SystemAccess(bool AllItems, bool pathDiscovery)
		{
			_ignoreCase = true;
			_set = new StringExpressionSet(this._ignoreCase, true);
			_allItems = AllItems;
			_pathDiscovery = pathDiscovery;
		}

		public SystemAccess(StringExpressionSet set, bool AllItems, bool pathDiscovery)
		{
			_ignoreCase = true;
			_set = set;
			_set.SetThrowOnRelative(true);
			_allItems = AllItems;
			_pathDiscovery = pathDiscovery;
		}

		/// <summary>
		/// Recupera a raiz do caminho.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private static string GetRoot(string path)
		{
			string str = path.Substring(0, 3);
			if(str.EndsWith(@":\", StringComparison.Ordinal))
				return str;
			return null;
		}

		/// <summary>
		/// Adiciona expressões para o acesso.
		/// </summary>
		/// <param name="values">Expressões que serão adicionadas.</param>
		/// <param name="checkForDuplicates">True para ver</param>
		public void AddExpressions(List<string> values, bool checkForDuplicates)
		{
			_allItems = false;
			_set.AddExpressions(values, checkForDuplicates);
		}

		/// <summary>
		/// Vericica se o acesso é vazio.
		/// </summary>
		/// <returns></returns>
		public bool IsEmpty()
		{
			if(_allItems)
				return false;
			if(_set != null)
				return _set.IsEmpty();
			return true;
		}

		/// <summary>
		/// Cria um cópia da instancia.
		/// </summary>
		/// <returns></returns>
		public SystemAccess Copy()
		{
			return new SystemAccess(this);
		}

		/// <summary>
		/// Faz uma intercessão dos acessos.
		/// </summary>
		/// <param name="operand"></param>
		/// <returns></returns>
		public SystemAccess Intersect(SystemAccess operand)
		{
			if(operand == null)
				return null;
			if(_allItems)
			{
				if(operand._allItems)
					return new SystemAccess(true, this._pathDiscovery);
				return new SystemAccess(operand._set.Copy(), false, _pathDiscovery);
			}
			if(operand._allItems)
				return new SystemAccess(_set.Copy(), false, _pathDiscovery);
			StringExpressionSet set = new StringExpressionSet(_ignoreCase, true);
			string[] strArray3 = _set.Intersect(operand._set).ToStringArray();
			if(strArray3 != null)
				set.AddExpressions(strArray3, !set.IsEmpty(), false);
			return new SystemAccess(set, false, _pathDiscovery);
		}

		/// <summary>
		/// Realiza a união com o acesso informado.
		/// </summary>
		/// <param name="operand"></param>
		/// <returns></returns>
		public SystemAccess Union(SystemAccess operand)
		{
			if(operand == null)
			{
				if(!this.IsEmpty())
					return this.Copy();
				return null;
			}
			if(_allItems || operand._allItems)
				return new SystemAccess(true, _pathDiscovery);
			return new SystemAccess(this._set.Union(operand._set), false, this._pathDiscovery);
		}

		/// <summary>
		/// Verifica se o acesso informado é um subconjunto da instancia.
		/// </summary>
		/// <param name="operand"></param>
		/// <returns></returns>
		public bool IsSubsetOf(SystemAccess operand)
		{
			if(operand == null)
				return this.IsEmpty();
			if(!operand._allItems && ((!this._pathDiscovery || !_set.IsSubsetOfPathDiscovery(operand._set)) && !this._set.IsSubsetOf(operand._set)))
				return false;
			return true;
		}

		/// <summary>
		/// Compara um objeto com a instancia.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		public override bool Equals(object obj)
		{
			SystemAccess operand = obj as SystemAccess;
			if(operand == null)
				return (this.IsEmpty() && (obj == null));
			if(this._pathDiscovery)
				return ((this._allItems && operand._allItems) || (operand._set.IsSubsetOf(this._set)));
			if(!this.IsSubsetOf(operand))
				return false;
			if(!operand.IsSubsetOf(this))
				return false;
			return true;
		}

		/// <summary>
		/// Itens do acesso.
		/// </summary>
		/// <returns></returns>
		public string[] ToStringArray()
		{
			return _set.ToStringArray();
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			if(_allItems)
				return "*AllItems*";
			string str = "";
			string str2 = _set.ToString();
			if((str2 != null) && (str2.Length > 0))
			{
				str = str2;
			}
			return str;
		}
	}
}
