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

namespace Colosoft.Text.Jep.Parser
{
	using Colosoft.Text.Jep;
	using System;
	using System.Collections;

	public abstract class SimpleNode : INode
	{
		protected int _id;

		protected INode[] children;

		protected Hashtable hooks;

		protected INode parent;

		protected IParser parser;

		public SimpleNode(int i)
		{
			this._id = i;
		}

		public SimpleNode(IParser p, int i) : this(i)
		{
			this.parser = p;
		}

		public object ChildrenAccept(IParserVisitor visitor, object data)
		{
			if(this.children != null)
			{
				for(int i = 0; i < this.children.Length; i++)
				{
					this.children[i].JjtAccept(visitor, data);
				}
			}
			return data;
		}

		public void Dump(string prefix)
		{
			Console.WriteLine(this.ToString(prefix));
			if(this.children != null)
			{
				for(int i = 0; i < this.children.Length; i++)
				{
					SimpleNode node = (SimpleNode)this.children[i];
					if(node != null)
					{
						node.Dump(prefix + " ");
					}
				}
			}
		}

		public object GetHook(string key)
		{
			if(this.hooks == null)
			{
				return null;
			}
			return this.hooks[key];
		}

		public virtual string GetName()
		{
			throw new EvaluationException("GetName method of SimpleNode called directly - should only be called from an AST node");
		}

		public virtual Operator GetOperator()
		{
			throw new EvaluationException("GetOperator method of SimpleNode called directly - should only be called from an AST node");
		}

		public virtual IPostfixMathCommand GetPFMC()
		{
			throw new EvaluationException("GetPFMC method of SimpleNode called directly - should only be called from an AST node");
		}

		public ICollection HookKeys()
		{
			if(this.hooks == null)
			{
				return null;
			}
			return this.hooks.Keys;
		}

		public virtual object JjtAccept(IParserVisitor visitor, object data)
		{
			throw new JepException("Illegal node type encountered");
		}

		public void JjtAddChild(INode n, int i)
		{
			if(this.children == null)
			{
				this.children = new INode[i + 1];
			}
			else if(i >= this.children.Length)
			{
				INode[] destinationArray = new INode[i + 1];
				Array.Copy(this.children, 0, destinationArray, 0, this.children.Length);
				this.children = destinationArray;
			}
			this.children[i] = n;
		}

		public void JjtClose()
		{
		}

		public INode JjtGetChild(int i)
		{
			return this.children[i];
		}

		public int JjtGetNumChildren()
		{
			if(this.children != null)
			{
				return this.children.Length;
			}
			return 0;
		}

		public INode JjtGetParent()
		{
			return this.parent;
		}

		public void JjtOpen()
		{
		}

		public void JjtSetParent(INode n)
		{
			this.parent = n;
		}

		public object SetHook(string key, object hook)
		{
			object obj2;
			if(this.hooks == null)
			{
				this.hooks = new Hashtable();
			}
			try
			{
				obj2 = this.hooks[key];
			}
			catch(Exception)
			{
				obj2 = null;
			}
			this.hooks[key] = hook;
			return obj2;
		}

		public override string ToString()
		{
			return "";
		}

		public virtual string ToString(string prefix)
		{
			return (prefix + this.ToString());
		}

		public int Id
		{
			get
			{
				return this._id;
			}
		}
	}
}
