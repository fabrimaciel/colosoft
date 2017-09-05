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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Colosoft.Runtime
{
	/// <summary>
	/// Gerenciador de memória.
	/// </summary>
	public class MemoryManager
	{
		/// <summary>
		/// Hash dos provedores de objetos do gerenciador.
		/// </summary>
		private Hashtable _objectProviders = new Hashtable();

		/// <summary>
		/// Recupera o provedor associado com o tipo informado.
		/// </summary>
		/// <param name="objType">Tipo do objeto do provedor.</param>
		/// <returns></returns>
		public ObjectProvider GetProvider(Type objType)
		{
			if(_objectProviders.Contains(objType))
				return (ObjectProvider)_objectProviders[objType];
			return null;
		}

		/// <summary>
		/// Registra um provedor de objetos.
		/// </summary>
		/// <param name="proivder"></param>
		public void RegisterObjectProvider(ObjectProvider proivder)
		{
			if(!_objectProviders.Contains(proivder.ObjectType))
				_objectProviders.Add(proivder.ObjectType, proivder);
		}
	}
}
