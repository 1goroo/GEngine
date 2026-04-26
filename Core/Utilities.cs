using Microsoft.Xna.Framework;
namespace GEngine.Core
{
    public class Utilities
    {
        public static float GetPercentByScreen(int pixel, bool isWidth) => (float)pixel / Config.pixelScreenWidth;
        public static Vector2 GetPercentByScreen(Vector2 pixel)
            => new Vector2((float)pixel.X / Config.pixelScreenWidth, (float)pixel.Y / Config.pixelScreenHeight);
    }
}
