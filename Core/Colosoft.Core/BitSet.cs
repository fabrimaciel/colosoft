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
using System.Threading.Tasks;

namespace Colosoft
{
	/// <summary>
	/// Representa um conjunto de bits.
	/// </summary>
	[Serializable]
	public class BitSet : ICloneable
	{
		private byte _bitset;

		/// <summary>
		/// Dados do conjunto.
		/// </summary>
		public byte Data
		{
			get
			{
				return _bitset;
			}
			set
			{
				_bitset = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public BitSet()
		{
		}

		/// <summary>
		/// Cria uma instancia já definindo um conjunto inicial.
		/// </summary>
		/// <param name="bitset">Valor do conjunto inicial.</param>
		public BitSet(byte bitset)
		{
			_bitset = bitset;
		}

		/// <summary>
		/// Verifica se o bti informado está no conjunto.
		/// </summary>
		/// <param name="bit">Bit que será comparado.</param>
		/// <returns></returns>
		public bool IsAnyBitSet(byte bit)
		{
			return ((_bitset & bit) != 0);
		}

		/// <summary>
		/// Verifica se o byte informado está no conjunto.
		/// </summary>
		/// <param name="bit"></param>
		/// <returns></returns>
		public bool IsBitSet(byte bit)
		{
			return ((_bitset & bit) == bit);
		}

		/// <summary>
		/// Define e remove os bits do conjunto.
		/// </summary>
		/// <param name="bitsToSet">Bit que será marcado.</param>
		/// <param name="bitsToUnset">Bit que será desmarcado.</param>
		public void Set(byte bitsToSet, byte bitsToUnset)
		{
			this.SetBit(bitsToSet);
			this.UnsetBit(bitsToUnset);
		}

		/// <summary>
		/// Define um bit para o conjunto.
		/// </summary>
		/// <param name="bit"></param>
		public void SetBit(byte bit)
		{
			_bitset = (byte)(_bitset | bit);
		}

		/// <summary>
		/// Remove o bit do conjunto.
		/// </summary>
		/// <param name="bit"></param>
		public void UnsetBit(byte bit)
		{
			_bitset = (byte)(this._bitset & Convert.ToByte((int)(~bit & 0xff)));
		}

		/// <summary>
		/// Cria um clone da instancia.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			BitSet set = new BitSet();
			set._bitset = _bitset;
			return set;
		}
	}
}
