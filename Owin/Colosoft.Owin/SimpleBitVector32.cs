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
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Colosoft.Web.Util
{
	[Serializable, StructLayout(LayoutKind.Sequential)]
	internal struct SimpleBitVector32
	{
		private int data;

		internal SimpleBitVector32(int data)
		{
			this.data = data;
		}

		internal int IntegerValue
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}

		internal bool this[int bit]
		{
			get
			{
				return ((this.data & bit) == bit);
			}
			set
			{
				int data = this.data;
				if(value)
				{
					this.data = data | bit;
				}
				else
				{
					this.data = data & ~bit;
				}
			}
		}

		internal int this[int mask, int offset]
		{
			get
			{
				return ((this.data & mask) >> offset);
			}
			set
			{
				this.data = (this.data & ~mask) | (value << offset);
			}
		}

		internal void Set(int bit)
		{
			this.data |= bit;
		}

		internal void Clear(int bit)
		{
			this.data &= ~bit;
		}
	}
}
