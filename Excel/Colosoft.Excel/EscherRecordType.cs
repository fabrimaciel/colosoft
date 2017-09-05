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

namespace Colosoft.Excel
{
	public class EscherRecordType
	{
		public const UInt16 MsofbtDggContainer = 0xF000;

		public const UInt16 MsofbtBstoreContainer = 0xF001;

		public const UInt16 MsofbtDgContainer = 0xF002;

		public const UInt16 MsofbtSpgrContainer = 0xF003;

		public const UInt16 MsofbtSpContainer = 0xF004;

		public const UInt16 MsofbtSolverContainer = 0xF005;

		public const UInt16 MsofbtDgg = 0xF006;

		public const UInt16 MsofbtBSE = 0xF007;

		public const UInt16 MsofbtDg = 0xF008;

		public const UInt16 MsofbtSpgr = 0xF009;

		public const UInt16 MsofbtSp = 0xF00A;

		public const UInt16 MsofbtOPT = 0xF00B;

		public const UInt16 MsofbtTextbox = 0xF00C;

		public const UInt16 MsofbtClientTextbox = 0xF00D;

		public const UInt16 MsofbtAnchor = 0xF00E;

		public const UInt16 MsofbtChildAnchor = 0xF00F;

		public const UInt16 MsofbtClientAnchor = 0xF010;

		public const UInt16 MsofbtClientData = 0xF011;

		public const UInt16 MsofbtConnectorRule = 0xF012;

		public const UInt16 MsofbtAlignRule = 0xF013;

		public const UInt16 MsofbtArcRule = 0xF014;

		public const UInt16 MsofbtClientRule = 0xF015;

		public const UInt16 MsofbtCLSID = 0xF016;

		public const UInt16 MsofbtCalloutRule = 0xF017;

		public const UInt16 MsofbtBlipStart = 0xF018;

		public const UInt16 MsofbtBlipBitmapPS = 0xF01A;

		public const UInt16 MsofbtBlipBitmapJPEG = 0xF01D;

		public const UInt16 MsofbtBlipBitmapPNG = 0xF01E;

		public const UInt16 MsofbtBlipBitmapDIB = 0xF01F;

		public const UInt16 MsofbtBlipEnd = 0xF117;

		public const UInt16 MsofbtRegroupItems = 0xF118;

		public const UInt16 MsofbtSelection = 0xF119;

		public const UInt16 MsofbtColorMRU = 0xF11A;

		public const UInt16 MsofbtDeletedPspl = 0xF11D;

		public const UInt16 MsofbtSplitMenuColors = 0xF11E;

		public const UInt16 MsofbtOleObject = 0xF11F;

		public const UInt16 MsofbtColorScheme = 0xF120;
	}
}
