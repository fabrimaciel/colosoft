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
using Colosoft.Configuration;

namespace Colosoft
{
	/// <summary>
	/// Classe com métodos para auxiliar na leitura da configuração do sistema.
	/// </summary>
	public static class SystemConfigurationExtensions
	{
		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <returns></returns>
		public static int Get(this ISystemConfiguration configuration, string path, int defaultValue = 0)
		{
			var result = 0;
			if(int.TryParse(Configuration.Configuration.Instance.ReadPath(path), out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <returns></returns>
		public static short Get(this ISystemConfiguration configuration, string path, short defaultValue = 0)
		{
			short result = 0;
			if(short.TryParse(Configuration.Configuration.Instance.ReadPath(path), out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <returns></returns>
		public static long Get(this ISystemConfiguration configuration, string path, long defaultValue = 0)
		{
			long result = 0;
			if(long.TryParse(Configuration.Configuration.Instance.ReadPath(path), out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <returns></returns>
		public static bool Get(this ISystemConfiguration configuration, string path, bool defaultValue = false)
		{
			bool result = false;
			if(bool.TryParse(Configuration.Configuration.Instance.ReadPath(path), out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <param name="provider">Provedor de formatação.</param>
		/// <param name="style">Estilo da tada.</param>
		/// <returns></returns>
		public static DateTime Get(this ISystemConfiguration configuration, string path, DateTime defaultValue, IFormatProvider provider = null, System.Globalization.DateTimeStyles style = System.Globalization.DateTimeStyles.None)
		{
			DateTime result = DateTime.Now;
			if(DateTime.TryParse(Configuration.Configuration.Instance.ReadPath(path), provider, style, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="provider">Provedor de formatação.</param>
		/// <param name="style">Estilo da tada.</param>
		/// <returns></returns>
		public static DateTime Get(this ISystemConfiguration configuration, string path, IFormatProvider provider = null, System.Globalization.DateTimeStyles style = System.Globalization.DateTimeStyles.None)
		{
			DateTime result = DateTime.Now;
			if(DateTime.TryParse(Configuration.Configuration.Instance.ReadPath(path), provider, style, out result))
				return result;
			return DateTime.Now;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <param name="style">Estilo numérico do valor.</param>
		/// <param name="provider">Cultura que será usada na conversão.</param>
		/// <returns></returns>
		public static double Get(this ISystemConfiguration configuration, string path, double defaultValue = 0, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Any, IFormatProvider provider = null)
		{
			double result = 0;
			if(double.TryParse(Configuration.Configuration.Instance.ReadPath(path), style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <param name="style">Estilo numérico do valor.</param>
		/// <param name="provider">Cultura que será usada na conversão.</param>
		/// <returns></returns>
		public static float Get(this ISystemConfiguration configuration, string path, float defaultValue = 0, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Any, IFormatProvider provider = null)
		{
			float result = 0;
			if(float.TryParse(Configuration.Configuration.Instance.ReadPath(path), style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <param name="defaultValue">Valor padrão caso não tenho recupera o valor.</param>
		/// <param name="style">Estilo numérico do valor.</param>
		/// <param name="provider">Cultura que será usada na conversão.</param>
		/// <returns></returns>
		public static decimal Get(this ISystemConfiguration configuration, string path, decimal defaultValue = 0, System.Globalization.NumberStyles style = System.Globalization.NumberStyles.Any, IFormatProvider provider = null)
		{
			decimal result = 0;
			if(decimal.TryParse(Configuration.Configuration.Instance.ReadPath(path), style, provider, out result))
				return result;
			return defaultValue;
		}

		/// <summary>
		/// Recupera o valor da configuração no caminho informado.
		/// </summary>
		/// <param name="configuration">Instancia da configuração do sistema.</param>
		/// <param name="path">Caminho com os dados da configuração.</param>
		/// <returns></returns>
		public static string Get(this ISystemConfiguration configuration, string path)
		{
			return Configuration.Configuration.Instance.ReadPath(path);
		}
	}
}
