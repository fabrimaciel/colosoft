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

namespace Colosoft.Text.Jep.ConfigurableParser
{
	using Colosoft.Text.Jep.DataStructures;
	using System;
	using System.Collections.Generic;

	public class Lookahead2Enumerator<T> where T : class
	{
		private IExtendedEnumerator<T> it;

		private T next;

		private T nextnext;

		public Lookahead2Enumerator(IExtendedEnumerator<T> input)
		{
			this.next = default(T);
			this.nextnext = default(T);
			this.it = input;
			if(this.it.HasNext())
			{
				this.next = this.it.Next();
			}
			if(this.it.HasNext())
			{
				this.nextnext = this.it.Next();
			}
		}

		public Lookahead2Enumerator(List<T> input)
		{
			this.next = default(T);
			this.nextnext = default(T);
			this.it = new ExtendedEnumerator<T>(input);
			if(this.it.HasNext())
			{
				this.next = this.it.Next();
			}
			if(this.it.HasNext())
			{
				this.nextnext = this.it.Next();
			}
		}

		public void Consume()
		{
			this.next = this.nextnext;
			if(this.it.HasNext())
			{
				this.nextnext = this.it.Next();
			}
			else
			{
				this.nextnext = default(T);
			}
		}

		public T PeekNext()
		{
			return this.next;
		}

		public T PeekNextnext()
		{
			return this.nextnext;
		}
	}
}
