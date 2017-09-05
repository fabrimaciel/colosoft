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
using System.Collections;
using Lucene.Net.Analysis;

namespace Colosoft.SearchEngine.Lucene
{
	/// <summary>
	/// Armazena os palavra de fins usadas pelo sistema.
	/// </summary>
	public static class Stopwords
	{
		/// <summary>
		/// Stopwords da ligua portuguesa.
		/// </summary>
		public static readonly string[] PORTUGUESE;

		/// <summary>
		/// Conjunto de termos em portugues.
		/// </summary>
		public static readonly Hashtable PORTUGUESE_SET;

		static Stopwords()
		{
			PORTUGUESE = new string[] {
				"a",
				"ainda",
				"alem",
				"ambas",
				"ambos",
				"antes",
				"ao",
				"aonde",
				"aos",
				"apos",
				"aquele",
				"aqueles",
				"as",
				"assim",
				"com",
				"como",
				"contra",
				"contudo",
				"cuja",
				"cujas",
				"cujo",
				"cujos",
				"da",
				"das",
				"de",
				"dela",
				"dele",
				"deles",
				"demais",
				"depois",
				"desde",
				"desta",
				"deste",
				"dispoe",
				"dispoem",
				"diversa",
				"diversas",
				"diversos",
				"do",
				"dos",
				"durante",
				"e",
				"ela",
				"elas",
				"ele",
				"eles",
				"em",
				"entao",
				"entre",
				"essa",
				"essas",
				"esse",
				"esses",
				"esta",
				"estas",
				"este",
				"estes",
				"ha",
				"isso",
				"isto",
				"logo",
				"mais",
				"mas",
				"mediante",
				"menos",
				"mesma",
				"mesmas",
				"mesmo",
				"mesmos",
				"na",
				"nas",
				"nao",
				"nas",
				"nem",
				"nesse",
				"neste",
				"nos",
				"o",
				"os",
				"ou",
				"outra",
				"outras",
				"outro",
				"outros",
				"pelas",
				"pelas",
				"pelo",
				"pelos",
				"perante",
				"pois",
				"por",
				"porque",
				"portanto",
				"proprio",
				"propios",
				"quais",
				"qual",
				"qualquer",
				"quando",
				"quanto",
				"que",
				"quem",
				"quer",
				"se",
				"seja",
				"sem",
				"sendo",
				"seu",
				"seus",
				"sob",
				"sobre",
				"sua",
				"suas",
				"tal",
				"tambem",
				"teu",
				"teus",
				"toda",
				"todas",
				"todo",
				"todos",
				"tua",
				"tuas",
				"tudo",
				"um",
				"uma",
				"umas",
				"uns"
			};
			var stopSet = new CharArraySet(PORTUGUESE.Length, false);
			stopSet.AddAll(new System.Collections.ArrayList(PORTUGUESE));
			PORTUGUESE_SET = CharArraySet.UnmodifiableSet(stopSet);
		}
	}
}
