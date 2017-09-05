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

namespace Colosoft.Drawing
{
	/// <summary>
	/// Assinatura de um graphics.
	/// </summary>
	public interface IGraphics
	{
		/// <summary>
		/// Inicia uma entidade.
		/// </summary>
		/// <param name="entity"></param>
		void BeginEntity(object entity);

		/// <summary>
		/// Define uma fonte.
		/// </summary>
		/// <param name="f"></param>
		void SetFont(Font f);

		/// <summary>
		/// Define uma cor.
		/// </summary>
		/// <param name="c"></param>
		void SetColor(Color c);

		/// <summary>
		/// Limpa com a cor informada.
		/// </summary>
		/// <param name="c"></param>
		void Clear(Color c);

		/// <summary>
		/// Preenche o poligono.
		/// </summary>
		/// <param name="poly"></param>
		void FillPolygon(Polygon poly);

		/// <summary>
		/// Desenha o poligono.
		/// </summary>
		/// <param name="poly"></param>
		/// <param name="w"></param>
		void DrawPolygon(Polygon poly, float w);

		/// <summary>
		/// Preenche o retangulo.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		void FillRect(float x, float y, float width, float height);

		/// <summary>
		/// Desenha o retangulo.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="w"></param>
		void DrawRect(float x, float y, float width, float height, float w);

		/// <summary>
		/// Preenche o retangulo arredondado.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="radius"></param>
		void FillRoundedRect(float x, float y, float width, float height, float radius);

		/// <summary>
		/// Desenha o retangulo arredondado.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="radius"></param>
		/// <param name="w"></param>
		void DrawRoundedRect(float x, float y, float width, float height, float radius, float w);

		/// <summary>
		/// Preenche o oval.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		void FillOval(float x, float y, float width, float height);

		/// <summary>
		/// Desenha o oval.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="w"></param>
		void DrawOval(float x, float y, float width, float height, float w);

		/// <summary>
		/// Inicia as linhas.
		/// </summary>
		/// <param name="rounded"></param>
		void BeginLines(bool rounded);

		/// <summary>
		/// Desenha a linha.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		/// <param name="ex"></param>
		/// <param name="ey"></param>
		/// <param name="w"></param>
		void DrawLine(float sx, float sy, float ex, float ey, float w);

		/// <summary>
		/// Finaliza as linhas.
		/// </summary>
		void EndLines();

		/// <summary>
		/// Preenche o arco.
		/// </summary>
		/// <param name="cx"></param>
		/// <param name="cy"></param>
		/// <param name="radius"></param>
		/// <param name="startAngle"></param>
		/// <param name="endAngle"></param>
		void FillArc(float cx, float cy, float radius, float startAngle, float endAngle);

		/// <summary>
		/// Desenha o arco.
		/// </summary>
		/// <param name="cx"></param>
		/// <param name="cy"></param>
		/// <param name="radius"></param>
		/// <param name="startAngle"></param>
		/// <param name="endAngle"></param>
		/// <param name="w"></param>
		void DrawArc(float cx, float cy, float radius, float startAngle, float endAngle, float w);

		/// <summary>
		/// Desenha a imagem.
		/// </summary>
		/// <param name="img"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		void DrawImage(IImage img, float x, float y, float width, float height);

		/// <summary>
		/// Desenha o texto.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="lineBreak"></param>
		/// <param name="align"></param>
		void DrawString(string s, float x, float y, float width, float height, LineBreakMode lineBreak, TextAlignment align);

		/// <summary>
		/// Desenha o texto.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		void DrawString(string s, float x, float y);

		/// <summary>
		/// Salva o estado.
		/// </summary>
		void SaveState();

		/// <summary>
		/// Define o retangulo para aparar.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <param name="width"></param>
		/// <param name="height"></param>
		void SetClippingRect(float x, float y, float width, float height);

		/// <summary>
		/// Traduz as coordenadas.
		/// </summary>
		/// <param name="dx"></param>
		/// <param name="dy"></param>
		void Translate(float dx, float dy);

		/// <summary>
		/// Justa a escala.
		/// </summary>
		/// <param name="sx"></param>
		/// <param name="sy"></param>
		void Scale(float sx, float sy);

		/// <summary>
		/// Restaura o estado.
		/// </summary>
		void RestoreState();

		/// <summary>
		/// Recupera as métricas da fonte.
		/// </summary>
		/// <returns></returns>
		IFontMetrics GetFontMetrics();

		/// <summary>
		/// Recupera a image do arquivo.
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		IImage ImageFromFile(string path);
	}
}
