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

using System.Text.RegularExpressions;
using Colosoft.Excel.ExcelXml.Extensions;
using System.Globalization;

namespace Colosoft.Excel.ExcelXml
{
	internal static class FormulaParser
	{
		static Regex RangeRegex = new Regex(@"^((\[(?<File>[\w\.]+)\])?('?(?<Sheet>.+)'?!))?R(?<RowStart>\d+)?C(?<ColStart>\d+)?(:R(?<RowEnd>\d+)?C(?<ColEnd>\d+)?)?$");

		static Regex AbsoluteRangeRegex = new Regex(@"^((\[(?<File>[\w\.]+)\])?('?(?<Sheet>.+)'?!))?R(\[(?<RowStart>[\-0-9]+)\])?C(\[(?<ColStart>[\-0-9]+)\])?(:R(\[(?<RowEnd>[\-0-9]+)\])?C(\[(?<ColEnd>[\-0-9]+)\])?)?$");

		static Regex FunctionRegex = new Regex(@"^(?<FunctionName>[\w\+\-]+)\((?<Parameters>.*)\)$");

		static Regex PrintRowRegex = new Regex(@"^('?(?<Sheet>.+)'?!)?R(?<RowStart>\d+)(:R(?<RowEnd>\d+))?$");

		static Regex PrintColumnsRegex = new Regex(@"^('?(?<Sheet>.+)'?!)?C(?<ColStart>\d+)(:C(?<ColEnd>\d+))?$");

		private static Cell GetCell(Worksheet ws, Cell cell, bool absolute, int row, int col)
		{
			if(absolute)
			{
				col = cell.CellIndex + col;
				row = cell.ParentRow.RowIndex + row;
			}
			return ws[col, row];
		}

		private static void ParseFormula(Cell cell, Formula formula, string formulaText)
		{
			Match match;
			ParseArgumentType pat = GetArgumentType(formulaText, out match);
			switch(pat)
			{
			case ParseArgumentType.Function:
			{
				string function = match.Groups["FunctionName"].Value;
				Formula subFormula = new Formula();
				subFormula.Add(function).StartGroup();
				string[] parameters = match.Groups["Parameters"].Value.Split(new[] {
					','
				});
				foreach (string parameter in parameters)
					ParseFormula(cell, subFormula, parameter);
				subFormula.EndGroup();
				formula.Add(subFormula);
				break;
			}
			case ParseArgumentType.Range:
			case ParseArgumentType.AbsoluteRange:
			{
				Range range = new Range(formulaText);
				formula.Add(range);
				break;
			}
			case ParseArgumentType.None:
			{
				formula.Add(formulaText);
				break;
			}
			}
		}

		internal static ParseArgumentType GetArgumentType(string argument, out Match match)
		{
			Match matchFunction = FunctionRegex.Match(argument);
			if(matchFunction.Success)
			{
				match = matchFunction;
				return ParseArgumentType.Function;
			}
			Match matchRange = RangeRegex.Match(argument);
			if(matchRange.Success)
			{
				match = matchRange;
				return ParseArgumentType.Range;
			}
			Match matchAbsoluteRange = AbsoluteRangeRegex.Match(argument);
			if(matchAbsoluteRange.Success)
			{
				match = matchAbsoluteRange;
				return ParseArgumentType.AbsoluteRange;
			}
			match = null;
			return ParseArgumentType.None;
		}

		internal static void Parse(Cell cell, string formulaText)
		{
			if(formulaText[0] != '=')
			{
				cell.Value = formulaText;
				cell.Content = ContentType.UnresolvedValue;
			}
			formulaText = formulaText.Substring(1);
			Formula formula = new Formula();
			ParseFormula(cell, formula, formulaText);
			if(formula.parameters[0].ParameterType == ParameterType.Formula)
			{
				cell.Value = formula.parameters[0].Value as Formula;
				cell.Content = ContentType.Formula;
			}
			else
			{
				cell.Value = formula;
				cell.Content = ContentType.Formula;
			}
		}

		internal static bool ParseRange(Cell cell, Match match, out Range range, bool absolute)
		{
			range = null;
			if(match.Groups["File"].Success)
				return false;
			Worksheet ws;
			if(match.Groups["Sheet"].Success)
			{
				string sheet = match.Groups["Sheet"].Value;
				if(sheet.Right(1) == "'")
					sheet = sheet.Left(sheet.Length - 1);
				ws = cell.GetParentBook()[sheet];
			}
			else
				ws = cell.ParentRow.ParentSheet;
			if(ws == null)
				return false;
			int cellFromRow = 0;
			int cellFromCol = 0;
			if(match.Groups["RowStart"].Success)
			{
				if(int.TryParse(match.Groups["RowStart"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cellFromRow))
				{
					if(!absolute)
						cellFromRow--;
				}
			}
			if(match.Groups["ColStart"].Success)
			{
				if(int.TryParse(match.Groups["ColStart"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cellFromCol))
				{
					if(!absolute)
						cellFromCol--;
				}
			}
			Cell cellTo = null;
			Cell cellFrom = GetCell(ws, cell, absolute, cellFromRow, cellFromCol);
			if(match.Groups["RowEnd"].Success)
			{
				int cellToRow = 0;
				int cellToCol = 0;
				if(match.Groups["RowEnd"].Success)
				{
					if(int.TryParse(match.Groups["RowEnd"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cellToRow))
					{
						if(!absolute)
							cellToRow--;
					}
				}
				if(match.Groups["ColEnd"].Success)
				{
					if(int.TryParse(match.Groups["ColEnd"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out cellToCol))
					{
						if(!absolute)
							cellToCol--;
					}
				}
				cellTo = GetCell(ws, cell, absolute, cellToRow, cellToCol);
			}
			range = new Range(cellFrom, cellTo);
			return true;
		}

		internal static void ParsePrintHeaders(Worksheet ws, string range)
		{
			string[] printOptions = range.Split(new[] {
				','
			});
			foreach (string po in printOptions)
			{
				Match match = PrintRowRegex.Match(po);
				if(match.Success)
				{
					int start;
					if(int.TryParse(match.Groups["RowStart"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out start))
					{
						int end = 0;
						if(!(match.Groups["RowEnd"].Success && int.TryParse(match.Groups["RowEnd"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out end)))
						{
							end = start;
						}
						ws.PrintOptions.SetTitleRows(start, end);
					}
				}
				else
				{
					match = PrintColumnsRegex.Match(po);
					if(match.Success)
					{
						int start;
						if(int.TryParse(match.Groups["ColStart"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out start))
						{
							int end = 0;
							if(!(match.Groups["ColEnd"].Success && int.TryParse(match.Groups["ColEnd"].Value, NumberStyles.Integer, CultureInfo.InvariantCulture, out end)))
							{
								end = start;
							}
							ws.PrintOptions.SetTitleColumns(start, end);
						}
					}
				}
			}
		}
	}
}
