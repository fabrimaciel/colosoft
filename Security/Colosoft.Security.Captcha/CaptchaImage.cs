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
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using Colosoft.Security.CaptchaSupport;

namespace Colosoft.Security.Captcha
{
	internal class CaptchaImage
	{
		public string Text
		{
			get
			{
				return this.text;
			}
		}

		public Bitmap Image
		{
			get
			{
				return this.image;
			}
		}

		public int Width
		{
			get
			{
				return this.width;
			}
		}

		public int Height
		{
			get
			{
				return this.height;
			}
		}

		private string text;

		private int width;

		private int height;

		private string familyName;

		private Bitmap image;

		private Random random = new Random();

		public CaptchaImage(string s, int width, int height)
		{
			this.text = s;
			this.SetDimensions(width, height);
			this.GenerateImage();
		}

		public CaptchaImage(string s, int width, int height, string familyName)
		{
			this.text = s;
			this.SetDimensions(width, height);
			this.SetFamilyName(familyName);
			this.GenerateImage();
		}

		~CaptchaImage()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if(disposing)
				this.image.Dispose();
		}

		private void SetDimensions(int width, int height)
		{
			if(width <= 0)
				throw new ArgumentOutOfRangeException("width", width, "Argument out of range, must be greater than zero.");
			if(height <= 0)
				throw new ArgumentOutOfRangeException("height", height, "Argument out of range, must be greater than zero.");
			this.width = width;
			this.height = height;
		}

		private void SetFamilyName(string familyName)
		{
			try
			{
				Font font = new Font(this.familyName, 12F);
				this.familyName = familyName;
				font.Dispose();
			}
			catch
			{
				this.familyName = System.Drawing.FontFamily.GenericSerif.Name;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
		private void GenerateImage()
		{
			Bitmap bitmap = new Bitmap(this.width, this.height, PixelFormat.Format32bppArgb);
			Graphics g = Graphics.FromImage(bitmap);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			Rectangle rect = new Rectangle(0, 0, this.width, this.height);
			HatchBrush hatchBrush = new HatchBrush(HatchStyle.SmallConfetti, Color.LightGray, Color.White);
			g.FillRectangle(hatchBrush, rect);
			SizeF size;
			float fontSize = rect.Height + 1;
			Font font = new Font(this.familyName, fontSize, FontStyle.Bold);
			do
			{
				fontSize--;
				font.Dispose();
				font = new Font(this.familyName, fontSize, FontStyle.Bold);
				size = g.MeasureString(this.text, font);
			}
			while (size.Width > rect.Width);
			StringFormat format = new StringFormat();
			format.Alignment = StringAlignment.Center;
			format.LineAlignment = StringAlignment.Center;
			GraphicsPath path = new GraphicsPath();
			path.AddString(this.text, font.FontFamily, (int)font.Style, font.Size, rect, format);
			float v = 4F;
			PointF[] points =  {
				new PointF(this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
				new PointF(rect.Width - this.random.Next(rect.Width) / v, this.random.Next(rect.Height) / v),
				new PointF(this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v),
				new PointF(rect.Width - this.random.Next(rect.Width) / v, rect.Height - this.random.Next(rect.Height) / v)
			};
			using (Matrix matrix = new Matrix())
			{
				matrix.Translate(0F, 0F);
				path.Warp(points, rect, matrix, WarpMode.Perspective, 0F);
			}
			hatchBrush.Dispose();
			hatchBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.LightGray, Color.DarkGray);
			g.FillPath(hatchBrush, path);
			int m = Math.Max(rect.Width, rect.Height);
			for(int i = 0; i < (int)(rect.Width * rect.Height / 30F); i++)
			{
				int x = this.random.Next(rect.Width);
				int y = this.random.Next(rect.Height);
				int w = this.random.Next(m / 50);
				int h = this.random.Next(m / 50);
				g.FillEllipse(hatchBrush, x, y, w, h);
			}
			font.Dispose();
			hatchBrush.Dispose();
			g.Dispose();
			path.Dispose();
			format.Dispose();
			this.image = bitmap;
		}
	}
}
