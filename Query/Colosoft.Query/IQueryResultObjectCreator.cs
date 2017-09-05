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
	/// Assinatura das classes responsáveis pela criação de uma instancia para 
	/// o resultado da consulta.
	/// </summary>
	public interface IQueryResultObjectCreator
	{
		/// <summary>
		/// Cria uma nova instancia do tipo do resultado da consulta.
		/// </summary>
		/// <returns></returns>
		object Create();
	}
	/// <summary>
	/// Implementação padrão do <see cref="IQueryResultObjectCreator"/>.
	/// </summary>
	public class QueryResultObjectCreator : IQueryResultObjectCreator
	{
		private Func<object> _creator;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="creator"></param>
		public QueryResultObjectCreator(Func<object> creator)
		{
			creator.Require("creator").NotNull();
			_creator = creator;
		}

		/// <summary>
		/// Cria um creator a partir do tipo informado.
		/// </summary>
		/// <param name="type"></param>
		public QueryResultObjectCreator(Type type)
		{
			type.Require("type").NotNull();
			var emptyConstructor = type.GetConstructor(new Type[0]);
			if(emptyConstructor == null)
			{
				_creator = new Func<object>(() =>  {
					throw new QueryException(ResourceMessageFormatter.Create(() => Properties.Resources.Exception_NotFoundEmptyConstructorForType, type.FullName).Format());
				});
			}
			else
				_creator = new Func<object>(() =>  {
					try
					{
						return Activator.CreateInstance(type);
					}
					catch(System.Reflection.TargetInvocationException ex)
					{
						throw ex.InnerException;
					}
				});
		}

		/// <summary>
		/// Cria uma nova instancia do tipo do resultado da consulta.
		/// </summary>
		/// <returns></returns>
		public object Create()
		{
			return _creator();
		}
	}
}
