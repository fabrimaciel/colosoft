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

namespace Colosoft.Domain
{
	/// <summary>
	/// Classe que representa a estratégia de execução de uma evento.
	/// </summary>
	public class DomainEventExecutionStrategy : IDomainEventExecutionStrategy
	{
		private Type _domainEventType;

		private readonly Threading.SimpleMonitor _monitor = new Threading.SimpleMonitor();

		private Action<object[]> _action;

		private Predicate<object[]> _filter;

		private string _name;

		/// <summary>
		/// Evento acionado quando a instancia for liberada.
		/// </summary>
		public event EventHandler Disposed;

		/// <summary>
		/// Tipo do evento de domínio associado.
		/// </summary>
		public Type DomainEventType
		{
			get
			{
				return _domainEventType;
			}
		}

		/// <summary>
		/// Verifica se a execução pode ser realizada.
		/// </summary>
		/// <param name="arguments">Argumentos que serão usados na validação da execução.</param>
		/// <returns></returns>
		public bool CanExecute(params object[] arguments)
		{
			try
			{
				if(_monitor.Busy)
					return false;
				if(_filter != null)
					return _filter(arguments);
				return true;
			}
			finally
			{
				_monitor.Enter();
			}
		}

		/// <summary>
		/// Executa a estratégia.
		/// </summary>
		/// <param name="arguments">Argumentos que serão usados na execução.</param>
		public void Execute(params object[] arguments)
		{
			_action(arguments);
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="name">Nome associado.</param>
		/// <param name="domainEventType">Tipo do evento de domínio.</param>
		/// <param name="action">Ação da estratégia.</param>
		/// <param name="filter">Filtro que será usado para validar a execução da estratégia.</param>
		public DomainEventExecutionStrategy(string name, Type domainEventType, Action<object[]> action, Predicate<object[]> filter)
		{
			domainEventType.Require("domainEventType").NotNull();
			action.Require("action").NotNull();
			_name = name;
			_domainEventType = domainEventType;
			_action = action;
			_filter = filter;
		}

		/// <summary>
		/// Recupera o texto que representa a instancia.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if(!string.IsNullOrEmpty(_name))
				return string.Format("[Name : {0}]", _name);
			return base.ToString();
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
			if(Disposed != null)
				Disposed(this, EventArgs.Empty);
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			_monitor.Dispose();
		}
	}
}
