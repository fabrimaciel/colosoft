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

namespace Colosoft
{
	/// <summary>
	/// Assinatura da classe que faz acesso a área de transferencia do sistema.
	/// </summary>
	public interface IClipboard
	{
		/// <summary>
		/// Define uma imagem para a área de transferência.
		/// </summary>
		/// <param name="image"></param>
		void SetImage(Media.Drawing.IImage image);

		/// <summary>
		/// Armazena um texto Unicode na área de transferencia.
		/// </summary>
		/// <param name="text">Texto que será armazenado.</param>
		void SetText(string text);

		/// <summary>
		/// Armazena um texto na área de transferencia.
		/// </summary>
		/// <param name="text">Texto que será armazenado.</param>
		/// <param name="format">Formato do texto.</param>
		void SetText(string text, Text.TextDataFormat format);
	}
	/// <summary>
	/// Implementação fake da área de transferencia.
	/// </summary>
	class BaseClipboard : IClipboard
	{
		/// <summary>
		/// Define uma imagem para a área de transferência.
		/// </summary>
		/// <param name="image"></param>
		public void SetImage(Media.Drawing.IImage image)
		{
		}

		/// <summary>
		/// Armazena um texto Unicode na área de transferencia.
		/// </summary>
		/// <param name="text">Texto que será armazenado.</param>
		public void SetText(string text)
		{
		}

		/// <summary>
		/// Armazena um texto na área de transferencia.
		/// </summary>
		/// <param name="text">Texto que será armazenado.</param>
		/// <param name="format">Formato do texto.</param>
		public void SetText(string text, Text.TextDataFormat format)
		{
		}
	}
	/// <summary>
	/// Fornece acesso a área de transferencia do sistema.
	/// </summary>
	public static class Clipboard
	{
		private static IClipboard _default;

		private static IClipboard _instance;

		/// <summary>
		/// Instancia padrão.
		/// </summary>
		public static IClipboard Instance
		{
			get
			{
				return _instance ?? _default;
			}
			set
			{
				_instance = value;
			}
		}

		/// <summary>
		/// Construtor estático.
		/// </summary>
		static Clipboard()
		{
			_default = new BaseClipboard();
			_instance = _default;
		}
	}
}
