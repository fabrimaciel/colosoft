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

using System.Drawing;

namespace Colosoft.Excel.ExcelXml
{
	/// <summary>
	/// Style font options
	/// </summary>
	public interface IFontOptions
	{
		/// <summary>
		/// Name of font
		/// </summary>
		string Name
		{
			get;
			set;
		}

		/// <summary>
		/// Size of font
		/// </summary>
		int Size
		{
			get;
			set;
		}

		/// <summary>
		/// Bold?
		/// </summary>
		bool Bold
		{
			get;
			set;
		}

		/// <summary>
		/// Underline?
		/// </summary>
		bool Underline
		{
			get;
			set;
		}

		/// <summary>
		/// Italic?
		/// </summary>
		bool Italic
		{
			get;
			set;
		}

		/// <summary>
		/// Strikeout?
		/// </summary>
		bool Strikeout
		{
			get;
			set;
		}

		/// <summary>
		/// Font foreground color
		/// </summary>
		Color Color
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Cell interior options
	/// </summary>
	public interface IInteriorOptions
	{
		/// <summary>
		/// Cell fill color
		/// </summary>
		Color Color
		{
			get;
			set;
		}

		/// <summary>
		/// Pattern color
		/// </summary>
		Color PatternColor
		{
			get;
			set;
		}

		/// <summary>
		/// Pattern style
		/// </summary>
		Pattern Pattern
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Cell alignment options
	/// </summary>
	public interface IAlignmentOptions
	{
		/// <summary>
		/// Vertical alignment settings
		/// </summary>
		VerticalAlignment Vertical
		{
			get;
			set;
		}

		/// <summary>
		/// Horizontal alignment settings
		/// </summary>
		HorizontalAlignment Horizontal
		{
			get;
			set;
		}

		/// <summary>
		/// Cell indent
		/// </summary>
		int Indent
		{
			get;
			set;
		}

		/// <summary>
		/// Rotation angle
		/// </summary>
		int Rotate
		{
			get;
			set;
		}

		/// <summary>
		/// Wrap enabled?
		/// </summary>
		bool WrapText
		{
			get;
			set;
		}

		/// <summary>
		/// Shrink the text to fit cell?
		/// </summary>
		bool ShrinkToFit
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Cell border options
	/// </summary>
	public interface IBorderOptions
	{
		/// <summary>
		/// Border sides
		/// </summary>
		BorderSides Sides
		{
			get;
			set;
		}

		/// <summary>
		/// Width of border
		/// </summary>
		int Weight
		{
			get;
			set;
		}

		/// <summary>
		/// Border line style
		/// </summary>
		Borderline LineStyle
		{
			get;
			set;
		}

		/// <summary>
		/// Border color
		/// </summary>
		Color Color
		{
			get;
			set;
		}
	}
	/// <summary>
	/// Style settings container
	/// </summary>
	public interface IStyle
	{
		/// <summary>
		/// Font options
		/// </summary>
		IFontOptions Font
		{
			get;
			set;
		}

		/// <summary>
		/// Alignment options
		/// </summary>
		IAlignmentOptions Alignment
		{
			get;
			set;
		}

		/// <summary>
		/// Cell fill options
		/// </summary>
		IInteriorOptions Interior
		{
			get;
			set;
		}

		/// <summary>
		/// Cell border options
		/// </summary>
		IBorderOptions Border
		{
			get;
			set;
		}

		/// <summary>
		/// Cell display format
		/// </summary>
		DisplayFormatType DisplayFormat
		{
			get;
			set;
		}

		/// <summary>
		/// Custom display format string
		/// </summary>
		string CustomFormatString
		{
			get;
			set;
		}
	}
}
