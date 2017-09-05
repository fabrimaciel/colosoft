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
	/// Implementa da estratégia de vinculação usando as propriedade 
	/// do tipo da instancia onde a vinculação será feita.
	/// </summary>
	public class TypeBindStrategy : IQueryResultBindStrategy, IQueryResultObjectCreator, IDisposable
	{
		private IQueryResultObjectCreator _creator;

		/// <summary>
		/// Armazena a relação dos esquemas já carregados.
		/// </summary>
		private List<TypeBindRecordDescriptorSchema> _schemas = new List<TypeBindRecordDescriptorSchema>();

		/// <summary>
		/// Tipo usado pela estratégia.
		/// </summary>
		public Type Type
		{
			get;
			set;
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="type">Tipo que será usado pela instancia.</param>
		/// <param name="creator">Instancia responsável por criar um instancia do tipo informado.</param>
		public TypeBindStrategy(Type type, IQueryResultObjectCreator creator = null)
		{
			type.Require("type").NotNull();
			if(creator == null)
				_creator = new QueryResultObjectCreator(type);
			else
				_creator = creator;
			Type = type;
		}

		/// <summary>
		/// Destrutor.
		/// </summary>
		~TypeBindStrategy()
		{
			Dispose(false);
		}

		/// <summary>
		/// Recupera o esquema que será usado para converter os dados do registro.
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		private TypeBindRecordDescriptorSchema GetRecordSchema(Record.RecordDescriptor descriptor)
		{
			lock (_schemas)
			{
				if(_schemas.Count > 0)
					foreach (var i in _schemas)
						if(i.Equals(descriptor))
							return i;
				var schema = new TypeBindRecordDescriptorSchema(this, descriptor);
				_schemas.Add(schema);
				return schema;
			}
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		/// <param name="disposing"></param>
		protected virtual void Dispose(bool disposing)
		{
			while (_schemas.Count > 0)
			{
				var schema = _schemas[0];
				_schemas.RemoveAt(0);
				schema.Dispose();
			}
			_creator = null;
			Type = null;
		}

		/// <summary>
		/// Libera a instancia.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		public IEnumerable<string> Bind(IRecord record, BindStrategyMode mode, ref object instance)
		{
			record.Require("record").NotNull();
			if(instance == null && _creator != null)
				instance = _creator.Create();
			return CreateSession(record.Descriptor).Bind(record, mode, ref instance);
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos no registro para o objeto informado.
		/// </summary>
		/// <typeparam name="T">Tipo que será usado na operação.</typeparam>
		/// <param name="record">Registro da consulta.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="instance">Instancia onde será vinculado os dados.</param>
		/// <returns>Relação das propriedade na qual o bind foi aplicado.</returns>
		public IEnumerable<string> Bind<T>(IRecord record, BindStrategyMode mode, ref T instance)
		{
			record.Require("record").NotNull();
			object instance2 = instance;
			if(instance2 == null && _creator != null)
				instance2 = _creator.Create();
			var result = CreateSession(record.Descriptor).Bind(record, mode, ref instance2);
			instance = (T)instance2;
			return result;
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos na enumeração de registro informados
		/// e retorna uma enumeração das instancias preechidas.
		/// </summary>
		/// <param name="records">Registros que serão processados.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="creator">Instancia responsável pela criação dos objetos.</param>
		/// <returns></returns>
		public System.Collections.IEnumerable Bind(IEnumerable<IRecord> records, BindStrategyMode mode, IQueryResultObjectCreator creator)
		{
			IQueryResultBindStrategySession session = null;
			foreach (var i in records)
			{
				if(session == null)
					session = CreateSession(i.Descriptor);
				object result = creator.Create();
				session.Bind(i, mode, ref result);
				yield return result;
			}
		}

		/// <summary>
		/// Executa a vinculação dos dados contidos na enumeração de registro informados
		/// e retorna uma enumeração das instancias preechidas.
		/// </summary>
		/// <param name="records">Registros que serão processados.</param>
		/// <param name="mode">Modo da estratégia.</param>
		/// <param name="creator">Instancia responsável pela criação dos objetos.</param>
		/// <returns></returns>
		public IEnumerable<T> Bind<T>(IEnumerable<IRecord> records, BindStrategyMode mode, IQueryResultObjectCreator creator)
		{
			IQueryResultBindStrategySession session = null;
			foreach (var i in records)
			{
				if(session == null)
					session = CreateSession(i.Descriptor);
				object result = creator.Create();
				session.Bind(i, mode, ref result);
				yield return (T)result;
			}
		}

		/// <summary>
		/// Cria uma sessão de estratégia de vinculação para o descritor informado.
		/// </summary>
		/// <param name="recordDescriptor">Descritor dos registros para fazer a vinculação.</param>
		/// <returns></returns>
		public IQueryResultBindStrategySession CreateSession(Record.RecordDescriptor recordDescriptor)
		{
			return new TypeBindStrategySession(GetRecordSchema(recordDescriptor));
		}

		/// <summary>
		/// Cria uma nova instancia do tipo do resultado da consulta.
		/// </summary>
		/// <returns></returns>
		public object Create()
		{
			return _creator.Create();
		}
	}
	/// <summary>
	/// Implementa da estratégia de vinculação usando as propriedade 
	/// do tipo da instancia onde a vinculação será feita.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class TypeBindStrategy<T> : TypeBindStrategy
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="creator">Instancia responsável por criar um instancia do tipo informado.</param>
		public TypeBindStrategy(IQueryResultObjectCreator creator = null) : base(typeof(T), creator)
		{
		}
	}
}
