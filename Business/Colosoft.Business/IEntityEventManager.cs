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
	/// Armazena os argumentos do erro ocorrido ao registrar 
	/// as informações do evento de uma entidade.
	/// </summary>
	public class RegisterEntityEventInfoErrorArgs : EventArgs
	{
		/// <summary>
		/// Nome do tipo da entidade associada.
		/// </summary>
		public Reflection.TypeName TypeName
		{
			get;
			set;
		}

		/// <summary>
		/// Informações do evento que deveria ser registrad.
		/// </summary>
		public EntityEventInfo EventInfo
		{
			get;
			set;
		}

		/// <summary>
		/// Erro associado.
		/// </summary>
		public Exception Error
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="typeName"></param>
		/// <param name="eventInfo"></param>
		/// <param name="error"></param>
		public RegisterEntityEventInfoErrorArgs(Reflection.TypeName typeName, EntityEventInfo eventInfo, Exception error)
		{
			this.TypeName = typeName;
			this.EventInfo = eventInfo;
			this.Error = error;
		}
	}
	/// <summary>
	/// Representa o método do evento acionado quando ocorre um erro
	/// ao registrar as informações do evento de uma entidade.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void RegisterEntityEventInfoErrorHandler (object sender, RegisterEntityEventInfoErrorArgs e);
	/// <summary>
	/// Assinatura do gerenciador de eventos da entidade.
	/// </summary>
	public interface IEntityEventManager : IDisposable
	{
		/// <summary>
		/// Evento acionado quando ocorre um erro ao registra as informações do 
		/// evento de uma entidade.
		/// </summary>
		event RegisterEntityEventInfoErrorHandler RegisterEventInfoError;

		/// <summary>
		/// Registra o tipo da entidade que será geranciada.
		/// </summary>
		/// <param name="entityType"></param>
		void Register(Type entityType);
	}
}
