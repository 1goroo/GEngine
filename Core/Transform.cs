using Microsoft.Xna.Framework;

namespace GEngine.Core
{
    public class Transform(Vector2 position, Vector2? scale = null)
    {
        public Vector2 position = position;
        public Vector2 scale = scale ?? Vector2.One;
        public enum Anchor { Center, Top, Bottom, Left, Right, TopLeft, TopRight, BottomLeft, BottomRight }
        public static Vector2 GetAnchor(Anchor anchor)
        {
            Vector2 returnVector = Vector2.Zero;
            switch (anchor)
            {
                case Anchor.Center: returnVector = Vector2.Zero; break;
                case Anchor.Top: returnVector = new Vector2(0, Config.pixelScreenHeight * 0.5f); break;
                case Anchor.Bottom: returnVector = new Vector2(0, Config.pixelScreenHeight * -0.5f); break;
                case Anchor.Left: returnVector = new Vector2(Config.pixelScreenWidth * -0.5f, 0); break;
                case Anchor.Right: returnVector = new Vector2(Config.pixelScreenWidth * 0.5f, 0); break;
                case Anchor.TopLeft: returnVector = new Vector2(Config.pixelScreenWidth * -0.5f, Config.pixelScreenHeight * 0.5f); break;
                case Anchor.TopRight: returnVector = new Vector2(Config.pixelScreenWidth * 0.5f, Config.pixelScreenHeight * 0.5f); break;
                case Anchor.BottomLeft: returnVector = new Vector2(Config.pixelScreenWidth * -0.5f, Config.pixelScreenHeight * -0.5f); break;
                case Anchor.BottomRight: returnVector = new Vector2(Config.pixelScreenWidth * 0.5f, Config.pixelScreenHeight * -0.5f); break;
            }
            return returnVector;
        }

        public Vector2 GetBasePosition(Vector2 size) => new Vector2(
                (Config.pixelScreenWidth - size.X * scale.X) * 0.5f + position.X,
                (Config.pixelScreenHeight - size.Y * scale.Y) * 0.5f - position.Y);
        public static Vector2 ScreenToGamePosition(Vector2 position)
        {
            Vector2 BackBufferSize = Core.Instance.displayManager.BackBufferSize;
            float minCoef = Core.Instance.displayManager.minCoef;

            float offsetX = (BackBufferSize.X - Config.pixelScreenWidth * minCoef) * Config.ScreenDrawAnchor.X;
            float offsetY = (BackBufferSize.Y - Config.pixelScreenHeight * minCoef) * Config.ScreenDrawAnchor.Y;

            return new Vector2((position.X - offsetX) / minCoef - Config.pixelScreenWidth * Config.ScreenDrawAnchor.X,
                -((position.Y - offsetY) / minCoef - Config.pixelScreenHeight * Config.ScreenDrawAnchor.Y));
        }
        public static Vector2 GameToScreenPosition(Vector2 position, float width, float height) => new Vector2(
            (Config.pixelScreenWidth - width) * 0.5f + position.X,
            (Config.pixelScreenHeight - height) * 0.5f - position.Y);
    }
}
