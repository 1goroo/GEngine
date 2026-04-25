using FontStashSharp;
using GEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Utilities;

namespace GEngine.Graphics
{
    public struct ASCIIImage
    {
        public string[] text { get; private set; }
        public SpriteFontBase spf { get; private set; }
        public Color color { get; private set; }
        public float height;
        public float width;
        public ASCIIImage(string[] _text, SpriteFontBase _spf, Color _color)
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
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            //Vector2 vector = Converter.Vector2Position(position, height, width);
            Vector2 vector = Transform.GameToScreenPosition(position, height, width);
            if (text == null) return;
            for (int i = 0; i < text.Length; i++)
            {
                spriteBatch.DrawString(spf, text[i], new Vector2(vector.X, vector.Y + spf.LineHeight * i), color);
            }
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Vector2 scale)
        {
            Vector2 vector = Transform.GameToScreenPosition(position, width * scale.X, height * scale.Y);
            if (text == null) return;
            for (int i = 0; i < text.Length; i++)
            {
                spriteBatch.DrawString(spf, text[i], new Vector2(vector.X, vector.Y + spf.LineHeight * i * scale.Y), color, 0, default, scale);
            }
        }
        public void Draw(SpriteBatch spriteBatch, Transform transform)
        {
            Vector2 vector = Transform.GameToScreenPosition(transform.position, width * transform.scale.X, height * transform.scale.Y);
            if (text == null) return;
            for (int i = 0; i < text.Length; i++)
            {
                spriteBatch.DrawString(spf, text[i], new Vector2(vector.X , vector.Y  + spf.LineHeight * i * transform.scale.Y), color, 0, default, transform.scale);
            }
        }
    }
}
