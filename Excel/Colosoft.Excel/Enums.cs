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
using System.Diagnostics.CodeAnalysis;

namespace Colosoft.Excel.ExcelXml
{
	internal enum ParseArgumentType
	{
		None,
		Function,
		Range,
		AbsoluteRange
	}
	/// <summary>
	/// Formula parameter types
	/// </summary>
	public enum ParameterType
	{
		/// <summary>
		/// A unknown string parameter
		/// </summary>
		String,
		/// <summary>
		/// A Range
		/// </summary>
		Range,
		/// <summary>
		/// A Formula
		/// </summary>
		Formula
	}
	/// <summary>
	/// The default display format of the cell in excel
	/// </summary>
	public enum DisplayFormatType
	{
		/// <summary>
		/// General format
		/// </summary>
		None,
		/// <summary>
		/// Displays anything as text (i.e. Left aligned without formatting)
		/// </summary>
		Text,
		/// <summary>
		/// Displays numeric values with two fixed decimals
		/// </summary>
		Fixed,
		/// <summary>
		/// Displays numeric values with two fixed decimals and digit grouping
		/// </summary>
		Standard,
		/// <summary>
		/// Displays numeric values as percentage values
		/// </summary>
		Percent,
		/// <summary>
		/// Displays numeric values in scientific notation
		/// </summary>
		Scientific,
		/// <summary>
		/// Displays numeric or date values as formatted date values
		/// </summary>
		GeneralDate,
		/// <summary>
		/// Displays numeric or date values as short date format
		/// </summary>
		ShortDate,
		/// <summary>
		/// Displays numeric or date values as long date format
		/// </summary>
		LongDate,
		/// <summary>
		/// Displays numeric or date values in time format
		/// </summary>
		Time,
		/// <summary>
		/// Custom defined format
		/// </summary>
		Custom
	}
	/// <summary>
	/// The cell content type
	/// </summary>
	public enum ContentType
	{
		/// <summary>
		/// Cell does not contain anything
		/// </summary>
		None,
		/// <summary>
		/// Cell contains a string
		/// </summary>
		String,
		/// <summary>
		/// Cell contains a number
		/// </summary>
		Number,
		/// <summary>
		/// Cell contains a DateTime value
		/// </summary>
		DateTime,
		/// <summary>
		/// Cell contains a bool value
		/// </summary>
		Boolean,
		/// <summary>
		/// Cell contains a formula
		/// </summary>
		Formula,
		/// <summary>
		/// Cell contains a formula which cannot be resolved
		/// </summary>
		UnresolvedValue
	}
	/// <summary>
	/// Cell's vertical alignment values
	/// </summary>
	public enum VerticalAlignment
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Top aligned
		/// </summary>
		Top,
		/// <summary>
		/// Centered
		/// </summary>
		Center,
		/// <summary>
		/// Bottom aligned
		/// </summary>
		Bottom,
		/// <summary>
		/// Justified
		/// </summary>
		Justify,
		/// <summary>
		/// Distributed
		/// </summary>
		Distributed
	}
	/// <summary>
	/// Cell's horizontal alignment values
	/// </summary>
	public enum HorizontalAlignment
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Left aligned
		/// </summary>
		Left,
		/// <summary>
		/// Centered
		/// </summary>
		Center,
		/// <summary>
		/// Right aligned
		/// </summary>
		Right,
		/// <summary>
		/// Streched to fill
		/// </summary>
		Fill,
		/// <summary>
		/// Justified
		/// </summary>
		Justify,
		/// <summary>
		/// Distributed
		/// </summary>
		Distributed
	}
	/// <summary>
	/// Different style of border lines
	/// </summary>
	public enum Borderline
	{
		/// <summary>
		/// Continuous line
		/// </summary>
		Continuous,
		/// <summary>
		/// Dash line
		/// </summary>
		Dash,
		/// <summary>
		/// DashDot line
		/// </summary>
		DashDot,
		/// <summary>
		/// DashDotDot line
		/// </summary>
		DashDotDot,
		/// <summary>
		/// Double line
		/// </summary>
		Double,
		/// <summary>
		/// Dot line
		/// </summary>
		Dot,
		/// <summary>
		/// SlantDashDot line
		/// </summary>
		SlantDashDot
	}
	/// <summary>
	/// Different type of border sides.
	/// </summary>
	/// <remarks>Multiple values can be combined by an or (i.e. "|") operation.</remarks>
	[Flags]
	public enum BorderSides
	{
		/// <summary>
		/// No border
		/// </summary>
		None = 0,
		/// <summary>
		/// Cell has a top border
		/// </summary>
		Top = 1,
		/// <summary>
		/// Cell has a left border
		/// </summary>
		Left = 2,
		/// <summary>
		/// Cell has a botom border
		/// </summary>
		Bottom = 4,
		/// <summary>
		/// Cell has a right border
		/// </summary>
		Right = 8,
		/// <summary>
		/// Cell has full border on all sides
		/// </summary>
		All = 15
	}
	/// <summary>
	/// Different types of cell background patterns
	/// </summary>
	public enum Pattern
	{
		/// <summary>
		/// Solid
		/// </summary>
		Solid,
		/// <summary>
		/// Gray25
		/// </summary>
		Gray25,
		/// <summary>
		/// Gray50
		/// </summary>
		Gray50,
		/// <summary>
		/// Gray75
		/// </summary>
		Gray75,
		/// <summary>
		/// Gray125
		/// </summary>
		Gray125,
		/// <summary>
		/// Gray0625
		/// </summary>
		Gray0625,
		/// <summary>
		/// HorzStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Horz")]
		HorzStripe,
		/// <summary>
		/// VertStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vert")]
		VertStripe,
		/// <summary>
		/// ReverseDiagStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		ReverseDiagStripe,
		/// <summary>
		/// DiagStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		DiagStripe,
		/// <summary>
		/// DiagCross
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		DiagCross,
		/// <summary>
		/// ThickDiagCross
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		ThickDiagCross,
		/// <summary>
		/// ThinHorzStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Horz")]
		ThinHorzStripe,
		/// <summary>
		/// ThinVertStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vert")]
		ThinVertStripe,
		/// <summary>
		/// ThinReverseDiagStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		ThinReverseDiagStripe,
		/// <summary>
		/// ThinDiagStripe
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		ThinDiagStripe,
		/// <summary>
		/// ThinHorzCross
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Horz")]
		ThinHorzCross,
		/// <summary>
		/// ThinDiagCross
		/// </summary>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Diag")]
		ThinDiagCross
	}
	/// <summary>
	/// Page layout
	/// </summary>
	public enum PageLayout
	{
		/// <summary>
		/// None
		/// </summary>
		None,
		/// <summary>
		/// Centers the page horizontally
		/// </summary>
		CenterHorizontal,
		/// <summary>
		/// Centers the page vertically
		/// </summary>
		CenterVertical,
		/// <summary>
		/// Centers the page vertically and horizontally
		/// </summary>
		CenterVerticalAndHorizontal
	}
	/// <summary>
	/// Orientation mode
	/// </summary>
	public enum PageOrientation
	{
		/// <summary>
		/// None.
		/// </summary>
		None,
		/// <summary>
		/// Landscape orientation
		/// </summary>
		Landscape,
		/// <summary>
		/// Portrait orientation
		/// </summary>
		Portrait
	}
}
