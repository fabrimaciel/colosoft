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
using System.Runtime.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Representa uma coleção de parametros.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlRoot(Namespace = Namespaces.Query)]
	public class QueryParameterCollection : List<QueryParameter>, IQueryParameterContainerExt, ISerializable, Colosoft.Serialization.ICompactSerializable, ICloneable
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public QueryParameterCollection() : base()
		{
		}

		/// <summary>
		/// Cria uma nova instancia a partir de uma enumeração existente.
		/// </summary>
		/// <param name="collection"></param>
		public QueryParameterCollection(IEnumerable<QueryParameter> collection) : base(collection)
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo a sua capacidade inicial.
		/// </summary>
		/// <param name="capacity"></param>
		public QueryParameterCollection(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected QueryParameterCollection(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("C");
			for(var i = 0; i < count; i++)
				this.Add((QueryParameter)info.GetValue(i.ToString(), typeof(QueryParameter)));
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">parameter value</param>
		public QueryParameterCollection Add(string name, object value)
		{
			base.Add(new QueryParameter(name, value));
			return this;
		}

		/// <summary>
		/// Remove todos os parametros.
		/// </summary>
		public void RemoveAllParameters()
		{
			base.Clear();
		}

		void IQueryParameterContainer.Add(QueryParameter parameter)
		{
			Add(parameter);
		}

		/// <summary>
		/// Remove o parametro informado.
		/// </summary>
		/// <param name="parameter"></param>
		bool IQueryParameterContainerExt.Remove(QueryParameter parameter)
		{
			return Remove(parameter);
		}

		/// <summary>
		/// Recupera os dados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			info.AddValue("C", this.Count);
			for(var i = 0; i < Count; i++)
				info.AddValue(i.ToString(), this[i]);
		}

		/// <summary>
		/// Deserializa usando o CompactSerializer.
		/// </summary>
		/// <param name="reader">Representa o compact reader.</param>
		public void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			var count = reader.ReadInt32();
			for(int i = 0; i < count; i++)
			{
				var parameter = new QueryParameter();
				parameter.Deserialize(reader);
				Add(parameter);
			}
		}

		/// <summary>
		/// Serializa usando o CompactSerializer.
		/// </summary>
		/// <param name="writer">Representa o compact writer.</param>
		public void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			writer.Write(this.Count);
			for(var i = 0; i < Count; i++)
				((Colosoft.Serialization.ICompactSerializable)this[i]).Serialize(writer);
		}

		/// <summary>
		/// Clona a coleção de parâmetros.
		/// </summary>
		/// <returns></returns>
		public object Clone()
		{
			return new QueryParameterCollection(this.Select(f => (QueryParameter)f.Clone()));
		}
	}
}
