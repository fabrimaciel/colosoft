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
	/// Classe que representa um StoredProcedure de persistência.
	/// </summary>
	public class PersistenceStoredProcedure : IEnumerable<PersistenceParameter>
	{
		private List<PersistenceParameter> _parameters = new List<PersistenceParameter>();

		private int _commandTimeout = 30;

		/// <summary>
		/// Construtor padrâo.
		/// </summary>
		/// <param name="name">Nome da StoredProcedure.</param>
		/// <param name="providerName">Nome do provedor de configuração para StoredProcedure.</param>
		public PersistenceStoredProcedure(string name, string providerName = null)
		{
			Name = Colosoft.Query.StoredProcedureName.Parse(name);
			ProviderName = providerName;
		}

		/// <summary>
		/// Nome da StoredProcedure
		/// </summary>
		public Colosoft.Query.StoredProcedureName Name
		{
			get;
			set;
		}

		/// <summary>
		/// Nome do provedor de configuração para StoredProcedure.
		/// </summary>
		public string ProviderName
		{
			get;
			set;
		}

		/// <summary>
		/// Tempo de espera, em segundos, da execução do comando até gerar um erro. O valor padrão é 30 segundos.
		/// </summary>
		public int CommandTimeout
		{
			get
			{
				return _commandTimeout;
			}
			set
			{
				_commandTimeout = value;
			}
		}

		/// <summary>
		/// Número de parâmetros.
		/// </summary>
		public int Count
		{
			get
			{
				return _parameters.Count;
			}
		}

		/// <summary>
		/// Recupera o valor de um parâmetro pelo índice.
		/// </summary>
		/// <param name="index">Índice do parâmetro.</param>
		/// <returns>Valor do parâmetro a ser retornado.</returns>
		public object this[int index]
		{
			get
			{
				return _parameters[index].Value;
			}
			set
			{
				_parameters[index].Value = value;
			}
		}

		/// <summary>
		/// Recupera valor do parâmetro pelo nome.
		/// </summary>
		/// <param name="name">Nome do parâmetro</param>
		/// <returns>Valor do parâmetro a ser retornado.</returns>
		public object this[string name]
		{
			get
			{
				return _parameters.Where(p => p.Name == name).Select(f => f.Value).FirstOrDefault();
			}
			set
			{
				_parameters[_parameters.FindIndex(p => p.Name == name)].Value = value;
			}
		}

		/// <summary>
		/// Adiciona um parâmetro a procedure.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="direction">Direção do parâmetro.</param>
		/// <returns>Retorna própria instãncia.</returns>
		public PersistenceStoredProcedure AddParameter(string name, object value, Colosoft.Query.ParameterDirection direction = Colosoft.Query.ParameterDirection.Input)
		{
			_parameters.Add(new PersistenceParameter(name, value, direction));
			return this;
		}

		/// <summary>
		/// Adiciona um parâmetro a procedure.
		/// </summary>
		/// <param name="name">Nome do parâmetro.</param>
		/// <param name="value">Valor do parâmetro.</param>
		/// <param name="size">Tamanho do parâmtro.</param>
		/// <param name="direction">Direção do parâmetro.</param>
		/// <returns>Retorna própria instãncia.</returns>
		public PersistenceStoredProcedure AddParameter(string name, object value, int size, Colosoft.Query.ParameterDirection direction = Colosoft.Query.ParameterDirection.Input)
		{
			_parameters.Add(new PersistenceParameter(name, value, size, direction));
			return this;
		}

		/// <summary>
		/// Adiciona parâmetro a procedure.
		/// </summary>
		/// <param name="parameter">Parâmetro representado por objeto do tipo <see cref="PersistenceParameter"/>.</param>
		/// <returns>Retorna própria instância.</returns>
		public PersistenceStoredProcedure AddParameter(PersistenceParameter parameter)
		{
			_parameters.Add(parameter);
			return this;
		}

		/// <summary>
		/// Remove um parâmetro baseado no nome.
		/// </summary>
		/// <param name="name">Nome do parâmetro a ser removido.</param>
		/// <returns>Retorna própria instância.</returns>
		public PersistenceStoredProcedure RemoveParameter(string name)
		{
			_parameters.RemoveAll(p => p.Name == name);
			return this;
		}

		/// <summary>
		/// Remove um parâmetro baseado no índice.
		/// </summary>
		/// <param name="index">Índice do parâmetro a ser removido.</param>
		/// <returns>Retorna própria intância.</returns>
		public PersistenceStoredProcedure RemoveParameter(int index)
		{
			_parameters.RemoveAt(index);
			return this;
		}

		/// <summary>
		/// Recupera enumerador genérico.
		/// </summary>
		/// <returns>Retorna enumerador genérico.</returns>
		public IEnumerator<PersistenceParameter> GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}

		/// <summary>
		/// Recupera enumerador não genérico.
		/// </summary>
		/// <returns>Retorna enumerador não genérico.</returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _parameters.GetEnumerator();
		}
	}
}
