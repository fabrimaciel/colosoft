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

namespace Colosoft.WebControls.Route
{
	/// <summary>
	/// Armazena as informações da rota.
	/// </summary>
	public class RouteInfo
	{
		private string _name;

		private string _location;

		private string _path;

		private string _destinationPage;

		private bool _isCompiled = true;

		private string _actionName;

		private bool _isDirectory = false;

		private bool _isSecurePage = false;

		private bool _forceNoSecurePage = false;

		/// <summary>
		/// Nome do roteamento.
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			internal set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Localização que ativa o roteamento.
		/// </summary>
		public string Location
		{
			get
			{
				return _location;
			}
			internal set
			{
				_location = value;
			}
		}

		/// <summary>
		/// Informações do path aceito pelo roteamento.
		/// </summary>
		public string Path
		{
			get
			{
				return _path;
			}
			internal set
			{
				_path = value;
			}
		}

		/// <summary>
		/// Página de destino do roteamento.
		/// </summary>
		public string DestinationPage
		{
			get
			{
				return _destinationPage;
			}
			internal set
			{
				_destinationPage = value;
			}
		}

		/// <summary>
		/// Determina que o conteúdo é compilado.
		/// </summary>
		public bool Compiled
		{
			get
			{
				return _isCompiled;
			}
			internal set
			{
				_isCompiled = value;
			}
		}

		/// <summary>
		/// Nome da ação relacionada.
		/// </summary>
		public string ActionName
		{
			get
			{
				return _actionName;
			}
			internal set
			{
				_actionName = value;
			}
		}

		/// <summary>
		/// Identifica se o roteamento é para um diretório.
		/// </summary>
		public bool IsDirectory
		{
			get
			{
				return _isDirectory;
			}
			set
			{
				_isDirectory = value;
			}
		}

		/// <summary>
		/// Identifica que a página exige segurança.
		/// </summary>
		public bool IsSecurePage
		{
			get
			{
				return _isSecurePage;
			}
			set
			{
				_isSecurePage = value;
			}
		}

		/// <summary>
		/// Força a rota não atender páginas de segurança.
		/// </summary>
		public bool ForceNoSecurePage
		{
			get
			{
				return _forceNoSecurePage;
			}
			set
			{
				_forceNoSecurePage = value;
			}
		}

		/// <summary>
		/// Construtor padrao.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="location"></param>
		/// <param name="destinationPage"></param>
		/// <param name="path"></param>
		/// <param name="compiled"></param>
		/// <param name="actionName"></param>
		/// <param name="isDirectory">Identifica que o roteamento é para um diretório.</param>
		/// <param name="isSecurePage"></param>
		/// <param name="forceNoSecurePage"></param>
		public RouteInfo(string name, string location, string destinationPage, string path, bool compiled, string actionName, bool isDirectory, bool isSecurePage, bool forceNoSecurePage)
		{
			_name = name;
			_location = location;
			_destinationPage = destinationPage;
			_path = path;
			_isCompiled = compiled;
			_actionName = actionName;
			_isDirectory = isDirectory;
			_isSecurePage = isSecurePage;
			_forceNoSecurePage = forceNoSecurePage;
		}

		/// <summary>
		/// Método acionado quando inicia uma requisição para a rota.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		internal RouteActionBeginResponse BeginRequest(RouteActionBeginRequest request)
		{
			if(!string.IsNullOrEmpty(ActionName))
			{
				var action = RouteSettings.GetAction(ActionName);
				if(action != null)
					return action.BeginRequest(this, request);
			}
			return null;
		}

		/// <summary>
		/// Método acionado quando a rota está sendo pré-carregada.
		/// </summary>
		internal RouteActionResponse PreLoad(RouteActionRequest request)
		{
			if(!string.IsNullOrEmpty(ActionName))
			{
				var action = RouteSettings.GetAction(ActionName);
				if(action != null)
					return action.PreLoad(this, request);
			}
			return null;
		}

		/// <summary>
		/// Método acionado quando a rota for carregada.
		/// </summary>
		internal void Load()
		{
			if(!string.IsNullOrEmpty(ActionName))
			{
				var action = RouteSettings.GetAction(ActionName);
				if(action != null)
					action.Load(this);
			}
		}
	}
}
