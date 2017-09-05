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

namespace Colosoft.Query
{
	/// <summary>
	/// Assinatura de um registro.
	/// </summary>
	public interface IRecord
	{
		/// <summary>
		/// Instancia do resultado onde o registro está sendo recuperado.
		/// </summary>
		IQueryResult QueryResult
		{
			get;
			set;
		}

		/// <summary>
		/// Descritor do registro.
		/// </summary>
		Record.RecordDescriptor Descriptor
		{
			get;
		}

		/// <summary>
		/// Quantidade de campos no resultado.
		/// </summary>
		int FieldCount
		{
			get;
		}

		/// <summary>
		/// Recupera o valor da coluna na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		RecordValue this[int i]
		{
			get;
		}

		/// <summary>
		/// Recupera um valor pelo nome da coluna.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		RecordValue this[string name]
		{
			get;
		}

		/// <summary>
		/// Define os valores do registro.
		/// </summary>
		/// <param name="values"></param>
		void SetValues(object[] values);

		/// <summary>
		/// Recupera os valores da registro para o vetor informado.
		/// </summary>
		/// <param name="values"></param>
		/// <returns></returns>
		int GetValues(object[] values);

		/// <summary>
		/// Recupera o valor <see cref="Boolean"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		bool GetBoolean(int i);

		/// <summary>
		/// Recupera o valor <see cref="Boolean"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		bool GetBoolean(string name);

		/// <summary>
		/// Recupera o valor <see cref="Byte"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		byte GetByte(int i);

		/// <summary>
		/// Recupera o valor <see cref="Byte"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		byte GetByte(string name);

		/// <summary>
		/// Recupera os bytes do campo na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length);

		/// <summary>
		/// Recupera os bytes do campo com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fieldOffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		long GetBytes(string name, long fieldOffset, byte[] buffer, int bufferoffset, int length);

		/// <summary>
		/// Recupera o valor <see cref="char"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		char GetChar(int i);

		/// <summary>
		/// Recupera o valor <see cref="char"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		char GetChar(string name);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="i"></param>
		/// <param name="fieldoffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="fieldoffset"></param>
		/// <param name="buffer"></param>
		/// <param name="bufferoffset"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		long GetChars(string name, long fieldoffset, char[] buffer, int bufferoffset, int length);

		/// <summary>
		/// Recupera o nome do tipo de dados do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		string GetDataTypeName(int i);

		/// <summary>
		/// Recupera o nome do tipo de dados do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		string GetDataTypeName(string name);

		/// <summary>
		/// Recupera o valor <see cref="DateTime"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		DateTime GetDateTime(int i);

		/// <summary>
		/// Recupera o valor <see cref="DateTime"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		DateTime GetDateTime(string name);

		/// <summary>
		/// Recupera o valor <see cref="decimal"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		decimal GetDecimal(int i);

		/// <summary>
		/// Recupera o valor <see cref="decimal"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		decimal GetDecimal(string name);

		/// <summary>
		/// Recupera o valor <see cref="double"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		double GetDouble(int i);

		/// <summary>
		/// Recupera o valor <see cref="double"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		double GetDouble(string name);

		/// <summary>
		/// Recupera o tipo do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		Type GetFieldType(int i);

		/// <summary>
		/// Recupera o tipo do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		Type GetFieldType(string name);

		/// <summary>
		/// Recupera o valor <see cref="float"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		float GetFloat(int i);

		/// <summary>
		/// Recupera o valor <see cref="float"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		float GetFloat(string name);

		/// <summary>
		/// Recupera o valor <see cref="Guid"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		Guid GetGuid(int i);

		/// <summary>
		/// Recupera o valor <see cref="Guid"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		Guid GetGuid(string name);

		/// <summary>
		/// Recupera o valor <see cref="Int16"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		short GetInt16(int i);

		/// <summary>
		/// Recupera o valor <see cref="Int16"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		short GetInt16(string name);

		/// <summary>
		/// Recupera o valor <see cref="Int32"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		int GetInt32(int i);

		/// <summary>
		/// Recupera o valor <see cref="Int32"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		int GetInt32(string name);

		/// <summary>
		/// Recupera o valor <see cref="Int64"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo</param>
		/// <returns></returns>
		long GetInt64(int i);

		/// <summary>
		/// Recupera o valor <see cref="Int64"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		long GetInt64(string name);

		/// <summary>
		/// Recupera o nome do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		string GetName(int i);

		/// <summary>
		/// Recupera a posição ordinal do campos com o nome informado.
		/// </summary>
		/// <param name="name">Nome do campo.</param>
		/// <returns></returns>
		int GetOrdinal(string name);

		/// <summary>
		/// Recupera o valor <see cref="String"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		string GetString(int i);

		/// <summary>
		/// Recupera o valor <see cref="String"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		string GetString(string name);

		/// <summary>
		/// Recupera o valor <see cref="DateTimeOffset"/> do campo na posição informada.
		/// </summary>
		/// <param name="i">Posição do campo.</param>
		/// <returns></returns>
		DateTimeOffset GetDateTimeOffset(int i);

		/// <summary>
		/// Recupera o valor <see cref="DateTimeOffset"/> do campo com o nome informado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		DateTimeOffset GetDateTimeOffset(string name);

		/// <summary>
		/// Recupera o valor do campo na posição informada.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		object GetValue(int i);

		/// <summary>
		/// Recupera o valor da campo com o nome innformado.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object GetValue(string name);

		/// <summary>
		/// Verifica se o campo na posição informada possui um valor nulo.
		/// </summary>
		/// <param name="i"></param>
		/// <returns></returns>
		bool IsDBNull(int i);

		/// <summary>
		/// Verifica se o campo com o onme informado possui um valor nulo.
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		bool IsDBNull(string name);
	}
}
