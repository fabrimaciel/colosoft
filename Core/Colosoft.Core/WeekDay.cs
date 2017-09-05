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
	/// Representa o provedor de tradução do dias da semana.
	/// </summary>
	class WeekDayTranslateProvider : Globalization.ITranslateProvider
	{
		/// <summary>
		/// Recupera as traduções.
		/// </summary>
		/// <returns></returns>
		public IEnumerable<Globalization.TranslateInfo> GetTranslates()
		{
			yield return new Globalization.TranslateInfo(WeekDay.Sunday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Sunday));
			yield return new Globalization.TranslateInfo(WeekDay.Monday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Monday));
			yield return new Globalization.TranslateInfo(WeekDay.Tuesday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Tuesday));
			yield return new Globalization.TranslateInfo(WeekDay.Wednesday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Wednesday));
			yield return new Globalization.TranslateInfo(WeekDay.Thursday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Thursday));
			yield return new Globalization.TranslateInfo(WeekDay.Friday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Friday));
			yield return new Globalization.TranslateInfo(WeekDay.Saturday, ResourceMessageFormatter.Create(() => Properties.Resources.WeekDay_Saturday));
		}
	}
	/// <summary>
	/// Enumerador de dias da semana.
	/// </summary>
	[Flags]
	[Translate(typeof(WeekDayTranslateProvider))]
	public enum WeekDay : byte
	{
		/// <summary>
		/// Domingo.
		/// </summary>
		Sunday = 1,
		/// <summary>
		/// Segunda.
		/// </summary>
		Monday = 2,
		/// <summary>
		/// Terça.
		/// </summary>
		Tuesday = 4,
		/// <summary>
		/// Quarta.
		/// </summary>
		Wednesday = 8,
		/// <summary>
		/// Quinta.
		/// </summary>
		Thursday = 16,
		/// <summary>
		/// Sexta.
		/// </summary>
		Friday = 32,
		/// <summary>
		/// Sábado.
		/// </summary>
		Saturday = 64,
		/// <summary>
		/// Todos os dias.
		/// </summary>
		AllDays = Sunday | Monday | Tuesday | Wednesday | Thursday | Friday | Saturday
	}
}
