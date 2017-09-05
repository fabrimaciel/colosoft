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

namespace Colosoft.Cryptography
{
	/// <summary>
	/// Armazena os dados da chave de criptografia.
	/// </summary>
	public class RSACryptografyKey
	{
		/// <summary>
		/// BitStrength
		/// </summary>
		public int BitStrength
		{
			get;
			set;
		}

		/// <summary>
		/// Valor da chave.
		/// </summary>
		public RSAKey RSAKeyValue
		{
			get;
			set;
		}

		/// <summary>
		/// Classe que armazena os dados da chave.
		/// </summary>
		public class RSAKey
		{
			/// <summary>
			/// Modulus.
			/// </summary>
			public string Modulus
			{
				get;
				set;
			}

			/// <summary>
			/// Exponent
			/// </summary>
			public string Exponent
			{
				get;
				set;
			}

			/// <summary>
			/// P
			/// </summary>
			public string P
			{
				get;
				set;
			}

			/// <summary>
			/// Q
			/// </summary>
			public string Q
			{
				get;
				set;
			}

			/// <summary>
			/// DP
			/// </summary>
			public string DP
			{
				get;
				set;
			}

			/// <summary>
			/// DQ
			/// </summary>
			public string DQ
			{
				get;
				set;
			}

			/// <summary>
			/// InverseQ
			/// </summary>
			public string InverseQ
			{
				get;
				set;
			}

			/// <summary>
			/// D
			/// </summary>
			public string D
			{
				get;
				set;
			}
		}
	}
}
