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

namespace Colosoft.Caching.Configuration.Dom
{
	/// <summary>
	/// Armazena os dados do provedor de assembly,
	/// </summary>
	[Serializable]
	public class ProviderAssembly : ICloneable
	{
		private string _fullProviderName;

		private string _assemblyName;

		private string _className;

		/// <summary>
		/// Nome do assembly associado.
		/// </summary>
		[ConfigurationAttribute("assembly-name")]
		public string AssemblyName
		{
			get
			{
				return _assemblyName;
			}
			set
			{
				_assemblyName = value;
			}
		}

		/// <summary>
		/// Nome da classe do assembly.
		/// </summary>
		[ConfigurationAttribute("class-name")]
		public string ClassName
		{
			get
			{
				return _className;
			}
			set
			{
				_className = value;
			}
		}

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		[ConfigurationAttribute("full-name")]
		public string FullProviderName
		{
			get
			{
				return _fullProviderName;
			}
			set
			{
				_fullProviderName = value;
			}
		}

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			ProviderAssembly assembly = new ProviderAssembly();
			assembly.AssemblyName = (this.AssemblyName != null) ? ((string)this.AssemblyName.Clone()) : null;
			assembly.ClassName = (this.ClassName != null) ? ((string)this.ClassName.Clone()) : null;
			assembly.FullProviderName = (this.FullProviderName != null) ? ((string)this.FullProviderName.Clone()) : null;
			return assembly;
		}
	}
}
