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
	/// Armazena os dados da origem de fundo.
	/// </summary>
	[Serializable]
	public class BackingSource : ICloneable
	{
		private Readthru _readthru;

		private Writethru _writehtru;

		/// <summary>
		/// COnfigurações do leitor.
		/// </summary>
		[ConfigurationSection("read-thru")]
		public Readthru Readthru
		{
			get
			{
				return _readthru;
			}
			set
			{
				_readthru = value;
			}
		}

		/// <summary>
		/// Configurações do escritor.
		/// </summary>
		[ConfigurationSection("write-thru")]
		public Writethru Writethru
		{
			get
			{
				return _writehtru;
			}
			set
			{
				_writehtru = value;
			}
		}

		/// <summary>
		/// Cria um clone da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			BackingSource source = new BackingSource();
			source.Readthru = (this.Readthru != null) ? ((Readthru)this.Readthru.Clone()) : null;
			source.Writethru = (this.Writethru != null) ? ((Writethru)this.Writethru.Clone()) : null;
			return source;
		}
	}
}
