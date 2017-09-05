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

namespace Colosoft.Text.Jep.Standard
{
	using Colosoft.Text.Jep;
	using Colosoft.Text.Jep.Functions;
	using Colosoft.Text.Jep.Parser;
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	public class StandardEvaluator : IEvaluator, IJepComponent, IParserVisitor
	{
		private bool _trapInfinity;

		private bool _trapNaN;

		private bool _trapNullValues;

		[NonSerialized]
		private Stack<object> stack = new Stack<object>();

		public object Eval(INode node)
		{
			try
			{
				node.JjtAccept(this, null);
			}
			catch(EvaluationException exception)
			{
				throw exception;
			}
			catch(JepException exception2)
			{
				throw new EvaluationException("", exception2);
			}
			return this.stack.Pop();
		}

		public object Evaluate(INode node)
		{
			this.stack.Clear();
			try
			{
				node.JjtAccept(this, null);
			}
			catch(EvaluationException exception)
			{
				throw exception;
			}
			catch(JepException exception2)
			{
				throw new EvaluationException("", exception2);
			}
			if(this.stack.Count != 1)
			{
				throw new EvaluationException("Stack corrupted");
			}
			return this.stack.Pop();
		}

		public IJepComponent GetLightWeightInstance()
		{
			return new StandardEvaluator {
				_trapNullValues = this._trapNullValues
			};
		}

		public void GetObjectData(SerializationInfo info, StreamingContext context)
		{
		}

		public void Init(JepInstance jep)
		{
		}

		public object Visit(ASTConstant node, object data)
		{
			object item = node.Value;
			if(this._trapNaN && (((item is double) && double.IsNaN((double)item)) || ((item is float) && float.IsNaN((float)item))))
			{
				throw new EvaluationException("NaN constant value detected");
			}
			if(this._trapInfinity && (((item is double) && double.IsInfinity((double)item)) || ((item is float) && float.IsInfinity((float)item))))
			{
				throw new EvaluationException("Infinite constant value " + item.ToString() + "detected");
			}
			this.stack.Push(item);
			return null;
		}

		public object Visit(ASTFunNode node, object data)
		{
			this.VisitFun(node);
			return null;
		}

		public object Visit(ASTOpNode node, object data)
		{
			this.VisitFun(node);
			return null;
		}

		public object Visit(ASTVarNode node, object data)
		{
			Variable var = node.Var;
			if(var == null)
			{
				throw new EvaluationException("var was null in StandardEvaluator");
			}
			object item = var.Value;
			if(this._trapNullValues && (item == null))
			{
				throw new EvaluationException("Could not evaluate " + node.GetName() + ": no value set for the variable.");
			}
			if(this._trapNaN && (((item is double) && double.IsNaN((double)item)) || ((item is float) && float.IsNaN((float)item))))
			{
				throw new EvaluationException("NaN value detected for variable " + node.GetName());
			}
			if(this._trapInfinity && (((item is double) && double.IsInfinity((double)item)) || ((item is float) && float.IsInfinity((float)item))))
			{
				throw new EvaluationException("Infinite value " + item.ToString() + "detected for variable " + node.GetName());
			}
			this.stack.Push(item);
			return data;
		}

		protected void VisitFun(ASTFunNode node)
		{
			IPostfixMathCommand pFMC = node.GetPFMC();
			if(pFMC == null)
			{
				throw new EvaluationException("No function class associated with " + node.GetName());
			}
			if(pFMC is ICallbackEvaluation)
			{
				object item = ((ICallbackEvaluation)pFMC).Evaluate(node, this);
				if(this._trapNaN && (((item is double) && double.IsNaN((double)item)) || ((item is float) && float.IsNaN((float)item))))
				{
					throw new EvaluationException("NaN value detected for result of function/operator " + node.GetName());
				}
				if(this._trapInfinity && (((item is double) && double.IsInfinity((double)item)) || ((item is float) && float.IsInfinity((float)item))))
				{
					throw new EvaluationException("Infinite value " + item.ToString() + "detected for result of function/operator " + node.GetName());
				}
				this.stack.Push(item);
			}
			else
			{
				int n = node.JjtGetNumChildren();
				for(int i = 0; i < n; i++)
				{
					INode node2 = node.JjtGetChild(i);
					try
					{
						node2.JjtAccept(this, null);
					}
					catch(JepException exception)
					{
						throw new EvaluationException("", exception);
					}
				}
				int numberOfParameters = pFMC.GetNumberOfParameters();
				if((numberOfParameters != -1) && (numberOfParameters != n))
				{
					throw new EvaluationException(string.Concat(new object[] {
						"Incorrect number of children ",
						n,
						". Should have been ",
						numberOfParameters
					}));
				}
				pFMC.SetCurNumberOfParameters(n);
				pFMC.Run(this.stack);
				object obj3 = this.stack.Peek();
				if(this._trapNaN && (((obj3 is double) && double.IsNaN((double)obj3)) || ((obj3 is float) && float.IsNaN((float)obj3))))
				{
					throw new EvaluationException("NaN value detected for result of function/operator " + node.GetName());
				}
				if(this._trapInfinity && (((obj3 is double) && double.IsInfinity((double)obj3)) || ((obj3 is float) && float.IsInfinity((float)obj3))))
				{
					throw new EvaluationException("Infinite value " + obj3.ToString() + "detected for result of function/operator " + node.GetName());
				}
			}
		}

		public bool TrapInfinity
		{
			get
			{
				return this._trapInfinity;
			}
			set
			{
				this._trapInfinity = value;
			}
		}

		public bool TrapNaN
		{
			get
			{
				return this._trapNaN;
			}
			set
			{
				this._trapNaN = value;
			}
		}

		public bool TrapNullValues
		{
			get
			{
				return this._trapNullValues;
			}
			set
			{
				this._trapNullValues = value;
			}
		}
	}
}
