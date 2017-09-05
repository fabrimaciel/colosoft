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
	/// Classe responsável por gerar o identificador unico para os observers.
	/// </summary>
	public static class QueryResultChangedObserverUidGenerator
	{
		private static object _objLock = new object();

		private static ulong _lastUid;

		/// <summary>
		/// Cria um novo identificador.
		/// </summary>
		/// <returns></returns>
		public static ulong CreateUid()
		{
			lock (_objLock)
			{
				_lastUid++;
				if(_lastUid > ulong.MaxValue - 1)
					_lastUid = 0;
				return _lastUid;
			}
		}
	}
	/// <summary>
	/// Assinatura das classe que é um observer das modificações
	/// sobre o resultado de uma consulta.
	/// </summary>
	public interface IQueryResultChangedObserver : IDisposable
	{
		/// <summary>
		/// Identificador único associado com o observer.
		/// </summary>
		ulong Uid
		{
			get;
		}

		/// <summary>
		/// Identifica se o observer está vivo.
		/// </summary>
		bool IsAlive
		{
			get;
		}

		/// <summary>
		/// Avalia se o registro possui vinculo
		/// com o resultado da consulta.
		/// </summary>
		/// <param name="record">Instancia do registro que será usado no calculo.</param>
		/// <returns></returns>
		bool Evaluate(IRecord record);

		/// <summary>
		/// Avalia se a chave pertence a algum dos registros
		/// associados com o resultado da consulta.
		/// </summary>
		/// <param name="recordKey">Chave que será usada na avaliação.</param>
		/// <returns></returns>
		bool Evaluate(RecordKey recordKey);

		/// <summary>
		/// Método acionado quando o registro for inserido.
		/// </summary>
		/// <param name="record"></param>
		void OnRecordInserted(IRecord record);

		/// <summary>
		/// Método acionado quando registro for apagado.
		/// </summary>
		/// <param name="recordKey">Chave do registro apagado.</param>
		void OnRecordDeleted(RecordKey recordKey);
	}
}
