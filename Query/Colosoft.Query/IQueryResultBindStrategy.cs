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
	/// Possíveis modos de estratégia de bind.
	/// </summary>
	public enum BindStrategyMode
	{
		/// <summary>
		/// Identifica se é para realizar o bind de todos os dados
		/// do registro para a instancia
		/// </summary>
		All,
		/// <summary>
		/// Identifica se é para realizar o bind somente os
		/// dados diferentes do registro em relação com a instancia.
		/// </summary>
		Differences
	}
	/// <summary>
	/// Assinatura das classes com a estratégia de vinculação do resultado
	/// de uma consulta.
	/// </summary>
	public interface IQueryResultBindStrategy
	{
		/// <summary>
		/// Cria uma sessão de estratégia de vinculação para o descritor informado.
		/// </summary>
		/// <param name="recordDescriptor">Descritor dos registros para fazer a vinculação.</param>
		/// <returns></returns>
		IQueryResultBindStrategySession CreateSession(Record.RecordDescriptor recordDescriptor);

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		IEnumerable<string> Bind(IRecord record, BindStrategyMode mode, ref object instance);

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado na operação.</typeparam>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		IEnumerable<string> Bind<T>(IRecord record, BindStrategyMode mode, ref T instance);

		/// <summary>
		/// Executa a vinculação dos dados contidos na enumeração de registro informados
		/// e retorna uma enumeração das instancias preechidas.
		/// </summary>
		/// <param name="records">Registros que serão processados.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="creator">Instancia responsável pela criação dos objetos.</param>
		/// <returns></returns>
		System.Collections.IEnumerable Bind(IEnumerable<IRecord> records, BindStrategyMode mode, IQueryResultObjectCreator creator);

		/// <summary>
		/// Executa a vinculação dos dados contidos na enumeração de registro informados
		/// e retorna uma enumeração das instancias preechidas.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado na operação.</typeparam>
		/// <param name="records">Registros que serão processados.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="creator">Instancia responsável pela criação dos objetos.</param>
		/// <returns></returns>
		IEnumerable<T> Bind<T>(IEnumerable<IRecord> records, BindStrategyMode mode, IQueryResultObjectCreator creator);
	}
	/// <summary>
	/// Assinatura da sessão de estratégia de vinculação do resultado da uma consulta.
	/// </summary>
	public interface IQueryResultBindStrategySession
	{
		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		IEnumerable<string> Bind(IRecord record, BindStrategyMode mode, ref object instance);
	}
}
