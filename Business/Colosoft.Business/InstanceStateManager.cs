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

namespace Colosoft.Business
{
	/// <summary>
	/// Assinatura da classe reponsável por monitorar os eventos
	/// ocorridos nos InstanceState do sistema.
	/// </summary>
	public interface IInstanceStateObserver
	{
		/// <summary>
		/// Notifica que ocorreu um erro ao carregar as expecialização para a entidade.
		/// </summary>
		/// <param name="entityTypeName">Nome da entidade na qual a especialização foi carregada.</param>
		/// <param name="specialization">Instancia da especialização caso ela tenha sido carregada.</param>
		/// <param name="error">Error ocorrido.</param>
		void OnEntitySpecializationError(Colosoft.Reflection.TypeName entityTypeName, Colosoft.Validation.IEntitySpecialization specialization, Exception error);
	}
	/// <summary>
	/// Agreagador dos observers <see cref="IInstanceStateObserver"/>.
	/// </summary>
	public class AggregateInstanceStateObserver : AggregateObserver<IInstanceStateObserver>
	{
		/// <summary>
		/// Notifica que ocorreu um erro ao carregar as expecialização para a entidade.
		/// </summary>
		/// <param name="entityTypeName">Nome da entidade na qual a especialização foi carregada.</param>
		/// <param name="specialization">Instancia da especialização caso ela tenha sido carregada.</param>
		/// <param name="error">Error ocorrido.</param>
		public void OnEntitySpecializationError(Colosoft.Reflection.TypeName entityTypeName, Colosoft.Validation.IEntitySpecialization specialization, Exception error)
		{
			foreach (var i in this.Observers)
				i.OnEntitySpecializationError(entityTypeName, specialization, error);
		}
	}
	/// <summary>
	/// Classe que gerencia os estados de instancia do sistema.
	/// </summary>
	public static class InstanceStateManager
	{
		class Info
		{
			/// <summary>
			/// Identifica que está com erro na especialização.
			/// </summary>
			public bool ContainsSpecializationError;
		}

		private static AggregateInstanceStateObserver _observer;

		private static Dictionary<Reflection.TypeName, Info> _infos = new Dictionary<Reflection.TypeName, Info>(Reflection.TypeName.TypeNameFullNameComparer.Instance);

		/// <summary>
		/// Instancia do observer associado.
		/// </summary>
		public static AggregateInstanceStateObserver Observer
		{
			get
			{
				return _observer;
			}
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static InstanceStateManager()
		{
			_observer = new AggregateInstanceStateObserver();
		}

		/// <summary>
		/// Recupera a instancia das informações da entidade.
		/// </summary>
		/// <param name="entityTypeName"></param>
		/// <returns></returns>
		private static Info GetInfo(Reflection.TypeName entityTypeName)
		{
			lock (_infos)
			{
				Info info = null;
				if(!_infos.TryGetValue(entityTypeName, out info))
				{
					info = new Info();
					_infos.Add(entityTypeName, info);
				}
				return info;
			}
		}

		/// <summary>
		/// Verifica se o tipo da entidade possui erro na carga da especialização.
		/// </summary>
		/// <param name="entityTypeName"></param>
		/// <returns></returns>
		public static bool ContainsSpecializationError(Reflection.TypeName entityTypeName)
		{
			entityTypeName.Require("entityTypeName").NotNull();
			lock (_infos)
				return _infos.ContainsKey(entityTypeName);
		}

		/// <summary>
		/// Registra um erro na carga da expecialização entidade.
		/// </summary>
		/// <param name="entityTypeName"></param>
		/// <param name="entitySpecialization"></param>
		/// <param name="error"></param>
		public static void RegisterSpecializationError(Reflection.TypeName entityTypeName, Colosoft.Validation.IEntitySpecialization entitySpecialization, Exception error)
		{
			entityTypeName.Require("entityTypeName").NotNull();
			GetInfo(entityTypeName).ContainsSpecializationError = true;
			Observer.OnEntitySpecializationError(entityTypeName, entitySpecialization, error);
		}
	}
}
