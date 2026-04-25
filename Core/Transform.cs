using Microsoft.Xna.Framework;
using System;

namespace GEngine.Core
{
    public class Transform(Vector2 position, Vector2? scale = null)
    {
        public Vector2 position = position;
        public Vector2 scale = scale ?? Vector2.One;
        public Vector2 GetBasePosition(Vector2 size)
        {
            Vector2 graphicsSize = new(Config.pixelScreenWidth, Config.pixelScreenHeight);
            return new Vector2(graphicsSize.X * 0.5f + position.X - size.X * scale.X * 0.5f,
                graphicsSize.Y * 0.5f - position.Y - size.Y * scale.Y * 0.5f);
        }
        public static Vector2 ScreenToGamePosition(Vector2 position)
        {
            Vector2 BackBufferSize = new Vector2(Core.Instance.displayManager.graphicsDevice.PresentationParameters.BackBufferWidth,
                Core.Instance.displayManager.graphicsDevice.PresentationParameters.BackBufferHeight);
            float coefWidth = BackBufferSize.X / Config.pixelScreenWidth;
            float coefHeight = BackBufferSize.Y / Config.pixelScreenHeight;
            float minCoef = Math.Min(coefWidth, coefHeight);

            float offsetX = (BackBufferSize.X - Config.pixelScreenWidth * minCoef) * Config.ScreenDrawAnchore.X;
            float offsetY = (BackBufferSize.Y - Config.pixelScreenHeight * minCoef) * Config.ScreenDrawAnchore.Y;

            return new Vector2((position.X - offsetX) / minCoef - Config.pixelScreenWidth * Config.ScreenDrawAnchore.X,
                -((position.Y - offsetY) / minCoef - Config.pixelScreenHeight * Config.ScreenDrawAnchore.Y));
        }
        public static Vector2 GameToScreenPosition(Vector2 position, float width, float height)
            => new Vector2(Config.pixelScreenWidth * 0.5f + position.X - width * 0.5f, Config.pixelScreenHeight * 0.5f - position.Y - height * 0.5f);
    }
}
