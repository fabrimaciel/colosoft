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

namespace Colosoft.Data
{
	/// <summary>
	/// Implementação de um agregador da sessão de persistencia.
	/// </summary>
	public class AggregatePersistenceSession : PersistenceSession
	{
		private PersistenceSession _owner;

		private List<PersistenceAction> _parent;

		/// <summary>
		/// Evento acioando quando a sessão for executada.
		/// </summary>
		public override event PersistenceSessionExecutedHandler Executed {
			add
			{
				_owner.Executed += value;
			}
			remove {
				_owner.Executed -= value;
			}
		}

		/// <summary>
		/// Recupera a ação pelo indice informado.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public override PersistenceAction this[int index]
		{
			get
			{
				return _parent[index];
			}
		}

		/// <summary>
		/// Quantidade de ações da sessão.
		/// </summary>
		public override int Count
		{
			get
			{
				return _parent.Count;
			}
		}

		/// <summary>
		/// Identifica a sessão raiz.
		/// </summary>
		public override IPersistenceSession Root
		{
			get
			{
				return _owner.Root;
			}
		}

		/// <summary>
		/// Ações da sessão.
		/// </summary>
		protected override List<PersistenceAction> Actions
		{
			get
			{
				return _parent;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="owner">Instancia da sessão de persistencia pai.</param>
		/// <param name="parent">Instancia da lista pai.</param>
		public AggregatePersistenceSession(PersistenceSession owner, List<PersistenceAction> parent)
		{
			owner.Require("owner").NotNull();
			parent.Require("parent").NotNull();
			_owner = owner;
			_parent = parent;
		}

		/// <summary>
		/// Recupera a instancia responsável pela execução da sessão.
		/// </summary>
		/// <returns></returns>
		protected internal override IPersistenceExecuter CreateExecuter()
		{
			return _owner.CreateExecuter();
		}

		/// <summary>
		/// Recupera as propriedades que compõem as chave do tipo da instancia informado.
		/// </summary>
		/// <param name="instanceType">Tipo da instancia.</param>
		/// <returns></returns>
		protected internal override IEnumerable<string> GetKeyProperties(Type instanceType)
		{
			return _owner.GetKeyProperties(instanceType);
		}

		/// <summary>
		/// Recupera as propriedades que podem ser persistidas do tipo informado.
		/// </summary>
		/// <param name="actionType">Tipo de ação que será realizada.</param>
		/// <param name="instanceType">Tipo da instancia onde as propriedades estão inseridas.</param>
		/// <param name="propertyNames">Nomes das propriedades que se deseja utilizar.</param>
		/// <param name="isConditional">True se operação de persistência for condicional.</param>
		/// <param name="direction">Direção que os nomes das propriedades terão no comando. (Default: DirectionPropertiesName.Inclusion)</param>
		/// <returns></returns>
		protected internal override IEnumerable<System.Reflection.PropertyInfo> GetPersistenceProperties(PersistenceActionType actionType, Type instanceType, string[] propertyNames, bool isConditional, DirectionPropertiesName direction = DirectionPropertiesName.Inclusion)
		{
			return _owner.GetPersistenceProperties(actionType, instanceType, propertyNames, isConditional, direction);
		}

		/// <summary>
		/// Recupera a ação pelo identificador informado.
		/// </summary>
		/// <param name="actionId"></param>
		/// <returns></returns>
		public override PersistenceAction GetAction(int actionId)
		{
			return _parent.Where(f => f.ActionId == actionId).FirstOrDefault();
		}

		/// <summary>
		/// Cria o id da ação.
		/// </summary>
		/// <returns></returns>
		protected internal override int CreateActionId()
		{
			return _owner.CreateActionId();
		}

		/// <summary>
		/// Recupera o validator.
		/// </summary>
		/// <returns></returns>
		protected internal override IPersistenceSessionValidator GetValidator()
		{
			return _owner.GetValidator();
		}
	}
}
