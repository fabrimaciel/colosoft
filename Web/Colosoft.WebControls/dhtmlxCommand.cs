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

namespace Colosoft.WebControls.dhtmlx
{
	/// <summary>
	/// Representa as informações de um comando a ser
	/// chamado por um componente dhtmlx.
	/// </summary>
	public class dhtmlxCommand
	{
		private string _commandName;

		private List<string> _parameters = new List<string>();

		/// <summary>
		/// Nome do comando.
		/// </summary>
		public string CommandName
		{
			get
			{
				return _commandName;
			}
			set
			{
				_commandName = value;
			}
		}

		/// <summary>
		/// Parametros do comando.
		/// </summary>
		public IList<string> Parameters
		{
			get
			{
				return _parameters;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="commandName">Nome do comando.</param>
		/// <param name="parameters">Parametros do comando.</param>
		public dhtmlxCommand(string commandName, params string[] parameters)
		{
			_commandName = commandName;
			_parameters.AddRange(parameters);
		}

		internal void LoadElement(XmlDocument doc, XmlElement parent)
		{
			XmlElement cmd = doc.CreateElement("call");
			cmd.SetAttribute("command", _commandName);
			foreach (string p in _parameters)
			{
				XmlElement param = doc.CreateElement("param");
				param.InnerText = p;
				cmd.AppendChild(param);
			}
			parent.AppendChild(cmd);
		}
	}
}
