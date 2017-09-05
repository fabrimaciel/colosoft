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
using System.Runtime.Serialization;
using System.Text;

namespace Colosoft.Data
{
	/// <summary>
	/// Implemnetação de uma coleção de parametros de persistencia.
	/// </summary>
	[Serializable]
	[System.Xml.Serialization.XmlRoot(Namespace = Namespaces.Data)]
	public class PersistenceParameterCollection : List<PersistenceParameter>, ISerializable, Colosoft.Serialization.ICompactSerializable, ICloneable
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public PersistenceParameterCollection() : base()
		{
		}

		/// <summary>
		/// Cria uma nova instancia a partir de uma enumeração existente.
		/// </summary>
		/// <param name="collection"></param>
		public PersistenceParameterCollection(IEnumerable<PersistenceParameter> collection) : base(collection)
		{
		}

		/// <summary>
		/// Cria uma nova instancia já definindo a sua capacidade inicial.
		/// </summary>
		/// <param name="capacity"></param>
		public PersistenceParameterCollection(int capacity) : base(capacity)
		{
		}

		/// <summary>
		/// Construtor usado na deserialização dos dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected PersistenceParameterCollection(SerializationInfo info, StreamingContext context)
		{
			var count = info.GetInt32("C");
			for(var i = 0; i < count; i++)
				this.Add((PersistenceParameter)info.GetValue(i.ToString(), typeof(PersistenceParameter)));
		}

		/// <summary>
		/// Adicionar um parametro.
		/// </summary>
		/// <param name="name">Nome do parametro.</param>
		/// <param name="value">parameter value</param>
		public PersistenceParameterCollection Add(string name, object value)
		{
			base.Add(new PersistenceParameter(name, value));
			return this;
		}

		/// <summary>
		/// Remove todos os parametros.
		/// </summary>
		public void RemoveAllParameters()
		{
			base.Clear();
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
				var parameter = new PersistenceParameter();
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
			return new PersistenceParameterCollection(this.Select(f => (PersistenceParameter)f.Clone()));
		}
	}
}
