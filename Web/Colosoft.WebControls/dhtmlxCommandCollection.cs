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
using System.Text;
using System.Collections;
using System.ComponentModel;

namespace Colosoft.WebControls.dhtmlx
{
	public class dhtmlxCommandCollection : ICollection, IEnumerable
	{
		private ArrayList _commands = new ArrayList();

		public int IndexOf(dhtmlxCommand command)
		{
			if(command != null)
			{
				return _commands.IndexOf(command);
			}
			return -1;
		}

		public void Add(string commandName, params string[] parameters)
		{
			AddAt(-1, commandName, parameters);
		}

		public void AddAt(int index, string commandName, params string[] parameters)
		{
			AddAt(index, new dhtmlxCommand(commandName, parameters));
		}

		public void Add(dhtmlxCommand command)
		{
			this.AddAt(-1, command);
		}

		public void AddAt(int index, dhtmlxCommand command)
		{
			if(command == null)
			{
				throw new ArgumentNullException("command");
			}
			if(index == -1)
			{
				_commands.Add(command);
			}
			else
			{
				_commands.Insert(index, command);
			}
		}

		public void Remove(dhtmlxCommand command)
		{
			int index = this.IndexOf(command);
			if(index >= 0)
			{
				this.RemoveAt(index);
			}
		}

		public void RemoveAt(int index)
		{
			if((index < 0) || (index >= this.Count))
			{
				throw new ArgumentOutOfRangeException("index");
			}
			_commands.RemoveAt(index);
		}

		public void Clear()
		{
			_commands.Clear();
		}

		[Browsable(false)]
		public dhtmlxCommand this[int index]
		{
			get
			{
				return (dhtmlxCommand)_commands[index];
			}
		}

		public void CopyTo(Array array, int index)
		{
			if(array == null)
			{
				throw new ArgumentNullException("array");
			}
			IEnumerator enumerator = this.GetEnumerator();
			while (enumerator.MoveNext())
			{
				array.SetValue(enumerator.Current, index++);
			}
		}

		public int Count
		{
			get
			{
				return _commands.Count;
			}
		}

		public bool IsSynchronized
		{
			get
			{
				return false;
			}
		}

		public object SyncRoot
		{
			get
			{
				return false;
			}
		}

		public IEnumerator GetEnumerator()
		{
			return _commands.GetEnumerator();
		}
	}
}
