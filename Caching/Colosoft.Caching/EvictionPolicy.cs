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
	public class EvictionPolicy : ICloneable
	{
		private string _defaultPriority;

		private bool _enabled;

		private decimal _evictionRatio;

		private string _policy;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			EvictionPolicy policy = new EvictionPolicy();
			policy.Enabled = Enabled;
			policy.DefaultPriority = (DefaultPriority != null) ? ((string)DefaultPriority.Clone()) : null;
			policy.EvictionRatio = EvictionRatio;
			policy.Policy = Policy;
			return policy;
		}

		/// <summary>
		/// Prioridade padrão.
		/// </summary>
		[ConfigurationAttribute("default-priority")]
		public string DefaultPriority
		{
			get
			{
				return _defaultPriority;
			}
			set
			{
				_defaultPriority = value;
			}
		}

		/// <summary>
		/// Identifica se está habilitado.
		/// </summary>
		[ConfigurationAttribute("enabled")]
		public bool Enabled
		{
			get
			{
				return _enabled;
			}
			set
			{
				_enabled = value;
			}
		}

		/// <summary>
		/// Raio.
		/// </summary>
		[ConfigurationAttribute("eviction-ratio", "%")]
		public decimal EvictionRatio
		{
			get
			{
				return _evictionRatio;
			}
			set
			{
				_evictionRatio = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		[ConfigurationAttribute("policy")]
		public string Policy
		{
			get
			{
				return _policy;
			}
			set
			{
				_policy = value;
			}
		}
	}
}
