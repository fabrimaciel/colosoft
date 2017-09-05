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
	/// 
	/// </summary>
	[Serializable]
	public class Provider : ICloneable
	{
		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Provider provider = new Provider();
			provider.AsyncMode = this.AsyncMode;
			provider.ProviderName = (this.ProviderName != null) ? ((string)this.ProviderName.Clone()) : null;
			provider.AssemblyName = (this.AssemblyName != null) ? ((string)this.AssemblyName.Clone()) : null;
			provider.ClassName = (this.ClassName != null) ? ((string)this.ClassName.Clone()) : null;
			provider.FullProviderName = (this.FullProviderName != null) ? ((string)this.FullProviderName.Clone()) : null;
			provider.Parameters = (this.Parameters != null) ? (this.Parameters.Clone() as Parameter[]) : null;
			return provider;
		}

		/// <summary>
		/// Nome do assembly onde o provedor está inserido.
		/// </summary>
		[ConfigurationAttribute("assembly-name")]
		public string AssemblyName
		{
			get;
			set;
		}

		/// <summary>
		/// Trabalha com modo assincrono.
		/// </summary>
		[ConfigurationAttribute("async-mode")]
		public bool AsyncMode
		{
			get;
			set;
		}

		/// <summary>
		/// Nome da classe.
		/// </summary>
		[ConfigurationAttribute("class-name")]
		public string ClassName
		{
			get;
			set;
		}

		/// <summary>
		/// Nome completo do provedor.
		/// </summary>
		[ConfigurationAttribute("full-name")]
		public string FullProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Identifica se é o provedor padrão.
		/// </summary>
		[ConfigurationAttribute("default-provider")]
		public bool IsDefaultProvider
		{
			get;
			set;
		}

		/// <summary>
		/// Parametros de configuração.
		/// </summary>
		[ConfigurationSection("param")]
		public Parameter[] Parameters
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do provedor.
		/// </summary>
		[ConfigurationAttribute("provider-name")]
		public string ProviderName
		{
			get;
			set;
		}
	}
}
