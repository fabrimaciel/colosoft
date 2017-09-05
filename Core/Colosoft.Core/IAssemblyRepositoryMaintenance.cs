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

namespace Colosoft.Reflection
{
	/// <summary>
	/// Armazena o resultado da execução da manuteção do repositório de assemblies.
	/// </summary>
	public class AssemblyRepositoryMaintenanceExecuteResult : IEnumerable<AssemblyRepositoryMaintenanceExecuteResult.Entry>
	{
		private List<Entry> _entries;

		/// <summary>
		/// Quantidade de entradas
		/// </summary>
		public int Count
		{
			get
			{
				return _entries.Count;
			}
		}

		/// <summary>
		/// Recupera a entrada na posição informada.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public Entry this[int index]
		{
			get
			{
				return _entries[index];
			}
		}

		/// <summary>
		/// Verifica se o resultado possui algum erro.
		/// </summary>
		public bool HasError
		{
			get
			{
				return _entries.Where(f => f.Type == EntryType.Error).Any();
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="entries"></param>
		public AssemblyRepositoryMaintenanceExecuteResult(IEnumerable<Entry> entries)
		{
			entries.Require("entries").NotNull();
			_entries = new List<Entry>(entries);
		}

		/// <summary>
		/// Tipo de entrada.
		/// </summary>
		public enum EntryType
		{
			/// <summary>
			/// Error.
			/// </summary>
			Error,
			/// <summary>
			/// Informação.
			/// </summary>
			Info,
			/// <summary>
			/// Alerta
			/// </summary>
			Warn
		}

		/// <summary>
		/// Armazena os dados da entrada.
		/// </summary>
		public class Entry
		{
			/// <summary>
			/// Mensagem da entrada.
			/// </summary>
			public IMessageFormattable Message
			{
				get;
				set;
			}

			/// <summary>
			/// Tipo da entrada.
			/// </summary>
			public EntryType Type
			{
				get;
				set;
			}

			/// <summary>
			/// Erro relacioando.
			/// </summary>
			public Exception Error
			{
				get;
				set;
			}

			/// <summary>
			/// Construtor padrão.
			/// </summary>
			/// <param name="message"></param>
			/// <param name="type"></param>
			/// <param name="error"></param>
			public Entry(IMessageFormattable message, EntryType type, Exception error = null)
			{
				this.Message = message;
				this.Type = type;
				this.Error = error;
			}
		}

		/// <summary>
		/// Recupera o enumerador das entradas.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<AssemblyRepositoryMaintenanceExecuteResult.Entry> GetEnumerator()
		{
			return _entries.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador das entradas.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _entries.GetEnumerator();
		}
	}
	/// <summary>
	/// Assinatura das classes reponsáveis pela manutenção do repositório de assemblies.
	/// </summary>
	public interface IAssemblyRepositoryMaintenance : IDisposable
	{
		/// <summary>
		/// Nome da instancia.
		/// </summary>
		string Name
		{
			get;
		}

		/// <summary>
		/// Executa a manutenção do repositório.
		/// </summary>
		AssemblyRepositoryMaintenanceExecuteResult Execute();
	}
}
