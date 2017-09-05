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

namespace Colosoft.Globalization
{
	/// <summary>
	/// Assinatura de um provedor de tradução.
	/// </summary>
	public interface ITranslateProvider
	{
		/// <summary>
		/// Recupera as traduções.
		/// </summary>
		/// <returns></returns>
		IEnumerable<TranslateInfo> GetTranslates();
	}
	/// <summary>
	/// Assinatura de um provedor de traduções multiplas.
	/// </summary>
	public interface IMultipleTranslateProvider : ITranslateProvider
	{
		/// <summary>
		/// Recupera as traduções.
		/// </summary>
		/// <param name="groupKey">Chave do grupo de tradução.</param>
		/// <returns></returns>
		IEnumerable<TranslateInfo> GetTranslates(object groupKey);
	}
	/// <summary>
	/// Assinatura da estrutura que armazena as informação da tradução.
	/// </summary>
	public class TranslateInfo
	{
		/// <summary>
		/// Valor que presenta a chave.
		/// </summary>
		public int Value
		{
			get
			{
				if(Key != null)
				{
					var keyType = Key.GetType();
					if(keyType.IsEnum)
						keyType = Enum.GetUnderlyingType(keyType);
					return (keyType == typeof(int) ? (int)Key : keyType == typeof(short) ? (short)Key : keyType == typeof(long) ? (int)(long)Key : keyType == typeof(uint) ? (int)(uint)Key : keyType == typeof(ulong) ? (int)(ulong)Key : keyType == typeof(ushort) ? (ushort)Key : keyType == typeof(byte) ? (byte)Key : 0);
				}
				return 0;
			}
		}

		/// <summary>
		/// Chave associada com a tradução.
		/// </summary>
		public object Key
		{
			get;
			set;
		}

		/// <summary>
		/// Texto da tradução.
		/// </summary>
		public IMessageFormattable Text
		{
			get;
			set;
		}

		/// <summary>
		/// Tradução do texto.
		/// </summary>
		public string Translation
		{
			get
			{
				return Text.Format();
			}
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="text"></param>
		public TranslateInfo(object key, IMessageFormattable text)
		{
			this.Key = key;
			this.Text = text;
		}
	}
}
