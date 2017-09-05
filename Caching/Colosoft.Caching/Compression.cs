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
	/// Armazena os dados que configuração a compressão.
	/// </summary>
	[Serializable]
	public class Compression : ICloneable
	{
		private bool _enabled;

		private int _threshold = 100;

		/// <summary>
		/// Clona os dados da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			Compression compression = new Compression();
			compression.Enabled = Enabled;
			compression.Threshold = Threshold;
			return compression;
		}

		/// <summary>
		/// Identifica se a compactação está ativa.
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
		/// Limite de compactação.
		/// </summary>
		[ConfigurationAttribute("threshold", "kb")]
		public int Threshold
		{
			get
			{
				return _threshold;
			}
			set
			{
				_threshold = value;
			}
		}
	}
}
