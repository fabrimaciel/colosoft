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
using System.Xml.Serialization;
using Colosoft.Serialization;

namespace Colosoft.Query
{
	/// <summary>
	/// Termo de uma query.
	/// </summary>
	[Serializable]
	public class QueryTerm : ConditionalTerm
	{
		private QueryInfo _queryInfo;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		public QueryTerm()
		{
			_queryInfo = new QueryInfo();
		}

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="queryInfo"></param>
		public QueryTerm(QueryInfo queryInfo)
		{
			_queryInfo = queryInfo;
		}

		/// <summary>
		/// Construtor usado para serialização.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected QueryTerm(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context)
		{
		}

		/// <summary>
		/// Informações da query.
		/// </summary>
		public QueryInfo QueryInfo
		{
			get
			{
				return _queryInfo;
			}
			set
			{
				_queryInfo = value;
			}
		}

		/// <summary>
		/// Serializa em xml.
		/// </summary>
		/// <param name="reader"></param>
		protected override void ReadXml(System.Xml.XmlReader reader)
		{
			((IXmlSerializable)_queryInfo).ReadXml(reader);
		}

		/// <summary>
		/// Deserializa em xml.
		/// </summary>
		/// <param name="writer"></param>
		protected override void WriteXml(System.Xml.XmlWriter writer)
		{
			((System.Xml.Serialization.IXmlSerializable)_queryInfo).WriteXml(writer);
		}

		/// <summary>
		/// Serializa com o compact serializer.
		/// </summary>
		/// <param name="writer"></param>
		protected override void Serialize(Colosoft.Serialization.IO.CompactWriter writer)
		{
			((ICompactSerializable)_queryInfo).Serialize(writer);
		}

		/// <summary>
		/// Deserializa com o compact serializer.
		/// </summary>
		/// <param name="reader"></param>
		protected override void Deserialize(Colosoft.Serialization.IO.CompactReader reader)
		{
			((ICompactSerializable)_queryInfo).Deserialize(reader);
		}

		/// <summary>
		/// Recupera os dados serializados da instancia.
		/// </summary>
		/// <param name="info"></param>
		/// <param name="context"></param>
		protected override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Clona o termo condicional.
		/// </summary>
		/// <returns></returns>
		public override object Clone()
		{
			return new QueryTerm(_queryInfo);
		}
	}
}
