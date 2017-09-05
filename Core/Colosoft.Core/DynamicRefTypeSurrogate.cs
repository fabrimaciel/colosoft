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
using Colosoft.Serialization.IO;

namespace Colosoft.Serialization.Surrogates
{
	/// <summary>
	/// Implementação do subtituto de serialização para a referencia de um tipo.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class DynamicRefTypeSurrogate<T> : SerializationSurrogate where T : class, new()
	{
		/// <summary>
		/// Instancia do delegate padrão para o construtor.
		/// </summary>
		protected DefaultConstructorDelegate _newMethod;

		/// <summary>
		/// Delegate do método usado para ler os dados.
		/// </summary>
		protected ReadObjectDelegate _readMethod;

		/// <summary>
		/// Delegate do método usado para escreve os dados.
		/// </summary>
		protected WriteObjectDelegate _writeMethod;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public DynamicRefTypeSurrogate() : base(typeof(T))
		{
			this.CommonConstruct();
			_readMethod = DynamicSurrogateBuilder.CreateReaderDelegate(typeof(T));
			_writeMethod = DynamicSurrogateBuilder.CreateWriterDelegate(typeof(T));
		}

		/// <summary>
		/// Cria uma instancia com o delegate dos métodos para leitura e escrita dos dados.
		/// </summary>
		/// <param name="read">Método usado para fazer a leitura dos dados.</param>
		/// <param name="write">Método usado para escreve os dados.</param>
		public DynamicRefTypeSurrogate(ReadObjectDelegate read, WriteObjectDelegate write) : base(typeof(T))
		{
			this.CommonConstruct();
			_readMethod = read;
			_writeMethod = write;
		}

		/// <summary>
		/// Cria o construtor comum.
		/// </summary>
		private void CommonConstruct()
		{
			if(!typeof(T).IsPublic)
				this._newMethod = SurrogateHelper.CreateDefaultConstructorDelegate(typeof(T));
		}

		/// <summary>
		/// Cria um nova instancia do tipo.
		/// </summary>
		/// <returns></returns>
		public override object CreateInstance()
		{
			if(typeof(T).IsPublic)
				return Activator.CreateInstance<T>();
			return _newMethod();
		}

		/// <summary>
		/// Recupera uma instancia com base nos dados do leitor informado.
		/// </summary>
		/// <param name="reader"></param>
		/// <returns></returns>
		public override object Read(CompactBinaryReader reader)
		{
			if(typeof(T).IsPublic)
			{
				return this._readMethod(reader, Activator.CreateInstance<T>());
			}
			return this._readMethod(reader, this._newMethod());
		}

		/// <summary>
		/// Salva os dados do tipo.
		/// </summary>
		/// <param name="reader"></param>
		public override void Skip(CompactBinaryReader reader)
		{
			if(typeof(T).IsPublic)
				this._readMethod(reader, Activator.CreateInstance<T>());
			else
				this._readMethod(reader, this._newMethod());
		}

		/// <summary>
		/// Escreve os dados da instancia informado no escritor.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="graph"></param>
		public override void Write(CompactBinaryWriter writer, object graph)
		{
			if(base.SubTypeHandle > 0)
				writer.Write(base.SubTypeHandle);
			_writeMethod(writer, graph);
		}
	}
}
