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
using System.Drawing;

namespace Colosoft.Excel
{
	public class ColorPalette
	{
		public Dictionary<int, Color> Palette = new Dictionary<int, Color>();

		public ColorPalette()
		{
			Palette.Add(0, Color.Black);
			Palette.Add(1, Color.White);
			Palette.Add(2, Color.Red);
			Palette.Add(3, Color.Green);
			Palette.Add(4, Color.Blue);
			Palette.Add(5, Color.Yellow);
			Palette.Add(6, Color.Magenta);
			Palette.Add(7, Color.Cyan);
			Palette.Add(0x1F, Color.FromArgb(204, 204, 255));
			Palette.Add(0x40, SystemColors.Window);
			Palette.Add(0x41, SystemColors.WindowText);
			Palette.Add(0x43, SystemColors.WindowFrame);
			Palette.Add(0x4D, SystemColors.ControlText);
			Palette.Add(0x4E, SystemColors.Control);
			Palette.Add(0x4F, Color.Black);
			Palette.Add(0x50, SystemColors.Info);
			Palette.Add(0x51, SystemColors.InfoText);
			Palette.Add(0x7FFF, SystemColors.WindowText);
		}

		public Color this[int index]
		{
			get
			{
				if(Palette.ContainsKey(index))
				{
					return Palette[index];
				}
				return Color.White;
			}
			set
			{
				Palette[index] = value;
			}
		}
	}
}
