using FontStashSharp;
using GEngine.Core;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace GEngine.Graphics
{
    public abstract class ASCIILineShader
    {
        protected Color[] DrawableColor;
        protected string DrawableText;
        protected Vector2 DrawPoint;
        protected Vector2 DrawScale;
        public abstract void PreDraw(Transform transform, string baseText, Color[] baseColor, Dictionary<string, object> args = null);
        public abstract Color[] ChangeColor(Color[] baseColor);
        public abstract string ChangeText(string baseText);
        public virtual void Draw(SpriteBatch spriteBatch, SpriteFontBase spf, string baseText, Color[] baseColor) =>
            spriteBatch.DrawString(spf, DrawableText, DrawPoint, DrawableColor, 0, default, DrawScale);

        //protected void SetBasePositionArgs(Transform position, SpriteFontBase spf, float width, float height, int line)
        //{
        //    Vector2 vector = Transform.GameToScreenPosition(position.position, width * position.scale.X, height * position.scale.Y);
        //    new Vector2(vector.X, vector.Y + spf.LineHeight * line * position.scale.Y);
        //}
    }
    public class BaseShader : ASCIILineShader
    {
        public override void PreDraw(Transform transform, string baseText, Color[] baseColor, Dictionary<string, object> args = null)
        {
            DrawableColor = ChangeColor(baseColor);
            DrawableText = ChangeText(baseText);
        }
        public override Color[] ChangeColor(Color[] baseColor) => baseColor;
        public override string ChangeText(string baseText) => baseText;
    }
}
