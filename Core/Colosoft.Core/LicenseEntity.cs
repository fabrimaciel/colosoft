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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Colosoft.Licensing
{
	/// <summary>
	/// Possíveis situações da licença.
	/// </summary>
	public enum LicenseStatus
	{
		/// <summary>
		/// Idenfinida.
		/// </summary>
		UNDEFINED = 0,
		/// <summary>
		/// Válida.
		/// </summary>
		VALID = 1,
		/// <summary>
		/// Inválida.
		/// </summary>
		INVALID = 2,
		/// <summary>
		/// Crackeada.
		/// </summary>
		CRACKED = 4
	}
	/// <summary>
	/// Possíveis tipos de licença.
	/// </summary>
	public enum LicenseTypes
	{
		/// <summary>
		/// Desconhecido.
		/// </summary>
		[Description("Unknown")]
		Unknown = 0,
		/// <summary>
		/// Única.
		/// </summary>
		[Description("Single")]
		Single = 1,
		/// <summary>
		/// Por volume.
		/// </summary>
		[Description("Volume")]
		Volume = 2
	}
	/// <summary>
	/// Estrutura base para a entidade de licença.
	/// </summary>
	public abstract class LicenseEntity
	{
		/// <summary>
		/// Nome da aplicação.
		/// </summary>
		[Browsable(false)]
		[XmlIgnore]
		[ShowInLicenseInfo(false)]
		public string AppName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Identificador único.
		/// </summary>
		[Browsable(false)]
		[XmlElement("UID")]
		[ShowInLicenseInfo(false)]
		public string UID
		{
			get;
			set;
		}

		/// <summary>
		/// Tipo de licença.
		/// </summary>
		[Browsable(false)]
		[XmlElement("Type")]
		[ShowInLicenseInfo(true, "Type", ShowInLicenseInfoAttribute.FormatType.EnumDescription)]
		public LicenseTypes Type
		{
			get;
			set;
		}

		/// <summary>
		/// Data de criação.
		/// </summary>
		[Browsable(false)]
		[XmlElement("CreateDateTime")]
		[ShowInLicenseInfo(true, "Creation Time", ShowInLicenseInfoAttribute.FormatType.DateTime)]
		public DateTime CreateDateTime
		{
			get;
			set;
		}

		/// <summary>
		/// For child class to do extra validation for those extended properties
		/// </summary>
		/// <param name="validationMsg"></param>
		/// <returns></returns>
		public abstract LicenseStatus DoExtraValidation(out string validationMsg);
	}
}
