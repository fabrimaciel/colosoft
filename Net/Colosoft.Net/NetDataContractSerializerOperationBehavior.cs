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
using System.Runtime.Serialization;
using System.Xml;

namespace Colosoft.Net.Serialization
{
	/// <summary>
	/// Classe responsável pela serialização dos dados das operações do WebService.
	/// </summary>
	public class NetDataContractSerializerOperationBehavior : System.ServiceModel.Description.DataContractSerializerOperationBehavior
	{
		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="operationDescription"></param>
		public NetDataContractSerializerOperationBehavior(System.ServiceModel.Description.OperationDescription operationDescription) : base(operationDescription)
		{
		}

		/// <summary>
		/// Cria uma instancia do serializador para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="ns"></param>
		/// <param name="knownTypes"></param>
		/// <returns></returns>
		public override XmlObjectSerializer CreateSerializer(Type type, string name, string ns, IList<Type> knownTypes)
		{
			return base.CreateSerializer(type, name, ns, knownTypes);
		}

		/// <summary>
		/// Cria uma instancia para o serializador para o tipo informado.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="name"></param>
		/// <param name="ns"></param>
		/// <param name="knownTypes"></param>
		/// <returns></returns>
		public override XmlObjectSerializer CreateSerializer(Type type, XmlDictionaryString name, XmlDictionaryString ns, IList<Type> knownTypes)
		{
			return base.CreateSerializer(type, name, ns, knownTypes);
		}
	}
}
