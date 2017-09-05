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

namespace Colosoft.Security.Remote.Client.ProfileProviderServiceReference
{
    static class Helpers
    {
        public static DateTimeOffset ReadDateTimeOffset(this System.Xml.XmlReader reader)
        {
            reader.ReadStartElement();

            reader.ReadStartElement("DateTime");
            var dateTime = reader.ReadContentAsDateTime();
            reader.ReadEndElement();

            reader.ReadStartElement("OffsetMinutes");
            var offsetMinutes = (short)reader.ReadContentAsInt();
            reader.ReadEndElement();

            reader.ReadEndElement();

            return new DateTimeOffset(dateTime, TimeSpan.FromMinutes(offsetMinutes));
        }

        public static void WriteDateTimeOffset(this System.Xml.XmlWriter writer, DateTimeOffset date, string rootName)
        {
            writer.WriteStartElement(rootName);

            writer.WriteStartElement("DateTime");
            writer.WriteValue(date.DateTime);
            writer.WriteEndElement();

            writer.WriteStartElement("OffsetMinutes");
            writer.WriteValue(date.Offset.TotalMinutes);
            writer.WriteEndElement();

            writer.WriteEndElement();

        }
    }
}
