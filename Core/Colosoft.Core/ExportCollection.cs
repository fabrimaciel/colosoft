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

namespace Colosoft.Reflection.Composition
{
	/// <summary>
	/// Implementação da coleção de exports.
	/// </summary>
	[Serializable]
	public class ExportCollection : ICollection<IExport>, Serialization.ICompactSerializable, System.Runtime.Serialization.ISerializable
	{
		private List<IExport> _exports = new List<IExport>();

		/// <summary>
		/// Identifica se a coleção é somente leitura.
		/// </summary>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Quantidade de itens na coleção.
		/// </summary>
		public int Count
		{
			get
			{
				return _exports.Count;
			}
		}

		/// <summary>
		/// Recupera e define o valor para a posição da coleção.
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		public IExport this[int index]
		{
			get
			{
				return _exports[index];
			}
			set
			{
				_exports[index] = value;
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public ExportCollection()
		{
		}

		/// <summary>
		/// Inicializa a coleção com os itens informados.
		/// </summary>
		/// <param name="items">Relação dos itens que serão usados na inicialização.</param>
		public ExportCollection(IEnumerable<IExport> items)
		{
			_exports.AddRange(items);
		}

		/// <summary>
		/// Construtor usado para deserializar os dados.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected ExportCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			var count = info.GetInt32("Count");
			for(var i = 0; i < count; i++)
				_exports.Add((IExport)info.GetValue(i.ToString(), typeof(Export)));
		}

		/// <summary>
		/// Adiciona o item para a coleção.
		/// </summary>
		/// <param name="item"></param>
		public void Add(IExport item)
		{
			_exports.Add(item);
		}

		/// <summary>
		/// Limpa os itens da coleção.
		/// </summary>
		public void Clear()
		{
			_exports.Clear();
		}

		/// <summary>
		/// Verifica se o item informado está na coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Contains(IExport item)
		{
			return _exports.Contains(item);
		}

		/// <summary>
		/// Copia os dados para o vetor informado.
		/// </summary>
		/// <param name="array"></param>
		/// <param name="arrayIndex"></param>
		public void CopyTo(IExport[] array, int arrayIndex)
		{
			_exports.CopyTo(array, arrayIndex);
		}

		/// <summary>
		/// Remove o item da coleção.
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public bool Remove(IExport item)
		{
			return _exports.Remove(item);
		}

		/// <summary>
		/// Remove o item no indice informado.
		/// </summary>
		/// <param name="index"></param>
		public void RemoveAt(int index)
		{
			_exports.RemoveAt(index);
		}

		/// <summary>
		/// Deserializa os dados.
		/// </summary>
		/// <param name="reader"></param>
		void Serialization.ICompactSerializable.Deserialize(Serialization.IO.CompactReader reader)
		{
			_exports.Clear();
			var count = reader.ReadInt32();
			for(var i = 0; i < count; i++)
			{
				var export = new Export();
				((Serialization.ICompactSerializable)export).Deserialize(reader);
				_exports.Add(export);
			}
		}

		/// <summary>
		/// Serializa os dados.
		/// </summary>
		/// <param name="writer"></param>
		void Serialization.ICompactSerializable.Serialize(Serialization.IO.CompactWriter writer)
		{
			writer.Write(Count);
			foreach (var item in _exports)
			{
				if(!(item is Serialization.ICompactSerializable))
					((Serialization.ICompactSerializable)new Export(item)).Serialize(writer);
				else
					((Serialization.ICompactSerializable)item).Serialize(writer);
			}
		}

		/// <summary>
		/// Recupera os dados que serão usados pela serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		void System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("Count", _exports.Count);
			for(var i = 0; i < _exports.Count; i++)
				info.AddValue(i.ToString(), _exports[i] is Export ? _exports[i] : new Export(_exports[i]), typeof(Export));
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<IExport> GetEnumerator()
		{
			return _exports.GetEnumerator();
		}

		/// <summary>
		/// Recupera o enumerador dos itens.
		/// </summary>
		/// <returns></returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return _exports.GetEnumerator();
		}
	}
}
