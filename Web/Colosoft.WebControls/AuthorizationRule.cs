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

namespace Colosoft.WebControls.Route.Security
{
	/// <summary>
	/// Ações da regra da autorização.
	/// </summary>
	public enum AuthorizationRuleAction
	{
		Deny = 0,
		Allow = 1
	}
	/// <summary>
	/// Tipo de regras de autorização.
	/// </summary>
	public enum AuthorizationRuleType
	{
		Users = 1,
		Roles = 2,
		HttpVerbs = 3
	}
	/// <summary>
	/// Representa uma regra de autorização.
	/// </summary>
	public class AuthorizationRule
	{
        /// <summary>
        /// Idenficador para recupera a regra de acesso usada na requisição.
        /// </summary>
        internal const string AUTH_RULE_CONTEXT_ID = "ROUTE_AUTH_RULE";

        private StringCollection _data;

		private string _path;

		private string _name;

		private bool _loginPageRedirect = true;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public AuthorizationRule()
		{
		}

		/// <summary>
		/// Constrói uma instancia apartir de uma regra.
		/// </summary>
		/// <param name="ruleId"></param>
		internal AuthorizationRule(Guid ruleId)
		{
			this.RuleId = ruleId;
		}

		/// <summary>
		/// Ação da regra.
		/// </summary>
		public AuthorizationRuleAction Action
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo da regra.
		/// </summary>
		public AuthorizationRuleType Type
		{
			get;
			set;
		}

		/// <summary>
		/// Identificarod o caminho.
		/// </summary>
		public Guid PathId
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador do pai do caminho.
		/// </summary>
		public Guid ParentId
		{
			get;
			set;
		}

		/// <summary>
		/// Identificador da regra.
		/// </summary>
		public Guid RuleId
		{
			get;
			set;
		}

		/// <summary>
		/// Situação da regra.
		/// </summary>
		public bool Status
		{
			get;
			set;
		}

		/// <summary>
		/// Caminho
		/// </summary>
		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this._path = value;
			}
		}

		/// <summary>
		/// Nome da regra.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Dados da regra.
		/// </summary>
		public StringCollection Data
		{
			get
			{
				if(this._data == null)
					this._data = new StringCollection();
				return this._data;
			}
			set
			{
				if(value != null)
					Helper.ValidateDataCollection(value);
				this._data = value;
			}
		}

		/// <summary>
		/// Identifica se a regra terá o redirecionamento para a página de login.
		/// </summary>
		public bool LoginPageRedirect
		{
			get
			{
				return _loginPageRedirect;
			}
			set
			{
				_loginPageRedirect = value;
			}
		}

		/// <summary>
		/// Nome do pai.
		/// </summary>
		public string ParentName
		{
			get
			{
				return Authorization.FindNamePath(ParentId);
			}
		}

		/// <summary>
		/// Dados formatados para exibição.
		/// </summary>
		public string DataDisplay
		{
			get
			{
				return Helper.ConvertCollectionInString(Data);
			}
		}

		/// <summary>
		/// A propriedade Everyone retorna um valor booleano indicando se o caractere “*” está definido na propriedade Data
		/// </summary>
		public bool Everyone
		{
			get
			{
				if(_data != null)
					return _data.Contains("*");
				return false;
			}
		}

		/// <summary>
		/// A propriedade Anonymous também retorna um valor booleano indicando a existência do caractere “?”. 
		/// </summary>
		public bool Anonymous
		{
			get
			{
				if(_data != null)
					return _data.Contains("?");
				return false;
			}
		}
	}
}
