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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Representa um componente Sxs.
	/// </summary>
	internal class SxsComponent
	{
		private string _name;

		private string _processorArchitecture;

		private string _version;

		/// <summary>
		/// Nome do componente.
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
		/// Arquitetura do processador.
		/// </summary>
		public string ProcessorArchitecture
		{
			get
			{
				return _processorArchitecture;
			}
			set
			{
				_processorArchitecture = value;
			}
		}

		/// <summary>
		/// Versão do componente.
		/// </summary>
		public string Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="version"></param>
		/// <param name="processorArchitecture"></param>
		public SxsComponent(string name, string version, string processorArchitecture)
		{
			_name = name;
			_version = version;
			_processorArchitecture = processorArchitecture;
		}

		/// <summary>
		/// Recupera o caminho completo.
		/// </summary>
		/// <returns></returns>
		public string GetFullPath()
		{
			foreach (string dir in System.IO.Directory.GetDirectories(System.IO.Path.GetFullPath(Environment.GetFolderPath(Environment.SpecialFolder.System) + @"\..\WinSxs")))
				if((dir.Contains(_name) && dir.Contains(_version)) && dir.Contains(_processorArchitecture))
					return dir;
			return "";
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return (_name + " " + _version + "(" + _processorArchitecture + ")");
		}
	}
}
