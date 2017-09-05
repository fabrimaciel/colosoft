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
using System.Text;
using Colosoft.Configuration.Attributes;
using System.Reflection;

namespace Colosoft.Configuration.Targets
{
	/// <summary>
	/// Identifica o campo alvo do atributo do arquivo de configuração.
	/// </summary>
	internal class FieldTarget : ElementTarget
	{
		/// <summary>
		/// Propriedade relacionado ao alvo.
		/// </summary>
		public readonly FieldInfo RepresentFieldInfo;

		/// <summary>
		/// Construtor padrão.
		/// </summary>
		/// <param name="ca"></param>
		/// <param name="fi"></param>
		public FieldTarget(ConfigurationAttribute ca, FieldInfo fi) : base(ca)
		{
			this.RepresentFieldInfo = fi;
		}

		/// <summary>
		/// Configura o valor ao campo.
		/// </summary>
		/// <param name="target">Objeto que contem o campo.</param>
		/// <param name="node">Nó do arquivo XML contendo as informações.</param>
		public override void Configure(object target, System.Xml.XmlNode node)
		{
			object value = Colosoft.Reflection.TypeConverter.Get(RepresentFieldInfo.FieldType, node);
			RepresentFieldInfo.SetValue(target, value, Reflection.ReflectionFlags.InstanceCriteria, null, null);
		}
	}
}
