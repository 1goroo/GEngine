using Microsoft.Xna.Framework;
namespace GEngine.Core
{
    public class Utilities
    {
        public static float GetPercentByScreen(int pixel, bool isWidth) =>
            isWidth ? (float)pixel / Config.pixelScreenWidth : (float)pixel / Config.pixelScreenHeight;
        public static Vector2 GetPercentByScreen(Vector2 pixel)
            => new Vector2(pixel.X / Config.pixelScreenWidth, pixel.Y / Config.pixelScreenHeight);
    }
}
