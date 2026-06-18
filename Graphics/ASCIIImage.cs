using FontStashSharp;
using GEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GEngine.Graphics
{
    /// <summary>
    /// Stores ASCII image for drawing
    /// </summary>
    public struct ASCIIImage
    {
        public string[] text { get; private set; }
        public SpriteFontBase spf { get; private set; }
        public Color color { get; private set; }
        public float height;
        public float width;
        ASCIILine[] lines;
        ASCIILineShader lineShader = new BaseShader();
        public ASCIIImage(string[] _text, SpriteFontBase _spf, Color _color, ASCIILineShader lineShader = null)
        {
            text = _text;
            spf = _spf;
            color = _color;
            height = text.Length * spf.LineHeight;
            width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                float maxWidth = spf.MeasureString(text[i]).X;
                if (width < maxWidth) width = maxWidth;
            }
            lines = new ASCIILine[text.Length];
            for (int i = 0; i < text.Length; i++) 
                lines[i] = new ASCIILine(text[i], Array.Empty<Color>(), color);
        }
        public ASCIIImage(string[] _text, SpriteFontBase _spf, Color baseColor, Color[][] currentColor)
        {
            text = _text;
            spf = _spf;
            color = baseColor;
            height = text.Length * spf.LineHeight;
            width = 0;
            for (int i = 0; i < text.Length; i++)
            {
                float maxWidth = spf.MeasureString(text[i]).X;
                if (width < maxWidth) width = maxWidth;
            }
            lines = new ASCIILine[text.Length];
            for (int i = 0; i < text.Length; i++)
            {
                if (currentColor.Length - i > 0)
                    lines[i] = new ASCIILine(text[i], currentColor[i], color);
                else
                {
                    Color[] newColor = new Color[text[i].Length];
                    Array.Fill<Color>(newColor, color);
                    lines[i] = new ASCIILine(text[i], newColor, color);
                }
            }
        }
        /// <summary>
        /// Drawing ASCIIImage in game position
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            Vector2 vector = Transform.GameToScreenPosition(position, height, width);
            if (text == null) return;
            for (int i = 0; i < text.Length; i++)
            {
                lineShader.PreDraw(new Transform(position), lines[i].text, lines[i].lineColor);
                spriteBatch.DrawString(spf, lineShader.ChangeText(lines[i].text), new Vector2(vector.X, vector.Y + spf.LineHeight * i),
                    lineShader.ChangeColor(lineShader.ChangeColor(lines[i].lineColor)), 0, default);
            }
        }
        /// <summary>
        /// Drawing ASCIIImage in game position
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale)
        {
            Vector2 vector = Transform.GameToScreenPosition(position, width * scale.X, height * scale.Y);
            if (text == null) return;
            for (int i = 0; i < text.Length; i++)
            {
                lineShader.PreDraw(new Transform(position,scale), lines[i].text, lines[i].lineColor);
                spriteBatch.DrawString(spf, lineShader.ChangeText(lines[i].text), new Vector2(vector.X, vector.Y + spf.LineHeight * i * scale.Y),
                    lineShader.ChangeColor(lineShader.ChangeColor(lines[i].lineColor)), 0, default, scale);
            }
        }
        /// <summary>
        /// Drawing ASCIIImage in game position
        /// </summary>
        public void Draw(SpriteBatch spriteBatch, Transform transform)
        {
            Vector2 vector = Transform.GameToScreenPosition(transform.position, width * transform.scale.X, height * transform.scale.Y);
            if (text == null) return;
            for (int i = 0; i < lines.Length; i++)
            {
                lineShader.PreDraw(transform, lines[i].text, lines[i].lineColor);
                spriteBatch.DrawString(spf, lineShader.ChangeText(lines[i].text), new Vector2(vector.X, vector.Y + spf.LineHeight * i * transform.scale.Y),
                    lineShader.ChangeColor(lineShader.ChangeColor(lines[i].lineColor)), 0, default, transform.scale);
            }

        }
        /// <summary>
        /// Stores line of ASCII image
        /// </summary>
        private struct ASCIILine
        {
            public string text;
            public Color[] lineColor;
            public ASCIILine(string text, Color[] color, Color baseColor)
            {
                this.text = text;
                lineColor = new Color[text.Length];

                for (int i = 0; i < lineColor.Length; i++)
                    lineColor[i] = baseColor;

                int colorsToCopy = Math.Min(text.Length, color.Length);
                for (int i = 0; i < colorsToCopy; i++)
                    lineColor[i] = color[i];
            }
        }
    }
}
