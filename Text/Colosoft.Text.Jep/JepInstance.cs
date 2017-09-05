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

namespace Colosoft.Text.Jep
{
	using Colosoft.Text.Jep.Parser;
	using Colosoft.Text.Jep.Types;
	using System;
	using System.IO;
	using System.Runtime.Serialization;

	[Serializable]
	public class JepInstance
	{
		protected bool _allowAssignment;

		protected bool _allowUndeclared;

		protected IEvaluator _evaluator;

		protected FunctionTable _funTab;

		protected bool _implicitMul;

		[NonSerialized]
		protected INode _lastRootNode;

		protected NodeFactory _nodeFac;

		protected INumberFactory _numFac;

		protected OperatorTable _opTab;

		protected IParser _parser;

		protected Colosoft.Text.Jep.PrintVisitor _pv;

		protected VariableFactory _varFac;

		protected VariableTable _varTab;

		[NonSerialized]
		private int pcount;

		public JepInstance() : this(new Colosoft.Text.Jep.Standard.StandardComponents())
		{
		}

		public JepInstance(ComponentSet compSet) : this(compSet.GetComponents())
		{
		}

		public JepInstance(IJepComponent[] components)
		{
			this._implicitMul = true;
			this._allowUndeclared = true;
			this._allowAssignment = true;
			this.SetComponents(components);
		}

		public Variable AddConstant(string name, object value)
		{
			return this._varTab.AddConstant(name, value);
		}

		public IPostfixMathCommand AddFunction(string name, IPostfixMathCommand pfmc)
		{
			return this._funTab.AddFunction(name, pfmc);
		}

		public bool AddStandardConstants()
		{
			try
			{
				this._varTab.AddVariable("pi", this._numFac.CreateNumber((double)3.1415926535897931));
				this._varTab.AddVariable("e", this._numFac.CreateNumber((double)2.7182818284590451));
			}
			catch(Colosoft.Text.Jep.ParseException)
			{
				return false;
			}
			catch(JepException)
			{
				return false;
			}
			return true;
		}

		public Variable AddVariable(string name)
		{
			return this._varTab.AddVariable(name);
		}

		public Variable AddVariable(string name, double value)
		{
			return this._varTab.AddVariable(name, new JepDouble(value));
		}

		public Variable AddVariable(string name, object value)
		{
			return this._varTab.AddVariable(name, value);
		}

		public Variable AddVariable(string name, double re, double im)
		{
			return this._varTab.AddVariable(name, new Complex(re, im));
		}

		public INode ContinueParsing()
		{
			if(this.pcount++ > 50)
			{
			}
			return this._parser.ContinueParse();
		}

		public object Evaluate()
		{
			return this.Evaluate(this._lastRootNode);
		}

		public object Evaluate(INode node)
		{
			if(node != null)
			{
				return this._evaluator.Evaluate(node);
			}
			return null;
		}

		public double EvaluateD()
		{
			object obj2 = this.Evaluate(this._lastRootNode);
			if(obj2 is JepDouble)
			{
				return ((JepDouble)obj2).DoubleValue;
			}
			if(!(obj2 is bool))
			{
				throw new EvaluationException("Result could not be converted to the double type");
			}
			return (((bool)obj2) ? 1.0 : 0.0);
		}

		public object GetDefaultValue()
		{
			return this._varFac.DefaultValue;
		}

		public Variable GetVariable(string name)
		{
			return this._varTab.GetVariable(name);
		}

		public object GetVariableValue(string name)
		{
			Variable variable = this._varTab.GetVariable(name);
			if(variable == null)
			{
				return null;
			}
			return variable.Value;
		}

		public void InitMultiParse(TextReader reader)
		{
			this._parser.Restart(reader);
		}

		public void InitMultiParse(string str)
		{
			this._parser.Restart(new StringReader(str));
		}

		public INode Parse(TextReader reader)
		{
			if(this.pcount++ > 50)
			{
			}
			this._lastRootNode = this._parser.Parse(reader);
			return this._lastRootNode;
		}

		public INode Parse(string str)
		{
			return this.Parse(new StringReader(str));
		}

		public void Print()
		{
			this._pv.Print(this._lastRootNode);
		}

		public void Print(INode node)
		{
			this._pv.Print(node);
		}

		public void Print(INode node, TextWriter output)
		{
			this._pv.Print(node, output);
		}

		public void PrintLine()
		{
			this._pv.PrintLine(this._lastRootNode);
		}

		public void PrintLine(INode node)
		{
			this._pv.PrintLine(node);
		}

		public void PrintLine(INode node, TextWriter output)
		{
			this._pv.PrintLine(node, output);
		}

		private void ReadObject(Formatter stream)
		{
			this.ReinitializeComponents();
		}

		public void ReinitializeComponents()
		{
			this._nodeFac.Init(this);
			this._varFac.Init(this);
			this._varTab.Init(this);
			this._funTab.Init(this);
			this._opTab.Init(this);
			this._parser.Init(this);
			this._evaluator.Init(this);
			this._pv.Init(this);
		}

		public string RootNodeToString()
		{
			return this._pv.ToString(this._lastRootNode);
		}

		public void SetComponents(IJepComponent[] components)
		{
			foreach (IJepComponent component in components)
			{
				if(component is INumberFactory)
				{
					this._numFac = (INumberFactory)component;
				}
				else if(component is VariableFactory)
				{
					this._varFac = (VariableFactory)component;
				}
				else if(component is NodeFactory)
				{
					this._nodeFac = (NodeFactory)component;
				}
				else if(component is VariableTable)
				{
					this._varTab = (VariableTable)component;
				}
				else if(component is FunctionTable)
				{
					this._funTab = (FunctionTable)component;
				}
				else if(component is OperatorTable)
				{
					this._opTab = (OperatorTable)component;
				}
				else if(component is IParser)
				{
					this._parser = (IParser)component;
				}
				else if(component is IEvaluator)
				{
					this._evaluator = (IEvaluator)component;
				}
				else if(component is Colosoft.Text.Jep.PrintVisitor)
				{
					this._pv = (Colosoft.Text.Jep.PrintVisitor)component;
				}
			}
			this.ReinitializeComponents();
		}

		public void SetDefaultValue(object defaultValue)
		{
			if(defaultValue is double)
			{
				this._varFac.DefaultValue = new JepDouble((double)defaultValue);
			}
			else if(defaultValue is int)
			{
				this._varFac.DefaultValue = new JepDouble((double)((int)defaultValue));
			}
			else
			{
				this._varFac.DefaultValue = defaultValue;
			}
		}

		public string ToString(INode node)
		{
			return this._pv.ToString(node);
		}

		public bool AllowAssignment
		{
			get
			{
				return this._allowAssignment;
			}
			set
			{
				this._allowAssignment = value;
				this._opTab.GetAssign().SetFlag(0x4000, !value);
				this.ReinitializeComponents();
			}
		}

		public bool AllowUndeclared
		{
			get
			{
				return this._allowUndeclared;
			}
			set
			{
				this._allowUndeclared = value;
			}
		}

		public IEvaluator Evaluator
		{
			get
			{
				return this._evaluator;
			}
			set
			{
				this._evaluator = value;
				this.ReinitializeComponents();
			}
		}

		public FunctionTable FunTab
		{
			get
			{
				return this._funTab;
			}
			set
			{
				this._funTab = value;
				this._funTab.Init(this);
			}
		}

		public bool ImplicitMul
		{
			get
			{
				return this._implicitMul;
			}
			set
			{
				this._implicitMul = value;
			}
		}

		public INode LastRootNode
		{
			get
			{
				return this._lastRootNode;
			}
		}

		public NodeFactory NodeFac
		{
			get
			{
				return this._nodeFac;
			}
			set
			{
				this._nodeFac = value;
				this._nodeFac.Init(this);
			}
		}

		public INumberFactory NumFac
		{
			get
			{
				return this._numFac;
			}
			set
			{
				this._numFac = value;
				this._numFac.Init(this);
			}
		}

		public OperatorTable OpTab
		{
			get
			{
				return this._opTab;
			}
			set
			{
				this._opTab = value;
				this._opTab.Init(this);
			}
		}

		public IParser Parser
		{
			get
			{
				return this._parser;
			}
			set
			{
				this._parser = value;
				this.ReinitializeComponents();
			}
		}

		public Colosoft.Text.Jep.PrintVisitor PrintVisitor
		{
			get
			{
				return this._pv;
			}
			set
			{
				this._pv = value;
				this._pv.Init(this);
			}
		}

		public VariableFactory VarFac
		{
			get
			{
				return this._varFac;
			}
			set
			{
				this._varFac = value;
				this._varFac.Init(this);
			}
		}

		public VariableTable VarTab
		{
			get
			{
				return this._varTab;
			}
			set
			{
				this._varTab = value;
				this._varTab.Init(this);
			}
		}
	}
}
