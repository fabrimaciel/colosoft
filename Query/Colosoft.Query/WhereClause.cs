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

namespace Colosoft.Query
{
	/// <summary>
	/// Classe que representa a clausa where.
	/// </summary>
	public class WhereClause : Colosoft.Query.IWhereClause
	{
		private ConditionalContainer _container;

		/// <summary>
		/// Pai
		/// </summary>
		protected IWhereClause Owner
		{
			get;
			set;
		}

		/// <summary>
		/// Instancia do container.
		/// </summary>
		public virtual ConditionalContainer Container
		{
			get
			{
				return _container;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="container"></param>
		public WhereClause(ConditionalContainer container)
		{
			container.Require("container").NotNull();
			_container = container;
		}

		/// <summary>
		/// Construtor para filhos.
		/// </summary>
		protected WhereClause()
		{
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual IWhereClause And(Conditional conditional)
		{
			Container.And(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual IWhereClause Or(Conditional conditional)
		{
			Container.Or(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo AND.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual IWhereClause And(string expression)
		{
			Container.And(expression);
			return this;
		}

		/// <summary>
		/// Adiciona uma condição do tipo OR.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual IWhereClause Or(string expression)
		{
			Container.Or(expression);
			return this;
		}

		/// <summary>
		/// Adiciona um container de condição do tipo AND.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual IWhereClause And(ConditionalContainer conditional)
		{
			Container.And(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona um container de condição do tipo OR.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual IWhereClause Or(ConditionalContainer conditional)
		{
			Container.Or(conditional);
			return this;
		}

		/// <summary>
		/// Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual IWhereClause Start(Conditional conditional)
		{
			Container.Start(conditional);
			return this;
		}

		/// <summary>
		///Adiciona a condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="expression"></param>
		/// <returns></returns>
		public virtual IWhereClause Start(string expression)
		{
			Container.Start(expression);
			return this;
		}

		/// <summary>
		/// Adiciona o container condição inicial. Essa operação limpa todas a outras condições já existentes.
		/// </summary>
		/// <param name="conditional"></param>
		/// <returns></returns>
		public virtual IWhereClause Start(ConditionalContainer conditional)
		{
			Container.Start(conditional);
			return this;
		}

		/// <summary>
		/// Texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Container.ToString();
		}

		/// <summary>
		/// Adiciona um parametro.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public virtual IWhereClause Add(string name, object value)
		{
			Container.Add(new QueryParameter(name, value));
			return this;
		}

		/// <summary>
		/// Adiciona o parametro para o clausula.
		/// </summary>
		/// <param name="parameter"></param>
		/// <returns></returns>
		public virtual IWhereClause Add(QueryParameter parameter)
		{
			Container.Add(parameter);
			return this;
		}
	}
	/// <summary>
	/// Representa um clausula condicional Where.
	/// </summary>
	public class QueryableWhereClause : WhereClause
	{
		private Queryable _queryable;

		/// <summary>
		/// Instancia do container.
		/// </summary>
		public override ConditionalContainer Container
		{
			get
			{
				var container = _queryable.WhereClause;
				if(container == null)
				{
					container = new ConditionalContainer();
					_queryable.WhereClause = container;
				}
				return container;
			}
		}

		/// <summary>
		/// Instancia do Queryable associado.
		/// </summary>
		public Queryable Queryable
		{
			get
			{
				return _queryable;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		public QueryableWhereClause(Queryable queryable)
		{
			queryable.Require("queryable").NotNull();
			_queryable = queryable;
		}
	///// <summary>
	///// Adiciona um parametro.
	///// </summary>
	///// <param name="name"></param>
	///// <param name="value"></param>
	///// <returns></returns>
	///// <summary>
	///// Adiciona o parametro para o clausula.
	///// </summary>
	///// <param name="parameter"></param>
	///// <returns></returns>
	}
	/// <summary>
	/// Representa um clausula condicional Where para join.
	/// </summary>
	public class JoinWhereClause : WhereClause
	{
		private Queryable _queryable;

		private int _joinIndex;

		/// <summary>
		/// Instancia do container.
		/// </summary>
		public override ConditionalContainer Container
		{
			get
			{
				var container = _queryable.Joins[_joinIndex].Conditional;
				if(container == null)
				{
					container = new ConditionalContainer();
					_queryable.Joins[_joinIndex].Conditional = container;
				}
				return container;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryable"></param>
		/// <param name="joinIndex"></param>
		public JoinWhereClause(Queryable queryable, int joinIndex)
		{
			queryable.Require("queryable").NotNull();
			_queryable = queryable;
			_joinIndex = joinIndex;
		}
	}
}
